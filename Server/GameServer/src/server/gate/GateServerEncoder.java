package server.gate;

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
public class GateServerEncoder extends ByteArrayEncoder
{
    Logger logger = Logger.getLogger(GateServerEncoder.class);
    @Override
    protected void encode(ChannelHandlerContext ctx, byte[] msg, List<Object> out) throws Exception
    {
        super.encode(ctx,msg,out);
    }
}
