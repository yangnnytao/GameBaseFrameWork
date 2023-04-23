using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 主要处理资源 </summary>
public class GameDataManager : Singleton<GameDataManager>
{

    public PlayerData mPlayerData;

    public override void InitDataM()
    {
        Debug.Log("GameDataManager--------InitDataM");
        base.InitDataM();
        LoginInit(SystemInfo.deviceUniqueIdentifier);
    }

    public override void DestroyM()
    {
        base.DestroyM();

    }

    public void Save()
    {
        if (mPlayerData != null)
        {

            PersistentMaster.SaveJSON(mPlayerData, "PlayerData");

        }
    }

    public void LoginInit(string uId)
    {
        //mPlayerData = TryGetPlayerDataFromString(PersistentMaster.GetString("PlayerData"));
        mPlayerData = PersistentMaster.GetJSONData<PlayerData>("PlayerData");
        if (mPlayerData == null)
        {
            mPlayerData = new PlayerData();
            mPlayerData.mInstanceID = uId;
            mPlayerData.SetCreateTime();
            mPlayerData.SetLoginTime();
        }

        if (TimeManager.Instance.IsNewDay(mPlayerData.GetLoginTime(), false))
        {
            mPlayerData.ResetEveryDay();
        }

        mPlayerData.SetLoginTime();

    }

    public void EnterPlayWorld()
    {


        TimeManager.Instance.AddEvent(TimeManager.EVENT_TIMEFLOW_MINUTE, (objs) =>
        {
            int value = (int)objs[0];
            if ((value + 1) % 3 == 0)
                Save();
        }, true);//每隔3分钟自动保存数据

        TimeManager.Instance.AddEvent(TimeManager.EVENT_TIMEFLOW_DAY, (objs) =>
        {
            mPlayerData.ResetEveryDay();
        }, true);//每天重置数据
        TimeManager.Instance.AddEvent(TimeManager.EVENT_TIMEFLOW_SECOND, (objs) =>
        {
          
        }, true);//每天重置数据
        Save();
    }

    public void RestSingleData()
    {
        mPlayerData.RestEverySingle();
    }
}
