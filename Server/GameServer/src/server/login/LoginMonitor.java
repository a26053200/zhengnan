package server.login;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.common.Monitor;
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
        String username = jsonObject.get("username").toString();
        String password = jsonObject.get("password").toString();

        Jedis db = RedisClient.getInstance().getDB(0);
        String account_id = db.hget(username, RedisKeys.account_id);
        //帐号存在就验证密码
        if (account_id == null) {//新建账号和密码
            long aid = IdGenerator.getInstance().nextId();
            db.hset(username, RedisKeys.account_id, Long.toString(aid));
            db.hset(username, RedisKeys.account_username, username);
            db.hset(username, RedisKeys.account_password, password);
            db.hset(username, RedisKeys.account_ip, ctx.channel().remoteAddress().toString());
            db.hset(username, RedisKeys.account_reg_time, new Date().toString());
        } else {
            String db_password = db.hget(username, RedisKeys.account_password);
            if (db_password == password) {//密码正确，登陆成功
                logger.info(String.format("用户:%s 登陆成功", username));
            } else {//密码错误，登陆失败
                logger.info(String.format("用户:%s 密码错误，登陆失败", username));
            }
        }
    }

    private void login(JSONObject jsonObject)
    {

    }
}
