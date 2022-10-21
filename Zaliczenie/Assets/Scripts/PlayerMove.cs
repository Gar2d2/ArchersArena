using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerMove : MonoBehaviour
{

    public Gamepad m_playerGamepad { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_playerGamepad.aButton.IsPressed())
        {
            int debug = 0;
        }
    }
}
