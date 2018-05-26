package core.interfaces;

import io.netty.buffer.ByteBuf;


/**    
 * @FileName: Ivo.java  
 * @Package:core.interfaces  
 * @Description: TODO 结构读取接口
 * @author: zhengnan   
 * @date:2018年5月3日 上午11:40:40
 */
public interface Ivo {
	void fromBuffrtReader(ByteBuf bytes);
}
