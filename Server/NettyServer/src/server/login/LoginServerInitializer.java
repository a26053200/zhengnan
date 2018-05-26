package server.login;

import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.socket.SocketChannel;
import io.netty.handler.codec.serialization.ClassResolvers;

/**    
 * @FileName: LoginServerInitializer.java  
 * @Package:server.login  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 上午11:04:23
 */
public class LoginServerInitializer extends ChannelInitializer<SocketChannel> {

	@Override
	protected void initChannel(SocketChannel sch) throws Exception {

		ChannelPipeline pipeline = sch.pipeline();
		
//		pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192,  
//                Delimiters.lineDelimiter()));
		
		pipeline.addLast("decoder", new LoginDecoder(ClassResolvers.cacheDisabled(null)));
        pipeline.addLast("encoder", new LoginEncoder());

        // and then business logic.
        pipeline.addLast("handler", new LoginServerHandler());
	}
}
