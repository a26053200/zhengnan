package server.account;

import com.alibaba.fastjson.JSONObject;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.http.DefaultFullHttpResponse;
import io.netty.handler.codec.http.FullHttpResponse;
import io.netty.handler.codec.http.HttpHeaderValues;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.common.Action;
import server.common.Monitor;
import server.common.ReturnCode;
import server.login.LoginMonitor;
import server.redis.RedisClient;
import server.redis.RedisKeys;
import utils.BytesUtils;
import utils.IdGenerator;
import utils.JwtHelper;

import java.util.Date;

import static io.netty.handler.codec.http.HttpHeaderNames.*;
import static io.netty.handler.codec.http.HttpResponseStatus.OK;
import static io.netty.handler.codec.http.HttpVersion.HTTP_1_1;

/**
 * @ClassName: AccountMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/7 11:50
 */
public class AccountMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(LoginMonitor.class);

    //Token 密钥
    public final static String tokenSecretKey = "emhlbmduYW56aGVuZ3lpdGFuZ2h1aWp1YW4=";
    //Token 过期时间
    public final static int expiresSecond = 5 * 60 * 60 * 1000;

    public AccountMonitor()
    {
        super();
    }

    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String action = jsonObject.get("action").toString();

        switch (action) {
            case Action.LOGIN_ACCOUNT:
                login(ctx, jsonObject);
                break;
        }
    }

    private void login(ChannelHandlerContext ctx, JSONObject recvJson)
    {
        String username = recvJson.get("username").toString();
        String password = recvJson.get("password").toString();

        String account_id = db.hget(username, RedisKeys.account_id);
        //帐号存在就验证密码
        if (account_id == null) {//新建账号和密码
            long aid = IdGenerator.getInstance().nextId();
            db.hset(username, RedisKeys.account_id, Long.toString(aid));
            db.hset(username, RedisKeys.account_username, username);
            db.hset(username, RedisKeys.account_password, password);
            db.hset(username, RedisKeys.account_ip, ctx.channel().remoteAddress().toString());
            db.hset(username, RedisKeys.account_reg_time, new Date().toString());
            logger.info(String.format("用户:%s 登陆成功", username));
            onLoginSuccess(ctx, account_id);
        } else {
            String db_password = db.hget(username, RedisKeys.account_password);
            if (db_password.equals(password)) {//密码正确，登陆成功
                try {
                    logger.info(String.format("用户:%s 登陆成功", username));
                    onLoginSuccess(ctx, account_id);
                } catch (Exception ex) {
                    ex.printStackTrace();
                    onLoginFail(ctx, recvJson, ReturnCode.Code.FETCH_GAME_SERVER_LIST_ERROR);
                }
            } else {//密码错误，登陆失败
                onLoginFail(ctx, recvJson, ReturnCode.Code.WRONG_PASSWORD);
            }
        }
    }

    private void onLoginSuccess(ChannelHandlerContext ctx, String account_id)
    {
        //游戏服务器的网关地址列表 json
        JSONObject gameServerJson = JSONObject.parseObject(db.get("GameServer"));
        JSONObject rspdJson = new JSONObject();
        rspdJson.put("aid", account_id);
        rspdJson.put("token", JwtHelper.createJWT(account_id, expiresSecond));
        rspdJson.put("srvList", gameServerJson);
        httpResponse(ctx, rspdJson.toString());
    }

    private void onLoginFail(ChannelHandlerContext ctx, JSONObject jsonObject, ReturnCode.Code code)
    {
        logger.info(String.format("用户:%s 登陆失败. 原因:%s", jsonObject.get("username"), ReturnCode.getMsg(code)));
        httpResponse(ctx, ReturnCode.getMsg(code));
    }

    private void httpResponse(ChannelHandlerContext ctx, String msg)
    {
        logger.info(String.format("[Rspd]:%s", msg));
        FullHttpResponse response = new DefaultFullHttpResponse(
                HTTP_1_1, OK, Unpooled.wrappedBuffer(BytesUtils.string2Bytes(msg)));
        response.headers().set(CONTENT_TYPE, "text/plain");
        response.headers().set(CONTENT_LENGTH, response.content().readableBytes());
        response.headers().set(CONNECTION, HttpHeaderValues.KEEP_ALIVE);
        ctx.write(response);
    }
}
