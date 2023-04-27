using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class TableDataManager : Singleton<TableDataManager>
{

    //public static string XmlFilePath
    //{
    //    get
    //    {
    //        return Application.dataPath + "/GameAssets/Data/Table/TempExcel/";
    //    }
    //}

    ///// <summary> 时间配置表 </summary>
    public static List<RandomEventData> mRandomEventTableData = new List<RandomEventData>();
    public static List<LanguageData> mLanguageTableData = new List<LanguageData>();

    public override void InitDataM()
    {
        base.InitDataM();

        mRandomEventTableData = ConversionXml<RandomEventData>("RandomEventTable");//加载成就任务


        mLanguageTableData = ConversionXml<LanguageData>("LanguageTable");//加载成就任务


    }

    /// <summary> 解析XML </summary>
    public List<T> ConversionXml<T>(string xmlName)
    {
        return XMLSerializationMaster.Serialization<T>(LoadAssetXml(xmlName), "Table");
    }

    /// <summary> 加载文件 </summary>
    public static string LoadAssetXml(string xmlName)
    {
        return Common.GetGameAsset<TextAsset>(xmlName, Common.resPath_Xml).text;
        //return File.ReadAllText(XmlFilePath + xmlName + ".xml");
    }
}



