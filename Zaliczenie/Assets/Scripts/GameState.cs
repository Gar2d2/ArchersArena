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
    // Start is called before the first frame update
    void Start()
    {
        HidePlayersUi();
        m_respawnMenu.SetActive(true);
        m_gameUI.SetActive(false);
        m_level.SetActive(false);
        m_infoText.text = "";
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

    private void SetupActivePlayersUI()
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
