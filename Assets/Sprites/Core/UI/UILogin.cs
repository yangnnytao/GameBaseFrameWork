using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 登录UI
/// </summary>
namespace BaseFrame
{
    public class UILogin : UIBase
    {
        [SerializeField]
        private Button _startBtn;
        [SerializeField]
        private Text _descText;

        /// <summary> 需要注册的消息列表 </summary>
        private List<string> MessageList
        {
            get
            {
                return new List<string>()
                {
                    NotiConst.SUCCESS_SELECTSERVER,
                    NotiConst.SUCCESS_LOGIN_NAMEANDPASSWORD,
                };
            }
        }

        private void Awake()
        {
            RemoveMessage(this, MessageList);
            RegisterMessage(this, MessageList);

        }
        /// <summary> 添加监听 </summary>
        protected override void OnAddListener()
        {
            _startBtn.onClick.AddListener(OnStartGameButtonPress);
        }
        /// <summary> 监听删除 </summary>
        protected override void OnRemoveListener()
        {
            _startBtn.onClick.RemoveListener(OnStartGameButtonPress);
        }    

        protected override void InitUI()
        {
            base.InitUI();
        }


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void OnStartGameButtonPress()
        {
            GameMode.ChangeGameMode<PlayMode>();
        }

    }
}