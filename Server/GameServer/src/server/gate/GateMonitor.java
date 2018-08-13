package server.gate;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.*;
import utils.BytesUtils;

/**
 * @ClassName: GateMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/8 23:01
 */
public class GateMonitor extends Monitor
{
    final static Logger logger = Logger.getLogger(GateMonitor.class);

    public GateMonitor()
    {
        super();
    }

    private ChannelHandlerContext gameServerContext;

    public ChannelHandlerContext getGameServerContext()
    {
        return gameServerContext;
    }

    @Override
    protected void initDB(){ }//网关服务器不需要数据库

    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String server = jsonObject.get(FieldName.SERVER).toString();
        switch (server)
        {
            //转发给游戏服务器
            case ServerConstant.ServerName.GAME_SERVER:
                jsonObject.put(FieldName.CHANNEL_ID, ctx.channel().id().asLongText());
                regContext(ctx);
                forwardToGameServer(jsonObject);
                break;
            //直接转发给客户端
            case ServerConstant.ServerName.CLIENT:
                String channelId = jsonObject.getString(FieldName.CHANNEL_ID);
                jsonObject.remove(FieldName.CHANNEL_ID);
                jsonObject.remove(FieldName.SERVER);
                ChannelHandlerContext clientCtx = getContext(channelId);
                if (clientCtx != null)
                    forwardToClient(clientCtx, jsonObject);
                else
                    logger.info("Client has not ChannelHandlerContext");
                break;
            //网关服务器的消息直接处理
            case ServerConstant.ServerName.GATE_SERVER:
            {
                String action = jsonObject.getString(Action.NAME);
                switch (action)
                {
                    case Action.HANDSHAKE_GAME2GATE:
                        gameServerContext = ctx;
                        logger.info("The game server and gateway server shook hands successfully.");
                        break;
                }
            }
                break;
        }
    }



    //转发给游戏服务器
    private void forwardToGameServer(JSONObject jsonObject)
    {
        byte[] bytes = BytesUtils.string2Bytes(jsonObject.toString());
        gameServerContext.channel().writeAndFlush(bytes);
    }

    //直接转发给客户端
    private void forwardToClient(ChannelHandlerContext clientCtx, JSONObject jsonObject)
    {
        byte[] bytes = BytesUtils.packBytes(BytesUtils.string2Bytes(jsonObject.toString()));
        clientCtx.channel().writeAndFlush(bytes);
    }

    //直接转发给客户端
    public void notifyGameServerClientOffline(ChannelHandlerContext ctx)
    {
        JSONObject jsonObject = new JSONObject();
        jsonObject.put(Action.NAME, ModuleNames.Player + "@" + Action.LOGOUT_GAME_SEVER);
        jsonObject.put(FieldName.CHANNEL_ID, ctx.channel().id().asLongText());
        jsonObject.put(FieldName.DATA, "");
        forwardToGameServer(jsonObject);
    }
}
