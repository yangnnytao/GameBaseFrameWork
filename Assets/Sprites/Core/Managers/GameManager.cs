using BaseFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager :ManagerBase<GameManager>
{
    protected static bool initialize = false;

    public static GameManager Instance
    {
        get
        {
            return AppFacade.Instance.GetManager<GameManager>(ManagerName.Game); ;
        }
    }

    /// <summary> 初始化游戏管理器 </summary>
    void Awake()
    {
        Init();
    }

    /// <summary> 初始化 </summary>
    void Init()
    {
        DontDestroyOnLoad(gameObject);  //防止销毁自己

        //ljs 暂时注销，等AB部署完后开放
        ////从Resource目录加载资源 加载
        //GUIManager.Instance.ShowUIFromResource<LoadingUI>("LoadingUI", false);
        //CheckExtractResource(); //释放资源
        OnInitialize();//ljs add AB版本后记得关闭

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;
    }

    void OnInitialize()
    {

        GameApp.Instance.Initialize();

        initialize = true;
    }
}
