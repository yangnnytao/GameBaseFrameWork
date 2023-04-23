using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerOfManager : Singleton<ManagerOfManager>
{
    public void InitAllManagerM()
    {
        TableDataManager.Instance.InitDataM();
        GameDataManager.Instance.InitDataM();
    }

    public void DestroyAllManagerM()
    {
        TableDataManager.Instance.DestroyM();
        GameDataManager.Instance.DestroyM();
    }
}
