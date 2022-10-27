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
 

    public static SpawnManager instance { get; private set; }
    [SerializeField]
    private GameObject m_playerPrefab;

    public List<GameObject> m_PlayerImages;


    private List<SpawnPoint> m_spawnPoints = new List<SpawnPoint>();

    private Keyboard m_keyboardToSet = null;
    private Mouse m_mouseToSet = null;

    private List<int> m_usedSpawnPointsIndexes = new List<int>();

    private GameState m_GS;
    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        m_spawnPoints.Add(spawnPoint); 
    }
    
    public void RespawnPlayers()
    {
        m_usedSpawnPointsIndexes.Clear();
        foreach ( var activePlayer in m_GS.GetActivePlayers())
        {
            var player = Instantiate(m_playerPrefab, GetNextSpawnPointPosition(), Quaternion.identity);
            m_GS.playerID_PlayerObject[activePlayer.ID] = player;

            var moveComp = player.GetComponent<PlayerMove>();
            if (moveComp)
            {
                if(activePlayer.gamepad != null)
                {
                    moveComp.m_playerGamepad = activePlayer.gamepad;
                }
                else if(activePlayer.keyboard!= null && activePlayer.mouse != null)
                {
                    moveComp.m_playerKeyboard = activePlayer.keyboard;
                    moveComp.m_playerMouse= activePlayer.mouse;
                }
            }
        }
    }
    // Start is called before the first frame update 
    void Start()
    {
        m_GS = GameState.instance;
        foreach (var image in m_PlayerImages)
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
        var activePlayers = m_GS.GetActivePlayers();
        if (activePlayers.Count() >= MAX_PLAYERS)
        {
            return;
        }
        
        var playerDevice = new GameState.ActivePlayer();
        playerDevice.gamepad = (Gamepad)ctx.control.device;
        playerDevice.ID = activePlayers.Count();
        m_PlayerImages[playerDevice.ID].SetActive(true);
        m_GS.AddActivePlayer(playerDevice);
        ctx.action.ChangeBinding(0).Erase();


    }
    private void HandleKeyboardInput(CallbackContext ctx)
    {
        var activePlayers = m_GS.GetActivePlayers();
        if (activePlayers.Count() >= MAX_PLAYERS)
        {
            return;
        }
    
        var playerDevice = new GameState.ActivePlayer();
        playerDevice.keyboard = m_keyboardToSet;
        playerDevice.mouse = m_mouseToSet;
        playerDevice.ID = activePlayers.Count();
        m_PlayerImages[playerDevice.ID].SetActive(true);
        m_GS.AddActivePlayer(playerDevice);
        ctx.action.ChangeBinding(0).Erase();
    }
    private Vector2 GetNextSpawnPointPosition()
    {
        Vector2 spawn = new Vector2(0.0f,0.0f);
        if(m_spawnPoints.Count < m_usedSpawnPointsIndexes.Count)
        {
            return spawn;
        }

        int spawnPointIndex = 0;
        if(m_usedSpawnPointsIndexes.Count == 0)
        {
            if(m_spawnPoints.Count > 0)
            {
                spawnPointIndex = Random.Range(0, m_spawnPoints.Count - 1);
            }
        }
        else //find farthest spawn point
        {
            double distance = double.NegativeInfinity;

            foreach(var point in m_spawnPoints)
            {
                double sumDistances = 0;
                foreach(var usedPoint in m_usedSpawnPointsIndexes)
                {
                    sumDistances += Vector2.Distance(point.transform.position, m_spawnPoints[usedPoint].transform.position);
                }
                if(sumDistances > distance)
                {
                    distance = sumDistances;
                    spawnPointIndex = m_spawnPoints.IndexOf(point);
                }
            }
            

        }

        spawn = m_spawnPoints[spawnPointIndex].transform.position;
        m_usedSpawnPointsIndexes.Add(spawnPointIndex);
        return spawn;
    }
}
