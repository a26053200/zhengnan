package business.monitor;

import core.interfaces.IVoNotifHandler;
import business.vo.UserVo;
import business.vo.VoBase;
import debug.Debug;
import event.EventDispatcher;
import event.IEventHandler;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import server.dict.GateHubCmdDict;
import server.dict.LoginServerCmdDict;
import server.event.GateServerEventDispatcher;
import server.event.events.GateEvent;
import server.monitor.MonitorBase;

/**    
 * @FileName: GateClientMnt.java  
 * @Package:business.monitor  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 上午10:13:03
 */
public class GateClientMnt extends MonitorBase {

	protected EventDispatcher gateEDP;
	
	protected IEventHandler<GateEvent> client2LoginNotifyHandler;
	
	protected IVoNotifHandler loginServerRtNotifyHandler;
	
	public GateClientMnt(Channel channel)
	{
		super(channel);
	}
	@Override
	public void registNotify() {

		gateEDP = GateServerEventDispatcher.getInstance();
		
		this.regVO(LoginServerCmdDict.RT_GATE_LOGIN, UserVo.class);
		
		this.addVoNotify(LoginServerCmdDict.RT_GATE_LOGIN,loginServerRtNotifyHandler);
		
		gateEDP.addEventListener(GateHubCmdDict.HUB_CLIENT_TO_LOGIN,client2LoginNotifyHandler);
	}

	@Override
	protected void initHandler()
	{
		client2LoginNotifyHandler = new IEventHandler<GateEvent>() 
		{
			@Override
			public void handleEvent(GateEvent event)
			{
				UserVo vo = (UserVo)event.getVoBase();
				vo.setCmd(LoginServerCmdDict.REQ_GATE_LOGIN);
				clientChannel.write(vo);
				Debug.log("发送数据:LoginServer <--- Gateway");
			}
		};
		loginServerRtNotifyHandler = new IVoNotifHandler() {
			
			@Override
			public VoBase handler(VoBase vo, ChannelHandlerContext ctx) {
				gateEDP.dispatchEvent(new GateEvent(GateHubCmdDict.HUB_LOGIN_TO_CLIENT, vo));
				return null;
			}
		};
	}
	
	@Override
	public byte[] notifyRspd(Channel ch) {
		// TODO Auto-generated method stub
		return null;
	}
}
