package server.account;

import common.log.Debug;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.codec.http.HttpRequestDecoder;
import io.netty.handler.codec.http.HttpResponseEncoder;
import org.apache.log4j.Logger;
import server.common.BaseServer;
import server.login.LoginServer;
import server.redis.RedisClient;
import utils.IdGenerator;

/**
 * @ClassName: AccountServer
 * @Description: AccountServer
 * @Author: zhengnan
 * @Date: 2018/6/7 11:41
 */
public class AccountServer extends BaseServer
{
    public static final String ServerName = "AccountServer";

    public AccountServer(int port)
    {
        super(port);
    }

    @Override
    public void run() throws Exception
    {
        Logger logger = Logger.getLogger(LoginServer.class);
        EventLoopGroup bossGroup = new NioEventLoopGroup();
        EventLoopGroup workerGroup = new NioEventLoopGroup();
        AccountMonitor monitor = new AccountMonitor();
        try {
            ServerBootstrap b = new ServerBootstrap();
            b.group(bossGroup, workerGroup).channel(NioServerSocketChannel.class)
                    .childHandler(new ChannelInitializer<SocketChannel>()
                    {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception
                        {
                            // server端发送的是httpResponse，所以要使用HttpResponseEncoder进行编码
                            ch.pipeline().addLast(new HttpResponseEncoder());
                            // server端接收到的是httpRequest，所以要使用HttpRequestDecoder进行解码
                            ch.pipeline().addLast(new HttpRequestDecoder());
                            ch.pipeline().addLast(new AccountServerInboundHandler(monitor));
                        }
                    }).option(ChannelOption.SO_BACKLOG, 128)
                    .childOption(ChannelOption.SO_KEEPALIVE, true);

            //连接数据库
            RedisClient.getInstance().connectDB("127.0.0.1");
            //Id生成器
            IdGenerator.init(Thread.currentThread().getId());
            logger.info(ServerName + " startup successful!!!");
            ChannelFuture f = b.bind(port).sync();

            f.channel().closeFuture().sync();

            logger.info(ServerName + " close up...");
        } finally {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();
        }
    }

    public static void main(String[] args) throws Exception
    {
        Debug.initLog("[" + ServerName + "]");
        int port;
        if (args.length > 0) {
            port = Integer.parseInt(args[0]);
        } else {
            port = 8080;
        }
        new AccountServer(port).run();
    }
}
