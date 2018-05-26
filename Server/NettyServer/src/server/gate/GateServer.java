package server.gate;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.Channel;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import debug.Debug;

/**    
 * @FileName: GateServer.java  
 * @Package:server.gate  
 * @Description: TODO 网关服务器
 * @author: zhengnan   
 * @date:2018年5月10日 上午9:48:42
 */
public class GateServer {

	private final int port;
	public GateServer(int port)
	{
		this.port = port;
	}
	public void service()
    {
		EventLoopGroup bossGroup = new NioEventLoopGroup();
		EventLoopGroup workerGroup = new NioEventLoopGroup();
		try {
			Debug.log("网关服务器开始启动...");
			ServerBootstrap b = new ServerBootstrap();
			b.group(bossGroup, workerGroup)
					.channel(NioServerSocketChannel.class)
					.childHandler(new GateServerInitializer());
			
			Channel ch = b.bind(port).sync().channel();
			Debug.log("网关服务器启动完成。");
			ch.closeFuture().sync();
		} catch (Exception ex) {
			ex.printStackTrace();
		} finally {
			bossGroup.shutdownGracefully();
			workerGroup.shutdownGracefully();
		}
    }
}
