package server.game;

import io.netty.bootstrap.Bootstrap;
import io.netty.channel.Channel;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioSocketChannel;
import org.apache.log4j.Logger;

/**
 * @ClassName: GameClient
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/10 22:00
 */
public class GameClient
{
    final static Logger logger = Logger.getLogger(GameClient.class);
    private final String host;
    private final int port;
    private final String serverName;

    private GameMonitor monitor;

    private Channel channel;

    public boolean isDead = true;

    public GameClient(String host, int port, GameMonitor monitor, String serverName)
    {
        this.host = host;
        this.port = port;
        this.monitor = monitor;
        this.serverName = serverName;
    }

    public void run() throws Exception
    {
        EventLoopGroup group = new NioEventLoopGroup();
        try
        {
            logger.info("游戏服务器客户端开始启动...");
            Bootstrap b = new Bootstrap();
            b.group(group)
                    .channel(NioSocketChannel.class)
                    .handler(new ChannelInitializer<SocketChannel>()
                    {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception
                        {
                            ch.pipeline().addLast(new GameServerDecoder(monitor));
                            ch.pipeline().addLast(new GameServerEncoder(monitor));
                            ch.pipeline().addLast(new GameClientHandler(monitor, serverName));
                        }
                    });

            ChannelFuture f = b.connect(host, port).sync();
            channel = f.channel();
            logger.info("GameClient connect " + this.serverName + " successful!!!");
            monitor.SetGameServerClient(this);
            monitor.handshake(channel);
            f.channel().closeFuture().sync();
        }
        finally
        {
            group.shutdownGracefully();
            isDead = true;
            logger.info("GameClient disconnect from " + this.serverName);
        }
    }
    public Channel GetChanel()
    {
        return channel;
    }
    public static void start(final String serverName, final String host, final int port, final GameMonitor monitor)
    {
        new Thread(new Runnable()
        {
            @Override
            public void run()
            {
                logger.info("游戏服务器客户端连接服务器:" + serverName);
                GameClient client = null;
                try
                {
                    client = new GameClient(host, port, monitor, serverName);
                    client.run();
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
            }
        }, "GameClient --> " + serverName).start();
    }
}
