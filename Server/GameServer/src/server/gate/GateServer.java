package server.gate;

import common.log.Debug;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import org.apache.log4j.Logger;
import server.common.BaseServer;
import server.simplechat.SimpleChatServer;
import server.simplechat.SimpleChatServerInitializer;

/**
 * @ClassName: GateServer
 * @Description: 网关服务器
 * @Author: zhengnan
 * @Date: 2018/6/1 20:49
 */
public class GateServer extends BaseServer
{
    public GateServer(int port)
    {
        super(port);
    }
    @Override
    public void run() throws Exception
    {
        Logger logger = Logger.getLogger(GateServer.class);
        EventLoopGroup bossGroup = new NioEventLoopGroup(); // (1)
        EventLoopGroup workerGroup = new NioEventLoopGroup();
        try {
            ServerBootstrap b = new ServerBootstrap(); // (2)
            b.group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class) // (3)
                    .childHandler(new GateServerInitializer())  //(4)
                    .option(ChannelOption.SO_BACKLOG, 128)          // (5)
                    .childOption(ChannelOption.SO_KEEPALIVE, true); // (6)

            logger.info("GateServer startup successful!!!");

            // 绑定端口，开始接收进来的连接
            ChannelFuture f = b.bind(port).sync(); // (7)

            // 等待服务器  socket 关闭 。
            // 在这个例子中，这不会发生，但你可以优雅地关闭你的服务器。
            f.channel().closeFuture().sync();

        } finally {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();

            logger.info("GateServer close up successful!!!");
        }
    }
    public static void main(String[] args) throws Exception {
        Debug.initLog("[GateServer]");
        int port;
        if (args.length > 0) {
            port = Integer.parseInt(args[0]);
        } else {
            port = 8080;
        }
        new GateServer(port).run();
    }
}
