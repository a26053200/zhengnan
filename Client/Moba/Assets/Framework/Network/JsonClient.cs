using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
//
// Class Introduce
// Author: zhengnan
// Create: 2018/5/31 23:01:28
// 
public class JsonClient :  ClientBase
{
    public delegate void CmdHandler(NetData data);
    public delegate void CmdJsonHandler(string json);

    protected Dictionary<int, int> _errorMessages;

    protected Dictionary<NetCmd, RspdType> _rspdTypeMap;

    protected Dictionary<NetCmd, List<CmdHandler>> _rspdListMap;

    protected Dictionary<int, List<CmdJsonHandler>> _rspdJsonListMap;

    protected Dictionary<NetCmd, CmdMethodInfo> _cmdMethodInfoListMap;

    protected Queue<RcvData> _rcvDataQueue; //接收到的数据队列

    protected MethodInfo JsonCodeMethod;

    protected override void Awake()
    {
        base.Awake();
        _errorMessages = new Dictionary<int, int>();
        _rspdTypeMap = new Dictionary<NetCmd, RspdType>();
        _rspdListMap = new Dictionary<NetCmd, List<CmdHandler>>();
        _rspdJsonListMap = new Dictionary<int, List<CmdJsonHandler>>();
        _cmdMethodInfoListMap = new Dictionary<NetCmd, CmdMethodInfo>();
        _rcvDataQueue = new Queue<RcvData>();

        JsonCodeMethod = typeof(JCode).GetMethod("Decode", BindingFlags.Static | BindingFlags.Public);
    }
    protected override void Update()
    {
        base.Update();
        while (_rcvDataQueue.Count > 0)
        {
            RcvData rcvData = _rcvDataQueue.Dequeue();
            doReceive(rcvData);
        }
    }
    public void regRspd(NetCmd pactetId, Type type)
    {
        RspdType rspdType = null;
        if (!_rspdTypeMap.ContainsKey(pactetId))
        {
            MethodInfo method = JsonCodeMethod.MakeGenericMethod(type);
            rspdType = new RspdType(type, method);
            _rspdTypeMap.Add(pactetId, rspdType);
        }
    }
    public void delRspd(NetCmd pactetId)
    {
        if (_rspdTypeMap.ContainsKey(pactetId))
            _rspdTypeMap.Remove(pactetId);
    }
    public void addSocketListener(NetCmd packetId, CmdHandler handler)
    {
        List<CmdHandler> rspdList = null;
        if (!_rspdListMap.TryGetValue(packetId, out rspdList))
        {
            rspdList = new List<CmdHandler>();
            _rspdListMap.Add(packetId, rspdList);
        }
        rspdList.Add(handler);
    }
    public void removeSocketListener(NetCmd packetId, CmdHandler handler)
    {
        List<CmdHandler> rspdList = null;
        if (_rspdListMap.TryGetValue(packetId, out rspdList))
            if (rspdList.Contains(handler))
                rspdList.Remove(handler);
    }
    /*
    public void addServiceListener(NetCmd cmd, BaseService service, MethodInfo method, Type type)
    {
        CmdMethodInfo cmdMethod = null;
        if (!_cmdMethodInfoListMap.TryGetValue(cmd, out cmdMethod))
        {
            cmdMethod = new CmdMethodInfo();
            _cmdMethodInfoListMap.Add(cmd, cmdMethod);
        }
        regRspd(cmd, type);
        cmdMethod.methodList.Add(method);
        cmdMethod.serviceList.Add(service);
    }
    public void removeServiceListener(NetCmd cmd, MethodInfo method)
    {
        CmdMethodInfo serviceType = null;
        if (_cmdMethodInfoListMap.TryGetValue(cmd, out serviceType))
            if (serviceType.methodList.Contains(method))
                serviceType.methodList.Remove(method);
    }
    */
    public void addJsonListener(int cmd, CmdJsonHandler handler)
    {
        List<CmdJsonHandler> rspdList = null;
        if (!_rspdJsonListMap.TryGetValue(cmd, out rspdList))
        {
            rspdList = new List<CmdJsonHandler>();
            _rspdJsonListMap.Add(cmd, rspdList);
        }
        rspdList.Add(handler);
    }
    public void removeJsonListener(int cmd, CmdJsonHandler handler)
    {
        List<CmdJsonHandler> rspdList = null;
        if (!_rspdJsonListMap.TryGetValue(cmd, out rspdList))
            if (rspdList.Contains(handler))
                rspdList.Remove(handler);
    }
    public void send(NetData obj)
    {
        socket.send(obj);
    }
    public void send(object obj)
    {
        socket.send(obj);
    }
    public void send(string json)
    {
        socket.send(json, true);
    }
    protected override void OnReceiveHandler(int cmdId, StringBuilder json)
    {
        base.OnReceiveHandler(cmdId, json);
#if UNITY_EDITOR
        //showErrorCodeTip(cmdId);
#endif
        if (!_rspdTypeMap.ContainsKey((NetCmd)cmdId) && !_rspdJsonListMap.ContainsKey(cmdId) && !_cmdMethodInfoListMap.ContainsKey((NetCmd)cmdId))
        {
            MyDebug.LogError(string.Format("[Socket] Receive command has not regist! NetCmd: {0}", cmdId));
        }
        else
        {
            //object obj = System.Activator.CreateInstance(rspdType);
            //MyDebug.Log("收到数据 NetCmd:" + " " + cmdId);
            RcvData rcvData = new RcvData(cmdId, json);
            _rcvDataQueue.Enqueue(rcvData);//进入消息处理队列
        }
    }
    private void doReceive(RcvData rcvData)
    {
        if (Global.EnableLogNetwork)
        {
#if UNITY_EDITOR
            if (rcvData.cmdId != 19999)
                MyDebug.Log(string.Format("[Socket] <color=#1fac75ff>recv json</color>:{0}\t{1}\n{2}", rcvData.cmdId, (NetCmd)rcvData.cmdId, JCode.ToFormart(rcvData.json.ToString())));
#else
            if (rcvData.cmdId != 19999) 
                MyDebug.Log(string.Format("[Socket] recv json:{0}\t{1}\n{2}", rcvData.cmdId, (NetCmd)rcvData.cmdId, JCode.ToFormart(rcvData.json.ToString())));
#endif
        }
        //默认监听对象
        RspdType rspdType = null;
        NetData obj = null;
        if (_rspdTypeMap.TryGetValue((NetCmd)rcvData.cmdId, out rspdType))
        {
            obj = (NetData)rspdType.method.Invoke(null, new object[1] { rcvData.json.ToString() });
            CmdMethodInfo cmdMethod = null;
            if (_cmdMethodInfoListMap.TryGetValue((NetCmd)rcvData.cmdId, out cmdMethod))
            {
                for (int i = 0; i < cmdMethod.methodList.Count; i++)
                {
                    //cmdMethod.methodList[i].Invoke(cmdMethod.serviceList[i], new object[] { obj });
                }
            }
        }
        if (obj != null)
        {
            List<CmdHandler> rspdList = null;
            if (_rspdListMap.TryGetValue((NetCmd)rcvData.cmdId, out rspdList))
            {
                List<CmdHandler> tempList = new List<CmdHandler>(rspdList.ToArray());
                for (int i = 0; i < tempList.Count; i++)
                {
                    tempList[i](obj as NetData);
                }
            }
        }
        else
        {
            MyDebug.LogError(string.Format("json 类型转换失败 cmd:{0}", (NetCmd)rcvData.cmdId));
        }
        //Lua 单独的 json 监听对象
        List<CmdJsonHandler> rspdJsonList = null;
        if (_rspdJsonListMap.TryGetValue(rcvData.cmdId, out rspdJsonList))
        {
            for (int i = 0; i < rspdJsonList.Count; i++)
            {
                rspdJsonList[i](rcvData.json.ToString());
            }
        }
    }
    /*
    protected void showErrorCodeTip(int cmdId)
    {
        if (_errorMessages.ContainsKey(cmdId))
        {
            var errorCode = _errorMessages[cmdId];
            _errorMessages.Remove(cmdId);
            var errorCodeStr = ReturnCode.translate(errorCode) ?? errorCode.ToString();
            SystemHit.Instance.ShowTip("请求失败, 错误码: " + errorCodeStr);
        }
    }
    */
    /// <summary>
    /// 取消这条消息的错误码弹出框
    /// </summary>
    public void SuppressErrorCodeTip(int cmdId)
    {
#if UNITY_EDITOR
        if (_errorMessages.ContainsKey(cmdId)) _errorMessages.Remove(cmdId);
#endif
    }

    public class RcvData
    {
        public int cmdId { get; private set; }
        public StringBuilder json { get; private set; }
        public RcvData(int cmdId, StringBuilder json)
        {
            this.cmdId = cmdId;
            this.json = json;
        }
    }
    public class CmdMethodInfo
    {
        //public List<BaseService> serviceList { get; private set; }
        public List<MethodInfo> methodList { get; private set; }
        public CmdMethodInfo()
        {
            //serviceList = new List<BaseService>();
            methodList = new List<MethodInfo>();
        }
    }
    public class RspdType
    {
        public MethodInfo method { get; private set; }
        public string cmdName { get; private set; }
        public Type rspdType { get; private set; }
        public RspdType(Type rspdType, MethodInfo method)
        {
            this.rspdType = rspdType;
            this.method = method;

            cmdName = rspdType.Name;
        }
    }
}

