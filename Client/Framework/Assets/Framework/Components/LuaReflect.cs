using System;
using System.Collections.Generic;
using LitJson;
using LuaInterface;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// <para></para>
    /// <para>Author: zhengnan </para>
    /// </summary> 
    public class LuaReflect : MonoBehaviour
    {
        [NoToLua]
        public Dictionary<string, string> keyValueMap { get; private set; }
        [NoToLua]
        public Dictionary<string, JsonData> jsonMap { get; private set; }
        [NoToLua]
        public Dictionary<string, List<JsonData>> jsonArrayMap { get; private set; }
        [NoToLua]
        public Dictionary<string, LuaFunction> luaFuncDict{ get; private set; }
        private void Awake()
        {
            jsonArrayMap = new Dictionary<string, List<JsonData>>();
            keyValueMap = new Dictionary<string, string>();
            jsonMap = new Dictionary<string, JsonData>();
            luaFuncDict = new Dictionary<string, LuaFunction>();
        }
        public void Push(string key, string value)
        {
            if (keyValueMap.ContainsKey(key))
                keyValueMap[key] = value;
            else
                keyValueMap.Add(key, value);
            Debug.Log("Push " + value);
        }
        
        public void PushLuaJson(string key, string json)
        {
            if (jsonMap.ContainsKey(key))
                jsonMap[key] = JsonMapper.ToObject(json);
            else
                jsonMap.Add(key, JsonMapper.ToObject(json));
            //Debug.Log($"LuaReflect PushLuaJson:{key} - {json}");
        }
        
        public void PushLuaJsonArray(string key, string json)
        {
            if (!jsonArrayMap.TryGetValue(key, out List<JsonData> list))
            {
                list = new List<JsonData>();
                jsonArrayMap.Add(key, list);
            }
            list.Add(JsonMapper.ToObject(json));
            //Debug.Log($"LuaReflect PushLuaJson:{key} - {json}");
        }
        public void PushLuaFunction(string key, LuaFunction func)
        {
            if (luaFuncDict.ContainsKey(key))
                luaFuncDict[key] = func;
            else
                luaFuncDict.Add(key, func);
            Debug.Log($"LuaReflect PushLuaFunction:{key} - {func.name}");
        }
        
        public void Dispose()
        {
            jsonMap.Clear();
        }
    }
}