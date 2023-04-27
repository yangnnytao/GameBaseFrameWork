﻿using UnityEngine;
using System.Collections;

namespace BaseFrame
{
    public class StartUpCommand : ControllerCommand
    {

        /// <summary> 执行命令 </summary>
        public override void Execute(IMessage message)
        {
            //  if (!Util.CheckEnvironment()) return;//ljs del
            GameObject gameMgr = GameObject.Find("GlobalGenerator");
            if (gameMgr != null)
            {
                AppView appView = gameMgr.AddComponent<AppView>();
            }
            //-----------------关联命令-----------------------
            //AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE, typeof(SocketCommand));

            //-----------------初始化管理器-----------------------
            //AppFacade.Instance.AddManager<LuaManager>(ManagerName.Lua);
            //AppFacade.Instance.AddManager<PanelManager>(ManagerName.Panel);
            //AppFacade.Instance.AddManager<SoundManager>(ManagerName.Sound);
            //AppFacade.Instance.AddManager<TimerManager>(ManagerName.Timer);
            //AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
            //AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
            //AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
            //AppFacade.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);

            AppFacade.Instance.AddManager<TimeManager>(ManagerName.Timer);
            AppFacade.Instance.AddManager<MyNetWorkManager>(ManagerName.Network);
            AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
            //AppFacade.Instance.AddManager<GameMsg>(ManagerName.GameMsg);
            //AppFacade.Instance.AddManager<MessageLogic>(ManagerName.MessageLogic);
        }
    }
}