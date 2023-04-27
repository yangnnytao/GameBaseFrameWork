using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public static class PersistentMaster
{
    public static string persistentPath = Application.persistentDataPath;  //默认保存路径


    public static void Save(string headName, string data, bool debug = false)
    {
        if (!System.IO.Directory.Exists(persistentPath))
        {
            Directory.CreateDirectory(persistentPath);
        }
        string filePath = Path.Combine(persistentPath, headName + ".dat");
        File.WriteAllText(filePath, data);
#if UNITY_EDITOR
        debug = true;
#endif
        if (debug) Debug.Log("write data ,path:" + filePath);
    }

    public static void Save(string headName, byte[] data, bool debug = false)
    {
        if (!System.IO.Directory.Exists(persistentPath))
        {
            Directory.CreateDirectory(persistentPath);
        }
        string filePath = Path.Combine(persistentPath, headName + ".bdat");
        File.WriteAllBytes(filePath, data);
        if (debug) Debug.Log("write data ,path:" + filePath);
    }
    public static byte[] GetByteData(string headName)
    {
        string filePath = Path.Combine(persistentPath, headName + ".bdat");
        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        return null;
    }

    public static string GetData(string headName)
    {
        string filePath = Path.Combine(persistentPath, headName + ".dat");
        if (File.Exists(filePath))
            return File.ReadAllText(filePath);
        return "";
    }

    public static void Delete(string headName)
    {
        File.Delete(Path.Combine(persistentPath, headName + ".dat"));
    }

    public static void ClearAllData()
    {
        var files = Directory.GetFiles(persistentPath);
        for (int i = 0; i < files.Length; i++)
        {
            string ext = Path.GetExtension(files[i]);
            if (ext.Equals(".dat") || ext.Equals(".bdat"))
            {
                File.Delete(Path.Combine(persistentPath, files[i]));
            }
        }
    }

    public static void SaveJSON(object t, string headName, bool debug = false)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(t);
            Save(headName, jsonData, debug);
        }
        catch (Exception e)
        {
            Debug.LogError("保存-" + headName + "-数据失败！：" + e);
        }
    }

    public static T GetJSONData<T>(string headName)
    {
        try
        {
            Debug.Log("GetJSONData---Start---");
            string jsonData = GetData(headName);
            if (string.IsNullOrEmpty(jsonData)) return default(T);
            SimpleJSON.JSONObject tempObj = SimpleJSON.JSON.Parse(jsonData).AsObject;
            ChackOldVersionData(ref tempObj);
            Debug.Log("GetJSONData---End---" + jsonData);
            return JsonConvert.DeserializeObject<T>(tempObj.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError("读取失败----" + e + "|" + headName);
            return default(T);
        }
    }

    private static void ChackOldVersionData(ref SimpleJSON.JSONObject obj_)
    {
        //检查现金
        if (!obj_.HasKey("mCash"))
        {
            Debug.Log(obj_.GetValueOrDefault("mCash", 0));
            //obj_.Add("mCash", 1);
        }
        //检查金币
        if (!obj_.HasKey("mScore"))
        {
            Debug.Log(obj_.GetValueOrDefault("mScore", 0));
        }
    }
}
