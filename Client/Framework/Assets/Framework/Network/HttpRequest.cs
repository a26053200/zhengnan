using UnityEngine;
using System.Collections;
using LitJson;
using System.Text;
//
// HttpRequest
// Author: zhengnan
// Create: 2018/6/7 22:06:20
// 
public class HttpRequest : MonoBehaviour
{
    public delegate void OnHttpRspd(JsonData netData);
    public void SendMsg(string requestUrl, JsonData json, OnHttpRspd callback)
    {
        string dataString = json.ToJson();
        StartCoroutine(Request(requestUrl, dataString, callback));
    }
    private IEnumerator Request(string requestUrl, string dataString, OnHttpRspd callback)
    {
        string error = null;
//        if (GlobalConsts.EnableLogNetwork)
//            MyDebug.Log(string.Format("[Http] {0}/{1}", requestUrl, dataString));
        using (var www = new WWW(requestUrl, Encoding.UTF8.GetBytes(dataString)))
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
                //去掉结尾的 '\0' 字符 要不然会解析不出json  这个查了很多资料 
                //最终通过打印2进制数组一个一个字节对比才发现的 - -!
                //string json = Encoding.UTF8.GetString(www.bytes,0, www.bytes.Length - 1);
                string json = Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length);
                //MyDebug.Log("[Http recv] " + json);
                JsonData netData = JsonMapper.ToObject(json);
                //SendMessage(callback, netData);
                callback.Invoke(netData);
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

