package server.common;

import com.alibaba.fastjson.JSONException;
import com.alibaba.fastjson.JSONObject;
import io.netty.buffer.ByteBuf;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelId;
import io.netty.channel.group.ChannelGroup;
import io.netty.channel.group.DefaultChannelGroup;
import io.netty.util.concurrent.Future;
import io.netty.util.concurrent.GenericFutureListener;
import io.netty.util.concurrent.GlobalEventExecutor;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.redis.RedisClient;
import utils.BytesUtils;
import utils.StringUtils;

import java.util.HashMap;

/**
 * @ClassName: Monitor
 * @Description: 负责解析指令, 之后执行转发到其他服务或者回给客户端
 * @Author: zhengnan
 * @Date: 2018/6/6 17:10
 */
public abstract class Monitor
{
    private final static Logger logger = Logger.getLogger(Monitor.class);

    /**
     * 多个客户端链接通道
     */
    protected ChannelGroup channelGroup;
    /**
     * 子Monitor
     */
    protected HashMap<String, ChannelHandlerContext> contextMap;
    /**
     * 数据库
     */
    protected Jedis db;
    /**
     * 子Monitor
     */
    protected HashMap<String, SubMonitor> subMonitorMap;

    public ChannelGroup getChannelGroup()
    {
        return channelGroup;
    }

    public Channel getChannel(ChannelId channelId)
    {
        return channelGroup.find(channelId);
    }

    public ChannelHandlerContext getContext(String channelId)
    {
        return contextMap.get(channelId);
    }

    public ChannelHandlerContext regContext(ChannelHandlerContext ctx)
    {
        String chId = ctx.channel().id().asLongText();
        if(!contextMap.containsKey(chId))
            contextMap.put(chId,ctx);
        return ctx;
    }

    public Monitor()
    {
        //所有已经链接的通道,用于广播
        channelGroup = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);
        contextMap = new HashMap<>();
        subMonitorMap = new HashMap<>();
        //初始化数据库
        initDB();
    }

    public void recvByteBuf(ChannelHandlerContext ctx, ByteBuf buf)
    {
        int msgLen = buf.readableBytes();
        long packHead = buf.readUnsignedInt();
        long packLen = packHead;
        String json = BytesUtils.readString(buf, (int) packLen);
        logger.info(String.format("[recv] msgLen:%d json:%s", msgLen, json));
        //已知JSONObject,目标要转换为json字符串
        try
        {
            JSONObject jsonObject = JSONObject.parseObject(json);
            RespondJson(ctx, jsonObject);
        }
        catch (JSONException ex)
        {
            ex.printStackTrace();
        }
    }

    public void recvJson(ChannelHandlerContext ctx, String json)
    {
        try
        {
            JSONObject jsonObject = JSONObject.parseObject(json);
            RespondJson(ctx, jsonObject);
        }
        catch (JSONException ex)
        {
            ex.printStackTrace();
        }
    }

    public void recvJsonBuff(ChannelHandlerContext ctx, ByteBuf buf)
    {
        String json = BytesUtils.readString(buf);
        recvJson(ctx,json);
    }

    public SubMonitor getSubMonitor(String mntName)
    {
        return subMonitorMap.get(mntName);
    }

    /**
     * @param ctx
     * @param jsonObject
     */
    protected abstract void RespondJson(ChannelHandlerContext ctx, JSONObject jsonObject);

    /**
     * 初始化数据库
     */
    protected abstract void initDB();

    protected void sendString(ChannelHandlerContext ctx, String msg)
    {
        ctx.channel().write(BytesUtils.string2Bytes(msg));
    }

    protected String[] getParams(JSONObject jsonObject)
    {
        String dataStr = jsonObject.getString("data");
        String[] params = dataStr.split("&");
        return  params;
    }
}
