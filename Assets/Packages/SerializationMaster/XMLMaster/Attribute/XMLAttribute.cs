using System;
using System.Collections;
using System.Collections.Generic;
 

public class XMLAttribute : CustomSerializableAttribute
{
    public bool toLanguage;
   
    public XMLAttribute(string excelFieldName) : base(excelFieldName)
    {


    }


    public XMLAttribute(string excelFieldName,bool toLanguage) : base(excelFieldName)
    {
        this.toLanguage = toLanguage;

    }
    /// <summary>
    /// 根据 char 转换成 list
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    public XMLAttribute(string excelFieldName, char splitChar) : base(excelFieldName, splitChar)
    {

    }
    /// <summary>
    /// 根据 char  获取索引值
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    /// <param name="useIndex">索引从0开始</param>
    public XMLAttribute(string excelFieldName, char splitChar, int useIndex) : base(excelFieldName, splitChar, useIndex)
    {

    }

}
