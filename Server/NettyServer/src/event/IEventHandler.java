package event;
/**    
 * @FileName: IEventHandler.java  
 * @Package:event  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月11日 下午1:34:45
 */
public interface IEventHandler<T> {

	void handleEvent(T event);
}
