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
    /// <summary>
    /// 服务器连接成功
    /// </summary>
    public const string SERVER_SOCKET_CONNECTED = "server socket connected";
    /// <summary>
    /// 服务器登陆成功
    /// </summary>
    public const string SERVER_LOGIN_SUCCESS = "mmo server login success";
    /// <summary>
    /// 服务器登陆失败
    /// </summary>
    public const string SERVER_LOGIN_FAIL = "mmo server login fail";
    /// <summary>
    /// 服务器进入成功
    /// </summary>
    public const string SERVER_ENTER_SUCCESS = "mmo server enter success";
    /// <summary>
    /// 服务器进入失败
    /// </summary>
    public const string SERVER_ENTER_FAIL = "mmo server enter fail";
    /// <summary>
    /// 服务器初始化成功
    /// </summary>
    public const string SERVER_INIT_SUCCESS = "mmo server init success";
    /// <summary>
    /// 服务器初始化失败
    /// </summary>
    public const string SERVER_INIT_FAIL = "mmo server init fail";
    /// <summary>
    /// 踢下线
    /// </summary>
    public const string MMO_KICK_PLAYER = "mmo kick player";
    public SocketEvent(string type)
        : base(type)
    {

    }
    public bool reConnect { get; private set; }
    public int kickCode { get; private set; }
    public SocketEvent(string type,bool reConnect)
        : base(type)
    {
        this.reConnect = reConnect;
    }
    public SocketEvent(string type, int kickCode)
        : base(type)
    {
        this.kickCode = kickCode;
    }
}
