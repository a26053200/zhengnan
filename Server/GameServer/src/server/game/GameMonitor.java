package server.game;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.*;
import server.game.modules.Player.PlayerMonitor;
import server.game.modules.Player.PlayerVo;
import server.game.modules.Role.RoleMonitor;
import server.gate.GateClient;
import server.redis.RedisClient;
import utils.BytesUtils;
import utils.IdGenerator;
import utils.JwtHelper;

/**
 * @ClassName: GateMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/8 23:01
 */
public class GameMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(GameMonitor.class);

    private GameClient gameServerClient;

    public GameMonitor()
    {
        super();
        subMonitorMap.put(ModuleNames.Player, new PlayerMonitor(this));
        subMonitorMap.put(ModuleNames.Role, new RoleMonitor(this));
    }
    public void SetGameServerClient(GameClient gameServerClient)
    {
        this.gameServerClient = gameServerClient;
    }

    public void handshake(Channel channel)
    {
        JSONObject sendJson = new JSONObject();
        sendJson.put(FieldName.SERVER, ServerConstant.ServerName.GATE_SERVER);
        sendJson.put(Action.NAME, Action.HANDSHAKE_GAME2GATE);
        channel.writeAndFlush(BytesUtils.packBytes(BytesUtils.string2Bytes(sendJson.toString())));
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
        String[] actions = jsonObject.get("action").toString().split("@");
        String moduleName = actions[0];
        String subAction = actions.length > 1 ? actions[1] : Action.NONE;
        logger.info("Recv action: " + jsonObject.get("action"));
        SubMonitor subMnt = subMonitorMap.get(moduleName);
        if (subMnt != null)
            subMnt.ActionHandler(ctx, jsonObject, subAction);
        else
            logger.error("There is no monitor for action:" + jsonObject.get("action"));
    }
}
