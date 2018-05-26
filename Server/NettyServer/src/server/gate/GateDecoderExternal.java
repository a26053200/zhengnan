package server.gate;

import business.monitor.GateMnt;
import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.MessageList;
import io.netty.handler.codec.bytes.ByteArrayDecoder;

/**    
 * @FileName: GateDecoderExternal.java  
 * @Package:server.gate  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午10:30:06
 */
public class GateDecoderExternal extends ByteArrayDecoder {

	protected GateMnt gateMnt;
	public GateDecoderExternal(GateMnt gateMnt)
	{
		this.gateMnt = gateMnt;
	}
    @Override
    protected void decode(ChannelHandlerContext ctx, ByteBuf msg, MessageList<Object> out) throws Exception {
    	super.decode(ctx, msg, out);
    	gateMnt.clientReceive(msg);
    	out.add(gateMnt);
    }
}
