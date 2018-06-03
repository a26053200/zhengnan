package server.gate;

import common.log.Debug;
import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.bytes.ByteArrayDecoder;
import utils.BytesUtils;

import java.util.List;

/**
 * @ClassName: GateServerDecoder
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:00
 */
public class GateServerDecoder extends ByteArrayDecoder
{
    @Override
    protected void decode(ChannelHandlerContext ctx, ByteBuf bytes, List<Object> out) throws Exception
    {
        super.decode(ctx, bytes, out);

        int msgLen = bytes.readableBytes();
        Debug.info("[recv] msgLen:"+msgLen);
        long packHead = bytes.readUnsignedInt();
        long packLen = packHead;
        String json = BytesUtils.readString(bytes,(int)packLen);
        Debug.info("[recv] json:"+json);
        //gateMnt.clientReceive(msg);
        //out.add(gateMnt);
    }
}
