using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance { get; private set; }
    [SerializeField]
    private GameObject m_playerPrefab;

    private List<SpawnPoint> m_spawnPoints = new List<SpawnPoint>();

    private Keyboard m_keyboardToSet = null;
    private Mouse m_mouseToSet = null;
    private List<Gamepad> m_allGamepads;
    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        m_spawnPoints.Add(spawnPoint); 
    }
    
    // Start is called before the first frame update 
    void Start()
    {
        m_keyboardToSet = Keyboard.current;
        m_mouseToSet = Mouse.current;
        m_allGamepads = Gamepad.all.ToList<Gamepad>();
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if(m_keyboardToSet != null && m_mouseToSet != null)
            {
                if(HandleKeyboardInput())
                {
                    m_keyboardToSet = null;
                    m_mouseToSet = null;
                }
            }
            foreach (var gamepad in m_allGamepads)
            {
               
                if(HandleGamepadInput(gamepad))
                {
                    break;
                }
            }
        }
    }

    private bool HandleGamepadInput(Gamepad gamepad)
    {
        if (!gamepad.aButton.IsPressed())
        {
            return false;
        }
        var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        var moveComp = player.GetComponent<PlayerMove>();
        if (moveComp)
        {
            moveComp.m_playerGamepad = gamepad;
            m_allGamepads.Remove(gamepad);
            return true;
        }
        
        return false;
    }
    private bool HandleKeyboardInput()
    {
        if(!m_keyboardToSet.spaceKey.isPressed)
        {
            return false;
        }
        var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        var moveComp = player.GetComponent<PlayerMove>();
        if (moveComp)
        {
            moveComp.m_playerKeyboard = m_keyboardToSet;
            moveComp.m_playerMouse = m_mouseToSet;
            return true;
        }

        return false;
    }
    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawn = new Vector2(0.0f,0.0f);
        if(m_spawnPoints.Count > 0)
        {
            spawn = m_spawnPoints[Random.Range(0, m_spawnPoints.Count-1)].transform.position;
        }
        return spawn;
    }
}
