using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BaseFrame
{
    /// <summary>
    /// 游戏状态：
    /// 和游戏场景地图 一 一对应
    ///  ljs
    /// </summary>
    public enum GameState
    {
        None,
        Login,
        Play,
    }
    /// <summary>
    /// 游戏状态基类
    /// </summary>
    public class GameMode : MonoBehaviour
    {
        public static GameState gameState = GameState.None;
        public static GameState mLastGameState = GameState.None;

        public virtual void OnEnable() {
            SingleSceneManager.Instance.LoadingStartFunc += LoadMapStart;
            SingleSceneManager.Instance.LoadingEndFunc += LoadMapDone;
            OnAddListener();
        }
        public virtual void OnDisable() {
            SingleSceneManager.Instance.LoadingStartFunc -= LoadMapStart;
            SingleSceneManager.Instance.LoadingEndFunc -= LoadMapDone;
            OnRemoveListener();
        }
        
        public void Start()
        {
            Init();
        }

        public virtual void Init() {
            InitUI();
        }

        public virtual void LoadMapStart() { }
        public virtual void LoadMapDone(string sceneName_)
        {
            SingleSceneManager.Instance.SetSceneName(sceneName_);
            //eAudioPlayType tempType = (eAudioPlayType)(UnitData.GetMapData(sceneName_).randomType);
            //AudioManager.Instance.SetBackgroundMusic(UnitData.GetMapBgMusic(sceneName_), tempType);
        }
        public virtual void OnLeaveMap() { }
        public virtual void OnAddListener() { }
        public virtual void OnRemoveListener() { }

        public virtual void Update() { }
        public virtual void InitUI() { }

        public static GameMode mCurrentMode { private set; get; }


        public IEnumerator freeAllMemory()
        {
            Resources.UnloadUnusedAssets(); 
            yield return null;
            System.GC.Collect();
            yield return null;
        }
        public static T ChangeGameMode<T>() where T : GameMode
        {
            if (mCurrentMode)
            {
                mLastGameState = gameState;
                mCurrentMode.OnLeaveMap();
                Destroy(mCurrentMode.gameObject);
            }
            GameObject go = new GameObject(typeof(T).ToString());
            DontDestroyOnLoad(go);
            T mode = go.AddComponent<T>();
            mCurrentMode = mode;

            return mode;
        }
    }
}