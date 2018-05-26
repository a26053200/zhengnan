package core.interfaces;

import io.netty.channel.ChannelHandlerContext;
import business.vo.VoBase;

/**    
 * @FileName: IVoNotifHandler.java  
 * @Package:core.interfaces  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 下午8:25:56
 */
public interface IVoNotifHandler {

	VoBase handler(VoBase vo, ChannelHandlerContext ctx);
}
