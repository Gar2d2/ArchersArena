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
        yield return new WaitForSecondsRealtime(delay);
        action.Invoke();
    }
    protected IEnumerator MakeActionInFixedTimesWithDelay(Action action, int times, float delay)
    {
       for(int i = 0; i < times; i++)
       {
           yield return MakeActionWithDelay(action, delay);
       }
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameState.instance.bGameIsPaused)
        {
            return;
        }
        foreach (Action a in m_methodsOnUpdate)
        {
            a.Invoke();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (GameState.instance.bGameIsPaused)
        {
            return;
        }
        foreach (Action a in m_methodsOnFixedUpdate)
        {
            a.Invoke();
        }
    }
}
