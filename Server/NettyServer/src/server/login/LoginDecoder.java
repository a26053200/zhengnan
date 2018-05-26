package server.login;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.serialization.ClassResolver;
import io.netty.handler.codec.serialization.ObjectDecoder;

/**    
 * @FileName: LoginDecoder.java  
 * @Package:server.login.codec  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午1:37:48
 */
public class LoginDecoder extends ObjectDecoder {

    public LoginDecoder(ClassResolver classResolver) {
		super(classResolver);
		// TODO Auto-generated constructor stub
	}

	@Override
    protected Object decode(ChannelHandlerContext ctx, ByteBuf in) throws Exception {
    	return super.decode(ctx, in);
    }
}
