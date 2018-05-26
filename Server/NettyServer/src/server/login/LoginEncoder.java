package server.login;

import java.io.Serializable;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.serialization.ObjectEncoder;

/**
 * @FileName: LoginEncoder.java
 * @Package:server.login.codec
 * @Description: TODO
 * @author: zhengnan
 * @date:2018年5月8日 下午1:38:19
 */
public class LoginEncoder extends ObjectEncoder {

	@Override
	protected void encode(ChannelHandlerContext ctx, Serializable msg, ByteBuf out) throws Exception {
		super.encode(ctx, msg, out);
	}
}
