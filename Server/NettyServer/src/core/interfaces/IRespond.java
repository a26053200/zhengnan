package core.interfaces;

import io.netty.buffer.ByteBuf;

/**    
 * @FileName: IRespond.java  
 * @Package:core.interfaces  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月3日 下午8:55:17
 */
public interface IRespond {
	/**
	 * 接收数据
	 * receive 接收数据<br>
	 * 接收数据<br>
	 */
	
	void receive(ByteBuf bytes) ;
	/**
	 * 设置包头<br>
	 */
	void setPackHead(int mainCmd, int subCmd, int rspdTime);
}
