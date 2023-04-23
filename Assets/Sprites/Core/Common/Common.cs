using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

/// <summary>
/// 一些公共函数 共用枚举，公共变量等
/// ljs 
/// </summary>
public class Common
{
    public static string resPath_Prefabs = "Prefabs/";
    public static string resPath_Sprites = "Sprites/";
    public static string resPath_UIPrefabs = "UIPrefabs/";
    public static string resPath_Xml = "Xmls/";
    public static string resPath_Sound = "Sound/";
    public static string resPath_Music = "Music/";

    public static T GetGameAsset<T>(string asset_, string path_) where T : UnityEngine.Object
    {
        if (asset_.Length == 0)
            return null;
#if USE_PACKAGE_MODE
        //return LuaFramework.ResourceManager.m_Instance.LoadAsset<T>(asset_, asset_);
#else
        string tempFileName = path_ + asset_;
        return Resources.Load<T>(tempFileName);
#endif
    }

    public static GameObject GetGUIPrefab(string asset_)
    {
#if USE_PACKAGE_MODE
            return LuaFramework.ResourceManager.m_Instance.LoadAsset<GameObject>(asset_, asset_);
#else
        string tempFileName = "GUI/" + asset_;
        return Resources.Load<GameObject>(tempFileName);
#endif
    }

    /// <summary>
    /// 协程
    /// </summary>
    /// <param name="routine_"></param>
    /// <param name="monoBehaviour_"></param>
    /// <returns></returns>
    public static Coroutine StartCoroutine(IEnumerator routine_, MonoBehaviour monoBehaviour_ = null)
    {
        if (monoBehaviour_ != null)
            return monoBehaviour_.StartCoroutine(routine_);
        return GameApp.Instance.mainMonoBehaviour.StartCoroutine(routine_);
    }

    /// <summary>
    /// 关闭协程
    /// </summary>
    /// <param name="routine_"></param>
    /// <param name="monoBehaviour_"></param>
    public static void StopCoroutine(IEnumerator routine_, MonoBehaviour monoBehaviour_)
    {
        if (monoBehaviour_ != null)
            monoBehaviour_.StopCoroutine(routine_);
        StopCoroutine(routine_);
    }

    /// <summary>
    /// 关闭协程
    /// </summary>
    /// <param name="routine_"></param>
    public static void StopCoroutine(IEnumerator routine_)
    {
        GameApp.Instance.mainMonoBehaviour.StopCoroutine(routine_);
    }

    /// <summary>
    /// 关闭协程
    /// </summary>
    /// /// <param name="routine_"></param>
    public static void StopCoroutine(string ActionName)
    {
        GameApp.Instance.mainMonoBehaviour.StopCoroutine(ActionName);
    }

    /// <summary>
    /// 定时调用方法
    /// </summary>
    /// <param name="funName_"></param>
    /// <param name="time_"></param>
    public static void InvokeFun(string funName_, float time_)
    {
        GameApp.Instance.mainMonoBehaviour.Invoke(funName_, time_);
    }

    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="funName_"></param>
    /// <param name="time_"></param>
    /// <param name="repeatRate"></param>
    public static void InvokeFun(string funName_, float time_, float repeatRate)
    {
        GameApp.Instance.mainMonoBehaviour.InvokeRepeating(funName_, time_, repeatRate);
    }

    /// <summary>
    /// 退出方法
    /// </summary>
    /// <param name="funName_"></param>
    public static void CancelInvokeFun(string funName_)
    {
        GameApp.Instance.mainMonoBehaviour.CancelInvoke(funName_);
    }

    public static T GetComponent<T>(GameObject obj) where T : Component
    {
        if (obj == null)
            return null;
        T commponent = obj.GetComponent<T>();
        if (commponent == null)
            commponent = obj.AddComponent<T>();
        return commponent;
    }

    public static T GetComponent<T>(Component com) where T : Component
    {
        if (com == null)
            return null;
        return GetComponent<T>(com.gameObject);
    }

