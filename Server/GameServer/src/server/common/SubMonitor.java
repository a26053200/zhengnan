package server.common;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import redis.clients.jedis.Jedis;
import server.common.session.Session;
import utils.BytesUtils;

/**
 * @ClassName: SubMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/1 0:05
 */
public abstract class SubMonitor
{
    protected Monitor base;

    protected Jedis db;

    public SubMonitor(Monitor base)
    {
        this.base = base;
        this.db = base.db;
    }

    public abstract void ActionHandler(ChannelHandlerContext ctx, JSONObject jsonObject, String subAction);

    //发送给网关
    protected void send2Gate(ChannelHandlerContext ctx, String msg)
    {
        ctx.channel().writeAndFlush(BytesUtils.packBytes(BytesUtils.string2Bytes(msg)));
    }
    //回应客户端请求
    protected void rspdClient(Session session)
    {
        rspdClient(session, null);
    }
    //回应客户端请求 带数据体
    protected void rspdClient(Session session, JSONObject sendJson)
    {
        String channelId = session.getChannelId();
        JSONObject rspdJson = new JSONObject();
        rspdJson.put(FieldName.SERVER, ServerConstant.ServerName.CLIENT);
        rspdJson.put(Action.NAME, session.getRqstAction());
        rspdJson.put(FieldName.CHANNEL_ID, channelId);
        rspdJson.put(FieldName.STATE, session.getState().ordinal());
        if(sendJson != null)
            rspdJson.put(FieldName.DATA, sendJson);
        send2Gate(session.getContext(), rspdJson.toString());
    }
    //推送给客户端
    protected void pushClient(Session session, JSONObject dataJson, String action)
    {
        JSONObject sendJson = new JSONObject();
        sendJson.put(FieldName.SERVER, ServerConstant.ServerName.CLIENT);
        sendJson.put(Action.NAME, action);
        sendJson.put(FieldName.CHANNEL_ID, session.getChannelId());
        sendJson.put(FieldName.STATE, session.getState().ordinal());
        sendJson.put(FieldName.DATA, dataJson);
        send2Gate(session.getContext(), sendJson.toString());
    }
}
