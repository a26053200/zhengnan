package server.login;

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
public class LoginServerEncoder extends ByteArrayEncoder
{
    protected LoginMonitor monitor;

    public LoginServerEncoder(LoginMonitor monitor)
    {
        this.monitor = monitor;
    }
    final static Logger logger = Logger.getLogger(LoginServerEncoder.class);
    @Override
    protected void encode(ChannelHandlerContext ctx, byte[] msg, List<Object> out) throws Exception
    {
        super.encode(ctx,msg,out);
    }
}
