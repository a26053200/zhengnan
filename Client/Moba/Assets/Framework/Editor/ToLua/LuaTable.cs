//
// Lua table
// Author: zhengnan
// Create: 2018/6/12 14:28:27
// 

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class LuaTable
{
    public string rootName { get; private set; }
    
    private Dictionary<string, Dictionary<string, object>> hashTable;

    public LuaTable()
    {
        hashTable = new Dictionary<string, Dictionary<string, object>>();
    }
    public LuaTable(string rootName)
    {
        this.rootName = rootName;
        hashTable = new Dictionary<string, Dictionary<string, object>>();
    }

    public Dictionary<string, object> SetTable(string key)
    {
        Dictionary<string, object> tableLine = null;
        if(!hashTable.TryGetValue(key,out tableLine))
        {
            tableLine = new Dictionary<string, object>();
            hashTable.Add(key, tableLine);
        }
        return tableLine;
    }
    public bool HasTable(string key)
    {
        return hashTable.ContainsKey(key);
    }
    public Dictionary<string, object> GetTable(string key)
    {
        Dictionary<string, object> tableLine = null;
        hashTable.TryGetValue(key, out tableLine);
        return tableLine;
    }

    public void SetHashTable(string key, string hash, object value)
    {
        Dictionary<string, object> tableLine = GetTable(key);
        if(tableLine != null)
            tableLine[hash] = value;
        else
            Debug.LogError(string.Format("table {0} has not table line {1}!", rootName, key));
    }

    public object GetHashTable(string key, string hash)
    {
        object obj = null;
        Dictionary<string, object> tableLine = GetTable(key);
        if (tableLine != null)
            tableLine.TryGetValue(hash, out obj);
        else
            Debug.LogError(string.Format("table {0} has not table line {1}!", rootName, key));
        return obj;
    }

    public string GetString(string key, string hash)
    {
        object obj = GetHashTable(key, hash);
        return obj != null ? obj.ToString() : "";
    }

    public int GetInt(string key, string hash)
    {
        string value = GetString(key, hash);
        int i = 0;
        int.TryParse(value, out i);
        return i;
    }

    public void fromTextLine(string[] textLine)
    {
        string[] head = GetFileAndValue(textLine[0], '=');
        rootName = head[0];
        for(int i=1; i<textLine.Length; i++)
        {
            string line = textLine[i];
            Dictionary<string, object> tableLine = new Dictionary<string, object>();
            int keyStart = line.IndexOf(".");
            int keyEnd = line.IndexOf("=");
            string key = line.Substring(keyStart + 1, keyEnd - keyStart - 1).Trim();
            int start = line.IndexOf("{");
            int end = line.LastIndexOf("}");
            line = line.Substring(start + 1, end - start - 1);
            if(String.IsNullOrEmpty(line))
            {
                Debug.Log("Lua 行数据为空");
            }
            else
            {
                hashTable.Add(key, tableLine);
                string[] data = GetFileAndValue(line, ',');
                for (int j = 0; j < data.Length; j++)
                {
                    SetFiledValue(tableLine, data[j]);
                }
            }
        }
    }

    private string[] GetFileAndValue(string src,char sp)
    {
        string[] s = src.Split(sp);
        for (int i = 0; i < s.Length; i++)
        {
            if (i == 0 || i % 2 == 0)//filed
                s[i] = s[i].Trim();
        }
        return s;
    }
    private void SetFiledValue(Dictionary<string, object> tableLine,string line)
    {
        string[] fieldValue = GetFileAndValue(line, '=');
        string field = fieldValue[0].Trim();
        string temp = StringUtils.Trim(fieldValue[1].ToString());
        int i = int.MinValue;
        float f = float.MinValue;
        bool b = false;
        if (int.TryParse(temp, out i))
            tableLine.Add(field,i);
        else if (float.TryParse(temp, out f))
            tableLine.Add(field, f);
        else if (bool.TryParse(temp, out b))
            tableLine.Add(field, b);
        else
        {
            int start = fieldValue[1].IndexOf("\"");
            int end = fieldValue[1].LastIndexOf("\"");
            fieldValue[1] = fieldValue[1].Substring(start + 1, end - start - 1);
            tableLine.Add(field, fieldValue[1]);
        }
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        //Header
        sb.AppendLine(string.Format("{0}={{}}", rootName));
        foreach (var key in hashTable.Keys)
        {
            StringBuilder lineSb = new StringBuilder();
            Dictionary<string, object> tableLine = hashTable[key];
            foreach (var field in tableLine.Keys)
                AddFiled(lineSb, field, tableLine[field]);
            sb.AppendLine(string.Format("{0}.{1}={{{2}}}", rootName, key, lineSb.ToString()));
        }
        return sb.ToString();
    }

    private void AddFiled(StringBuilder lineSb, string field, object value)
    {
        string temp = StringUtils.Trim(value.ToString());
        int i = int.MinValue;
        float f = float.MinValue;
        bool b = false;
        if (int.TryParse(temp,out i))
            lineSb.Append(string.Format("{0}={1},", field, i));
        else if (float.TryParse(temp, out f))
            lineSb.Append(string.Format("{0}={1},", field, f));
        else if (bool.TryParse(temp, out b))
            lineSb.Append(string.Format("{0}={1},", field, b.ToString().ToLower()));
        else
            lineSb.Append(string.Format("{0}=\"{1}\",", field, value.ToString()));
    }
}

