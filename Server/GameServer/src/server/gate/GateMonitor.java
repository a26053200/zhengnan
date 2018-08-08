package server.gate;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.Monitor;
import server.common.ServerConstant;
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

    private GateClient gameServerClient;

    public void SetGameServerClient(GateClient gameServerClinet)
    {
        this.gameServerClient = gameServerClinet;
    }

    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String server = jsonObject.get(ServerConstant.SERVER).toString();
        switch (server)
        {
            //转发给游戏服务器
            case ServerConstant.ServerName.GAME_SERVER:
                jsonObject.put(ServerConstant.CHANNEL_ID,ctx.channel().id().asLongText());
                regContext(ctx);
                forwardToGameServer(jsonObject);
                break;
            //直接转发给客户端
            case ServerConstant.ServerName.CLIENT:
                ChannelHandlerContext clientCtx = getContext(jsonObject.getString(ServerConstant.CHANNEL_ID));
                if(clientCtx != null)
                    forwardToClient(clientCtx, jsonObject);
                else
                    logger.info("Client has not ChannelHandlerContext");
                break;
            //网关服务器的消息直接处理
            case ServerConstant.ServerName.GATE_SERVER:
                break;
        }
    }

    @Override
    protected void initDB()
    {
        //网关服务器不需要数据库
    }

    //转发给游戏服务器
    private void forwardToGameServer(JSONObject jsonObject)
    {
        byte[] bytes = BytesUtils.string2Bytes(jsonObject.toString());
        this.gameServerClient.GetChanel().writeAndFlush(bytes);
    }
    //直接转发给客户端
    private void forwardToClient(ChannelHandlerContext clientCtx, JSONObject jsonObject)
    {
        byte[] bytes = BytesUtils.packBytes(BytesUtils.string2Bytes(jsonObject.toString()));
        clientCtx.channel().writeAndFlush(bytes);
    }

}
