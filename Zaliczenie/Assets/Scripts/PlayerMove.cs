using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMove : MonoBehaviour
{

    public Gamepad m_playerGamepad { get; set; }
    public Mouse m_playerMouse { get; set; }
    public Keyboard m_playerKeyboard { get; set; }

    private bool bIsWalking = false;
    public float m_playerSpeed = 1.0f;
    private InputAction m_accelerateAction = new InputAction();
    private Rigidbody2D m_rigidbody;    
    // Start is called before the first frame update
    void Start()
    {
        if(m_playerGamepad != null)
        {
            MapGamepad();
        }
        if(m_playerMouse != null && m_playerKeyboard != null)
        {
            MapMouseAndKeyboard();
        }
        m_accelerateAction.performed += Walk;
        m_accelerateAction.canceled += EndWalk;
        m_accelerateAction.Enable();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void MapGamepad()
    {
        //var action = new InputAction(binding: m_playerGamepad.aButton.ToString());
        var gamepadName = m_playerGamepad.name + "/";
        var jump = new InputAction(binding: gamepadName + m_playerGamepad.aButton.name);
        jump.performed += _ => Jump();
        jump.Enable();
        //var accelerateAction = new InputAction();
        m_accelerateAction.AddBinding(m_playerGamepad.rightStick);
       
    }
    private void MapMouseAndKeyboard()
    {
        //var action = new InputAction(binding: m_playerGamepad.aButton.ToString());
        var keyboardName = m_playerKeyboard.name + "/";
        var jump = new InputAction(binding: keyboardName + m_playerKeyboard.spaceKey.name);
        jump.performed += _ => Jump();
        jump.Enable();

        m_accelerateAction.AddCompositeBinding("2DVector") // Or "Dpad"
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
    }
    void Jump()
    {
        m_rigidbody.AddForce(new Vector2(0.0f, 5.0f), ForceMode2D.Impulse);
    }
    void Walk(CallbackContext ctx)
    {
        var moveVector = new Vector2(ctx.ReadValue<Vector2>().x, 0.0f);
        moveVector.x *= m_playerSpeed;
        moveVector.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = moveVector;
        bIsWalking = true;
    }
    void EndWalk(CallbackContext ctx)
    {
        //var moveVector = new Vector2(0.0f, 0.0f);
        //moveVector.y = m_rigidbody.velocity.y;
        //m_rigidbody.velocity = moveVector;
        bIsWalking = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if(!bIsWalking)
        {
            Vector2 moveInput = m_rigidbody.velocity;
            Mathf.SmoothDamp(m_rigidbody.velocity.x, 0.0f, ref moveInput.x, 1.0f);
            m_rigidbody.velocity = moveInput;
        }

    }
}
