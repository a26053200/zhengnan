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

public class AccountServerInboundHandler extends ChannelInboundHandlerAdapter
{

    final static Logger logger = Logger.getLogger(AccountServerInboundHandler.class.getName());

    private AccountMonitor monitor;

    private HttpRequest request = null;
    private FullHttpResponse response = null;

    public AccountServerInboundHandler(AccountMonitor monitor)
    {
        this.monitor = monitor;
    }

    @Override
    public void channelRead(ChannelHandlerContext ctx, Object msg)
            throws Exception
    {
        if (msg instanceof HttpRequest) {
            request = (HttpRequest) msg;
            String uri = request.uri();
            String res = "";
            try {
                res = uri.substring(1);
                response = new DefaultFullHttpResponse(HttpVersion.HTTP_1_1, HttpResponseStatus.OK, Unpooled.wrappedBuffer(res.getBytes("UTF-8")));
            } catch (Exception e) {//处理出错，返回错误信息
                res = "<html><body>Server Error</body></html>";
                response = new DefaultFullHttpResponse(HttpVersion.HTTP_1_1, HttpResponseStatus.OK, Unpooled.wrappedBuffer(res.getBytes("UTF-8")));
                setHeaders(response);
            }
            if (response != null)
                ctx.write(response);
            logger.info(res);
        }
        if (msg instanceof HttpContent) {
            HttpContent content = (HttpContent) msg;
            ByteBuf buf = content.content();
            logger.info(BytesUtils.readString(buf));
            buf.release();
        }
    }
    /**
     * 设置HTTP返回头信息
     */
    private void setHeaders(FullHttpResponse response) {
        response.headers().set(HttpHeaders.Names.CONTENT_TYPE, "text/html");
        response.headers().set(HttpHeaders.Names.CONTENT_LANGUAGE, response.content().readableBytes());
        if (HttpHeaders.isKeepAlive(request)) {
            response.headers().set(HttpHeaders.Names.CONNECTION, Values.KEEP_ALIVE);
        }
    }
    @Override
    public void channelReadComplete(ChannelHandlerContext ctx) throws Exception
    {
        System.out.println("server channelReadComplete..");
        ctx.flush();//刷新后才将数据发出到SocketChannel
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause)
            throws Exception
    {
        System.out.println("server exceptionCaught..");
        ctx.close();
    }

}
