package server.gate;

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
 * @ClassName: GateClient
 * @Description: 网关客户端
 * @Author: zhengnan
 * @Date: 2018/6/5 23:24
 */
public class GateClient
{
    final static Logger logger = Logger.getLogger(GateClient.class);
    private final String host;
    private final int port;
    private final String serverName;

    private GateMonitor monitor;

    private Channel channel;

    public boolean isDead = true;

    public GateClient(String host, int port, GateMonitor monitor, String serverName)
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
            logger.info("网关客户端开始启动...");
            Bootstrap b = new Bootstrap();
            b.group(group)
                    .channel(NioSocketChannel.class)
                    .handler(new ChannelInitializer<SocketChannel>()
                    {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception
                        {
                            ch.pipeline().addLast(new GateServerEncoder(monitor));
                            ch.pipeline().addLast(new GateServerDecoder(monitor));
                            ch.pipeline().addLast(new GateClientHandler(monitor, serverName));
                        }
                    });


            ChannelFuture f = b.connect(host, port).sync();
            channel = f.channel();
            logger.info("GateClient connect " + this.serverName + " successful!!!");
            monitor.SetGameServerClient(this);
            f.channel().closeFuture().sync();
        }
        finally
        {
            group.shutdownGracefully();
            isDead = true;
            logger.info("GateClient disconnect from " + this.serverName);
        }
    }

    public Channel GetChanel()
    {
        return channel;
    }

    public static void start(final String serverName, final String host, final int port, final GateMonitor monitor)
    {
        new Thread(new Runnable()
        {
            int clientNum = 1;
            @Override
            public void run()
            {
                while(true)
                {
                    if(clientNum > 1)
                        logger.info("网关客户端重新连接服务器:" + serverName + " " + clientNum++);
                    GateClient client = null;
                    try
                    {
                        client = new GateClient( host, port, monitor, serverName);
                        client.run();
                    }
                    catch (Exception e)
                    {
                        e.printStackTrace();
                    }
                    while (client != null && !client.isDead)
                    {
                        logger.info("wait dead ");
                        WaitForSecond();
                    }
                    WaitForSecond();
                }
            }
            private void WaitForSecond()
            {
                try
                {
                    Thread.sleep(10000);
                }
                catch (InterruptedException e)
                {
                    e.printStackTrace();
                }
            }
        }, "Gate_Client-->" + serverName).start();
    }
}
