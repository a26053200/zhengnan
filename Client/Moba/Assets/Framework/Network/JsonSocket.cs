
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


public class JsonSocket : SocketBase
{
    public const int MAX_PACK_LEN = 12288;

    public const int MAX_SEND = 2048;//缓冲区最大容量

    public delegate void OnReceiveHandler(int cmd, StringBuilder json);

    public OnReceiveHandler OnReceive;//接收数据回调

    private List<byte[]> _tmpDatas = new List<byte[]>();
    private CommandReader _reader;
    private string start = "\"cmd\":";
    private string end = "";
    private string key = "cmd";
    private Regex reg;
    public override void init()
    {
        MAX_READ = MAX_PACK_LEN;
        base.init();

        reg = new Regex("^.*" + start + "(?<" + key + ">[0-9]+)" + end + ".*$"); /* ^.* 以任意N个字符开头，一直到 \"cmd\": 提取后面纯数字的东西 **/
        _reader = new CommandReader();

    }
    protected override void doOnReceive(byte[] bytes, int length)
    {
        _tmpDatas.Clear();
        _reader.decode(bytes, length, _tmpDatas);
        for (int i = 0; i < _tmpDatas.Count; i++)
        {
            byte[] data = _tmpDatas[i];
            if (data != null && data.Length > 0)
            {
                string json = JCode.GetString(data, data.Length);
                StringBuilder sb = new StringBuilder(json);
                int cmd = int.Parse(getPT(sb));
                OnReceive(cmd, sb);
            }
        }
    }
    public void send(object data)
    {
        string json = JCode.Encode(data);
        StartCoroutine(doSendCo(json));
    }
    public void sendNoLogin(object data)
    {//没有登陆时用来发指令
        string json = JCode.Encode(data);
        send(json, false);
    }
    protected IEnumerator doSendCo(string json)
    {
        if (!sendComplete)
            yield return null;
        send(json, false);
    }
    public void send(string json,bool isLua = false)
    {
        if (!isConneted())
        {
            if (Global.EnableLogNetwork) MyDebug.LogError("[Socket] can't send msg. Socket is not connect.");
            return;
        }
        if (Global.EnableLogNetwork)
        {
            int cmd = getPT(json);
            MyDebug.Log(string.Format("[Socket] <color=#df5c4aff>send json</color>:{0}\t{1}\n{2}", cmd, JCode.ToFormart(json)));
        }
        byte[] dataBytes = JCode.GetBytes(json);
        byte[] sendByteBuff = new byte[dataBytes.Length + 4];
        byte[] lenBytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(dataBytes.Length));
        Array.Copy(lenBytes, sendByteBuff, lenBytes.Length);
        Array.Copy(dataBytes, 0, sendByteBuff, lenBytes.Length, dataBytes.Length);
        send(sendByteBuff, MAX_SEND);
    }
    //提取协议号
    public string getPT(StringBuilder sb)
    {
        Match match = reg.Match(sb.ToString());
        return match.Groups[key].Value;
    }
    public int getPT(string sb)
    {
        Match match = reg.Match(sb);
        return int.Parse(match.Groups[key].Value);
    }
}
