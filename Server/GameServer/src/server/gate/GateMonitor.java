package server.gate;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import org.apache.log4j.Logger;
import server.common.Action;
import server.common.Monitor;
import server.common.ServerConsts;
import server.game.GameServer;
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

    private GateClient gameServerClinet;

    public void SetGameServerClient(GateClient gameServerClinet)
    {
        this.gameServerClinet = gameServerClinet;
    }

    @Override
    protected void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject)
    {
        String server = jsonObject.get("server").toString();

        switch (server)
        {
            //转发给游戏服务器
            case ServerConsts.Name.GAME_SERVER:
                forwardToGameServer(jsonObject);
                break;
            //直接转发给客户端
            case ServerConsts.Name.CLIENT:
                break;
            //网关服务器的消息直接处理
            case ServerConsts.Name.GATE_SERVER:
                break;
        }
    }

    @Override
    protected void initDB()
    {
        //网关服务器不需要服务器
    }

    //转发给游戏服务器
    private void forwardToGameServer(JSONObject jsonObject)
    {
        byte[] bytes = BytesUtils.string2Bytes(jsonObject.toString());
        this.gameServerClinet.GetChanel().writeAndFlush(bytes);
    }
    //转发给游戏服务器
    private void forwardToClient(JSONObject jsonObject)
    {

    }

}
