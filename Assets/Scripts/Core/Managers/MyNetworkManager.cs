using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Net;
using System.Globalization;
using System.Text;
using SimpleJSON;
using BaseFrame;

public class MyNetWorkManager : ManagerBase<MyNetWorkManager>, ManagerInterface
{
    public static MyNetWorkManager Instance
    {
        get
        {
            return AppFacade.Instance.GetManager<MyNetWorkManager>(ManagerName.Network); ;
        }
    }
    public UnityEvent NetworkChangeEvent = new UnityEvent();

    public const int TIMEOUT = 5;//网络请求超时

    public void Init()
    {
        Debug.Log("MyNetWorkManager Init");
    }
    public void SendPostMessage(string url, Dictionary<string, string> post, UnityAction<String> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        StartCoroutine(SendHttpMessage(url, post, "POST", unityAction, errorAction, timeOut));
    }

    public void SendGetMessage(string url, Dictionary<string, string> post, UnityAction<String> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        StartCoroutine(SendHttpMessage(url, post, "GET", unityAction, errorAction, timeOut));
    }

    public void SendJson(string url, string jsonData, UnityAction<String> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        StartCoroutine(SendJsonMessage(url, jsonData, unityAction, errorAction, timeOut));
    }

    public void SendJson<T>(string url, string jsonData, UnityAction<T> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        StartCoroutine(SendJsonMessage(url, jsonData, unityAction, errorAction, timeOut));
    }

    IEnumerator SendJsonMessage(string url, string jsonData, UnityAction<String> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        float runTime = Time.unscaledTime;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
            yield break;
        }
        byte[] body = Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest unityWeb = new UnityWebRequest(url, "POST");
        unityWeb.uploadHandler = new UploadHandlerRaw(body);
        unityWeb.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        unityWeb.downloadHandler = new DownloadHandlerBuffer();
        unityWeb.timeout = timeOut;
        yield return unityWeb.SendWebRequest();
        if (!unityWeb.isHttpError && !unityWeb.isNetworkError)
        {
            if (unityAction != null)
            {
                string result = unityWeb.downloadHandler.text;
                Debug.Log(string.Format("url:{0}-->jsonData:{1},result:{2},runTime:{3}", url, jsonData, result, Time.unscaledTime - runTime));
                unityAction.Invoke(result);
            }
            else
            {
                Debug.Log(string.Format("url:{0}-->jsonData:{1}", url, jsonData));
            }
        }
        else
        {
            Debug.LogWarning(string.Format("url:{0}-->jsonData:{1},error:{2},runTime:{3}", url, jsonData, unityWeb.error, Time.unscaledTime - runTime));
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
        }
    }

    IEnumerator SendJsonMessage<T>(string url, string jsonData, UnityAction<T> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
            yield break;
        }
        float runTime = Time.unscaledTime;
        byte[] body = Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest unityWeb = new UnityWebRequest(url, "POST");
        unityWeb.uploadHandler = new UploadHandlerRaw(body);
        unityWeb.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        unityWeb.downloadHandler = new DownloadHandlerBuffer();
        unityWeb.timeout = timeOut;
        yield return unityWeb.SendWebRequest();
        if (!unityWeb.isHttpError && !unityWeb.isNetworkError)
        {
            if (unityAction != null)
            {
                string result = unityWeb.downloadHandler.text;

                Debug.Log(string.Format("url:{0}-->jsonData:{1},result:{2},runTime:{3}", url, jsonData, result, Time.unscaledTime - runTime));
                if (!string.IsNullOrEmpty(result))
                {
                    T t = JsonUtility.FromJson<T>(result);
                    unityAction.Invoke(t);
                }
            }
            else
            {
                Debug.Log(string.Format("url:{0}-->jsonData:{1}", url, jsonData));
            }
        }
        else
        {
            Debug.LogWarning(string.Format("url:{0}-->jsonData:{1},error:{2},runTime:{3}", url, jsonData, unityWeb.error, Time.unscaledTime - runTime));
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
        }
    }

    IEnumerator SendHttpMessage(string url, Dictionary<string, string> post = null, string method = "GET", UnityAction<string> unityAction = null, UnityAction errorAction = null, int timeOut = TIMEOUT)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
            yield break;
        }
        float runTime = Time.unscaledTime;
        UnityWebRequest request;

        if (string.Equals(method.ToUpper(), "POST"))
        {
            WWWForm form = new WWWForm();
            if (post != null && post.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in post)
                {
                    form.AddField(pair.Key, pair.Value);
                }
            }
            request = UnityWebRequest.Post(url, form);
        }
        else
        {
            if (post != null && post.Count > 0)
            {
                url += "?";
                foreach (KeyValuePair<string, string> pair in post)
                {
                    url += pair.Key + "=" + pair.Value + "&";
                }
                url = url.Substring(0, url.Length - 1);
            }
            request = UnityWebRequest.Get(url);
        }
        request.timeout = timeOut;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogWarning(string.Format("url:{0}-->post:{1},error:{2},runTime:{3}", url, post.GetString(), request.error, Time.unscaledTime - runTime));
            if (errorAction != null)
            {
                errorAction.Invoke();
            }
        }
        else
        {
            if (unityAction != null)
            {
                string text = request.downloadHandler.text;
                Debug.Log(string.Format("url:{0}-->post:{1},text:{2},runTime:{3}", url, post.GetString(), text, Time.unscaledTime - runTime));
                unityAction.Invoke(text);
            }
        }
    }
}

/// <summary>网络数据</summary>
public class URL
{
    public static string TIMEURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";

    public static string LOAD = "http://47.75.168.103:8882/load.do";
}