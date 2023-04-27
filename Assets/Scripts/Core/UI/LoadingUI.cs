using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 过场UI
/// </summary>
namespace BaseFrame
{
    public class typeLoadInfo
    {
        public float Progress;
        public string Hint1;
        public string Hint2;
    }
    public class LoadingUI : UIBase, IUpdateComponent
    {
        /// <summary>
        /// 加载条
        /// </summary>
        private Slider _progressBar;
        /// <summary>
        /// 进度条
        /// </summary>
        private Image _image; 
        /// <summary>
        /// 随机图片
        /// </summary>
        private Image _rangeIamge;
        /// <summary>
        /// 消息
        /// </summary>
        private Text _text;
        /// <summary>
        /// 提示文本1
        /// </summary>
        private Text _loadHintText1;
        /// <summary>
        /// 提示文本2
        /// </summary>
        private Text _loadHintText2;
        /// <summary>
        /// 流程参数
        /// </summary>
        private float _totalProcessValue;
        private float _displayPorcess = 0;
        ///<summary>
        /// 监听的消息
        ///</summary>
        List<string> MessageList
        {
            get
            {
                return new List<string>()
                {
                    NotiConst.UPDATE_MESSAGE,
                    NotiConst.UPDATE_EXTRACT,
                    NotiConst.UPDATE_DOWNLOAD,
                    NotiConst.UPDATE_PROGRESS,
                    NotiConst.LOAD_RPOGRESS,
                };
            }
        }
        private void Awake()
        {
            //初始消息列表
            RemoveMessage(this, MessageList);
            RegisterMessage(this, MessageList);
            //组件获取
            //_progressBar = transform.Find("Slider").GetComponent<Slider>();
            //_image = _progressBar.transform.Find("Image").GetComponent<Image>();
            ////_rangeIamge = transform.Find("RangeImage").GetComponent<Image>();
            //_rangeIamge = this.gameObject.GetComponent<Image>();
            //_text = transform.Find("Text").GetComponent<Text>();
            //_loadHintText1 = transform.Find("LoadHint1").GetComponent<Text>();
            //_loadHintText2 = transform.Find("LoadHint2").GetComponent<Text>();
            UpdateComponentManager.GetInstance().AddUpdateComponent(this, UPDATE_SPACE.TWENTIETH);
            _displayPorcess = 0f;
            InitUI();
            Debug.Log("-----------------------------LoadingUI----------------------------------");
        }

        private void OnDestroy()
        {
            UpdateComponentManager.GetInstance().DelUpdateComponent(this);
        }
        protected override void OnAddListener()
        {
            //Debug.Log("LoadingUI OnAddListener");
            //SingleSceneManager.Instance.UpdateLoadingFunc += UpdateLoadProcess;
            //LuaFramework.GameManager.m_Instance.UpdateLoadingFunc += UpdateLoadProcess;
            //SingleSceneManager.Instance.LoadingStartFunc += LoadMapStart;
            //SingleSceneManager.Instance.LoadingEndFunc += LoadMapEnd;
            GameApp.Instance.LoadGameDataProcessFunc += UpdateLoadProcess;
           // GameApp.Instance.LoadGameDataDoneFunc += LoadGameDataDone;

        }


        protected override void OnRemoveListener()
        {
            //Debug.Log("LoadingUI OnRemoveListener");
            //SingleSceneManager.Instance.UpdateLoadingFunc -= UpdateLoadProcess;
            //LuaFramework.GameManager.m_Instance.UpdateLoadingFunc -= UpdateLoadProcess;
            //SingleSceneManager.Instance.LoadingStartFunc -= LoadMapStart;
            //SingleSceneManager.Instance.LoadingEndFunc -= LoadMapEnd;
            GameApp.Instance.LoadGameDataProcessFunc -= UpdateLoadProcess;
           // GameApp.Instance.LoadGameDataDoneFunc -= LoadGameDataDone;
        }

        protected override void InitUI()
        {
            int tempId = Random.Range(0, 2);
            Debug.Log("Loading" + tempId);
            //_rangeIamge.sprite = Common.GetGameAsset<Sprite>("RangeImage" + tempId.ToString(), Common.resPath_Icon);
        }

        public void UpdateM(float deltaTime_, float fixedDeltaTime_, float realDeltaTime_)
        {
            _displayPorcess += 0.3f;
            _displayPorcess = Mathf.Min(_displayPorcess, _totalProcessValue);
            //_text.text = (int)_displayPorcess + "%";
            //_progressBar.value = _displayPorcess*0.01f;
            //_image.fillAmount = _displayPorcess * 0.01f;
        }

        public void UpdateLoadProcess(float value_, string loadHint1_, string loadHint2_ = "")
        {
            //_text.text = (int)(value_ * 100) + "%";
            //_progressBar.value = value_;
            //_image.fillAmount = value_;
            //_loadHintText1.text = loadHint1_;
            //_loadHintText2.text = loadHint2_;
        }

        public override void OnMessage(IMessage message)
        {
            string name = message.Name;
            object body = message.Body;
            switch (name)
            {
            case NotiConst.LOAD_RPOGRESS:
                    typeLoadInfo tempLoadInfo = body as typeLoadInfo;
                    _displayPorcess = _totalProcessValue;//更新值開始的位置
                    _totalProcessValue = tempLoadInfo.Progress;
                    //_loadHintText1.text = tempLoadInfo.Hint1;
                    //_loadHintText2.text = tempLoadInfo.Hint2;
                    if (_totalProcessValue == 100)
                        _displayPorcess = _totalProcessValue;
                    break;
            case NotiConst.UPDATE_MESSAGE:      //更新消息
                    //m_loadHintText.text = "正在更新...";
                    //UpdateMessage(body.ToString());
                    break;
                case NotiConst.UPDATE_EXTRACT:      //更新解压
                    //m_loadHintText.text = "正在解压资源...";
                    //_loadHintText2.text = body.ToString();
                    //  UpdateExtract(body.ToString());
                    break;
                case NotiConst.UPDATE_DOWNLOAD:     //更新下载
                                                    // UpdateDownload(body.ToString());
                    break;
                case NotiConst.UPDATE_PROGRESS:     //更新下载进度
                    //UpdateProgress(body.ToString());
                    //_loadHintText2.text = body.ToString();
                    break;
            }
        }


        //场景加载开始
        public void LoadMapStart()
        {

        }
        //场景加载结束
        public void LoadMapEnd()
        {
        }

        //public void LoadGameDataDone()
        //{
        //    Invoke("LoadDataDone", 1f);
        //}

        //private void LoadDataDone()
        //{
        //    //Debug.Log("LoadDataDone1111");
        //    int sleep = 1000;
        //    //GUIManager.Instance.ShowUIFromResource<UILogin>("UILogin", false);
        //}

    }
}
