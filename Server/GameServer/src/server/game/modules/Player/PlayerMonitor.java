package server.game.modules.Player;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.*;
import utils.IdGenerator;
import utils.TimeUtils;

import java.util.Date;
import java.util.HashMap;

/**
 * @ClassName: PlayerMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/1 0:12
 */
public class PlayerMonitor extends SubMonitor
{
    final static Logger logger = Logger.getLogger(PlayerMonitor.class);

    public HashMap<String, PlayerVo> playerMap;


    public PlayerMonitor(Monitor base)
    {
        super(base);
        playerMap = new HashMap<>();
    }

    @Override
    public void ActionHandler(ChannelHandlerContext ctx, JSONObject jsonObject, String subAction)
    {
        switch (subAction)
        {
            case Action.NONE:

                break;
            case Action.LOGIN_GAME_SEVER:
                login(ctx, jsonObject);
                break;
            case Action.LOGOUT_GAME_SEVER:
                logout(ctx, jsonObject);
                break;
            default:
                break;
        }
    }

    //处理登录游戏服务器
    public void login(ChannelHandlerContext ctx, JSONObject recvJson)
    {
        ParamParser param = new ParamParser(recvJson);
        String aid = param.getString(0);
        String channelId = recvJson.getString(ServerConstant.CHANNEL_ID);
        /*
        //验证登陆有效性
        String token = param.getString(1);
        if (JwtHelper.parseJWT(token))
            logger.info(String.format("User login game server success aid:%s token:%s", aid, token));
        else
            logger.info(String.format("User login game server fail aid:%s token:%s", aid, token));
        */

        PlayerVo player = getPlayerInfo(channelId);
        if (player == null)
        {
            player = new PlayerVo();
            player.fromDB(db, aid);
        }
        if (player.isEmpty())
        {//新玩家
            long playerId = IdGenerator.getInstance().nextId();//生成玩家Id
            player.setAccountId(aid);
            player.setId(Long.toString(playerId));
            player.setRegisterTime(TimeUtils.date2String(new Date()));
        }
        player.setLastLoginTime(TimeUtils.date2String(new Date()));//最近一次登陆时间
        player.writeDB(db);
        //返回玩家信息
        JSONObject sendJson = player.toJson();
        sendJson.put(ServerConstant.SERVER, ServerConstant.ServerName.CLIENT);
        sendJson.put(Action.NAME, Action.PLAYER_INFO);
        sendJson.put(ServerConstant.CHANNEL_ID, channelId);
        send2Gate(ctx, sendJson.toString());
    }

    //登出游戏服务器
    public void logout(ChannelHandlerContext ctx, JSONObject recvJson)
    {
        PlayerVo player = getPlayerInfo(recvJson.getString(ServerConstant.CHANNEL_ID));
        if (player != null)
        {
            playerMap.remove(player);
            player.setLastLogoutTime(TimeUtils.date2String(new Date()));//最近一次登出时间
            player.writeDB(db);
        }
    }

    private PlayerVo getPlayerInfo(String channelId)
    {
        return playerMap.get(channelId);
    }
}
