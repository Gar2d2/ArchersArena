using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class PlayerMove : UsingOnUpdateBase
{
    private bool m_bIsJumping = false;
    private bool m_bCanShoot = true;
    const int MAX_ARROWS = 8;
    [SerializeField]
    private int m_arrowsCount = 4;
    [SerializeField]
    private float m_smoothTime = 1.0f;
    [SerializeField]
    private float m_playerSpeed = 1.0f;
    [SerializeField]
    private GameObject m_projectilePrefab;
    [SerializeField]
    private float m_delayBetweenShots = 0.1f;

    private GameObject m_indicatorArrow;
    private List<GameObject> m_quiver = new List<GameObject>();
    public Mouse m_playerMouse { get; set; }
    public Keyboard m_playerKeyboard { get; set; }
    public Gamepad m_playerGamepad { get; set; }

    private InputAction m_walkingAction = new InputAction("Walking");
    private InputAction m_aimingAction = new InputAction("StartAiming");

    private Animator m_Animator;
    private Rigidbody2D m_rigidbody;
    private SpriteRenderer m_renderer;

    // Start is called before the first frame update
    void Start()
    {

        m_renderer = GetComponent<SpriteRenderer>();
        if (m_renderer == null)
        {
            Debug.LogError("Player Sprite is missing a renderer");
        }
        m_rigidbody = GetComponent<Rigidbody2D>();
        if (m_rigidbody == null)
        {
            Debug.LogError("Player Sprite is missing a rigidbody");
        }
        m_Animator = GetComponent<Animator>();
        if (m_Animator == null)
        {
            Debug.LogError("Player Sprite is missing a animator");
        }

        m_indicatorArrow = this.transform.Find("IndicatorArrow").gameObject;
        SetupQuiver();

        RenderArrow(false);

        //update animator
        AddActionOnFixedUpdate(() => m_Animator.SetFloat("playerSpeed", Math.Abs(m_rigidbody.velocity.x)));

        if (m_playerGamepad != null)
        {
            MapGamepad();
        }
        if (m_playerMouse != null && m_playerKeyboard != null)
        {
            MapMouseAndKeyboard();
        }
        m_walkingAction.performed += Walk;
        m_walkingAction.canceled += EndWalk;
        m_walkingAction.Enable();

        m_aimingAction.Enable();
    }

    private void SetupQuiver()
    {
        var quiver = this.transform.Find("quiver");
        for (int i = 0; i < quiver.childCount; i++)
        {
            m_quiver.Add(quiver.GetChild(i).gameObject);
        }
        foreach (var arrow in m_quiver)
        {
            arrow.SetActive(false);
        }
        for (int i = 0; i < m_arrowsCount; i++)
        {
            m_quiver[i].SetActive(true);
        }
    }

    private void MapGamepad()
    {
        var gamepadName = m_playerGamepad.name + "/";
        var jump = new InputAction(binding: gamepadName + m_playerGamepad.aButton.name);
        jump.performed += _ => Jump();
        jump.Enable();
        m_walkingAction.AddBinding(m_playerGamepad.leftStick);
        m_aimingAction.AddBinding(m_playerGamepad.rightTrigger);
        m_aimingAction.performed += StartAiming;
        m_aimingAction.canceled += StopAiming;

    }
    private void MapMouseAndKeyboard()
    {
        //var action = new InputAction(binding: m_playerGamepad.aButton.ToString());
        var keyboardName = m_playerKeyboard.name + "/";
        var jump = new InputAction(binding: keyboardName + m_playerKeyboard.spaceKey.name);
        jump.performed += _ => Jump();
        jump.Enable();

        m_walkingAction.AddCompositeBinding("2DVector") // Or "Dpad"
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

        m_aimingAction.AddBinding(m_playerMouse.leftButton);
        m_aimingAction.performed += StartAiming;
        m_aimingAction.canceled += StopAiming;
    }
    void Jump()
    {
        var colider = GetComponent<Collider2D>();
        if (colider == null || Physics2D.BoxCastAll(colider.bounds.center, colider.bounds.size, 0f, Vector2.down, 0.1f).Length <= 1)
        {
            return;
        }
        m_rigidbody.AddForce(new Vector2(0.0f, 11.0f), ForceMode2D.Impulse);
        m_Animator.SetBool("bIsJumping", true);
        m_bIsJumping = true;
    }
    void Walk(CallbackContext ctx)
    {
        RemoveActionFromUpdate(StopWalking);
        AddActionOnUpdate(OnWalking);

        var moveVector = new Vector2(ctx.ReadValue<Vector2>().x, 0.0f);
        moveVector.x *= m_playerSpeed;
        moveVector.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = moveVector;
        FlipSpriteTowardsMovement();
    }

    private void FlipSpriteTowardsMovement()
    {
        m_renderer.flipX = m_rigidbody.velocity.x < 0 ? true : false;
    }

    void EndWalk(CallbackContext ctx)
    {
        AddActionOnUpdate(StopWalking);
        RemoveActionFromUpdate(OnWalking);
    }


    private void StartAiming(CallbackContext ctx)
    {
        if(ctx.control.device is Mouse)
        {
            AddActionOnUpdate(IndicatorFollowMouse);
        }
        else if(ctx.control.device is Gamepad)
        {
            AddActionOnUpdate(IndicatorFollowRightStick);
        }
    }

    private void RenderArrow(bool bRender)
    {
        m_indicatorArrow.SetActive(bRender);
    }

    private void StopAiming(CallbackContext ctx)
    {
        RenderArrow(false);
        if (ctx.control.device is Mouse)
        {
            RemoveActionFromUpdate(IndicatorFollowMouse);
        }
        else if (ctx.control.device is Gamepad)
        {
            RemoveActionFromUpdate(IndicatorFollowRightStick);
        }
        
        if(!m_bCanShoot || m_arrowsCount <= 0)
        {
            return;
        }
        var direction = m_indicatorArrow.transform.up;
        direction.Normalize();
        var position = m_indicatorArrow.transform.position;
        var arrow = Instantiate(m_projectilePrefab, position, Quaternion.identity);

        var projectileComp = arrow.GetComponent<ProjectileBase>();

        if (projectileComp)
        {
            Fire(projectileComp);
        }

    }

    private void Fire(ProjectileBase projectileComp)
    {
        LowerArrowCount();
        m_bCanShoot = false;
        projectileComp.FireAtDirection(m_indicatorArrow.transform.up, 10, this.gameObject);
        m_Animator.SetTrigger("tShoot");
        m_renderer.flipX = m_indicatorArrow.transform.up.x > 0 ? true : false;
        StartCoroutine(MakeActionWithDelay(() => m_bCanShoot = true, m_delayBetweenShots));
    }
    private void StopWalking()
    {
        Vector2 moveInput = m_rigidbody.velocity;
        Mathf.SmoothDamp(m_rigidbody.velocity.x, 0.0f, ref moveInput.x, m_smoothTime);
        m_rigidbody.velocity = moveInput;
        m_Animator.speed = 1;
    }    
    private void OnWalking()
    {
        if(m_bIsJumping)
        {
            m_Animator.speed = 1;
            return;
        }
        m_Animator.speed = Math.Abs(m_rigidbody.velocity.x);

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Arrow")
        {
            if (col.gameObject.GetComponent<ProjectileBase>().bCanBePickedUp)
            {
                Destroy(col.gameObject);
                IncreaseArrowCount();
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        m_Animator.SetBool("bIsJumping", false);
        m_bIsJumping = false;
        if (col.collider.tag == "Arrow")
        {
            //TODO move this to function - unbind all from all actions
            m_Animator.SetTrigger("tDeath");
            m_bCanShoot = false;
            m_renderer.flipX = m_renderer.flipX ? false : true;
            AddActionOnUpdate(() => m_rigidbody.velocity = new Vector2(0f, 0f)); //disable input
        }

    }
    void OnDeath()
    {
        Destroy(this.gameObject);
    }
    void IncreaseArrowCount()
    {
        m_arrowsCount = Math.Clamp(m_arrowsCount + 1, 0, MAX_ARROWS);
        m_quiver[m_arrowsCount - 1].SetActive(true);
    }
    void LowerArrowCount()
    {
        m_arrowsCount = Math.Clamp(m_arrowsCount - 1, 0, MAX_ARROWS);
        m_quiver[m_arrowsCount].SetActive(false);
    }
    private void IndicatorFollowMouse()
    {
        var mouseScreenPosition = m_playerMouse.position.ReadValue();
        mouseScreenPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 dir = new Vector2(
            mouseScreenPosition.x - m_indicatorArrow.transform.position.x,
            mouseScreenPosition.y - m_indicatorArrow.transform.position.y
            );
        if(dir.magnitude < 0.1)
        {
            RenderArrow(false);
            return;
        }
        RenderArrow(true);
        m_indicatorArrow.transform.up = dir;
    }
    private void IndicatorFollowRightStick()
    {
        var gamepadVal = m_playerGamepad.rightStick.ReadValue();
        if(gamepadVal.magnitude < 0.3)
        {
            RenderArrow(false);
            return;
        }
        RenderArrow(true);
        gamepadVal.Normalize();
        m_indicatorArrow.transform.up = gamepadVal;
    }
}
