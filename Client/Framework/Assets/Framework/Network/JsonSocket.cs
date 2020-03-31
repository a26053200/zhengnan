using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Framework;
using MiscUtil.IO;


public class JsonSocket : SocketBase
{
    public const int MAX_PACK_LEN = 12288;

    public const int MAX_SEND = 2048;//缓冲区最大容量

    public delegate void OnReceiveHandler(IResponse response);

    public OnReceiveHandler OnReceive;//接收数据回调

    public bool isLoginSuccess = true; //是否重新登录成功,默认是登录成功的

    public bool isCompress = true;//默认使用压缩
    
    public Stack<IResponse> responseStack = new Stack<IResponse>();
    
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
        _reader.decode(bytes, length, OnResponse);
    }

    protected void OnResponse(EndianBinaryReader reader, int dataLen)
    {
        if(responseStack.Count == 0)
            responseStack.Push(new ReignResponse());
        var response = responseStack.Pop();
        response.Decode(reader, dataLen, isCompress);
        OnReceive(response);
    }
    public void send(JsonData data)
    {
        string json = data.ToJson();
        if (GlobalConsts.EnableLogNetwork)
        {
            //int cmd = getPT(json);
            Logger.Info(string.Format("[Socket] <color=#df5c4aff>send json</color>:{0}", json));
        }
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
            if (GlobalConsts.EnableLogNetwork) Logger.LogError("[Socket] can't send msg. Socket is not connect.");
            return;
        }
        
        byte[] dataBytes = JCode.GetBytes(json);
        //Array.Reverse(dataBytes);
        byte[] sendByteBuff = new byte[dataBytes.Length + 4];
        byte[] lenBytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(dataBytes.Length));
        Array.Copy(lenBytes, sendByteBuff, lenBytes.Length);
        Array.Copy(dataBytes, 0, sendByteBuff, lenBytes.Length, dataBytes.Length);
        send(sendByteBuff, MAX_SEND);
    }
    
    public void Send(IRequest rqst)
    {
        byte[] bytes = rqst.GetBytes();
//        if (GlobalConsts.EnableLogNetwork)
//        {
//            Logger.Info(string.Format("[Socket] <color=#df5c4aff>send request. len</color>:{0}", bytes.Length));
//        }
        StartCoroutine(DoSendCo(bytes));
    }
    
    protected IEnumerator DoSendCo(byte[] sendByteBuff)
    {
        if (!sendComplete)
            yield return null;
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
