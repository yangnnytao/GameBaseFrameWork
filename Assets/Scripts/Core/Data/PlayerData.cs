using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    /// <summary> 唯一编号 </summary>
    public string mInstanceID;
    /// <summary> 名字 </summary>
    public string mName;
    /// <summary> 创建时间 </summary>
    private long _creatTime;
    /// <summary> 登录时间 </summary>
    private long _loginTime;


    public PlayerData()
    {

    }

    /// <summary> 重置每天 </summary>
    public void ResetEveryDay()
    {

    }
    /// <summary> 重置每局 </summary>
    public void RestEverySingle()
    {

    }


    /// <summary> 设置登录时间 </summary>
    public void SetLoginTime()
    {
        _loginTime = TimeManager.GetTimeStamp();
    }

    /// <summary> 获取登录时间 </summary>
    public DateTime GetLoginTime()
    {
        return TimeManager.TicksToDate(_loginTime);
    }

    /// <summary> 设置创建时间 </summary>
    public void SetCreateTime()
    {
        _creatTime = TimeManager.GetTimeStamp();
    }

    /// <summary> 获取创建时间 </summary>
    public DateTime GetCreateTime()
    {
        return TimeManager.TicksToDate(_creatTime);
    }
}
