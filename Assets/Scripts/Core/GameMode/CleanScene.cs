using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using BaseFrame;

public class CleanScene : MonoBehaviour
{
    //异步对象
    private AsyncOperation async;

    //下一个场景的名称
    private static string _nextSceneName;

    void Awake()
    {
        Object[] objAry = Resources.FindObjectsOfTypeAll<Material>();

        for (int i = 0; i < objAry.Length; ++i)
        {
            objAry[i] = null;//解除资源的引用
        }
          
        Object[] objAry2 = Resources.FindObjectsOfTypeAll<Texture>();

        for (int i = 0; i < objAry2.Length; ++i)
        {
            objAry2[i] = null;
        }

        //卸载没有被引用的资源
        Resources.UnloadUnusedAssets();

        //立即进行垃圾回收
        GC.Collect();
        GC.WaitForPendingFinalizers();//挂起当前线程，直到处理终结器队列的线程清空该队列为止
        GC.Collect();

    }

    void Start()
    {
        Common.StartCoroutine(SingleSceneManager.Instance.LoadSceneAsync(_nextSceneName, null));
    }

    /// <summary>
    /// 静态方法，直接切换到ClearScene，此脚本挂在ClearScene场景下
    /// </summary>
    public static void LoadScene(string nextSceneName_)
    {
        _nextSceneName = nextSceneName_;
        SceneManager.LoadScene("CleanScene");
    }

}