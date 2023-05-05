using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinMenu : MonoBehaviour
{
public GameObject m_levelsObjects;
    public void StartGame()
    {
        m_levelsObjects.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
