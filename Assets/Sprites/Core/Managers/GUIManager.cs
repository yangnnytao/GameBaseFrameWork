using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

/// <summary>
/// UI管理模块
/// </summary>
namespace BaseFrame
{
    public class GUIManager : ManagerMono
    {
        //预设缓冲池
        private Dictionary<string, GameObject> m_uiPrefabCache = new Dictionary<string, GameObject>();

        //UI对象管理缓冲池
        private Stack<GameObject> m_uiStack = new Stack<GameObject>();

        public GameObject m_TopUI = null;//TopCanvas下唯一存在的UI

        public Canvas m_topCanvas
        {
            private set; get;
        }

        public Canvas m_rootCanvas
        {
            set; get;
        }
        public void InitDataM()
        {
        }
        public void DestroyM()
        {
        }

        public void ShowWaitingUI(int isShow_)
        {
            //ShowTopUI<WaitingUI>("WaitingUI");
        }

        public void CloseWaitingUI(bool isShow_)
        {
            if (m_TopUI != null)
            {
                Destroy(m_TopUI.gameObject);
            }
        }
        //public static Transform _RootCanvas;
        private static GUIManager m_instance;
        public static GUIManager Instance
        {
            get
            {
                if (null == m_instance)
                {
                    GameObject guiManager = new GameObject("_GUIManager");
                    guiManager.layer = LayerMask.NameToLayer("UI");
                    m_instance = guiManager.AddComponent<GUIManager>();
                    Camera camera = guiManager.AddComponent<Camera>();
                    camera.orthographicSize = Mathf.Tan(Mathf.Deg2Rad * 30f) * 100f;
                    // camera.orthographicSize = 3.6f;
                    camera.orthographic = true;
                    //camera.aspect
                    camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
                    camera.tag = "MainCamera";
                    camera.depth = -1;
                    camera.clearFlags = CameraClearFlags.Depth;
                    if (EventSystem.current != null)
                    {
                        DontDestroyOnLoad(EventSystem.current.gameObject);
                    }
                    else
                    {
                        guiManager.AddComponent<EventSystem>();
                        guiManager.AddComponent<StandaloneInputModule>();
                    }
                    DontDestroyOnLoad(guiManager);

                    m_instance.m_rootCanvas = CreateCanvas("RootCanvas", 100, camera, guiManager.transform, RenderMode.ScreenSpaceOverlay);
                    m_instance.m_topCanvas = CreateCanvas("TopCanvas", 100, camera, guiManager.transform, RenderMode.ScreenSpaceOverlay);
                    m_instance.m_topCanvas.sortingLayerName = "UI_Front";

                }
                return m_instance;
            }
        }

        private static Canvas CreateCanvas(string name, int sortOrder, Camera worldCamera, Transform parent, RenderMode renderMode = RenderMode.ScreenSpaceCamera)
        {
            GameObject rootCanvas = new GameObject(name);
            rootCanvas.layer = LayerMask.NameToLayer("UI");
            rootCanvas.AddComponent<RectTransform>();
            rootCanvas.transform.SetParent(parent, false);
            var canvas = rootCanvas.AddComponent<Canvas>();
            canvas.pixelPerfect = true;//字体清晰
            canvas.sortingOrder = sortOrder;
            canvas.worldCamera = worldCamera;
            canvas.renderMode = renderMode;
            //canvas.sortingLayerName = "";
            //canvas.planeDistance = 50;
            var scaler = rootCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            //scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;//屏幕适应
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.matchWidthOrHeight = 1f;//横版为1


            rootCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvas;
        }

        public Camera UICamera
        {
            get
            {
                return GetComponent<Camera>();
            }
        }

        private void PopUI(bool popOne_, bool isHide_)
        {
            if (popOne_)
            {
                while (m_uiStack.Count > 0)
                {
                    PopAndDestroyUI();
                }
            }
            else
            {
                if (m_uiStack.Count > 0)
                {
                    if (isHide_ && m_uiStack.Peek())//隐藏上一级窗口
                        m_uiStack.Peek().SetActive(false);
                }
            }
        }

        private void PopAndDestroyUI()
        {
            if (m_uiStack.Count > 0)
            {
                var top = m_uiStack.Pop();
                if (top)
                {
                    // top.transform.SetParent(null, false);
                    Destroy(top);
                }

            }
        }

        private GameObject AddTopChild(GameObject prefab)
        {
            var go = Instantiate(prefab);
            go.transform.SetParent(m_topCanvas.transform, false);
            return go;
        }

