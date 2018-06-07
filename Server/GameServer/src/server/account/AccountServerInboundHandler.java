package server.account;

/**
 * @ClassName: AccountServerInboundHandler
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/7 11:44
 */

import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.handler.codec.http.*;
import io.netty.handler.codec.http.HttpHeaders.Values;
import org.apache.log4j.Logger;
import utils.BytesUtils;
import utils.StringUtils;

public class AccountServerInboundHandler extends ChannelInboundHandlerAdapter
{

    final static Logger logger = Logger.getLogger(AccountServerInboundHandler.class.getName());

    private AccountMonitor monitor;

    private HttpRequest request = null;

    public AccountServerInboundHandler(AccountMonitor monitor)
    {
        this.monitor = monitor;
    }

    @Override
    public void channelRead(ChannelHandlerContext ctx, Object msg)
            throws Exception
    {
        String res = "";
        if (msg instanceof HttpRequest)
        {
            request = (HttpRequest) msg;
            String uri = request.uri();
            try
            {
                res = uri.substring(1);
                if (!StringUtils.isNullOrEmpty(res))
                {
                    logger.info("[recv]" + res);
                    monitor.recvJson(ctx, res);
                }
            }
            catch (Exception e)
            {//处理出错，返回错误信息
                e.printStackTrace();
                responseError(ctx, "Account Server Error");
            }
        }
        if (msg instanceof HttpContent)
        {
            try
            {
                HttpContent content = (HttpContent) msg;
                ByteBuf buf = content.content();
                res = BytesUtils.readString(buf);
                buf.release();
                if (StringUtils.isNullOrEmpty(res))
                {
                    responseError(ctx, "Http request data is Empty!");
                }
                else
                {
                    logger.info("[recv]" + res);
                    monitor.recvJson(ctx, res);
                }
            }
            catch (Exception e)
            {//处理出错，返回错误信息
                e.printStackTrace();
                responseError(ctx,"Account Server Error");
            }
        }
    }

    private void responseError(ChannelHandlerContext ctx, String errorMsg)
    {
        logger.error(errorMsg);
        ctx.channel().write(BytesUtils.string2Bytes(errorMsg));
    }

    @Override
    public void channelReadComplete(ChannelHandlerContext ctx) throws Exception
    {
        logger.info("http server channelReadComplete..");
        ctx.flush();//刷新后才将数据发出到SocketChannel
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause)
            throws Exception
    {
        logger.error("http server exceptionCaught..");
        ctx.close();
    }

}
