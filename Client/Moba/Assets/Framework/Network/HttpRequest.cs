﻿using UnityEngine;
using System.Collections;
using LitJson;
//
// HttpRequest
// Author: zhengnan
// Create: 2018/6/7 22:06:20
// 
public class HttpRequest : MonoBehaviour
{
    public void SendMsg(string requestUrl, JsonData json, string callback)
    {
        string dataString = json.ToJson();
        StartCoroutine(Request(requestUrl, dataString, callback));
    }
    private IEnumerator Request(string requestUrl, string dataString, string callback)
    {
        string error = null;
        if (Global.EnableLogNetwork)
            MyDebug.Log(string.Format("[Http] {0}{1}", requestUrl, dataString));
        using (var www = new WWW(requestUrl, System.Text.Encoding.UTF8.GetBytes(dataString)))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                error = www.error;
            }
            else if (string.IsNullOrEmpty(www.text))
            {
                error = "Receive data is Empty.";
            }
            else
            {
                MyDebug.Log("[Http recv]" + www.text);
                JsonData netData = new JsonData(www.text);
                SendMessage(callback, netData);
                yield break;
            }
        }
        if (error == null)
        {
            error = "Empty responding contents";
        }
        MyDebug.LogError("HTTP POST Error! Cmd:" + callback + " Error:" + error);
        //MyDebug.Log("进入游戏失败！服务器停机或维护。");
    }
}

