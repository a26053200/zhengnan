package server.event;

import event.EventDispatcher;

/**    
 * @FileName: ServerEventDispatcher.java  
 * @Package:server.events  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午1:18:19
 */
public class ServerEventDispatcher extends EventDispatcher{

	private static ServerEventDispatcher _instance;
	
	public static ServerEventDispatcher getInstance()
	{
		if(_instance == null)
			_instance = new ServerEventDispatcher();
		return _instance;
	}
}
