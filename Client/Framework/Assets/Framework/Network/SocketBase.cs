using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
public class SocketBase : MonoBehaviour
{
    public string host { get; protected set; }//服务器ip
    public int port { get; protected set; }//服务器端口
    public NetStatus status { get; protected set; }//连接状态
    public Socket socket { get; protected set; }//socket连接
    public EventDispatch eventDispatcher { get; protected set; }//事件派发器

    protected SocketAsyncEventArgs _recvArgs;

    protected int MAX_READ = 8192;//缓冲区最大容量

    protected byte[] _byteBuffer;

    protected bool sendComplete = false;

    protected int startConnectTime = 0;//开始连接时间
    public virtual void init()
    {
        eventDispatcher = new EventDispatch();
        _byteBuffer = new byte[MAX_READ];
        status = NetStatus.Disconnected;
    }
    public bool connect(string host, int port)
    {
        try
        {
            this.host = host;
            this.port = port;
            IPAddress ipAddress = IPAddress.Parse(this.host);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, this.port);
            status = NetStatus.Connecting;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.RemoteEndPoint = serverEndPoint;
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(onSocketConnectCompleted);
            arg.UserToken = this.socket;
            socket.ConnectAsync(arg);
        }
        catch (Exception e)
        {
            OnConnectedError("Connect SocketException:", e.Message);
            return false;
        }
        return true;

    }
    public bool isConneted()
    {
        return status == NetStatus.Connected;
    }
    private void onSocketConnectCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.NotConnected)
        {
            Logger.LogError("[Socket] has connected:" + e.SocketError);
            return;
        }
        if (e.LastOperation != SocketAsyncOperation.Connect || e.SocketError != SocketError.Success)
        {
            Logger.LogError("[Socket] connect error:" + e.SocketError);
            return;
        }
        Socket socket = e.UserToken as Socket;
        _recvArgs = new SocketAsyncEventArgs();
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(onReceiveCompleted);
        _recvArgs.SetBuffer(_byteBuffer, 0, _byteBuffer.Length);

        bool willRaiseEvent = socket.ReceiveAsync(_recvArgs);
        if (!willRaiseEvent)
        {
            Logger.LogError("[Socket] willRaiseEvent:");
            this.disconnect();
        }
        else
        {
            status = NetStatus.Connected;
            startConnectTime = Environment.TickCount;
            Logger.Info(string.Format("[Socket] Client has connected successful from server:{0}:{1}", host, port));
        }
    }

    private void onReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.LastOperation != SocketAsyncOperation.Receive)
        {
            Logger.LogError("[Socket] connect error:" + e.SocketError);
            return;
        }
        if (e.SocketError == SocketError.ConnectionAborted)
        {
            Logger.LogError("[Socket] connect has broken:" + e.SocketError);
            disconnect();
            return;
        }
        if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
        {
            if (GlobalConsts.EnableLogNetwork)
            {
                Logger.LogError("[Socket] ProcessReceive e.BytesTransferred  " + e.BytesTransferred + ",LastOperation:" + e.LastOperation + ",SocketError:" + e.SocketError);
                Logger.LogError("看到这条日志，因为收到空包，可能服务器把客户端踢掉了。请检查上条发送的指令逻辑");
            }
            disconnect();
            return;
        }
        try
        {
            //if (GlobalConsts.EnableLogNetwork)
            //    Logger.Log("[Socket] Receive data len:" + e.BytesTransferred);
            if (e.BytesTransferred >= MAX_READ)
                Logger.LogError("[Socket] Receive data too long. len:" + e.BytesTransferred);
            doOnReceive(_byteBuffer, e.BytesTransferred);
        }
        catch (Exception ex)
        {
            OnDataError(NetError.ReadException, ex.Message);
        }
        Array.Clear(_byteBuffer, 0, _byteBuffer.Length);   //清空数组
        _recvArgs.SetBuffer(_byteBuffer, 0, _byteBuffer.Length);
        //继续接受
        try
        {
            //post next recive
            bool willRaiseEvent = socket.ReceiveAsync(this._recvArgs);
            if (!willRaiseEvent)
            {
                onReceiveCompleted(sender, this._recvArgs);
            }
        }
        catch (ObjectDisposedException ode) // socket already closed
        {
            Logger.LogError("[Socket] ProcessReceive2  " + ode.Message);
        }
        catch (SocketException exp)
        {
            Logger.LogError("[Socket] ProcessReceive3  " + exp.Message);
        }
    }
    protected virtual void OnConnectedError(string errorReason, string msg = "")
    {
        Logger.LogError(errorReason + msg);
        eventDispatcher.dispatchEvent(new SocketEvent(SocketEvent.ERROR));
    }
    protected virtual void OnDataError(string errorReason, string msg = "")
    {
        Logger.LogError(errorReason + msg);
        eventDispatcher.dispatchEvent(new SocketEvent(SocketEvent.READ_ERROR));
    }
    protected virtual void doOnReceive(byte[] bytes, int length)
    {

    }
    public void send(byte[] dataBytes, int maxSend)
    {
        if (dataBytes.Length >= maxSend)
        {
            Logger.LogError(string.Format("[Socket] send data is more than{0}. len:{1}", maxSend, dataBytes.Length));
            return;
        }
        sendComplete = false;
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.SetBuffer(dataBytes, 0, dataBytes.Length);
        args.Completed += new EventHandler<SocketAsyncEventArgs>(onSendCompleted);
        try
        {
            bool willRaiseEvent = socket.SendAsync(args);
            if (!willRaiseEvent)
                onSendError(socket, args);
        }
        catch (Exception ex)
        {
            sendComplete = true;
            Logger.LogError("[Socket] send Exception:" + ex.Message);
        }
    }
    private void onSendError(Socket sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success)
        {
            Logger.LogError("[Socket] send error:" + e.SocketError);
        }
    }
    private void onSendCompleted(object sender, SocketAsyncEventArgs e)
    {
        sendComplete = true;
    }
    public virtual void disconnect()
    {
        if (status == NetStatus.Disconnected)
            return;
        status = NetStatus.Disconnected;
        Debug.Log("Try to close connect! This connection lasted " + (Environment.TickCount - startConnectTime) + "ms");
        if (socket != null)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                if (_recvArgs != null)
                    _recvArgs.Completed -= onReceiveCompleted;
                _recvArgs = null;
                Debug.Log("[Socket] close success! This connection lasted ");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[Socket] socket Shutdown Exception:" + ex.Message);
            }
            try
            {
                socket.Close();
            }
            catch (System.Exception ex1)
            {
                Debug.LogError("[Socket] socket close Exception:" + ex1.Message);
            }
        }
    }
    public virtual void dispose()
    {
        disconnect();
        eventDispatcher.removeAllEvent();
        //if (_outStream != null)
        //_outStream.Close();
    }
    void OnDestroy()
    {
        dispose();
    }
}
