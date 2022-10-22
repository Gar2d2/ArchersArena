using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = false;

        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.RegisterSpawnPoint(this);
        }
    }
}
