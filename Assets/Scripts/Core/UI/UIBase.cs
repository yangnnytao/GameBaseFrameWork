using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有UI的基类，这里定义消息机制，规范接口等
/// ljs add 
/// </summary>
namespace BaseFrame
{  
    public class UIBase : Base, IView
    {

        public enum UIType
        {
            NORMAl,  /// 普通模式
            TOP  /// 顶部模式
        }

        //需要消息处理的需要在子类中实现该接口
        public virtual void OnMessage(IMessage message)
        {


        }

        //在子类可以在该接口下进行初始化工作
        protected virtual void OnAddListener()
        {

        }

        //在子类可以在该接口下进行反初始化工作
        protected virtual void OnRemoveListener()
        {

        }

        protected virtual void SetUIMode()
        {

        }

        bool mIsInit = false;
        /// <summary>
        /// 如果子类重写该基类必须在重写方法的最后调用基类的InitUI（）
        /// </summary>
        protected virtual void InitUI()
        {
            SetUIMode();
        }
        /// <summary>
        /// UI层级类型 默认是普通层级
        /// 如果类型为TOP 那么UI的管理不会自动处理（打开新UI关闭当前UI）
        /// </summary>
        public virtual UIType GetUIType { get { return UIType.NORMAl; } }
        // public override UIType GetUIType { get { return UIType.TOP; } }
       // bool isShowMask = false;
        public virtual bool ShowMask
        {
            get { return false; }
        }

        public void Start()
        {
            //Debug.Log(this.name + " Start");
            InitUI();
            mIsInit = true;
            // OnAddListener();
        }
        public void CloseM(bool isDestroy_)
        {
            if (isDestroy_)
                GameObject.Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
        public virtual void OnEnable()
        {
            /*if (mIsInit) */
            OnAddListener();
        }
        public virtual void OnDisable()
        {
            //Debug.Log(this.name + " OnDisable");
            OnRemoveListener();
        }      
    }
}