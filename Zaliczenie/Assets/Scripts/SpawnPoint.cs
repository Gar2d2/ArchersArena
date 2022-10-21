using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    SpawnManager m_spawnManager;
    void Start()
    {
        if(m_spawnManager != null)
        {
            m_spawnManager.RegisterSpawnPoint(this);
        }
    }
}
