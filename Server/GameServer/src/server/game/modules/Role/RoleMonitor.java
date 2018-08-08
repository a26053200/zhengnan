package server.game.modules.Role;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import server.common.Action;
import server.common.Monitor;
import server.common.SubMonitor;

/**
 * @ClassName: RoleMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/1 0:12
 */
public class RoleMonitor extends SubMonitor
{
    public RoleMonitor(Monitor base)
    {
        super(base);
    }

    @Override
    public void ActionHandler(ChannelHandlerContext ctx, JSONObject jsonObject, String subAction)
    {
        switch (subAction)
        {
            case Action.NONE:

                break;
        }
    }
}
