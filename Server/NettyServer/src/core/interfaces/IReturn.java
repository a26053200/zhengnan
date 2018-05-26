package core.interfaces;

import io.netty.channel.Channel;


/**    
 * @FileName: IReturn.java  
 * @Package:core.interfaces  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月9日 上午9:19:45
 */
public interface IReturn {

	void send(Channel ch);
}
