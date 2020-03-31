using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Framework;

//
// Class Introduce
// Author: zhengnan
// Create: 2018/5/31 22:37:55
// 
public abstract class ClientBase:MonoBehaviour
{
    public enum ClientStatus
    {
        None,               //没有连接
        Connecting,         //连接中
        ConnectedSuccess,   //连接成功
        Disconnected,       //连接断开
        ConnectTimeout,     //连接超时
    }
    public string host { get; protected set; }//服务器ip
    public int port { get; protected set; }//服务器端口
    public JsonSocket socket { get; private set; }
    public EventDispatch eventDispatcher { get; private set; }//事件派发器
    public ClientStatus status { get; private set; }//客户端状态

    public int ReconnectTimes = 20;

    public float ReconnectDelay = 5f;

    private bool _isReconnect;//是否是重新连接

    private Coroutine _connectCor;
    protected virtual void Awake()
    {
        //ReconnectTimes = GameConfig.ins.MaxConnectTimes == 0 ? 20 : GameConfig.ins.MaxConnectTimes;
        status = ClientStatus.None;
        eventDispatcher = new EventDispatch();

        socket = gameObject.AddComponent<JsonSocket>();
        socket.init();
        socket.OnReceive = OnReceiveHandler;
        socket.eventDispatcher.addEventListener(SocketEvent.ERROR, onConnectedError);
        socket.eventDispatcher.addEventListener(SocketEvent.DISCONNECTED, onDisconnect);
    }
    public void reconnect()
    {
        _isReconnect = true;
        connect();
    }
    public virtual void connect(string host, int port)
    {
        this.host = host;
        this.port = port;
        connect();
    }
    protected virtual void Update()
    {
        if (status == ClientStatus.ConnectedSuccess)
        {//已经连接
            //checkNetwork();//暂时关闭重新登录功能
            if (status == ClientStatus.Disconnected)
            {//重新连接
                _isReconnect = true;
                socket.isLoginSuccess = false;//重连后记录没有登录成功
                MyDebug.Log("[Socket] NetWork has disconnected, then reconnect");
                eventDispatcher.dispatchEvent(new SocketEvent(SocketEvent.DISCONNECTED));
                //SystemBusy.show();
                connect();
            }
        }
        else if (status == ClientStatus.ConnectTimeout)
        {
            MyDebug.LogError("登录游戏服务器失败。");//先通知断开，重连以后做
            status = ClientStatus.None;
        }
        else
        {//未连接
            if (socket.isConneted())
            {
                status = ClientStatus.ConnectedSuccess;
                onConnectedSuccess(null);
                _isReconnect = false;
            }
        }
    }
    //重新连接
    protected void connect()
    {
        //if (LoginModel.ins.serverKicking)
            //return;
        status = ClientStatus.Connecting;
        if (_connectCor == null)
            _connectCor = StartCoroutine(doConnectCo());
    }
    protected IEnumerator doConnectCo()
    {
        int counter = 0;
        while (counter <= ReconnectTimes)
        {
            //if (LoginModel.ins.serverKicking)
                //break;
            counter++;
            MyDebug.Log(string.Format("[Socket] connecting to {0}:{1} in {2}/{3}", host, port, counter, ReconnectTimes));//第一次重连
            socket.connect(host, port);
            yield return new WaitForSeconds(ReconnectDelay);
            if (socket.status == NetStatus.Connected)
                break;
        }
        if (counter > ReconnectTimes)
        {//连接超时
            status = ClientStatus.ConnectTimeout;
        }
        StopCoroutine(_connectCor);
        _connectCor = null;
    }
    //网络检测
    protected void checkNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {//没有网络连接，无法进行游戏
            MyDebug.LogError("[Socket] client has bab network! it will reconnected.");
            if (socket.isConneted())
                socket.disconnect();
            status = ClientStatus.Disconnected;//连接已经断开
        }
        else if (socket.status == NetStatus.Disconnected)
        {
            MyDebug.LogError("[Socket] NetStatus is Disconnected! it will reconnected.");
            status = ClientStatus.Disconnected;//连接已经断开
        }
    }
    protected virtual void OnReceiveHandler(IResponse response)
    {

    }
    private void onDisconnect(EventObj evt)
    {
        //AlertManager.alert("登录游戏服务器失败。", "确定", Client.QuitGame);//先通知断开，重连以后做
        status = ClientStatus.Disconnected;//连接已经断开
    }
    private void onConnectedError(EventObj evt)
    {
        eventDispatcher.dispatchEvent(new JsonSocketEvent(JsonSocketEvent.SERVER_SOCKET_FAIL));
    }
    private void onConnectedSuccess(EventObj evt)
    {
        eventDispatcher.dispatchEvent(new JsonSocketEvent(JsonSocketEvent.SERVER_SOCKET_CONNECTED, _isReconnect));
    }
    public virtual void disconnect()
    {
        socket.disconnect();
    }
    public virtual void dispose()
    {
        socket.OnReceive = null;
        socket.eventDispatcher.removeEeventListener(SocketEvent.ERROR, onConnectedError);
        socket.eventDispatcher.removeEeventListener(SocketEvent.DISCONNECTED, onDisconnect);
        socket.dispose();
    }
}

