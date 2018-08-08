package server.common;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import redis.clients.jedis.Jedis;
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
        ctx.channel().write(BytesUtils.packBytes(BytesUtils.string2Bytes(msg)));
    }
}
