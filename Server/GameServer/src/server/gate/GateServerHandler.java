package server.gate;

import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;

/**
 * @ClassName: GateServerHandler
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:05
 */
public class GateServerHandler extends SimpleChannelInboundHandler<String>
{

    @Override
    protected void channelRead0(ChannelHandlerContext channelHandlerContext, String s) throws Exception
    {

    }
}
