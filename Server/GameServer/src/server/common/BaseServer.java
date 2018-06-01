package server.common;

/**
 * @ClassName: BaseServer
 * @Description: 服务器基类
 * @Author: zhengnan
 * @Date: 2018/6/1 20:51
 */
public abstract class BaseServer
{
    protected int port;

    public BaseServer(int port)
    {
        this.port = port;
    }

    public abstract void run() throws Exception;
}
