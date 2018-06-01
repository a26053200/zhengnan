package server.gate;

import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.socket.SocketChannel;
import io.netty.handler.codec.DelimiterBasedFrameDecoder;
import io.netty.handler.codec.Delimiters;
import io.netty.handler.codec.string.StringDecoder;
import io.netty.handler.codec.string.StringEncoder;
import server.simplechat.SimpleChatServerHandler;

/**
 * @ClassName: GateServerInitializer
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 20:56
 */
public class GateServerInitializer extends
        ChannelInitializer<SocketChannel>
{
    @Override
    public void initChannel(SocketChannel ch) throws Exception {
        ChannelPipeline pipeline = ch.pipeline();

        //pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192, Delimiters.lineDelimiter()));
        pipeline.addLast("decoder", new GateServerDecoder());
        pipeline.addLast("encoder", new GateServerEncoder());
        pipeline.addLast("handler", new GateServerHandler());

        System.out.println("GateServer:"+ch.remoteAddress() +"连接上");
    }
}
