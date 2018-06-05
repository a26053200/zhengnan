package server.login;

import common.log.Debug;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import org.apache.log4j.Logger;
import server.common.BaseServer;
import server.gate.GateServer;
import server.gate.GateServerInitializer;

/**
 * @ClassName: LoginServer
 * @Description: 登陆服务器
 * @Author: zhengnan
 * @Date: 2018/6/5 23:45
 */
public class LoginServer extends BaseServer
{
    public static final String ServerName = "LoginServer";
    public LoginServer(int port)
    {
        super(port);
    }
    @Override
    public void run() throws Exception
    {
        Logger logger = Logger.getLogger(LoginServer.class);
        EventLoopGroup bossGroup = new NioEventLoopGroup(); // (1)
        EventLoopGroup workerGroup = new NioEventLoopGroup();
        try {
            ServerBootstrap b = new ServerBootstrap(); // (2)
            b.group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class) // (3)
                    .childHandler(new LoginServerInitializer())  //(4)
                    .option(ChannelOption.SO_BACKLOG, 128)          // (5)
                    .childOption(ChannelOption.SO_KEEPALIVE, true); // (6)

            logger.info(ServerName + " startup successful!!!");

            ChannelFuture f = b.bind(port).sync(); // (7)

            f.channel().closeFuture().sync();

        } finally {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();

            logger.info(ServerName + "close up successful!!!");
        }
    }
    public static void main(String[] args) throws Exception {
        Debug.initLog("["+ServerName+"]");
        int port;
        if (args.length > 0) {
            port = Integer.parseInt(args[0]);
        } else {
            port = 8080;
        }
        new LoginServer(port).run();
    }
}
