package business.monitor;

import io.netty.channel.Channel;
import io.netty.channel.group.ChannelGroup;
import server.monitor.MonitorBase;

/**    
 * @FileName: GameMnt.java  
 * @Package:business.monitor  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月15日 下午8:08:51
 */
public class GameMnt extends MonitorBase {

	public GameMnt(ChannelGroup channelGroup) {
		super(channelGroup);
		// TODO Auto-generated constructor stub
	}

	@Override
	protected void initHandler() {
		// TODO Auto-generated method stub

	}

	@Override
	protected void registNotify() {
		// TODO Auto-generated method stub

	}

	@Override
	public byte[] notifyRspd(Channel ch) {
		// TODO Auto-generated method stub
		return null;
	}

}
