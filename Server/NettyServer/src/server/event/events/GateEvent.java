package server.event.events;

import business.vo.VoBase;
import event.Event;

/**    
 * @FileName: GateEvent.java  
 * @Package:server.event.events  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午2:06:39
 */
public class GateEvent extends Event {

	protected VoBase voBase;
	
	
	public VoBase getVoBase() {
		return voBase;
	}


	public GateEvent(int type,VoBase voBase)
	{
		super(type);
		this.voBase = voBase;
	}
}
