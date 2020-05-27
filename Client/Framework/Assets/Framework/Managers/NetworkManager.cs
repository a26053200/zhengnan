using System.Collections.Generic;
using Framework;
using LitJson;
using LuaInterface;

/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/7/9 22:20:39</para>
/// </summary> 
public class NetworkManager : BaseManager
{
    protected JsonClient jsonClient;

    protected HttpRequest httpRequest;

    protected Logger logger { get; private set; }

    public Dictionary<string, LuaFunction> luaFun = new Dictionary<string, LuaFunction>();

    protected LuaFunction httpCallback;

    protected LuaFunction jsonRecvCallback;

    private void Awake()
    {
        httpRequest = gameObject.AddComponent<HttpRequest>();
        jsonClient = gameObject.AddComponent<JsonClient>();
        jsonClient.eventDispatcher.addEventListener(JsonSocketEvent.SERVER_SOCKET_CONNECTED, OnSocketConnect);
        jsonClient.eventDispatcher.addEventListener(JsonSocketEvent.SERVER_SOCKET_FAIL, OnSocketConnectFail);
    }

    public void SetCompress(bool isCompress)
    {
        jsonClient.socket.isCompress = isCompress;
    }
    public void SetLuaFun(string funName, LuaFunction func)
    {
        luaFun[funName] = func;
    }

    public void HttpRequest(string url, string json, LuaFunction func)
    {
        JsonData netData = JsonMapper.ToObject(json);
        httpRequest.SendMsg(url, netData, delegate(JsonData data)
        {
            func.BeginPCall();
            func.Push(data.ToJson());
            func.PCall();
            func.EndPCall();
        });
    }

    public void SendJson(string json)
    {
        JsonData netData = JsonMapper.ToObject(json);
        jsonClient.SendJson(netData);
    }
    
    public void SendRequest(IRequest rqst)
    {
        jsonClient.Send(rqst);
    }
    
    void OnHttpRspd(JsonData json)
    {
        luaFun[NetworkFunction.OnHttpRspd].Call(json.ToJson());
    }
    public void Connect(string host, int port)
    {
        jsonClient.connect(host, port);
        jsonClient.SetJsonCallback(OnJsonRspd);
    }

    private void OnJsonRspd(ReignResponse response)
    {
        luaFun[NetworkFunction.OnJsonRspd].BeginPCall();
        luaFun[NetworkFunction.OnJsonRspd].Push(response);
        luaFun[NetworkFunction.OnJsonRspd].PCall();
        luaFun[NetworkFunction.OnJsonRspd].EndPCall();
    }

    private void OnSocketConnect(EventObj evt)
    {
        JsonSocketEvent jevt = evt as JsonSocketEvent;
        if(jevt.reConnect)
            luaFun[NetworkFunction.OnReConnect].Call();
        else
            luaFun[NetworkFunction.OnConnect].Call();
    }

    private void OnSocketConnectFail(EventObj evt)
    {
        luaFun[NetworkFunction.OnConnectFail].Call();
    }

    void OnDestroy()
    {
        jsonClient.eventDispatcher.removeEeventListener(JsonSocketEvent.SERVER_SOCKET_CONNECTED, OnSocketConnect);
        jsonClient.eventDispatcher.removeEeventListener(JsonSocketEvent.SERVER_SOCKET_FAIL, OnSocketConnectFail);
    }
}

