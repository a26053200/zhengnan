using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Web;
using System.ComponentModel;
using System.Reflection;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/11 1:23:04</para>
/// </summary> 
public static class StringExtensions
{
    /*
    public static void BindEnumList(this CheckBoxList ddl, Type obj)
    {
        if (!obj.IsEnum)
            throw new Exception("value not enum!");

        var itemArr = Enum.GetValues(obj);

        foreach (var item in itemArr)
        {
            ddl.Items.Add(new ListItem(item.ToString(), ((int)item).ToString()));
        }
    }


    public static void BindEnumList(this DropDownList ddl, Type obj)
    {
        if (!obj.IsEnum)
            throw new Exception("value not enum!");

        var itemArr = Enum.GetValues(obj);

        foreach (var item in itemArr)
        {
            ddl.Items.Add(new ListItem(item.ToString(), ((int)item).ToString()));
        }
    }


    public static void BindEnumDescriptionList(this DropDownList ddl, Type obj)
    {
        if (!obj.IsEnum)
        {
            throw new ArgumentException("enumItem requires a Enum ");
        }

        var itemArr = Enum.GetValues(obj);
        string[] names = Enum.GetNames(obj);
        FieldInfo fieldInfo;
        object[] attributes;
        DescriptionAttribute descriptionAttribute;


        foreach (string name in names)
        {
            fieldInfo = obj.GetField(name);
            attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var value = (int)fieldInfo.GetValue(typeof(string));
            if (attributes.Length > 0)
            {
                descriptionAttribute = attributes.First() as DescriptionAttribute;
                if (descriptionAttribute != null)
                {
                    ddl.Items.Add(new ListItem(descriptionAttribute.Description, value.ToString()));
                }
            }
        }
    }
    */


    public static int ToInt(this string value)
    {
        return Int32.Parse(value);
    }



    public static int ToInt(this string value, int defaultValue)
    {
        var result = defaultValue;
        return int.TryParse(value, out result) ? result : defaultValue;
    }

    public static int? ToNullableInt(this string value)
    {
        int result;

        if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
        {
            return null;
        }

        return result;
    }

    public static decimal ToDecimal(this string value)
    {
        return decimal.Parse(value);
    }

    public static decimal ToDecimal(this string value, decimal defaultValue)
    {
        var result = defaultValue;
        return decimal.TryParse(value, out result) ? result : defaultValue;
    }

    public static decimal ToRoundDecimal(this string value, decimal defaultValue, int decimals)
    {
        var result = defaultValue;
        result = Math.Round(decimal.TryParse(value, out result) ? result : defaultValue, decimals);
        return result;
    }


    public static decimal? ToNullableDecimal(this string value)
    {
        decimal result;
        if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out result))
        {
            return null;
        }
        return result;
    }

    public static short? ToNullableShort(this string value)
    {
        short result;

        if (string.IsNullOrEmpty(value) || !short.TryParse(value, out result))
        {
            return null;
        }

        return result;
    }

    public static DateTime? ToNullableDateTime(this string value)
    {
        DateTime result;

        if (DateTime.TryParse(value, out result))
        {
            return result;
        }

        return null;
    }

    public static DateTime ToDateTime(this string value)
    {
        return DateTime.Parse(value);
    }

    public static byte? ToNullableByte(this string value)
    {
        byte result;

        if (string.IsNullOrEmpty(value) || !byte.TryParse(value, out result))
        {
            return null;
        }

        return result;
    }


    public static bool? ToNullableBool(this string value)
    {
        bool result;

        if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out result))
        {
            return null;
        }

        return result;
    }

    public static bool ToBool(this string value)
    {
        return bool.Parse(value);
    }

    public static string[] SplitWithString(string sourceString, string splitString)
    {

        List<string> arrayList = new List<string>();
        string s = string.Empty;
        while (sourceString.IndexOf(splitString) > -1)
        {
            s = sourceString.Substring(0, sourceString.IndexOf(splitString));
            sourceString = sourceString.Substring(sourceString.IndexOf(splitString) + splitString.Length);
            arrayList.Add(s);
        }
        arrayList.Add(sourceString);
        return arrayList.ToArray();
    }
    /// <summary>
    /// 去掉字符串中的html
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    //public static string ToNoHtmlString(this string value)
    //{
    //return Util.StripHTML(value).Trim();
    //}



}

