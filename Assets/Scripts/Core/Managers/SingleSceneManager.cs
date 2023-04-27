//using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载管理模块BaseFrame
/// ljs
/// </summary>
namespace BaseFrame
{
    public class SingleSceneManager : Singleton<SingleSceneManager>
    {
        public delegate void UpdateLoadingProcess(float process_, string loadHint1_, string loadHint2_);
        public delegate void OnLoadingStart();
        public delegate void OnLoadingEnd(string sceneMap_);

        public event UpdateLoadingProcess UpdateLoadingFunc;
        public event OnLoadingStart LoadingStartFunc;
        public event OnLoadingEnd LoadingEndFunc;

        public SingleSceneManager()
        {
        }

        private AsyncOperation m_loadProcess = null;
        private bool m_isLoadCompleted = false;
        private string _screenName;
        public void SetSceneName(string screenName_)
        {
            _screenName = screenName_;
        }
        public string GetSceneName()
        { 
            return _screenName;
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName_">场景名字</param>
        public void LoadMap(string sceneName_)
        {
            ///初始化数据部分
            m_loadProcess = null;
            m_isLoadCompleted = false;
            Scene targetScene = SceneManager.GetSceneByName(sceneName_);

#if USE_PACKAGE_MODE
                string tempSceneName = string.Format("{0}{1}", sceneName_, AppConst.ExtName);
                LuaFramework.ResourceManager.m_Instance.LoadScene(tempSceneName, (bundle_) =>
                {
                    Common.StartCoroutine(LoadSceneAsync(sceneName_, bundle_));
                    //bundle_.Unload(false);
                });
#else
            Common.StartCoroutine(LoadSceneAsync(sceneName_, null));
#endif
        }

        public void ResetDataM()
        {
            m_loadProcess = null;
            m_isLoadCompleted = false;
        }

        public IEnumerator LoadScene(string sceneName_)
        {
            CleanScene.LoadScene(sceneName_);
            yield return null;
        }

       public IEnumerator LoadSceneAsync(string loadSceneName_, AssetBundle assetBundle_)
        {
            if (null != LoadingStartFunc)
            {
                if(null != UpdateLoadingFunc)
                    UpdateLoadingFunc(0, "正在进入", "");
                if(null != LoadingStartFunc)
                    LoadingStartFunc();

                // yield return new WaitForSeconds(1f);
            }

            int tempFlag = 1;
            while (!m_isLoadCompleted)
            {
                //string tempPrint = string.Format("{0}{1}", "LoadSceneAsync ", tempFlag++.ToString());
                //Debug.Log(tempPrint);
                if (null == m_loadProcess)
                {
                    Debug.Log("开始异步加载场景！");
                    m_loadProcess = SceneManager.LoadSceneAsync(loadSceneName_, LoadSceneMode.Single);
                }
                else
                {
                    if (tempFlag++ % 2 == 0)
                        yield return null;
                    try
                    {
                        if (null != UpdateLoadingFunc)//更新进度
                        {
                            Debug.Log(m_loadProcess.progress);
                            UpdateLoadingFunc(m_loadProcess.progress, "加载场景不消耗流量！", "");
                        }
                        if (m_loadProcess.isDone || m_loadProcess.progress == 1)
                        {
                           // UpdateLoadingFunc(m_loadProcess.progress, "加载场景完毕！", "");
                            m_isLoadCompleted = true;
                            //  yield return new WaitForEndOfFrame();
                            //UpdateLoadingFunc(m_loadProcess.progress, "加载场景完毕 1！", "");
                            if (null != assetBundle_)
                                assetBundle_.Unload(false);
                            if (null != LoadingEndFunc)
                            {//结束
                                LoadingEndFunc(loadSceneName_);
                            }
                            break;
                        }
                    }
                    catch
                    {
                        Debug.Log("catch!!!!!!!!!!!!!!!!!!!");
                        yield break;
                    }
                }
                //Debug.Log(tempPrint);
               // yield return null;
            }
            Debug.Log("LoadSceneAsync Done0");
            yield break;
        }


        // Update is called once per frame
        void Update()
        {
            //Debug.Log("progress:" + (float)m_loadProcess.progress * 100 + "%");

        }
    }
}
