using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;

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

        foreach(var gamepad in m_allGamepads)
        {
            var joinGame = new InputAction("join");
            joinGame.AddBinding(gamepad.aButton);
            joinGame.performed += HandleGamepadInput;
            joinGame.Enable();
        }
        var joinGameKeyboard = new InputAction("joinKb");
        joinGameKeyboard.AddBinding(m_keyboardToSet.spaceKey);
        joinGameKeyboard.performed += HandleKeyboardInput;
        joinGameKeyboard.Enable();
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
  

    private void HandleGamepadInput(CallbackContext ctx)
    {

        var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        var moveComp = player.GetComponent<PlayerMove>();
        if (moveComp)
        {
            moveComp.m_playerGamepad = (Gamepad)ctx.control.device;
            ctx.action.ChangeBinding(0).Erase();
        }
    }
    private void HandleKeyboardInput(CallbackContext ctx)
    {
        var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        var moveComp = player.GetComponent<PlayerMove>();
        if (moveComp && m_keyboardToSet != null && m_mouseToSet != null)
        {
            moveComp.m_playerKeyboard = m_keyboardToSet;
            moveComp.m_playerMouse = m_mouseToSet;
            ctx.action.ChangeBinding(0).Erase();
        }
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
