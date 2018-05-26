package server.login;

import org.apache.log4j.Logger;

import business.monitor.LoginMnt;
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
 * @FileName: LoginServerHandler.java  
 * @Package:server.login  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 上午9:43:57
 */
public class LoginServerHandler extends ChannelInboundHandlerAdapter{

	private static final String name = "LOGIN_SERVER_HANDLER";

	public static String getName() {
		return name;
	}

	private static Logger logger = Logger.getLogger(LoginServerHandler.class.getName());
	private static final ChannelGroup channels = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);

	protected LoginMnt loginMnt;
	@Override
	public void channelActive(ChannelHandlerContext ctx) throws Exception {
		super.channelActive(ctx);
		
		logger.info("channelActive:新网关连接:"+ctx.channel().remoteAddress().toString());
		
		channels.add(ctx.channel());
		
		if(loginMnt == null)
		{
			loginMnt = new LoginMnt(channels);
		}
	}
	@Override
	public void channelInactive(ChannelHandlerContext ctx) throws Exception {
		// TODO Auto-generated method stub
		super.channelInactive(ctx);
		
		logger.info("channelInactive:网关客户端断开连接登陆服务器:"+ctx);
		
		loginMnt.removeChannel(ctx.channel().id());
	}
	@Override
    public void messageReceived(ChannelHandlerContext ctx, MessageList<Object> msgs) throws Exception {
        // Echo back the received object to the client.
		Debug.log("收到数据:LoginServer <--- Gateway");
		for(Object obj : msgs)
		{
			VoBase vo = loginMnt.voReceive(obj,ctx);
			if(vo == null)
			{
				ctx.write(new ErrorVo("收到数据不能序列化"+msgs));
			}
		}
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
        Debug.log("登陆服务器异常中断"+ cause);
        ctx.close();
    }

}
