package business.monitor;

import core.interfaces.INotifyHandler;
import debug.Debug;
import event.EventDispatcher;
import event.IEventHandler;
import io.netty.channel.Channel;
import io.netty.channel.group.ChannelGroup;
import business.rspd.LoginRspd;
import business.rt.LoginRt;
import business.vo.UserVo;
import server.dict.GateCmdDict;
import server.dict.GateHubCmdDict;
import server.event.GateServerEventDispatcher;
import server.event.events.GateEvent;
import server.monitor.MonitorBase;
import server.rspd.ResponderBase;
import server.rt.ReturnBase;

/**    
 * @FileName: GateMnt.java  
 * @Package:server.monitor  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午9:55:28
 */
public class GateMnt extends MonitorBase {

	protected Channel currChannel;//当前正在通信的
	
	protected EventDispatcher gateEDP;
	
	protected INotifyHandler regLoginNotifyHandler;
	protected IEventHandler<GateEvent> login2ClientNotifyHandler;
	
	public GateMnt(ChannelGroup channelGroup)
	{
		super(channelGroup);
	}
	@Override
	protected void registNotify() {
		
		gateEDP = GateServerEventDispatcher.getInstance();
		
		initHandler();
		
		this.regRspd(GateCmdDict.REQ_LOGIN, LoginRspd.class.getName());
		
		this.addRspdNotify(GateCmdDict.REQ_LOGIN, regLoginNotifyHandler);
		
		gateEDP.addEventListener(GateHubCmdDict.HUB_LOGIN_TO_CLIENT,login2ClientNotifyHandler);
	}
	@Override
	public byte[] notifyRspd(Channel ch)
	{
		currChannel = ch;
		return super.ending(currRspd);
	}
	@Override
	protected void initHandler()
	{
		regLoginNotifyHandler = new INotifyHandler()
		{
			@Override
			public ReturnBase handler(ResponderBase rspd) {
				LoginRspd _rspd = (LoginRspd) rspd;
				if(_rspd!=null)
				{
					Debug.log(_rspd.userName + "登陆网关成功");
					UserVo vo = new UserVo();
					vo.valueFromRspd(_rspd, currChannel);//填充数据
					vo.tempId = setChannelMap(currChannel, vo.tempId);//
					gateEDP.dispatchEvent(new GateEvent(GateHubCmdDict.HUB_CLIENT_TO_LOGIN, vo));
				}
				return null;
			}
		};
		login2ClientNotifyHandler = new IEventHandler<GateEvent>()
		{
			@Override
			public void handleEvent(GateEvent event) {
				UserVo vo = (UserVo)event.getVoBase();
				Channel ch = userChannelMap.get(vo.tempId);
				LoginRt rt = new LoginRt(GateCmdDict.RT_LOGIN);
				rt.send(ch);
				Debug.log("发送数据:Gateway ---> Client");
			}
		};
	}
}
