using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUiAccessor : MonoBehaviour
{
    [SerializeField]
    List<GameObject> m_hpList;
    private int MAX_HP = 4;
    private int m_hp = 4;
    public bool LowerHpCount()
    {
        m_hpList[--m_hp].SetActive(false);
        return m_hp > 0;
    }
    public void ResetHp()
    {
        for(int i =0; i < MAX_HP; i++)
        {
            m_hpList[i].SetActive(true);
        }
        m_hp = 4;
    }
}