    public static T AddComponent<T>(GameObject obj) where T : Component
    {
        return GetComponent<T>(obj);
    }

    public static T AddComponent<T>(Component com) where T : Component
    {
        return GetComponent<T>(com);
    }

    public static void AddChild(Transform parent, Transform child)
    {
        if (parent == null || child == null) return;
        child.parent = parent;
        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = Vector3.zero;
    }

    public static void AddChild(GameObject parent, GameObject child)
    {
        if (parent == null || child == null) return;
        AddChild(parent.transform, child.transform);
    }

    public static void ClearChildren(GameObject obj)
    {
        if (obj == null) return;
        for (int i = 0; i < obj.transform.childCount; ++i)
            UnityEngine.Object.Destroy(obj.transform.GetChild(i).gameObject);
        obj.transform.DetachChildren();
    }

    public static void ClearChildren(Component com)
    {
        if (com == null) return;
        ClearChildren(com.gameObject);
    }

    public static void HideChildren<T>(GameObject obj)
    {
        if (obj == null) return;
        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            if (obj.transform.GetChild(i).GetComponent<T>() == null) continue;
            obj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void HideChildren<T>(Component com)
    {
        if (com == null) return;
        HideChildren<T>(com.gameObject);
    }

    /// <summary>
    /// 将枚举值转为字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index_"></param>
    /// <returns></returns>
    public static string GetStringFormIntEnum<T>(int index_)
    {
        return Enum.GetName(typeof(T), index_);
    }

    public static string GetColorLog(string str_, System.ConsoleColor color_)
    {
        str_ = "<color=" + color_.ToString() + ">" + str_ + "</color>";
        return str_;
    }

    public static string GetSizeLog(string str_, int size_)
    {
        str_ = "<size=" + size_ + ">" + str_ + "</size>";
        return str_;
    }

    /// <summary>
    /// 看向目标
    /// </summary>
    /// <param name="self_">自己</param>
    /// <param name="v3_">目标</param>
    /// <param name="isPlane_">是否是平面</param>
    public static void LookAtTarget(GameObject self_, Vector3 v3_, bool isPlane_)
    {
        self_.transform.LookAt(v3_);
        if (isPlane_)
            self_.transform.rotation = Quaternion.Euler(new Vector3(0, self_.transform.rotation.eulerAngles.y, self_.transform.rotation.eulerAngles.z));
    }

    /// <summary>
    /// 判断是否点击到UI
    /// </summary>
    public static bool IsCursorOverUserInterface()
    {
        // IsPointerOverGameObject check for left mouse (default)
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        // IsPointerOverGameObject check for touches
        for (int i = 0; i < Input.touchCount; ++i)
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                return true;

        // OnGUI check
        return GUIUtility.hotControl != 0;
    }

    /// <summary>
    /// 用于所有ScrollectTag设置
    /// </summary>
    /// <param name="RectData_"></param>
    /// <param name="MinImage_"></param>
    /// <param name="MaxImage_"></param>
    public static void SetScrollectValueChanged(ScrollRect RectData_, int minCoutn_, Image MinImage_, Image MaxImage_)
    {
        if (RectData_.content.childCount <= minCoutn_)
        {
            MinImage_.enabled = false;
            MaxImage_.enabled = false;
            return;
        }
        //Debug.Log(RectData_);
        if (RectData_.verticalNormalizedPosition <= 0.1)
        {
            MinImage_.enabled = false;
            MaxImage_.enabled = true;
        }
        else if (RectData_.verticalNormalizedPosition >= 0.99)
        {
            MinImage_.enabled = true;
            MaxImage_.enabled = false;
        }
        else
        {
            MinImage_.enabled = true;
            MaxImage_.enabled = true;
        }
    }

    /// <summary> 获取搜索到的数目 </summary>
    public static string Search_string(string s, string s1, string s2)
    {
        int n1, n2;
        n1 = s.IndexOf(s1, 0) + s1.Length - 1;   //开始位置
        n2 = s.IndexOf(s2, n1) + 1;               //结束位置
        return s.Substring(n1, n2 - n1);   //取搜索的条数，用结束的位置-开始的位置,并返回
    }

    ///// <summary>
    ///// 获取时间时分秒
    ///// </summary>
    ///// <param name="length">毫秒</param>
    ///// <returns></returns>
    //public static string GetMsToTimeString(int length)
    //{
    //    int hour = length / 1000 / 3600;
    //    int minute = length / 1000 % 3600 / 60;
    //    int second = length / 1000 % 60;
    //    return string.Format("{0:d2}:{1:d2}:{2:d2}", hour, minute, second);
    //}

    ///// <summary>
    ///// 获取时间时分秒
    ///// </summary>
    ///// <param name="length">秒</param>
    ///// <returns></returns>
    //public static string GetTimeString(int length)
    //{
    //    int hour = length / 3600;
    //    int minute = length % 3600 / 60;
    //    int second = length % 60;
    //    return string.Format("{0:d2}:{1:d2}:{2:d2}", hour, minute, second);
    //}

    ///// <summary>
    ///// 获取时间时分
    ///// </summary>
    ///// <param name="length"></param>
    ///// <returns></returns>
    //public static string GetTimeString2(int length)
    //{
    //    int hour = length / 3600;
    //    int minute = length % 3600 / 60;
    //    return string.Format("{0:d2}:{1:d2}", hour, minute);
    //}

    ///// <summary>
    ///// 得到当前时间
    ///// </summary>
    ///// <returns></returns>
    //public static string GetCurrentTime()
    //{
    //    System.DateTime dateTime = System.DateTime.Now;
    //    return string.Format("{0}/{1:D2}/{2:D2} {3:D2}:{4:D2}:{5:D2}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
    //}


    //public static Vector3 FormatVector3(Vector3 val)
    //{
    //    float x = val.x, y = val.y, z = val.z;
    //    if (Mathf.Abs(x) > 180)
    //    {
    //        if (x > 0)
    //            x = -(360 - x);
    //        else
    //            x = 360 - Mathf.Abs(x);
    //    }
    //    if (Mathf.Abs(y) > 180)
    //    {
    //        if (y > 0)
    //            y = -(360 - y);
    //        else
    //            y = 360 - Mathf.Abs(y);
    //    }
    //    if (Mathf.Abs(z) > 180)
    //    {
    //        if (z > 0)
    //            z = -(360 - z);
    //        else
    //            z = 360 - Mathf.Abs(z);
    //    }
    //    return new Vector3(x, y, z);

    //}
    ///// <summary>
    ///// 通过时间戳获得时间
    ///// </summary>
    ///// <param name="timestamp_"></param>
    ///// <returns></returns>
    //public static DateTime GetTimeFormTimestamp(long timestamp_)
    //{
    //    DateTime tempStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
    //    return tempStartTime.AddMilliseconds(timestamp_);
    //}
    ///// <summary>
    ///// 获取当前本地时间戳
    ///// </summary>
    ///// <returns></returns>
    //public static long GetCurrentTimeUnix()
    //{
    //    TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)));
    //    return (long)cha.TotalSeconds;
    //}

    //public static string GetDuration(long ms_)
    //{
    //    return /*ms_ + "...." +*/ ms_ / 60 / 1000 + "'" + ms_ / 1000 % 60 + '"';
    //    //return ms_.ToString();
    //}
    ///// <summary>
    ///// 时间格式显示
    ///// </summary>
    //public static string GetDataStr(int time1_, int time2_, int time3_, string symbol_)//最后一个为之间间隔符号,如":"或者"-"
    //{
    //    string tempStr = time1_.ToString() + symbol_ + time2_.ToString() + symbol_ + time3_.ToString();
    //    return tempStr;
    //}

}
