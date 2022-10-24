using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingOnUpdateBase : MonoBehaviour
{
    // Start is called before the first frame update
    HashSet<Action> m_methodsOnUpdate = new HashSet<Action>();
    HashSet<Action> m_methodsOnFixedUpdate = new HashSet<Action>();
    protected void AddActionOnUpdate(Action action)
    {
        m_methodsOnUpdate.Add(action);
    }
    protected void RemoveActionFromUpdate( Action action)
    {
        m_methodsOnUpdate.Remove(action);
    }
    protected void AddActionOnFixedUpdate(Action action)
    {
        m_methodsOnUpdate.Add(action);
    }
    protected void RemoveActionFromFixedUpdate(Action action)
    {
        m_methodsOnUpdate.Remove(action);
    }

    protected IEnumerator MakeActionWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        foreach (Action a in m_methodsOnUpdate)
        {
            a.Invoke();
        }
    }
    protected virtual void FixedUpdate()
    {
        foreach (Action a in m_methodsOnFixedUpdate)
        {
            a.Invoke();
        }
    }
}
