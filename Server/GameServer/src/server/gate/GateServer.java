package server.gate;

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
import server.common.ServerConsts;

/**
 * @ClassName: GateServer
 * @Description: 网关服务器
 * @Author: zhengnan
 * @Date: 2018/6/1 20:49
 */
public class GateServer extends BaseServer
{
    public static final String ServerName = "GateServer";
    public GateServer(int port)
    {
        super(ServerName, port);
    }
    @Override
    public void run() throws Exception
    {
        Logger logger = Logger.getLogger(GateServer.class);
        EventLoopGroup bossGroup = new NioEventLoopGroup(); // (1)
        EventLoopGroup workerGroup = new NioEventLoopGroup();
        GateMonitor monitor = new GateMonitor();
        try {
            ServerBootstrap b = new ServerBootstrap(); // (2)
            b.group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class) // (3)
                    .childHandler(new ChannelInitializer<SocketChannel>()
                    {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception
                        {
                            ch.pipeline().addLast(new GateServerDecoder(monitor));
                            ch.pipeline().addLast(new GateServerEncoder(monitor));
                            ch.pipeline().addLast(new GateServerHandler(monitor));
                        }
                    }).option(ChannelOption.SO_BACKLOG, 128)          // (5)
                    .childOption(ChannelOption.SO_KEEPALIVE, true); // (6)



            ChannelFuture f = b.bind(port).sync(); // (7)
            logger.info(ServerName + " startup successful!!!");
            //网关客户端连接游戏服务器
            GateClient.start(ServerConsts.Name.GAME_SERVER,"",8090,monitor);
            f.channel().closeFuture().sync();

            logger.info(ServerName + " close up...");
        } finally {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();
        }
    }


    public static void main(String[] args) throws Exception {
        Debug.initLog("["+ServerName+"]","log4j_gate_server.properties");
        int port;
        if (args.length > 0) {
            port = Integer.parseInt(args[0]);
        } else {
            port = 8081;
        }
        new GateServer(port).run();
    }
}