        private void AddTopUI(Transform ui_)
        {
            var tempCanvas = ui_.GetComponent<Canvas>();
            if (tempCanvas)
            {
                ui_.SetParent(transform, false);
                tempCanvas.worldCamera = m_topCanvas.worldCamera;
                tempCanvas.sortingOrder = m_topCanvas.sortingOrder + 1;
            }
            else
            {
                ui_.SetParent(m_topCanvas.transform, false);
            }
        }

        private string m_lastLoadingUIName;

        /// <summary>
        /// 这个接口从Resource目录加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefabName_"></param>
        /// <param name="push_"></param>
        /// <param name="loadFinishCallback_"></param>
        public void ShowUIFromResource<T>(string prefabName_, bool isPos_, bool isHide_, System.Action<T> loadFinishCallback_ = null) where T : UIBase
        {
            LoadUI(prefabName_, true, isPos_, isHide_, loadFinishCallback_);
        }

        //消除当前窗口，显示新的窗口
        //isPop 为true时，销毁已有的上层窗口，isHide没有意义；
        //isPop 为false时，不不销毁上层窗口：isHide为true时，隐藏已有上层窗口；isHide为false时，不隐藏已有上层窗口
        public void ShowUI<T>(string prefabName_, bool isPop_ = true, bool isHide_ = true, System.Action<T> loadFinishCallback_ = null) where T : UIBase
        {
            Debug.Log("LoadUI " + prefabName_);
#if USE_PACKAGE_MODE
            LoadUI(prefabName_, false,isPop_,isHide_, loadFinishCallback_);
#else
            LoadUI<T>(prefabName_, true, isPop_, isHide_, loadFinishCallback_);

#endif
        }

        /// <summary>
        /// 在当前UI下挂载新的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefabName_"></param>
        /// <param name="loadFinishCallback_"></param>
        public void ShowChildUI<T>(string prefabName_, GameObject parent_ = null, System.Action<T> loadFinishCallback_ = null) where T : MonoBehaviour
        {
            var tempInstance = GetChildUI<T>();
            if (tempInstance)
            {
                tempInstance.gameObject.SetActive(true);
                if (null != loadFinishCallback_)
                {
                    loadFinishCallback_(tempInstance);
                }
                return;
            }
            LoadChildUI(prefabName_, parent_, loadFinishCallback_);
        }

