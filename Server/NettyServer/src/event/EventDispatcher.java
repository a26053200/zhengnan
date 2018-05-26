package event;

import java.util.HashMap;
import java.util.Vector;

import debug.Debug;


/**    
 * @FileName: EventDispatcher.java  
 * @Package:event  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午1:29:05
 */
public abstract class EventDispatcher {

	public EventDispatcher()
	{
		_eventMap = new HashMap<Integer, Vector<IEventHandler>>();
	}
	private HashMap<Integer, Vector<IEventHandler>> _eventMap;
	
	public void addEventListener(int type,IEventHandler eventHandler)
	{
		if (null == eventHandler) 
			throw new RuntimeException("Can't add null as eventHandler");
		
		Vector<IEventHandler> vec = _eventMap.get(type);
		if(vec == null)
		{
			vec = new Vector<IEventHandler>();
			_eventMap.put(type, vec);
		}
		if(!vec.contains(eventHandler))
		{
			vec.add(eventHandler);
		}else{
			Debug.warning("重复添加事件");
		}
	}
	public void delEventListener(int type,IEventHandler eventHandler)
	{
		if (null == eventHandler) 
			throw new RuntimeException("Can't add null as eventHandler");
		if(_eventMap.get(type) != null)
		{
			Vector<IEventHandler> vec = _eventMap.get(type);
			if(vec.contains(eventHandler))
			{
				vec.remove(eventHandler);
			}
		}
	}
	public Boolean hasEventListener(int type,IEventHandler eventHandler)
	{
		if (null == eventHandler) 
			throw new RuntimeException("Can't add null as eventHandler");
		if(_eventMap.get(type) != null)
		{
			Vector<IEventHandler> vec = _eventMap.get(type);
			return vec.contains(eventHandler);
		}else{
			return false;
		}
	}
	public void dispatchEvent(Event event)
	{
		if (null == event) 
			throw new RuntimeException("Can't add null as evt");
		Vector<IEventHandler> vec = this._eventMap.get(event.type);
		if(vec != null)
		{
			for(IEventHandler handler : vec) 
			{
				event.target = this;
				handler.handleEvent(event);
			}
		}
	}
}
