using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventDispatcher : MonoBehaviour
{
    public delegate void eventHandler(params object[] objs);
    private Dictionary<int, List<eventHandler>> eventPool = new Dictionary<int, List<eventHandler>>();
    private Dictionary<int, List<eventHandler>> savedPool = new Dictionary<int, List<eventHandler>>();

    /// <summary>添加事件</summary>
    public void AddEvent(int id, eventHandler eh, bool save = false)
    {
        if (eventPool.ContainsKey(id))
        {
            if (!eventPool[id].Contains(eh))
            {
                eventPool[id].Add(eh);
            }
        }
        else
        {
            List<eventHandler> ehl = new List<eventHandler>();
            ehl.Add(eh);
            eventPool.Add(id, ehl);
        }
        if (save)
        {
            if (savedPool.ContainsKey(id))
            {
                if (!savedPool[id].Contains(eh))
                {
                    savedPool[id].Add(eh);
                }
            }
            else
            {
                List<eventHandler> ehl = new List<eventHandler>();
                ehl.Add(eh);
                savedPool.Add(id, ehl);
            }
        }
    }

    /// <summary>添加事件</summary>
    public void DoEvent(int id, params object[] objs)
    {
        if (eventPool.ContainsKey(id))
        {
            int count = eventPool[id].Count;
            for (int i = 0; i < count; i++)
            {
                if (eventPool[id][i].Target != null)
                {
                    try
                    {
                        eventPool[id][i](objs);
                    }
                    catch (System.Exception ex)
                    {
                        string info = "StackTrace--->" + ex.StackTrace + "\n";
                        info += "Message--->" + ex.Message + "\n";
                        info += "Source--->" + ex.Source + "\n";
                        info += "TargetSite--->" + ex.TargetSite + "\n";
                        info += "InnerException--->" + ex.InnerException + "\n";
                        info += "type--->" + ex.GetType() + "\n";
                        info += "toString--->" + ex.ToString() + "\n";

                        Debug.LogError(info);
                        throw ex;
                    }
                    if (!eventPool.ContainsKey(id))
                    {
                        return;
                    }
                    else if (count != eventPool[id].Count)
                    {
                        count = eventPool[id].Count;
                        i--;
                    }
                }
                else
                {
                    eventPool[id].RemoveAt(i);
                    i--;
                }
            }
        }
    }

    /// <summary>移除事件</summary>
    public void RemoveEvent(int id)
    {
        if (eventPool.ContainsKey(id))
        {
            eventPool.Remove(id);
        }
    }

    /// <summary>移除一个事件</summary>
    public void RemoveEvent(int id, eventHandler eh)
    {
        if (eventPool.ContainsKey(id))
        {
            if (eventPool[id].Contains(eh))
            {
                eventPool[id].Remove(eh);
                if (eventPool[id].Count == 0)
                {
                    RemoveEvent(id);
                }
            }
        }
    }

    /// <summary>清空事件</summary>
    public void ClearEvent()
    {
        eventPool.Clear();
        foreach (int key in savedPool.Keys)
        {
            List<eventHandler> temp = new List<eventHandler>();
            for (int i = 0; i < savedPool[key].Count; i++)
            {
                temp.Add(savedPool[key][i]);
            }
            eventPool.Add(key, temp);
        }
    }


}
