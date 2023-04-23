using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;

public static class XMLSerializationMaster  {
    public static bool displayDEBUG
    {
        get
        {
            return SerializationTools.displayDEBUG;
        }
    }
    public static List<T> Serialization<T>(string xml,string rootNodeName) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        List<FieldExcelReflectionHandler> handlers = new List<FieldExcelReflectionHandler>();
        Type type = typeof(T);
        var fields = type.GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            CustomSerializableAttribute attribute = SerializationTools.GetAttribute<CustomSerializableAttribute>(fields[i]);
            if (attribute == null) continue;
            FieldExcelReflectionHandler handler = new FieldExcelReflectionHandler(fields[i], attribute, 0);
            handlers.Add(handler);
        }


        List<T> array = new List<T>();
        
        XmlNodeList nodeList = xmlDoc.SelectSingleNode(rootNodeName).ChildNodes;
        int index = 0;
        foreach (XmlElement node in nodeList) {
          if(displayDEBUG)  Debug.Log("序列化第" + index +"数据中--------------------------");
            var instance = System.Activator.CreateInstance<T>();

            bool haveField = false;
            for (int i = 0; i < handlers.Count; i++) {
                //从child中找寻名字跟handlerType一致的类
                foreach (XmlElement child in node.ChildNodes)
                {
                    if (child.Name == handlers[i].attribute.fieldName)
                    {
                        try
                        {
                            string value = child.InnerText;
                            OrginValueToField(handlers[i], value, instance, index);
                            haveField = true;
                        }
                        catch (System.Exception e) {
                             
                            Debug.LogError(string.Format("序列化表格出错！第{0}行数据出错,源数据：{1},目标标签名:{2},目标转换类型:{3},报错信息{4}" ,index,child.InnerText, handlers[i].attribute.fieldName,handlers[i].field.FieldType.FullName, e) );
                            
                            continue;
                        }
                        break;
                    }
                }

            }
            if (haveField)
            {
                array.Add(instance);
            }
            else {
                Debug.LogError(index+"行数据没有对应数据类型!");
            }
            index++;
        }

        GC.Collect();
        return array;
    }

    private static void OrginValueToField(FieldExcelReflectionHandler handler,string orginValue,object instance,int debugIndex = 0) {

        if (displayDEBUG) Debug.Log("源数据值:"+orginValue);
        if (orginValue.GetType() == typeof(System.DBNull)) return;
        if (displayDEBUG) Debug.Log(handler.attribute.fieldName+"|" + handler.field.Name +"|"+orginValue);

        if (orginValue is string && string.IsNullOrEmpty(orginValue)) return;
 

        CustomSerializableAttribute attribute = handler.attribute;
        if (!attribute.split)
        {
            object endValue = SerializationTools.ChangeValueType(handler.field, orginValue);
            //Debug.LogError("not find language id : " + orginValue);
            if (attribute is XMLAttribute)
            {
                int lanID = 0;
                if ((attribute as XMLAttribute).toLanguage && int.TryParse(orginValue,out lanID))
                {
                    //if (TableDataManager.Instance.runtimeLanguageTable.ContainsKey(int.Parse(orginValue)))
                    //{
                    //    //endValue = LanguageManager.Instance.currentLanguage == LanguageType.chinese ? language.chinese : language.english;
                    //    endValue = TableManager.Instance.runtimeLanguageTable[int.Parse(orginValue)].value;
                    //}
                    //else {
                    //    Debug.LogError("not find language id : " + orginValue);
                    //    Debug.LogError(handler.attribute.fieldName + "|" + handler.field.Name + "|" + orginValue);
                    //}
                }
            }

            handler.field.SetValue(instance, endValue);
        }
        else
        {
            if (attribute.toList)
            {
                string[] str = (orginValue as string).Split(attribute.splitChar);
                if(str.Length==0)return;
                if (str.Length == 1 && str[0] == "0") return;
                Type listType = handler.field.FieldType.GetGenericArguments()[0];
                object listInstance = Activator.CreateInstance(handler.field.FieldType);
                MethodInfo methodInfo = listInstance.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                for (int z = 0; z < str.Length; z++)
                {

                    //object value = listType.IsEnum ? Convert.ChangeType(str[z], typeof(int)) : Convert.ChangeType(str[z], listType);
                    object value = SerializationTools.ChangeValueType(listType, str[z]);
                    methodInfo.Invoke(listInstance, new object[] { value });
                }
                handler.field.SetValue(instance, listInstance);
            }
            else
            {
                string[] str = (orginValue as string).Split(attribute.splitChar);
                if (attribute.useIndex < 0 || attribute.useIndex >= str.Length)
                {
                    Debug.LogWarning("序列化" + handler.field.Name + "第" + debugIndex + "行的" + attribute.fieldName + "属性不满足" + attribute.useIndex + "的个数，检测表格或者代码，已跳过使用默认值"); return;
                }

                string splitValue = str[attribute.useIndex];
                object endValue = SerializationTools.ChangeValueType(handler.field, splitValue);
                handler.field.SetValue(instance, endValue);
            }

        }
    }



   
}
