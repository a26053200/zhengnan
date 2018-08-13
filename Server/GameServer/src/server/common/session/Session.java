package server.common.session;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import server.common.Action;
import server.common.FieldName;
import server.common.ParamParser;
import server.common.ServerConstant;

/**
 * @ClassName: Session
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/9 23:21
 */
public class Session
{
    //通信信道
    private ChannelHandlerContext context;
    //接收到的josn
    private JSONObject recvJson;
    //会话状态
    private SessionStatus status;
    //会话状态
    private SessionState state;
    //接收到请参数
    private ParamParser param;
    //信道id
    private String channelId;
    //请求Action
    private String rqstAction;

    public Session(ChannelHandlerContext ctx, JSONObject recvJson)
    {
        this.context = ctx;
        this.recvJson = recvJson;
        this.state = SessionState.Success;
        this.status = SessionStatus.Free;

        this.param = new ParamParser(recvJson);
        this.channelId = recvJson.getString(FieldName.CHANNEL_ID);
        this.rqstAction = recvJson.getString(Action.NAME);
    }

    public ChannelHandlerContext getContext()
    {
        return context;
    }

    public JSONObject getRecvJson()
    {
        return recvJson;
    }

    public SessionStatus getStatus()
    {
        return status;
    }

    public SessionState getState()
    {
        return state;
    }

    public void setState(SessionState state)
    {
        this.state = state;
    }

    public void setStatus(SessionStatus status)
    {
        this.status = status;
    }

    public ParamParser getParam()
    {
        return param;
    }

    public String getChannelId()
    {
        return channelId;
    }

    public String getRqstAction()
    {
        return rqstAction;
    }
}
