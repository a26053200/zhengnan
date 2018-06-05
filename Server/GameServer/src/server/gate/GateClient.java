package server.gate;

import io.netty.bootstrap.Bootstrap;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioSocketChannel;
import io.netty.handler.codec.serialization.ClassResolvers;
import io.netty.handler.codec.serialization.ObjectDecoder;
import io.netty.handler.codec.serialization.ObjectEncoder;
import org.apache.log4j.Logger;

/**
 * @ClassName: GateClient
 * @Description: 网关客户端
 * @Author: zhengnan
 * @Date: 2018/6/5 23:24
 */
public class GateClient
{
    private static final Logger logger = Logger.getLogger(GateClient.class);
    private final String host;
    private final int port;
    private final int firstMessageSize;
    private final String serverName;

    public GateClient(String host, int port, int firstMessageSize, String serverName) {
        this.host = host;
        this.port = port;
        this.firstMessageSize = firstMessageSize;
        this.serverName = serverName;
    }

    public void run() throws Exception {
        EventLoopGroup group = new NioEventLoopGroup();
        try {
            logger.info("网关客户端开始启动...");
            Bootstrap b = new Bootstrap();
            b.group(group)
                    .channel(NioSocketChannel.class)
                    .handler(new ChannelInitializer<SocketChannel>() {
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception {
                            ch.pipeline().addLast(
                                    new ObjectEncoder(),
                                    new ObjectDecoder(ClassResolvers.cacheDisabled(null)),
                                    new GateClientHandler(firstMessageSize,serverName));
                        }
                    });

            // Start the connection attempt.
            b.connect(host, port).sync().channel().closeFuture().sync();
        } finally {
            group.shutdownGracefully();
        }
    }
    /**
     *
     * start方法的作用<br>
     * 方法适用条件<br>
     * @param serverName  连接服务器的名称
     * @return void
     * @exception
     * @since  1.0.0
     */
    public static void start(final String serverName,final int port)
    {
        new Thread(new Runnable() {

            @Override
            public void run() {
                try {
                    new GateClient("127.0.0.1",port,10,serverName).run();
                } catch (Exception e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        },"Gate_Client-->"+serverName).start();
    }
}
