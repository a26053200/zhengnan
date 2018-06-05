package server.login;

import com.alibaba.fastjson.JSONObject;
import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.bytes.ByteArrayDecoder;
import org.apache.log4j.Logger;
import utils.BytesUtils;

import java.util.List;

/**
 * @ClassName: GateServerDecoder
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/1 21:00
 */
public class LoginServerDecoder extends ByteArrayDecoder
{
    Logger logger = Logger.getLogger(LoginServerDecoder.class);
    @Override
    protected void decode(ChannelHandlerContext ctx, ByteBuf bytes, List<Object> out) throws Exception
    {
        super.decode(ctx, bytes, out);

        int msgLen = bytes.readableBytes();
        long packHead = bytes.readUnsignedInt();
        long packLen = packHead;
        String json = BytesUtils.readString(bytes,(int)packLen);
        logger.info(String.format("[recv] msgLen:%d json:%s",msgLen,json));
        //已知JSONObject,目标要转换为json字符串
        JSONObject jsonObject = JSONObject.parseObject(json);

        logger.info(String.format("用户:%s 登陆成功",jsonObject.get("username").toString()));
        //gateMnt.clientReceive(msg);
        //out.add(gateMnt);
    }
}
