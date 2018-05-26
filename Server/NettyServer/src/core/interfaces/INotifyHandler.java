package core.interfaces;

import server.rspd.ResponderBase;
import server.rt.ReturnBase;

/**    
 * @FileName: INotifyHandler.java  
 * @Package:core.interfaces  
 * @Description: TODO 消息处理器
 * @author: zhengnan   
 * @date:2018年5月4日 上午10:29:10
 */
public interface INotifyHandler {
	ReturnBase handler(ResponderBase rspd);
}
