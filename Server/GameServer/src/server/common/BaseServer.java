package server.common;

import org.apache.log4j.Logger;
import server.game.GameServer;
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
    private Logger logger = Logger.getLogger(BaseServer.class);

    protected int port;

    protected String serverName;

    protected String localHost;

    public BaseServer(String serverName, int port)
    {
        this.serverName = serverName;
        this.port = port;
        this.localHost = NetUtils.GetLocalHostAddress();
        logger.info("localhost " +  this.localHost + ":" +  this.port);
        //连接数据库
        RedisClient.getInstance().connectDB("127.0.0.1");
        //Id生成器
        IdGenerator.init(Thread.currentThread().getId());
    }

    public abstract void run() throws Exception;
}
