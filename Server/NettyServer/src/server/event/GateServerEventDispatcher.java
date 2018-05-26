package server.event;

import event.EventDispatcher;

/**    
 * @FileName: GateServerEventDispatcher.java  
 * @Package:server.events  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午2:02:14
 */
public class GateServerEventDispatcher extends EventDispatcher {

	private static GateServerEventDispatcher _instance;
	
	public static GateServerEventDispatcher getInstance()
	{
		if(_instance == null)
			_instance = new GateServerEventDispatcher();
		return _instance;
	}
}
