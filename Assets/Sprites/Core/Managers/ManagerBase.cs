using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BaseFrame;

public class ManagerBase<T> : EventDispatcher  where T : MonoBehaviour 
{
	private AppFacade m_Facade;
	private ThreadManager m_ThreadMgr;
	private GameManager m_GameManager;
	private TimeManager m_TimeManager;
	private MyNetWorkManager m_NetWorkManager;


}