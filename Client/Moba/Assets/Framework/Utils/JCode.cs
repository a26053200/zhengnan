using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;


public static class JCode
{
    //by chenliang
    //begin

    //更换JsonFx库为LitJson
    //增加LitJson自定义Json数据转换
    private static bool mIsInitLitJson = false;
    public static void InitLitJson()
    {
        if (mIsInitLitJson)
            return;
        mIsInitLitJson = true;

        __InitValueToString();
        __InitStringToValue();
        __InitValueToValue();
    }
    private static void __InitValueToString()
    {
        LitJson.JsonMapper.RegisterImporter<Int32, String>((Int32 jsonValue) =>
        {
            return jsonValue.ToString();
        });
        LitJson.JsonMapper.RegisterImporter<Double, String>((Double jsonValue) =>
        {
            return jsonValue.ToString();
        });
        LitJson.JsonMapper.RegisterImporter<long, String>((long jsonValue) =>
        {
            return jsonValue.ToString();
        });
        LitJson.JsonMapper.RegisterImporter<Boolean, String>((Boolean jsonValue) =>
        {
            return jsonValue.ToString();
        });
		LitJson.JsonMapper.RegisterImporter<Int64, String>((Int64 jsonValue) =>
		{
			return jsonValue.ToString();
		});
    }
    private static void __InitStringToValue()
    {
        LitJson.JsonMapper.RegisterImporter<String, Int32>((String jsonValue) =>
        {
            Int32 tmpValue = 0;
            if (!Int32.TryParse(jsonValue, out tmpValue))
                return 0;
            return tmpValue;
        });
        LitJson.JsonMapper.RegisterImporter<String, float>((String jsonValue) =>
        {
            float tmpValue = 0.0f;
            if (!float.TryParse(jsonValue, out tmpValue))
                return 0.0f;
            return tmpValue;
        });
        LitJson.JsonMapper.RegisterImporter<String, Double>((String jsonValue) =>
        {
            Double tmpValue = 0.0;
            if (!Double.TryParse(jsonValue, out tmpValue))
                return 0.0;
            return tmpValue;
        });
        LitJson.JsonMapper.RegisterImporter<String, long>((String jsonValue) =>
        {
            long tmpValue = 0;
            if (!long.TryParse(jsonValue, out tmpValue))
                return 0;
            return tmpValue;
        });
        LitJson.JsonMapper.RegisterImporter<String, Boolean>((String jsonValue) =>
        {
            if (jsonValue == "true")
                return true;
            return false;
        });
    }
    private static void __InitValueToValue()
    {
        LitJson.JsonMapper.RegisterImporter<Int32, Int64>((Int32 jsonValue) =>
        {
            Int64 tmpValue = (Int64)jsonValue;
            return tmpValue;
        });
		LitJson.JsonMapper.RegisterImporter<Double, Single>((Double jsonValue) =>
		{
			Single tmpValue = (Single)jsonValue;
			return tmpValue;
		});
    }

    //end
    //--------------------------------------------------------------------------------
    public static string Encode(object value)
    {
        //by chenliang
        //beign

//        return JsonWriter.Serialize(value);
//---------------
        return LitJson.JsonMapper.ToJson(value);

        //end
    }
    //--------------------------------------------------------------------------------
    //public static T Decode<T>(string context) where T : ICode
    public static T Decode<T>(string context)
    {
        //by chenliang
        //begin

//        return JsonReader.Deserialize<T>(context);
//---------------------
        return LitJson.JsonMapper.ToObject<T>(context);

        //end
    }
    //--------------------------------------------------------------------------------
    public static byte[] GetBytes(string data)
    {
        //return System.Text.Encoding.ASCII.GetBytes(data);
        return System.Text.Encoding.UTF8.GetBytes(data);
    }
    //--------------------------------------------------------------------------------
    public static string GetString(byte[] data)
    {
        //return System.Text.Encoding.ASCII.GetString(data);
        return System.Text.Encoding.UTF8.GetString(data);
    }
    public static string GetString(byte[] data,int length)
    {
        //return System.Text.Encoding.ASCII.GetString(data);
        return System.Text.Encoding.UTF8.GetString(data, 0, length);
    }
    public static Encoding GetBytesEncoding(byte[] bs)
    {
        int len = bs.Length;
        if (len >= 3 && bs[0] == 0xEF && bs[1] == 0xBB && bs[2] == 0xBF)
        {
            return Encoding.UTF8;
        }
        int[] cs = { 7, 5, 4, 3, 2, 1, 0, 6, 14, 30, 62, 126 };
        for (int i = 0; i < len; i++)
        {
            int bits = -1;
            for (int j = 0; j < 6; j++)
            {
                if (bs[i] >> cs[j] == cs[j + 6])
                {
                    bits = j;
                    break;
                }
            }
            if (bits == -1)
            {
                return Encoding.Default;
            }
            while (bits-- > 0)
            {
                i++;
                if (i == len || bs[i] >> 6 != 2)
                {
                    return Encoding.Default;
                }
            }
        }
        return Encoding.UTF8;
    }
    public static String ToFormart(String jsonStr)
    {
#if UNITY_EDITOR
        if (null == jsonStr || "".Equals(jsonStr)) return "";
        StringBuilder sb = new StringBuilder();
        char last = '\0';
        char current = '\0';
        int indent = 0;
        char[] charArr = jsonStr.ToCharArray();
        bool ignire = false;
        for (int i = 0; i < charArr.Length; i++)
        {
            last = current;
            current = charArr[i];
            switch (current)
            {
                case '"':
                    sb.Append(current);
                    ignire = !ignire;
                    break;
                case '{':
                case '[':
                    sb.Append(current);
                    sb.Append('\n');
                    indent++;
                    addIndentBlank(sb, indent);
                    break;
                case '}':
                case ']':
                    sb.Append('\n');
                    indent--;
                    addIndentBlank(sb, indent);
                    sb.Append(current);
                    break;
                case ',':
                    sb.Append(current);
                    if (!ignire && last != '\\')
                    {
                        sb.Append('\n');
                        addIndentBlank(sb, indent);
                    }
                    break;
                default:
                    sb.Append(current);
                    break;
            }
        }

        return sb.ToString();
#else
        return jsonStr;
#endif
    }
    private static void addIndentBlank(StringBuilder sb, int indent)
    {
        for (int i = 0; i < indent; i++)
        {
            sb.Append('\t');
        }
    }
}