using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SocketEvent : EventObj
{
    /// <summary>
    /// 连接失败
    /// </summary>
    public const string ERROR = "error";
    /// <summary>
    /// 连接断开
    /// </summary>
    public const string DISCONNECTED = "disconnected";
    /// <summary>
    /// 读取数据失败
    /// </summary>
    public const string READ_ERROR = "read error";
    
    public SocketEvent(string type)
        : base(type)
    {

    }
    
}
public class JsonSocketEvent : EventObj
{
    /// <summary>
    /// 服务器连接成功
    /// </summary>
    public const string SERVER_SOCKET_CONNECTED = "json server socket connected";
    /// <summary>
    /// 服务器连接 失败
    /// </summary>
    public const string SERVER_SOCKET_FAIL = "json server socket fail";

    public JsonSocketEvent(string type)
        : base(type)
    {

    }
    public bool reConnect { get; private set; }

    public JsonSocketEvent(string type, bool reConnect)
        : base(type)
    {
        this.reConnect = reConnect;
    }
   
}
