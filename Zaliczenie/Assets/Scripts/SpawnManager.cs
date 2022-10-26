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
    private int MAX_PLAYERS = 4;
    struct PlayerDevice
    {
        public int ID;
        public Gamepad gamepad;
        public Mouse mouse;
        public Keyboard keyboard;
    }
    List<PlayerDevice> m_activePlayersDevices = new List<PlayerDevice>();

    public static SpawnManager instance { get; private set; }
    [SerializeField]
    private GameObject m_playerPrefab;

    public List<GameObject> m_PlayerImages;


    private List<SpawnPoint> m_spawnPoints = new List<SpawnPoint>();

    private Keyboard m_keyboardToSet = null;
    private Mouse m_mouseToSet = null;

    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        m_spawnPoints.Add(spawnPoint); 
    }
    
    public void RespawnPlayers()
    {
        foreach (var id_device in m_activePlayersDevices)
        {

            var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            var moveComp = player.GetComponent<PlayerMove>();
            if (moveComp)
            {
                if(id_device.gamepad != null)
                {
                    moveComp.m_playerGamepad = id_device.gamepad;
                }
                else if(id_device.keyboard!= null && id_device.mouse != null)
                {
                    moveComp.m_playerKeyboard = id_device.keyboard;
                    moveComp.m_playerMouse= id_device.mouse;
                }
            }
        }
        GameState.instance.StartGame();
    }
    // Start is called before the first frame update 
    void Start()
    {
        foreach(var image in m_PlayerImages)
        {
            image.SetActive(false);
        }
        BindAllDevicesToSpawnMethods();
    }

    private void BindAllDevicesToSpawnMethods()
    {
        m_keyboardToSet = Keyboard.current;
        m_mouseToSet = Mouse.current;
        List<Gamepad> allGamepads = Gamepad.all.ToList<Gamepad>();

        foreach (var gamepad in allGamepads)
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

    }
  

    private void HandleGamepadInput(CallbackContext ctx)
    {
        if(m_activePlayersDevices.Count() >= MAX_PLAYERS)
        {
            return;
        }
        //var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        //var moveComp = player.GetComponent<PlayerMove>();
        //if (moveComp)
        //{
        //moveComp.m_playerGamepad = (Gamepad)ctx.control.device;
        var playerDevice = new PlayerDevice();
        playerDevice.gamepad = (Gamepad)ctx.control.device;
        playerDevice.ID = m_activePlayersDevices.Count();
        m_PlayerImages[playerDevice.ID].SetActive(true);
        m_activePlayersDevices.Add(playerDevice);
        ctx.action.ChangeBinding(0).Erase();
        //}

    }
    private void HandleKeyboardInput(CallbackContext ctx)
    {
        if (m_activePlayersDevices.Count() >= MAX_PLAYERS)
        {
            return;
        }
        //var player = Instantiate(m_playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        //var moveComp = player.GetComponent<PlayerMove>();
        //if (moveComp && m_keyboardToSet != null && m_mouseToSet != null)
        //{
        //    moveComp.m_playerKeyboard = m_keyboardToSet;
        //    moveComp.m_playerMouse = m_mouseToSet;
        //    ctx.action.ChangeBinding(0).Erase();
        //}
        var playerDevice = new PlayerDevice();
        playerDevice.keyboard = m_keyboardToSet;
        playerDevice.mouse = m_mouseToSet;
        playerDevice.ID = m_activePlayersDevices.Count();
        m_PlayerImages[playerDevice.ID].SetActive(true);
        m_activePlayersDevices.Add(playerDevice);
        ctx.action.ChangeBinding(0).Erase();
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
