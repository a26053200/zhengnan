package server.game;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.bytes.ByteArrayDecoder;
import org.apache.log4j.Logger;

import java.util.List;

/**
 * @ClassName: GateServerDecoder
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:00
 */
public class GameServerDecoder extends ByteArrayDecoder
{
    final static Logger logger = Logger.getLogger(GameServerDecoder.class);

    GameMonitor monitor;

    public GameServerDecoder(GameMonitor monitor)
    {
        this.monitor = monitor;
    }
    @Override
    protected void decode(ChannelHandlerContext ctx, ByteBuf msg, List<Object> out) throws Exception
    {
        super.decode(ctx, msg, out);
        monitor.recvJsonBuff(ctx,msg);
    }
}