        public void PopChildUI<T>(string prefabName_, bool isDestroy_) where T : MonoBehaviour
        {
            var tempInstance = GetChildUI<T>();
            if (tempInstance)
            {
                if (isDestroy_)
                    Destroy(tempInstance.gameObject);
                else
                    tempInstance.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 弹出最上层UI，并销毁，同时显示第二层UI
        /// </summary>
        public void PopUI()
        {
            if (m_uiStack.Count > 1)
            {
                PopAndDestroyUI();
                if (m_uiStack.Peek())
                    m_uiStack.Peek().SetActive(true);

            }
        }

        /// <summary>
        /// 显示第一个主UI
        /// </summary>
        public void ShowRootUI()
        {
            while (m_uiStack.Count > 1)
            {
                PopAndDestroyUI();
            }
            if (m_uiStack.Peek())
            {
                m_uiStack.Peek().SetActive(true);
            }
        }

        /// <summary>
        /// 关闭所有UI
        /// </summary>
        public void CloseAllUI()
        {
            while (m_uiStack.Count > 0)
            {
                PopAndDestroyUI();
            }
        }

        public void ClearM()
        {
            CloseAllUI();
            m_uiPrefabCache.Clear();
        }

        /// <summary>
        /// 获取当前最上层的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTopUI<T>() where T : MonoBehaviour
        {
            if (m_uiStack.Count > 0)
            {
                if (m_uiStack.Peek())
                {
                    return m_uiStack.Peek().GetComponentInChildren<T>();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前最上层UI的GameObject
        /// </summary>
        /// <returns></returns>
        public GameObject GetTopUIObj()
        {
            if (m_uiStack.Count > 0)
            {
                if (m_uiStack.Peek())
                {
                    return m_uiStack.Peek().gameObject;
                }
            }
            return null;
        }

        //private GameObject LoadTest<T>(string name_, Func<T, GameObject> funCallback_) where T : UIBase
        //{
        //    if(null != funCallback_)
        //    {
        //        T tempObject = TryLoadUIFromCache<T>(name_);
        //        return funCallback_(tempObject);
        //    }
        //    return null;
        //}
        //使用此接口加载一个窗口
        //Normal类型的窗口默认销毁上一个窗口，如果不想消除上一个窗口，则可以调用 PushUI接口
        private void LoadUI<T>(string prefabName_, bool isResData_, bool isPop_, bool isHide_, System.Action<T> loadFinishCallback_) where T : UIBase
        {
            T tempObject = TryLoadUIFromCache<T>(prefabName_);
            if (tempObject)
            {
                switch (tempObject.GetUIType)
                {
                    case UIBase.UIType.NORMAl:
                        PopUI(isPop_, isHide_);
                        m_uiStack.Push(tempObject.gameObject);
                        break;

                    case UIBase.UIType.TOP:
                        AddTopUI(tempObject.transform);
                        break;

                    default:
                        break;
                }

                if (null != loadFinishCallback_)
                    loadFinishCallback_(tempObject);

                return;
            }

            m_lastLoadingUIName = prefabName_;
            LoadingUI(prefabName_, isResData_, go_ =>
            {
                if (go_)
                {
                    // 确保是最后加载的UI显示出来，前面请求加载的UI不显示
                    // 防止出现 先后请求A, B B先加载成功显示了， 然后A才加载成功也显示出来的情况
                    if (m_lastLoadingUIName == prefabName_)
                    {
                        var tempUI = CreateUIFromPrefab(go_);
                        if (tempUI)
                        {
                            var t = tempUI.GetComponent<T>();
                            if (t == null)
                                t = tempUI.AddComponent<T>();
                            switch (t.GetUIType)
                            {
                                case UIBase.UIType.NORMAl:
                                    PopUI(isPop_, isHide_);
                                    m_uiStack.Push(tempUI.gameObject);
                                    break;
                                case UIBase.UIType.TOP:
                                    AddTopUI(t.transform);
                                    break;
                                default:
                                    break;
                            }

                            if (null != loadFinishCallback_)
                                loadFinishCallback_(t);
                        }
                    }
                }
            });
        }

        private void LoadingUI(string name_, bool isResData_, System.Action<GameObject> onLoaded_)
        {
            GameObject tempObject = null;
            if (m_uiPrefabCache.ContainsKey(name_))
            {
                tempObject = m_uiPrefabCache[name_];
            }
            else
            {
                if (isResData_)
                {
                    tempObject = Resources.Load<GameObject>("GUI/" + name_);
                }
                else
                {
#if USE_PACKAGE_MODE
                    tempObject = ResManager.LoadAsset<GameObject>(name_, name_);
#else
                    tempObject = Resources.Load<GameObject>("GUI/" + name_);
#endif
                }

                if (tempObject == null) return;

                m_uiPrefabCache[name_] = tempObject;
            }

            if (onLoaded_ != null)
            {
                onLoaded_(tempObject);
            }
        }

        private void LoadChildUI<T>(string prefabName_, GameObject parent_, System.Action<T> loadFinishCallBace_) where T : MonoBehaviour
        {
            T tempUI = TryLoadUIFromCache<T>(prefabName_);
            if (tempUI)
            {

                var tempRoot = parent_;
                if (null == tempRoot)
                {
                    if (null != m_uiStack.Peek() && false == m_uiStack.Peek().activeInHierarchy)//上层窗口不显示时，则直接删除
                    {
                        PopAndDestroyUI();
                    }
                    tempRoot = m_uiStack.Peek() ?? gameObject;
                }

                tempUI.transform.SetParent(tempRoot.transform, false);
                if (null != loadFinishCallBace_)
                {
                    loadFinishCallBace_(tempUI);
                }
                return;
            }

            LoadingUI(prefabName_, false, go =>
            {
                var tempUI2 = CreateUIFromPrefab(go);
                if (tempUI2)
                {
                    var tempRoot2 = parent_;
                    if (null == tempRoot2)
                    {
                        if (null != m_uiStack.Peek() && false == m_uiStack.Peek().activeInHierarchy)//上层窗口不显示时，则直接删除
                            PopAndDestroyUI();
                        tempRoot2 = m_uiStack.Peek() ?? gameObject;
                    }
                    tempUI2.transform.SetParent(tempRoot2.transform, false);

                    var script = tempUI2.AddComponent<T>();
                    if (null != loadFinishCallBace_)
                    {
                        loadFinishCallBace_(script);
                    }
                }

            });
        }

        /// <summary>
        /// 获得子UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetChildUI<T>() where T : MonoBehaviour
        {
            if (m_uiStack.Count > 0)
            {
                var peek = m_uiStack.Peek();
                if (peek)
                {
                    var parentUi = peek.transform;
                    for (int i = 0; i < parentUi.childCount; i++)
                    {
                        var child = parentUi.GetChild(i);
                        T t = child.GetComponent<T>();
                        if (t)
                        {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        private T TryLoadUIFromCache<T>(string name) where T : MonoBehaviour
        {
            GameObject prefab = null;
            if (m_uiPrefabCache.TryGetValue(name, out prefab))
            {
                var ui = CreateUIFromPrefab(prefab);
                T tempT = ui.GetComponent<T>();
                if (tempT)
                    return tempT;
                return ui.AddComponent<T>();
            }
            return null;
        }

        private GameObject CreateUIFromPrefab(GameObject prefab_)
        {
            var ui = Instantiate(prefab_) as GameObject;
            if (ui)
            {
                ui.name = ui.name.Replace("(Clone)", string.Empty);
                var canvas = ui.GetComponent<Canvas>();

                if (canvas)
                {
                    ui.transform.SetParent(transform, false);
                    canvas.worldCamera = UICamera;
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.scaleFactor = canvas.scaleFactor - 0.00001f;
                }
                else
                {
                    ui.transform.SetParent(m_rootCanvas.transform, false);
                    ui.transform.localScale = Vector3.one;
                }
            }
            return ui;
        }

        /// <summary>
        /// 显示tips
        /// </summary>
        /// <param name="msg_"></param>
        public void ShowMessage(string msg_, bool canGaveUp_ = false, System.Action callBack_confirm = null, System.Action callBack_cancel = null)
        {
            //ShowTopUI<TipsPopupUI>("TipsPopupUI");
            //TipsPopupUI.Instance.showMessage(msg_, canGaveUp_, callBack_confirm, callBack_cancel);
        }

        public void ConfirmDialog(string msg_, System.Action callBack = null)
        {
            //ShowTopUI<ConfirmDialogUI>("ConfirmDialogUI");
            //ConfirmDialogUI.Instance.show(msg_, callBack);
        }

        // 获取窗口句柄
        public T GetTheTopUI<T>()
        {
            if (m_TopUI == null || m_TopUI.GetComponent<T>() == null)
            {
                return default(T);
            }
            else
            {
                return m_TopUI.GetComponent<T>();
            }

        }

        //查找指定觉得UI控件对象
        public GameObject FindUI(string name_)
        {
            //Transform tempTF = _RootCanvas.Find(name_);
            Transform tempTF = m_rootCanvas.transform.Find(name_);
            if (null == tempTF)
                return null;
            return tempTF.gameObject;
        }

        public void ShowTopUI<T>(string prefabName_, System.Action<T> loadFinishCallback_ = null) where T : UIBase
        {
            if (m_TopUI != null)
            {
                Destroy(m_TopUI.gameObject);
            }
            LoadTopUI<T>(prefabName_, loadFinishCallback_);
        }

        public void PopTopUI()
        {
            if (m_TopUI != null)
            {
                Destroy(m_TopUI.gameObject);
            }
            m_TopUI = null;
        }

        private void LoadTopUI<T>(string prefabName_, System.Action<T> loadFinishCallback_) where T : UIBase
        {
            T tempObject = TryLoadUIFromCache<T>(prefabName_);
            if (tempObject)
            {
                AddTopUI(tempObject.transform);
                m_TopUI = tempObject.gameObject;
                if (null != loadFinishCallback_)
                {
                    loadFinishCallback_(tempObject);
                }
                return;
            }

            m_lastLoadingUIName = prefabName_;
            LoadingUI(prefabName_, false, go_ =>
            {
                if (go_)
                {
                    // 确保是最后加载的UI显示出来，前面请求加载的UI不显示
                    // 防止出现 先后请求A, B B先加载成功显示了， 然后A才加载成功也显示出来的情况
                    if (m_lastLoadingUIName == prefabName_)
                    {
                        var tempUI = CreateUIFromPrefab(go_);
                        if (tempUI)
                        {
                            var t = tempUI.AddComponent<T>();
                            m_TopUI = tempUI.gameObject;
                            AddTopUI(tempUI.transform);

                            if (null != loadFinishCallback_)
                                loadFinishCallback_(tempObject);
                        }
                    }
                }
            });
        }
    }
}