package server.common;

import com.alibaba.fastjson.JSONException;
import com.alibaba.fastjson.JSONObject;
import io.netty.buffer.ByteBuf;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelId;
import io.netty.channel.group.ChannelGroup;
import io.netty.channel.group.DefaultChannelGroup;
import io.netty.util.concurrent.GlobalEventExecutor;
import org.apache.log4j.Logger;
import utils.BytesUtils;

/**
 * @ClassName: Monitor
 * @Description: 负责解析指令,之后执行转发到其他服务或者回给客户端
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

    public Monitor()
    {
        //所有已经链接的通道,用于广播
        channelGroup = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);
    }

    public ChannelGroup getChannelGroup()
    {
        return channelGroup;
    }


    public Channel getChannel(ChannelId channelId)
    {
        return channelGroup.find(channelId);
    }


    public void recvJson(ChannelHandlerContext ctx, ByteBuf buf)
    {
        int msgLen = buf.readableBytes();
        long packHead = buf.readUnsignedInt();
        long packLen = packHead;
        String json = BytesUtils.readString(buf, (int) packLen);
        logger.info(String.format("[recv] msgLen:%d json:%s", msgLen, json));
        //已知JSONObject,目标要转换为json字符串
        try {
            JSONObject jsonObject = JSONObject.parseObject(json);
            RespondJson(ctx,jsonObject);
        }catch (JSONException ex)
        {
            ex.printStackTrace();
        }
    }

    protected abstract void RespondJson(ChannelHandlerContext ctx,JSONObject jsonObject);
}
