using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start()
    {
        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.RegisterSpawnPoint(this);
        }
    }
}
