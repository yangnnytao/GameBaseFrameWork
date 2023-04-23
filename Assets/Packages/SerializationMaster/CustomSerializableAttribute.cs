using System;
using System.Collections;
using System.Collections.Generic;
 

public class CustomSerializableAttribute : Attribute
{

    public string fieldName;
    public bool split;
    public char splitChar; //拆分字符
    public bool toList;//是否转换成List
    public int useIndex; //如果不转换成字符 则获取改索引的值


    public CustomSerializableAttribute(string excelFieldName)
    {
        this.fieldName = excelFieldName;
        split = false;

    }


    /// <summary>
    /// 根据 char 转换成 list
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    public CustomSerializableAttribute(string excelFieldName, char splitChar)
    {
        this.fieldName = excelFieldName;
        split = true;
        this.splitChar = splitChar;
        this.toList = true;
    }

    /// <summary>
    /// 根据 char  获取索引值
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    /// <param name="useIndex">索引从0开始</param>
    public CustomSerializableAttribute(string excelFieldName, char splitChar, int useIndex)
    {
        this.fieldName = excelFieldName;
        split = true;
        this.splitChar = splitChar;
        this.toList = false;
        this.useIndex = useIndex;
    }
}
