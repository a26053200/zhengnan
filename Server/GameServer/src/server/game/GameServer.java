package server.game;

import common.log.Debug;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import org.apache.log4j.Logger;
import server.common.BaseServer;
import server.gate.*;

/**
 * @ClassName: GameServer
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/8 23:47
 */
public class GameServer extends BaseServer
{
    public static final String ServerName = "GameServer";

    public GameServer(int port)
    {
        super(ServerName,port);
    }

    @Override
    public void run() throws Exception
    {
        Logger logger = Logger.getLogger(GameServer.class);
        EventLoopGroup bossGroup = new NioEventLoopGroup(); // (1)
        EventLoopGroup workerGroup = new NioEventLoopGroup();
        GameMonitor monitor = new GameMonitor();
        try
        {
            ServerBootstrap b = new ServerBootstrap(); // (2)
            b.group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class) // (3)
                    .childHandler(new ChannelInitializer<SocketChannel>()
                    {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception
                        {
                            ch.pipeline().addLast(new GameServerDecoder(monitor));
                            ch.pipeline().addLast(new GameServerEncoder(monitor));
                            ch.pipeline().addLast(new GameServerHandler(monitor));
                        }
                    }).option(ChannelOption.SO_BACKLOG, 128)          // (5)
                    .childOption(ChannelOption.SO_KEEPALIVE, true); // (6)


            ChannelFuture f = b.bind(port).sync(); // (7)
            logger.info(ServerName + " startup successful!!!");

            f.channel().closeFuture().sync();
            logger.info(ServerName + " close up...");
        }
        finally
        {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();
        }
    }

    public static void main(String[] args) throws Exception
    {
        Debug.initLog("[" + ServerName + "]","log4j_game_server.properties");
        int port;
        if (args.length > 0)
        {
            port = Integer.parseInt(args[0]);
        }
        else
        {
            port = 8090;
        }
        new GameServer(port).run();
    }
}
