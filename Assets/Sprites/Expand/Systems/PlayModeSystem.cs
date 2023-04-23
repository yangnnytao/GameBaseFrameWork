using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFrame;

public class PlayModeSystem : Singleton<PlayModeSystem>
{
    public override void InitDataM()
    {
        base.InitDataM();
        GUIManager.Instance.ShowUI<HallPanel>("HallPanel");
    }

    public override void DestroyM()
    {
        base.DestroyM();
    }

}
