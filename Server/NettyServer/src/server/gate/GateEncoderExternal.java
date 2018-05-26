package server.gate;

import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.MessageList;
import io.netty.handler.codec.bytes.ByteArrayEncoder;

/**    
 * @FileName: GateEncodeExternal.java  
 * @Package:server.gate  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午10:34:32
 */
public class GateEncoderExternal extends ByteArrayEncoder {

	@Override
	protected void encode(ChannelHandlerContext ctx, byte[] msg,
			MessageList<Object> out) throws Exception {
		// TODO Auto-generated method stub
		super.encode(ctx, msg, out);
	}

}
