using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab;

    private List<SpawnPoint> m_spawnPoints = new List<SpawnPoint>();

    private ReadOnlyArray<Gamepad> m_allGamepads;
    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        m_spawnPoints.Add(spawnPoint); 
    }

    // Start is called before the first frame update 
    void Start()
    {
        m_allGamepads = Gamepad.all;
    }
    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var gamepad in m_allGamepads)
            {
                if (gamepad.aButton.IsPressed())
                {
                    int debug = 0;
                }
            }
        }
    }
}
