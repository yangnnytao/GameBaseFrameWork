using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFrame;

public class PlayMode : GameMode
{
    public override void Init()
    {
        base.Init();
        gameState = GameState.Play;
        //SingleSceneManager.Instance.LoadMap("PlayMode");
        //这里可以初始化相关数据
        Common.StartCoroutine(iLoadMap());
    }

    public override void InitUI()
    {
        GUIManager.Instance.CloseAllUI();
    }

    public override void LoadMapStart()
    {
        base.LoadMapStart();
    }

    IEnumerator iLoadMap()
    {
        yield return new WaitForSeconds(0.5f);
        SingleSceneManager.Instance.ResetDataM();

        yield return Common.StartCoroutine(SingleSceneManager.Instance.LoadScene("PlayMode"));
        PlayModeSystem.Instance.InitDataM();
        yield return new WaitForSeconds(1f);
    }

    public override void LoadMapDone(string sceneName_)
    {
        base.LoadMapDone(sceneName_);

    }

    public override void OnAddListener()
    {
        base.OnAddListener();
    }

    public override void OnRemoveListener()
    {
        base.OnRemoveListener();
    }
    public override void OnLeaveMap()
    {
        base.OnLeaveMap();
    }


}
