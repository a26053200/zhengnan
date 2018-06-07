package server.login;

import com.alibaba.fastjson.JSONObject;
import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.bytes.ByteArrayDecoder;
import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.common.Monitor;
import server.redis.RedisClient;
import server.redis.RedisKeys;
import utils.*;

import java.util.Date;
import java.util.List;

/**
 * @ClassName: GateServerDecoder
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:00
 */
public class LoginServerDecoder extends ByteArrayDecoder
{
    final static Logger logger = Logger.getLogger(LoginServerDecoder.class);

    protected LoginMonitor monitor;

    public LoginServerDecoder(LoginMonitor monitor)
    {
        this.monitor = monitor;
    }
    @Override
    protected void decode(ChannelHandlerContext ctx, ByteBuf bytes, List<Object> out) throws Exception
    {
        super.decode(ctx, bytes, out);
        monitor.recvByteBuf(ctx, bytes);
    }
}
