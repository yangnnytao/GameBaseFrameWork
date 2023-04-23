using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 登录模式
/// 这里处理登录场景相关逻辑
/// </summary>
namespace BaseFrame
{
    public class LoginMode : GameMode
    {
        public override void Init()
        {
            base.Init();
            gameState = GameState.Login;
            SingleSceneManager.Instance.LoadMap("LoginMode");
            //这里可以初始化相关数据
        }

        public override void InitUI()
        {
            GUIManager.Instance.CloseAllUI();
           
        }

        public override void LoadMapStart()
        {
            base.LoadMapStart();
        }

        public override void LoadMapDone(string sceneName_)
        {
            base.LoadMapDone(sceneName_);
            //展示UI
            GUIManager.Instance.ShowUI<UILogin>("UILogin");
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

        //重新登录
        public void ReloadLogin()
        {

        }

    }
}
