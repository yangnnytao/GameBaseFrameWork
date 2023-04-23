using System;
 

public class ExcelAttribute : CustomSerializableAttribute
{

    public ExcelAttribute(string excelFieldName) : base(excelFieldName)
    {


    }
    /// <summary>
    /// 根据 char 转换成 list
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    public ExcelAttribute(string excelFieldName, char splitChar) : base(excelFieldName, splitChar)
    {

    }
    /// <summary>
    /// 根据 char  获取索引值
    /// </summary>
    /// <param name="excelFieldName"></param>
    /// <param name="splitChar"></param>
    /// <param name="useIndex">索引从0开始</param>
    public ExcelAttribute(string excelFieldName, char splitChar, int useIndex) : base(excelFieldName, splitChar, useIndex)
    {

    }


}
