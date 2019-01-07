public enum NetStatus
{
    /// <summary>
    /// 连接已经断开
    /// </summary>
    Disconnected,
    /// <summary>
    /// 连接中
    /// </summary>
    Connecting,
    /// <summary>
    /// 已经连接
    /// </summary>
    Connected,
}
public class NetError
{
    /// <summary>
    /// 读取数据连接异常
    /// </summary>
    public const string ReadException = "Read data Exception.";
    /// <summary>
    /// 数据长度异常
    /// </summary>
    public const string DataLengthException = "Data length Exception.";
}
