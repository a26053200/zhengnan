package server.gate;

import business.monitor.GateMnt;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.socket.SocketChannel;

/**    
 * @FileName: GateServerInitializer.java  
 * @Package:server.gate  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午9:52:12
 */
public class GateServerInitializer extends ChannelInitializer<SocketChannel> {

	private GateMnt gateMnt = new GateMnt(null);
	@Override
	protected void initChannel(SocketChannel sch) throws Exception {

		ChannelPipeline pipeline = sch.pipeline();
		
//		pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192,  
//                Delimiters.lineDelimiter()));
		
		pipeline.addLast("decoder", new GateDecoderExternal(gateMnt));
        pipeline.addLast("encoder", new GateEncoderExternal());

        // and then business logic.
        pipeline.addLast("handler", new GateServerHandler(gateMnt));
	}

}
