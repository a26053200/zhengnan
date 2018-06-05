package server.login;

import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.socket.SocketChannel;
import org.apache.log4j.Logger;
import server.gate.GateServerDecoder;
import server.gate.GateServerEncoder;
import server.gate.GateServerHandler;

/**
 * @ClassName: GateServerInitializer
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 20:56
 */
public class LoginServerInitializer extends
        ChannelInitializer<SocketChannel>
{
    Logger logger = Logger.getLogger(GateServerEncoder.class);
    @Override
    public void initChannel(SocketChannel ch) throws Exception {
        ChannelPipeline pipeline = ch.pipeline();

        //pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192, Delimiters.lineDelimiter()));
        pipeline.addLast("decoder", new LoginServerDecoder());
        pipeline.addLast("encoder", new LoginServerEncoder());
        pipeline.addLast("handler", new LoginServerHandler());

        logger.info("client ip:"+ch.remoteAddress() +" has connected");
    }
}
