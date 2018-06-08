package server.login;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.common.Action;
import server.common.Monitor;
import server.common.ReturnCode;
import server.redis.RedisClient;
import server.redis.RedisKeys;
import utils.IdGenerator;

import java.util.Date;

/**
 * @ClassName: LoginMonitor
 * @Description: 登陆服务器的监视器
 * @Author: zhengnan
 * @Date: 2018/6/6 17:41
 */
public class LoginMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(LoginMonitor.class);
    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {

    }

    @Override
    protected void initDB()
    {

    }
}
