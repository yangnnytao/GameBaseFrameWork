using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[Serializable]
public class RandomEventData
{
    [XML("id")]
    public int mBaseID;
    [XML("name")]
    public string mName;
    [XML("posX")]
    public int mPosX;
    [XML("posY")]
    public int mPosY;
    [XML("posZ")]
    public int mPosZ;

    [XML("event", ',')]
    private List<int> _eventList = new List<int>();



    public int GetRandomEvent
    {
        get
        {
            if (_eventList.Count == 0)
            {
                Debug.LogError("_eventList---error---" + _eventList.Count);
            }

            return _eventList[UnityEngine.Random.Range(0, _eventList.Count)];
        }
    }
}
