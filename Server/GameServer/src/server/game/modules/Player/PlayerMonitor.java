package server.game.modules.Player;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.*;
import server.common.session.Session;
import server.common.session.SessionStatus;
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

    private HashMap<String, PlayerVo> playerMap;


    public PlayerMonitor(Monitor base)
    {
        super(base);
        playerMap = new HashMap<>();
    }

    @Override
    public void ActionHandler(ChannelHandlerContext ctx, JSONObject jsonObject, String subAction)
    {
        Session session = new Session(ctx, jsonObject);
        switch (subAction)
        {
            case Action.NONE:

                break;
            case Action.LOGIN_GAME_SEVER:
                login(session);
                break;
            case Action.LOGOUT_GAME_SEVER:
                logout(session);
                break;
            default:
                break;
        }
    }

    //处理登录游戏服务器
    private void login(Session session)
    {
        String aid = session.getParam().getString(0);
        String channelId = session.getChannelId();
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
            player = new PlayerVo(aid);
            player.setAccountId(aid);
            player.fromDB(db);
            playerMap.put(channelId, player);
        }
        if (player.isEmpty())
        {//新玩家
            long playerId = IdGenerator.getInstance().nextId();//生成玩家Id
            player.setId(Long.toString(playerId));
            player.setRegisterTime(TimeUtils.date2String(new Date()));
        }
        player.setLastLoginTime(TimeUtils.date2String(new Date()));//最近一次登陆时间
        player.writeDB(db);
        //直接返回
        rspdClient(session);
        //推送玩家信息
        pushClient(session, player.toJson(), Action.PUSH_PLAYER_INFO);
    }

    //登出游戏服务器
    private void logout(Session session)
    {
        PlayerVo player = getPlayerInfo(session.getChannelId());
        if (player != null)
        {
            playerMap.remove(player);
            player.setLastLogoutTime(TimeUtils.date2String(new Date()));//最近一次登出时间
            player.writeDB(db);
            logger.info("The player has logout. id:" + player.getId() + "");
        }
    }

    public PlayerVo getPlayerInfo(String channelId)
    {
        return playerMap.get(channelId);
    }
}
