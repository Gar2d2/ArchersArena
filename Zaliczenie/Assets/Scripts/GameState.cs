using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GameState : UsingOnUpdateBase
{
    public static GameState instance { get; private set; }

    public bool bGameIsPaused = false;
    [SerializeField]
    private GameObject m_respawnMenu;
    [SerializeField]
    private GameObject m_level;
    [SerializeField]
    private GameObject m_gameUI;
    [SerializeField]
    private List<GameObject> playerID_displayUI;

    public TextMeshProUGUI m_infoText;
    public struct ActivePlayer
    {
        public int ID { get; set; }
        public Gamepad gamepad { get; set; }
        public Mouse mouse { get; set; }
        public Keyboard keyboard { get; set; }
        public PlayerUiAccessor playerUi {get;set;}
        public GameObject playerPawn { get; set; }
    }

    public List<ActivePlayer> m_activePlayers = new List<ActivePlayer>();

    public void SetPlayerPawn(int playerID, GameObject playerPawn)
    {
        for(int i=0; i< m_activePlayers.Count; i++)
        {
            if (m_activePlayers[i].ID == playerID)
            {
                ActivePlayer temp = m_activePlayers[i];
                temp.playerPawn = playerPawn;
                m_activePlayers[i] = temp;
            }
        }
    }
    public List<ActivePlayer> GetActivePlayers() { return m_activePlayers; }
    public ref List<ActivePlayer> GetActivePlayersReference() { return ref m_activePlayers; }
    public void AddActivePlayer(ActivePlayer player)
    {
        m_activePlayers.Add(player); 
    }
    

    [SerializeField]
    private int m_secondsBeforeStart = 3;

    void PauseGame(bool newState)
    {
        if(newState)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        bGameIsPaused = newState;
       
    }
    public void OnPlayerDeath(int ID)
    {
        //should be dictionary, but im too lazy to fix that
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            ActivePlayer player = m_activePlayers[i];
            if (player.ID == ID)
            {
                RemovePlayerHP(player);
                Destroy(player.playerPawn);
                SetPlayerPawn(player.ID, null);
            }
        }
        //check if we can determinate winner
        if (IsGameEnded())
        {
            string msg;
            if (m_activePlayers.Count == 0)
            {
                msg = " Everybody loosed";
            }
            else
            {
                msg = "Winner is: Player ";
                msg += m_activePlayers[0].ID + 1;

            }
            m_infoText.text = msg;
            StartCoroutine(MakeActionWithDelay(OnNewGameStarted, 5));
            return;
        }
        //check if end round
        if (IsRoundEnded())
        {
            StartCoroutine(MakeActionWithDelay(RespawnPlayersAndStartGame, 3));
        }


    }
    private bool IsRoundEnded()
    {
        int alivePlayers = 0;
        for(int i = 0; i < m_activePlayers.Count; i++)
        {
            if(m_activePlayers[i].playerPawn != null)
            {
                alivePlayers++;
            }    
        }
        return alivePlayers < 2;
    }
    private bool IsGameEnded()
    {
        if (m_activePlayers.Count < 2)
        {
            
            return true;
        }
        return false;
    }

    private void RemovePlayerHP(ActivePlayer player)
    {
        if (!player.playerUi.LowerHpCount())
        {
            m_activePlayers.Remove(player);
           
        }
    }

    void Start()
    {
        OnNewGameStarted();
    }

    private void OnNewGameStarted()
    {
        HidePlayersUi();
        m_respawnMenu.SetActive(true);
        m_gameUI.SetActive(false);
        m_level.SetActive(false);
        m_infoText.fontSize = 52;
        m_infoText.text = "";

        ClearActivePlayersList();
        DestroyAllArrows();
        SpawnManager.instance.PrepareForPlayersToJoin();
    }

    private static void DestroyAllArrows()
    {
        var arrows = GameObject.FindGameObjectsWithTag("Arrow");
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
    }

    public void ClearActivePlayersList()
    {
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            if (m_activePlayers[i].playerPawn != null)
            {
                m_activePlayers[i].playerPawn.GetComponent<PlayerMove>().RemoveBindings();
                Destroy(m_activePlayers[i].playerPawn);
            }
            m_activePlayers.RemoveAt(i);
        }
    }

    private void HidePlayersUi()
    {
        foreach (var playerUI in playerID_displayUI)
        {
            playerUI.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (m_activePlayers.Count == 0)
        {
            return;
        }
        m_respawnMenu.SetActive(false);
        m_gameUI.SetActive(true);
        m_level.SetActive(true);


        RespawnPlayersAndStartGame();
    }

    private void RespawnPlayersAndStartGame()
    {
        DestroyAllArrows(); 
        SpawnManager.instance.RespawnPlayers();
        SetupActivePlayersUI();
        PauseGameBeforeStart();
    }

    private void PauseGameBeforeStart()
    {
        PauseGame(true);
        int startingValue = m_secondsBeforeStart;
        StartCoroutine(MakeActionInFixedTimesWithDelay(() => Countdown(ref startingValue), startingValue + 1, 1f));
    }

    public void SetupActivePlayersUI()
    {
        for(int i =0; i< m_activePlayers.Count; i++)
        {
            if (m_activePlayers[i].ID > playerID_displayUI.Count)
            {
                return;
            }
            var playerUi = playerID_displayUI[m_activePlayers[i].ID];
            playerUi.SetActive(true);
            var playerPawn = m_activePlayers[i].playerPawn.GetComponent<PlayerMove>();
            if (playerPawn)
            {
                playerPawn.m_playerColorTriangle.GetComponent<Renderer>().material.color = playerUi.GetComponent<Image>().color;
            }
            ActivePlayer temp = m_activePlayers[i];
            temp.playerUi = playerUi.gameObject.GetComponent<PlayerUiAccessor>();
            m_activePlayers[i] = temp;

        }
    }


    public void Countdown(ref int value)
    {
        m_infoText.text = value.ToString();
        if (value-- <= 0)
        {
            m_infoText.text = "";
            PauseGame(false);
        }

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

}
