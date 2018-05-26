package server.gate;

import org.apache.log4j.Logger;

import business.monitor.GateMnt;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelFutureListener;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;
import io.netty.channel.group.ChannelGroup;
import io.netty.channel.group.DefaultChannelGroup;
import io.netty.util.concurrent.GlobalEventExecutor;
import server.gate.client.GateClient;
import server.monitor.MonitorBase;
import debug.Debug;

/**    
 * @FileName: GateSeverHandler.java  
 * @Package:server.gate  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午9:49:51
 */
public class GateServerHandler extends SimpleChannelInboundHandler<MonitorBase>{

	private static final String name = "GATEWAY_SERVER_HANDLER";

	public static String getName() {
		return name;
	}
	static Logger logger = Logger.getLogger(GateServerHandler.class.getName());

	static final ChannelGroup channels = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);

	protected GateMnt gateMnt;
	
	public GateServerHandler(GateMnt gateMnt)
	{
		this.gateMnt = gateMnt;
	}
	
	@Override
	public void channelActive(ChannelHandlerContext ctx) throws Exception {
		super.channelActive(ctx);
		logger.info("channelActive:新客户端连接:"+ctx.channel().remoteAddress().toString());
		channels.add(ctx.channel());
		if(gateMnt.getChannelGroup() == null)
		{
			gateMnt.setChannelGroup(channels);
			GateClient.start("LoginServer",8889);
			GateClient.start("GameServer",8887);
		}
	}


	@Override
	public void channelInactive(ChannelHandlerContext ctx) throws Exception {
		// TODO Auto-generated method stub
		super.channelInactive(ctx);
		
		logger.info("channelInactive:客户端断开连接:"+ctx);
		
		gateMnt.removeChannel(ctx.channel().id());
	}
	@Override
	public boolean acceptInboundMessage(Object msg) throws Exception {
		return super.acceptInboundMessage(msg);
	}

	@Override
    public void messageReceived(ChannelHandlerContext ctx, MonitorBase mnt) throws Exception {
		Debug.log("收到数据:Gateway <--- Client");
		byte[] msg = mnt.notifyRspd(ctx.channel());
		if(msg != null)
		{//是否有同步 return 指令
			ctx.channel().write(msg);
		}
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable throwable) throws Exception {
        throwable.printStackTrace();
        Debug.log("网关服务器异常中断");
        if (ctx.channel().isActive()) {
        	ctx.channel().write(Unpooled.EMPTY_BUFFER).addListener(ChannelFutureListener.CLOSE);
        }
    }
    
}
