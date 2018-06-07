package server.account;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.common.Action;
import server.common.Monitor;
import server.common.ReturnCode;
import server.login.LoginMonitor;
import server.redis.RedisClient;
import server.redis.RedisKeys;
import utils.IdGenerator;

import java.util.Date;

/**
 * @ClassName: AccountMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/7 11:50
 */
public class AccountMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(LoginMonitor.class);
    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String action = jsonObject.get("action").toString();

        switch(action)
        {
            case Action.LOGIN_ACCOUNT:
                login(ctx,jsonObject);
                break;
        }
    }

    private void login(ChannelHandlerContext ctx,JSONObject jsonObject)
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
            onLoginSuccess(ctx,jsonObject);
        } else {
            String db_password = db.hget(username, RedisKeys.account_password);
            if (db_password.equals(password)) {//密码正确，登陆成功
                onLoginSuccess(ctx,jsonObject);
            } else {//密码错误，登陆失败
                onLoginFail(ctx,jsonObject,"密码错误");
            }
        }
    }

    private void onLoginSuccess(ChannelHandlerContext ctx,JSONObject jsonObject)
    {
        String msg = String.format("用户:%s 登陆成功", jsonObject.get("username"));
        logger.info(msg);
        //ctx.channel().write(0);
        sendString(ctx,msg);
    }

    private void onLoginFail(ChannelHandlerContext ctx,JSONObject jsonObject,String errorMsg)
    {
        logger.info(String.format("用户:%s 登陆成功，登陆失败. 原因:%s", jsonObject.get("username"), errorMsg));
        sendReturnCode(ctx,ReturnCode.Code.WRONG_PASSWORD);
    }
}
