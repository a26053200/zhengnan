package business.monitor;

import core.interfaces.IVoNotifHandler;
import debug.Debug;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.group.ChannelGroup;
import business.vo.UserVo;
import business.vo.VoBase;
import server.dict.LoginServerCmdDict;
import server.monitor.MonitorBase;


/**    
 * @FileName: LoginMnt.java  
 * @Package:server.monitor  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午8:07:02
 */
public class LoginMnt extends MonitorBase {

	protected IVoNotifHandler gateLoginNotifyHandler;
	
	public LoginMnt(ChannelGroup channelGroup)
	{
		super(channelGroup);
	}
	@Override
	protected void registNotify() {
		
		this.regVO(LoginServerCmdDict.REQ_GATE_LOGIN, UserVo.class);
		
		this.addVoNotify(LoginServerCmdDict.REQ_GATE_LOGIN,gateLoginNotifyHandler);
	}

	@Override
	public byte[] notifyRspd(Channel ch)
	{
		return super.ending(currRspd);
	}
	@Override
	protected void initHandler() {
		// TODO Auto-generated method stub
		gateLoginNotifyHandler = new IVoNotifHandler() 
		{
			@Override
			public VoBase handler(VoBase vo, ChannelHandlerContext ctx) {
				UserVo user = (UserVo)vo;
				user.tempId = setChannelMap(ctx.channel(), user.tempId);
				vo.setCmd(LoginServerCmdDict.RT_GATE_LOGIN);
				Debug.log(user.userName + "登陆登陆服务器成功");
				ctx.channel().write(vo);//返回给网关
				Debug.log("发送数据:LoginServer ---> Gateway");
				return null;
			}
		};
	}
}
