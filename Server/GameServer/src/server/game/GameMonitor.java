package server.game;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.Action;
import server.common.Monitor;
import server.redis.RedisClient;

/**
 * @ClassName: GateMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/8 23:01
 */
public class GameMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(GameMonitor.class);

    public GameMonitor()
    {
        super();
    }

    @Override
    protected void initDB()
    {
        //获取数据库
        db = RedisClient.getInstance().getDB(1);
    }

    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String action = jsonObject.get("action").toString();

        switch (action) {
            case Action.LOGIN_GAME_SEVER:
                login(ctx, jsonObject);
                break;
        }
    }

    //处理登录游戏服务器
    private void login(ChannelHandlerContext ctx, JSONObject recvJson)
    {
        String token = recvJson.getString("token");
        logger.info(String.format("User login game server success{0}",token));
    }
}
