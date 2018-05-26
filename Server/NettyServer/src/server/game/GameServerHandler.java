package server.game;

import org.apache.log4j.Logger;

import business.monitor.GameMnt;
import business.vo.ErrorVo;
import business.vo.VoBase;
import debug.Debug;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.channel.MessageList;
import io.netty.channel.group.ChannelGroup;
import io.netty.channel.group.DefaultChannelGroup;
import io.netty.util.concurrent.GlobalEventExecutor;

/**    
 * @FileName: GameServerHandler.java  
 * @Package:server.game  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月15日 下午3:52:06
 */
public class GameServerHandler extends ChannelInboundHandlerAdapter{

	private static final String name = "GAME_SERVER_HANDLER";

	public static String getName() {
		return name;
	}

	private static Logger logger = Logger.getLogger(GameServerHandler.class.getName());
	private static final ChannelGroup channels = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);
	
	private GameMnt gameMnt;
	@Override
	public void channelActive(ChannelHandlerContext ctx) throws Exception {
		super.channelActive(ctx);
		
		logger.info("channelActive:新网关连接:"+ctx.channel().remoteAddress().toString());
		
		channels.add(ctx.channel());
		
		if(gameMnt == null)
		{
			gameMnt = new GameMnt(channels);
		}
	}
	@Override
	public void channelInactive(ChannelHandlerContext ctx) throws Exception {
		// TODO Auto-generated method stub
		super.channelInactive(ctx);
		
		logger.info("channelInactive:网关客户端断开连接游戏服务器:"+ctx.channel().remoteAddress().toString());
	}
	@Override
    public void messageReceived(ChannelHandlerContext ctx, MessageList<Object> msgs) throws Exception {
        // Echo back the received object to the client.
		Debug.log("收到数据:GameServer <--- Gateway");
		for(Object obj : msgs)
		{
			VoBase vo = gameMnt.voReceive(obj,ctx);
			if(vo == null)
			{
				ctx.write(new ErrorVo("收到数据不能序列化"+msgs));
			}
		}
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
        Debug.log("游戏服务器异常中断:"+ cause);
        ctx.close();
    }
}
