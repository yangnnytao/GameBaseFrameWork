﻿/* 
    LuaFramework Code By Jarjin lee
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFrame
{
    /// <summary>
    /// 事件命令
    /// </summary>
    public class ControllerCommand : ICommand
    {
        public virtual void Execute(IMessage message)
        {
        }
    }

    /// <summary>
    /// 外层封装-外观模式
    /// </summary>
    public class Facade
    {
        protected IController m_controller;
        static GameObject m_GameManager;
        static Dictionary<string, object> m_Managers = new Dictionary<string, object>();

        GameObject AppGameManager
        {
            get
            {
                if (m_GameManager == null)
                {
                    m_GameManager = GameObject.Find("_MainGameEntry/GameManager");
                }
                return m_GameManager;
            }
        }

        protected Facade()
        {
            InitFramework();
        }

        protected virtual void InitFramework()//初始化
        {
            if (m_controller != null) return;
            m_controller = Controller.Instance;
        }

        /// <summary> 注册命令 </summary>
        public virtual void RegisterCommand(string commandName, Type commandType)
        {
            m_controller.RegisterCommand(commandName, commandType);
        }

        /// <summary> 删除命令 </summary>
        public virtual void RemoveCommand(string commandName)
        {
            m_controller.RemoveCommand(commandName);
        }

        /// <summary> 检查命令 </summary>
        public virtual bool HasCommand(string commandName)
        {
            return m_controller.HasCommand(commandName);
        }

        /// <summary> 注册多个命令 </summary>
        public void RegisterMultiCommand(Type commandType, params string[] commandNames)
        {
            int count = commandNames.Length;
            for (int i = 0; i < count; i++)
            {
                RegisterCommand(commandNames[i], commandType);
            }
        }

        /// <summary> 删除多个命令 </summary>
        public void RemoveMultiCommand(params string[] commandName)
        {
            int count = commandName.Length;
            for (int i = 0; i < count; i++)
            {
                RemoveCommand(commandName[i]);
            }
        }

        /// <summary> 发送命令 </summary>
        public void SendMessageCommand(string message, object body = null)
        {
            m_controller.ExecuteCommand(new Message(message, body));
        }

        /// <summary> 添加管理器 </summary>
        public void AddManager(string typeName, object obj)
        {
            if (!m_Managers.ContainsKey(typeName))
            {
                m_Managers.Add(typeName, obj);
            }
        }

        /// <summary> 添加Unity对象 </summary>
        public T AddManager<T>(string typeName) where T : Component
        {
            object result = null;
            m_Managers.TryGetValue(typeName, out result);
            if (result != null)
            {
                return (T)result;
            }
            Component c = AppGameManager.AddComponent<T>();
            m_Managers.Add(typeName, c);
            return default(T);
        }

        /// <summary> 获取管理器 </summary>
        public T GetManager<T>(string typeName) where T : class
        {
            if (!m_Managers.ContainsKey(typeName))
            {
                return default(T);
            }
            object manager = null;
            m_Managers.TryGetValue(typeName, out manager);
            return (T)manager;
        }

        /// <summary> 删除管理器 </summary>
        public void RemoveManager(string typeName)
        {

            if (!m_Managers.ContainsKey(typeName))
            {
                return;
            }

            object manager = null;
            m_Managers.TryGetValue(typeName, out manager);

            Type type = manager.GetType();

            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                GameObject.Destroy((Component)manager);
            }

            m_Managers.Remove(typeName);
        }

    }
}