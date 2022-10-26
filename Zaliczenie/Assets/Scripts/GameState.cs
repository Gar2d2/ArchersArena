using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public TextMeshProUGUI m_infoText;
    


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
        m_respawnMenu.SetActive(true);
        m_gameUI.SetActive(false);
        m_level.SetActive(false);
        m_infoText.text = "";
    }
    public void StartGame()
    {
        m_respawnMenu.SetActive(false);
        m_gameUI.SetActive(true);
        m_level.SetActive(true);
        PauseGame(true);
        int startingValue = m_secondsBeforeStart;
        StartCoroutine(MakeActionInFixedTimesWithDelay(() => Countdown(ref startingValue), startingValue+1, 1f));
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
