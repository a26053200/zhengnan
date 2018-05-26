package server.login;

import debug.Debug;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.Channel;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;


/**    
 * @FileName: LoginServer.java  
 * @Package:server.login  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 上午9:37:07
 */
public class LoginServer {

	private final int port;
	public LoginServer(int port)
	{
		this.port = port;
	}
	public void service()
    {
		EventLoopGroup bossGroup = new NioEventLoopGroup();
		EventLoopGroup workerGroup = new NioEventLoopGroup();
		try {
			Debug.log("登陆服务器开始启动...");
			ServerBootstrap b = new ServerBootstrap();
			b.group(bossGroup, workerGroup)
					.channel(NioServerSocketChannel.class)
					.childHandler(new LoginServerInitializer());

			Channel ch = b.bind(port).sync().channel();
			Debug.log("登陆服务器启动完成。");
			ch.closeFuture().sync();
		} catch (Exception ex) {
			ex.printStackTrace();
		} finally {
			bossGroup.shutdownGracefully();
			workerGroup.shutdownGracefully();
		}
    }
}
