package event;
/**    
 * @FileName: Event.java  
 * @Package:event  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午1:32:42
 */
public abstract class Event {

	protected Object target;
	
	protected int type;

	public Object getTarget() {
		return target;
	}

	public int getType() {
		return type;
	}
	
	public Event(int type)
	{
		this.type = type;
	}
	
	/**
	 * 取消事件默认行为（阻止事件发生）
	 * preventDefault方法的作用<br>
	 * 方法适用条件<br> 
	 * @return void
	 * @exception 
	 * @since  1.0.0
	 */
	public void preventDefault()
	{
		
	}
	
}
