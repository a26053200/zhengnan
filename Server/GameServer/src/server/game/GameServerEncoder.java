package server.game;

import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.bytes.ByteArrayEncoder;
import org.apache.log4j.Logger;

import java.util.List;

/**
 * @ClassName: GateServerEncoder
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:01
 */
public class GameServerEncoder extends ByteArrayEncoder
{
    final static Logger logger = Logger.getLogger(GameServerEncoder.class);

    GameMonitor monitor;

    public GameServerEncoder(GameMonitor monitor)
    {
        this.monitor = monitor;
    }

    @Override
    protected void encode(ChannelHandlerContext ctx, byte[] msg, List<Object> out) throws Exception
    {
        logger.info("send msg:"+msg.length);
        super.encode(ctx,msg,out);
    }
}
