package server.gate.client;

import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.channel.MessageList;
import io.netty.util.concurrent.Future;
import io.netty.util.concurrent.GenericFutureListener;

import java.util.ArrayList;
import java.util.List;

import org.apache.log4j.Logger;

import business.monitor.GateClientMnt;
import business.vo.ErrorVo;
import business.vo.VoBase;
import debug.Debug;

/**
 * Handler implementation for the object echo client.  It initiates the
 * ping-pong traffic between the object echo client and server by sending the
 * first message to the server.
 */
public class GateClientHandler extends ChannelInboundHandlerAdapter {

	private static final String name = "GATEWAY_CLIENT_HANDLER";

	public static String getName() {
		return name;
	}
	private static Logger logger = Logger.getLogger(GateClientHandler.class.getName());
	
    private final List<Integer> firstMessage;
    protected GateClientMnt gateClientMnt;
    private String serverName;

    /**
     * Creates a client-side handler.
     */
    public GateClientHandler(int firstMessageSize, String serverName) {
        if (firstMessageSize <= 0) {
            throw new IllegalArgumentException("firstMessageSize: " + firstMessageSize);
        }
        firstMessage = new ArrayList<Integer>(firstMessageSize);
        for (int i = 0; i < firstMessageSize; i ++) {
            firstMessage.add(Integer.valueOf(i));
        }
        this.serverName = serverName;
    }

    @Override
	public void channelWritabilityChanged(ChannelHandlerContext ctx)
			throws Exception {
		// TODO Auto-generated method stub
		super.channelWritabilityChanged(ctx);
	}

	@Override
    public void channelActive(final ChannelHandlerContext ctx) throws Exception {
        // Send the first message if this handler is a client-side handler.
		ChannelFuture futute = ctx.write(firstMessage);
    	futute.addListener(new GenericFutureListener<Future<? super Void>>() 
			{
			@Override
			public void operationComplete(Future<? super Void> channel)
					throws Exception {
				Debug.log("网关到 "+serverName+" 测试消息发送成功");
				if(gateClientMnt == null)
		    	{
		    		gateClientMnt = new GateClientMnt(ctx.channel());
		    	}
			}
		});
    	logger.info("channelActive:网关客户端已经连接上服务:"+serverName);
    }

    @Override
    public void messageReceived(ChannelHandlerContext ctx, MessageList<Object> msgs) throws Exception {
        // Echo back the received object to the server.
    	Debug.log("收到数据:"+serverName+" ---> Gateway");
    	for(Object obj:msgs)
    	{
    		if(obj instanceof VoBase)
    		{
    			gateClientMnt.voReceive(obj, ctx);
    		}else{
    			ctx.write(new ErrorVo("收到数据不能序列化"+obj));
    		}
    	}
//    	UserVo vo = new UserVo();
//    	vo.ip = ctx.channel().remoteAddress().toString();
//    	vo.userId = 1;
//    	vo.userName = "张三";
//        ctx.write(vo);
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
    	Debug.log("网关客户端异常中断:"+ cause);
        ctx.close();
    }
}
