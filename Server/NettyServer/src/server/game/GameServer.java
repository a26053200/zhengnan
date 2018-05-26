package server.game;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.Channel;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.codec.serialization.ClassResolvers;
import io.netty.handler.codec.serialization.ObjectDecoder;
import io.netty.handler.codec.serialization.ObjectEncoder;
import debug.Debug;

/**    
 * @FileName: GameServer.java  
 * @Package:server.game  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月15日 下午3:49:37
 */
public class GameServer {

    private final int port;
    
    public GameServer(int port) {
        this.port = port;
    }
    public void service()
    {
    	EventLoopGroup bossGroup = new NioEventLoopGroup();
		EventLoopGroup workerGroup = new NioEventLoopGroup();
		try {
			Debug.log("游戏服务器开始启动...");
			ServerBootstrap b = new ServerBootstrap();
			b.group(bossGroup, workerGroup)
					.channel(NioServerSocketChannel.class)
					.childHandler(new ChannelInitializer<SocketChannel>() {
		                @Override
		                public void initChannel(SocketChannel ch) throws Exception {
		                    ch.pipeline().addLast(
		                            new ObjectEncoder(),
		                            new ObjectDecoder(ClassResolvers.cacheDisabled(null)),
		                            new GameServerHandler());
		                }
		             });
			Channel ch = b.bind(port).sync().channel();
			Debug.log("游戏服务器启动完成。");
			ch.closeFuture().sync();
		} catch (Exception ex) {
			ex.printStackTrace();
		} finally {
			bossGroup.shutdownGracefully();
			workerGroup.shutdownGracefully();
		}
    }
}
