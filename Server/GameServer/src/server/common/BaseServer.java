package server.common;

import server.redis.RedisClient;
import utils.IdGenerator;
import utils.NetUtils;

/**
 * @ClassName: BaseServer
 * @Description: 服务器基类
 * @Author: zhengnan
 * @Date: 2018/6/1 20:51
 */
public abstract class BaseServer
{
    protected int port;

    protected String serverName;

    protected String localHost;

    public BaseServer(String serverName, int port)
    {
        this.serverName = serverName;
        this.port = port;
        this.localHost = NetUtils.GetLocalHostAddress();
        //连接数据库
        RedisClient.getInstance().connectDB(this.localHost);
        //Id生成器
        IdGenerator.init(Thread.currentThread().getId());
    }

    public abstract void run() throws Exception;
}
