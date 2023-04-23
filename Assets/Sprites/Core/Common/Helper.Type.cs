using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///公共的Enum

/// <summary>统一的事件委托</summary>
public delegate void eventHandler();
/// <summary>统一的事件委托</summary>
public delegate void eventHandlerObj(params object[] objs);

/// <summary>
/// 0金币 1体力 2钱 3钻石
/// </summary>
public enum ResourceType
{
    GOLD = 0,//金币
    None = 1,//默认
    MONEY = 2,//钱
    diamond = 3,//钻石
    Undo = 4,//撤回
    MagicBan = 6,//魔法棒
    Score = 7,//分数

    item1 = 101,
    item2 = 102,
    item3 = 103,
    item4 = 104,
    item5 = 105,
    item6 = 106,
    item7 = 107,
    item8 = 108,
 
}

/// <summary>视频奖励类型</summary>
public enum RewardVideoType
{
    GOLD = 0,
    MONEY = 1,
}
/// <summary>UI类型</summary>
public enum UIType
{
    /// <summary>关闭相机</summary>
    CloseCamera,
    /// <summary>永远显示</summary>
    AlwaysShow,
    /// <summary>隐藏其他</summary>
    HideOther,
    /// <summary>永久 </summary>
    Forever,
}

/// <summary>游戏状态</summary>
public enum GameSceneState
{
    Logo = 1,
    Login = 2,
    Create = 3,
    Load = 4,
    Main = 5,
    Battle = 6,
}




