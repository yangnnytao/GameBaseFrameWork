using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyException
{
    public static void AddException(string info, Exception e = null)
    {
#if UNITY_EDITOR
        Debug.LogError(info + "--->" + e);
#elif BUGLY
        if (e != null){
            BuglyAgent.ReportException(e, info);
            Debug.LogWarning(info + "--->" + e);
        }
        else
            Debug.LogError(info);
#endif
    }
}