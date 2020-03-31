using System.Collections.Generic;
using LitJson;
using Framework;

//
// Class Introduce
// Author: zhengnan
// Create: 2018/5/31 23:01:28
// 
public class JsonClient :  ClientBase
{
    public delegate void CmdJsonHandler(ReignResponse response);

    public CmdJsonHandler jsonCallback;

    private Queue<ReignResponse> _rcvDataQueue; //接收到的数据队列

    protected override void Awake()
    {
        base.Awake();
        _rcvDataQueue = new Queue<ReignResponse>();
    }
    protected override void Update()
    {
        base.Update();
        while (_rcvDataQueue.Count > 0)
        {
            ReignResponse response = _rcvDataQueue.Dequeue();
            DoReceive(response);
            socket.responseStack.Push(response);
        }
    }
    
    public void SetJsonCallback(CmdJsonHandler handler)
    {
        this.jsonCallback = handler;
    }

    public void SendJson(JsonData obj)
    {
        socket.send(obj);
    }
    public void Send(string json)
    {
        socket.send(json, true);
    }
    public void Send(IRequest rqst)
    {
        socket.Send(rqst);
    }
    protected override void OnReceiveHandler(IResponse response)
    {
        base.OnReceiveHandler(response);
#if UNITY_EDITOR
        //showErrorCodeTip(cmdId);
#endif
        //object obj = System.Activator.CreateInstance(rspdType);
        //Logger.Log("收到数据 NetCmd:" + " " + cmdId);
        _rcvDataQueue.Enqueue(response as ReignResponse);//进入消息处理队列
    }
    private void DoReceive(ReignResponse response)
    {
        jsonCallback?.Invoke(response);
    }
}

