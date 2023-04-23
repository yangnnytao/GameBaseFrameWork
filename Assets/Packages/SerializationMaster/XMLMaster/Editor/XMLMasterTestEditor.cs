using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class XMLMasterTestEditor : Editor
{

    //[MenuItem("序列化工具/XML/案例测试")]
    //public static void Test()
    //{
    //    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetResources/Table/level.xml");
    //    Debug.Log(asset.text);
    //    var data = XMLSerializationMaster.Serialization<ExcelTestData>(asset.text, "Table");
    //    if (data == null) return;
    //    Debug.Log(data.Count);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        Debug.Log(data[i].ToString());
    //    }
    //}
    //[MenuItem("测试/双倍卡/任务双倍卡")]
    //public static void Test1()
    //{
    //    GetGoodPanel getGoodPanel = UIManager.CallPanel<GetGoodPanel>();
    //    getGoodPanel.SetPanel(new CommondResult((int)ResourceType.TaskDoubleCard));
    //}

    //[MenuItem("测试/双倍卡/清空任务双倍卡")]
    //public static void Test11()
    //{
    //   DoubleCardInfo cardInfo =  GameDataManager.PlayerData.doubleCardInfos.Find(x => x.type ==  DoubleCardType.task);
    //    if(cardInfo != null)
    //    {
    //        GameDataManager.PlayerData.doubleCardInfos.Remove(cardInfo);
    //    }
    //}

    //[MenuItem("测试/双倍卡/普通关卡双倍卡")]
    //public static void Test2()
    //{
    //    GetGoodPanel getGoodPanel = UIManager.CallPanel<GetGoodPanel>();
    //    getGoodPanel.SetPanel(new CommondResult((int)ResourceType.FightDoubleCard));
    //}
    //[MenuItem("测试/双倍卡/清空普通关卡双倍卡")]
    //public static void Test21()
    //{
    //    DoubleCardInfo cardInfo = GameDataManager.PlayerData.doubleCardInfos.Find(x => x.type == DoubleCardType.fight);
    //    if (cardInfo != null)
    //    {
    //        GameDataManager.PlayerData.doubleCardInfos.Remove(cardInfo);
    //    }
    //}

    //[MenuItem("测试/双倍卡/无尽关卡双倍卡")]
    //public static void Test3()
    //{
    //    GetGoodPanel getGoodPanel = UIManager.CallPanel<GetGoodPanel>();
    //    getGoodPanel.SetPanel(new CommondResult((int)ResourceType.endlessDoubleCard));
    //}

    //[MenuItem("测试/装备升级")]
    //public static void Test4()
    //{
    //    foreach (var item in TableManager.Instance.EquipmentDic)
    //    {
    //        for (int i = 1; i <=  item.Value.levelUpper; i++)
    //        {
    //            Debug.Log("装备id:" + item.Value.ID + ",等级：" + i + ",消耗的金币：" + item.Value.GetNeedMoney(i));
    //        }
    //    }
    //}

    //[MenuItem("测试/每日礼包过一天")]
    //public static void TestDayily()
    //{
    //    GameDataManager.PlayerData.dailyPackInfo.lastRefreshTime -= 24 * 3600 * 1000;
    //    GameDataManager.PlayerData.StorageLocal();
    //}
}
