package server.common;

import server.redis.RedisClient;
import utils.IdGenerator;

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
        //连接数据库
        RedisClient.getInstance().connectDB("127.0.0.1");
        //Id生成器
        IdGenerator.init(Thread.currentThread().getId());
    }

    public abstract void run() throws Exception;
}
