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

public class PlayerMove : MonoBehaviour
{
    private bool m_bIsJumping = false;
    public float m_playerSpeed = 1.0f;

    private GameObject m_indicatorArrow;

    public Gamepad m_playerGamepad { get; set; }
    public Mouse m_playerMouse { get; set; }
    public Keyboard m_playerKeyboard { get; set; }
    private Animator m_Animator;

    private InputAction m_walkingAction = new InputAction("Walking");
    private InputAction m_aimingAction = new InputAction("StartAiming");

    private Rigidbody2D m_rigidbody;
    HashSet<Action> m_methodsOnUpdate = new HashSet<Action>();
    // Start is called before the first frame update
    void Start()
    {
        m_indicatorArrow = this.transform.Find("IndicatorArrow").gameObject;

        RenderArrow(false);

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
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
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


        jump.performed += _ => Jump();
        jump.Enable();
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
        m_rigidbody.AddForce(new Vector2(0.0f, 7.0f), ForceMode2D.Impulse);
        m_Animator.SetBool("bIsJumping", true);
        m_bIsJumping = true;
    }
    void Walk(CallbackContext ctx)
    {
        m_methodsOnUpdate.Remove(StopWalking);
        m_methodsOnUpdate.Add(OnWalking);
        var moveVector = new Vector2(ctx.ReadValue<Vector2>().x, 0.0f);
        moveVector.x *= m_playerSpeed;
        moveVector.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = moveVector;


    }
    void EndWalk(CallbackContext ctx)
    {
        m_methodsOnUpdate.Add(StopWalking);
        m_methodsOnUpdate.Remove(OnWalking);
    }


    private void StartAiming(CallbackContext ctx)
    {
        RenderArrow(true);
        if(ctx.control.device is Mouse)
        {
            m_methodsOnUpdate.Add(IndicatorFollowMouse);
        }
        else if(ctx.control.device is Gamepad)
        {
            m_methodsOnUpdate.Add(IndicatorFollowRightStick);
        }
    }

    private void RenderArrow(bool bRender)
    {
        foreach (var rend in m_indicatorArrow.GetComponentsInChildren(typeof(Renderer)))
        {
            ((Renderer)rend).enabled = bRender;
        }
    }

    private void StopAiming(CallbackContext ctx)
    {
        RenderArrow(false);
        if (ctx.control.device is Mouse)
        {
            m_methodsOnUpdate.Remove(IndicatorFollowMouse);
        }
        else if (ctx.control.device is Gamepad)
        {
            m_methodsOnUpdate.Remove(IndicatorFollowRightStick);
        }
    }
    private void StopWalking()
    {
        Vector2 moveInput = m_rigidbody.velocity;
        Mathf.SmoothDamp(m_rigidbody.velocity.x, 0.0f, ref moveInput.x, 1.0f);
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

    void OnCollisionEnter2D(Collision2D col)
    {
        m_Animator.SetBool("bIsJumping", false);
        m_bIsJumping = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        m_Animator.SetFloat("playerSpeed", m_rigidbody.velocity.x);
        foreach (Action a in m_methodsOnUpdate)
        {
            a.Invoke();
        }

    }

    private void IndicatorFollowMouse()
    {
        var mouseScreenPosition = m_playerMouse.position.ReadValue();
        mouseScreenPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 dir = new Vector2(
            mouseScreenPosition.x - m_indicatorArrow.transform.position.x,
            mouseScreenPosition.y - m_indicatorArrow.transform.position.y
            );

        m_indicatorArrow.transform.up = dir;
    }
    private void IndicatorFollowRightStick()
    {
        var gamepadVal = m_playerGamepad.rightStick.ReadValue();
        if(gamepadVal.magnitude < 0.1)
        {
            return;
        }
        gamepadVal.Normalize();
        m_indicatorArrow.transform.up = gamepadVal;
    }
}
