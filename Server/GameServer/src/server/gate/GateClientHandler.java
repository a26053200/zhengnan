package server.gate;

import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.channel.SimpleChannelInboundHandler;
import io.netty.util.concurrent.Future;
import io.netty.util.concurrent.GenericFutureListener;
import org.apache.log4j.Logger;

import java.util.ArrayList;
import java.util.List;

/**
 * @ClassName: GateClientHandler
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/5 23:24
 */
public class GateClientHandler extends SimpleChannelInboundHandler<String>
{
    private static final Logger logger = Logger.getLogger(GateClientHandler.class);

    private final List<Integer> firstMessage;
    private String serverName;

    /**
     * Creates a client-side handler.
     */
    public GateClientHandler(int firstMessageSize, String serverName)
    {
        if (firstMessageSize <= 0)
        {
            throw new IllegalArgumentException("firstMessageSize: " + firstMessageSize);
        }
        firstMessage = new ArrayList<Integer>(firstMessageSize);
        for (int i = 0; i < firstMessageSize; i ++) {
            firstMessage.add(Integer.valueOf(i));
        }
        this.serverName = serverName;
    }

    @Override
    protected void channelRead0(ChannelHandlerContext channelHandlerContext, String s) throws Exception
    {
        logger.info("网关客户端收到消息" + s);
    }
}
