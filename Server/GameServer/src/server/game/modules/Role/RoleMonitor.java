package server.game.modules.Role;

import com.alibaba.fastjson.JSONObject;
import io.netty.channel.ChannelHandlerContext;
import server.common.*;
import server.common.session.Session;
import server.common.session.SessionState;
import server.game.GameMonitor;
import server.game.modules.Player.PlayerMonitor;
import server.game.modules.Player.PlayerVo;
import server.redis.RedisKeys;
import utils.IdGenerator;
import utils.MathUtils;
import utils.TimeUtils;

import java.util.*;

/**
 * @ClassName: RoleMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/1 0:12
 */
public class RoleMonitor extends SubMonitor
{
    private HashMap<String, RoleVo> roleMap;

    public RoleMonitor(GameMonitor base)
    {
        super(base);
        roleMap = new HashMap<>();
    }

    @Override
    public void ActionHandler(ChannelHandlerContext ctx, JSONObject jsonObject, String subAction)
    {
        Session session = new Session(ctx, jsonObject);
        switch (subAction)
        {
            case Action.NONE:

                break;
            case Action.ROLE_RANDOM_NAME:
                randomName(session);
                break;
            case Action.ROLE_CREATE:
                roleCreate(session);
                break;
            case Action.ROLE_SELECT_ENTER_GAME:
                roleSelectAndEnterGame(session);
                break;
        }
    }

    private void randomName(Session session)
    {
        List<String> nameList1 = db.lrange(RedisKeys.role_random_name1, 0, -1);
        List<String> nameList2 = db.lrange(RedisKeys.role_random_name2, 0, -1);
        List<String> nameList3 = db.lrange(RedisKeys.role_random_name3, 0, -1);

        String name1 = nameList1.get(MathUtils.randomInt(0, nameList1.size()));
        String name2 = nameList2.get(MathUtils.randomInt(0, nameList2.size()));
        String name3 = nameList3.get(MathUtils.randomInt(0, nameList3.size()));

        String roleRandomName = name3 + name1 + name2;


        JSONObject sendJson = new JSONObject();
        sendJson.put(FieldName.ROLE_NAME, roleRandomName);
        rspdClient(session, sendJson);
    }

    private void roleCreate(Session session)
    {
        //先检测是否有重名的
        String roleName = session.getParam().getString(0);
        Set<String> keys = db.keys(RedisKeys.role);
        Iterator<String> it = keys.iterator();
        String saveRoleName;
        while (it.hasNext())
        {
            String key = it.next();
            saveRoleName = db.hget(key, RedisKeys.role_name);
            if (roleName.equals(saveRoleName))
            {//角色名已经存在
                session.setState(SessionState.RoleNameHasAlreadyExists);
                return;
            }
        }
        //正式创建
        PlayerVo playerInfo = getPlayerInfo(session);
        RoleSimpleVo role = new RoleSimpleVo();
        long roleId = IdGenerator.getInstance().nextId();
        int currRoleNum = playerInfo.getRoleList().size();
        String roleKey = RedisKeys.role + ":" + playerInfo.getId() + ":" + currRoleNum;
        role.setPrimaryKey(roleKey);
        role.setId(Long.toString(roleId));
        role.setPlayerId(playerInfo.getId());
        role.setRoleName(roleName);
        role.setCreateTime(TimeUtils.new2String());
        role.writeDB(db);
        playerInfo.getRoleList().add(role);
        JSONObject sendJson = new JSONObject();
        sendJson.put(FieldName.ROLE_SIMPLE_INFO, role.toJson());
        rspdClient(session, sendJson);

        //推送玩家信息
        pushClient(session, playerInfo.toJson(), Action.PUSH_PLAYER_INFO);
    }

    private void roleSelectAndEnterGame(Session session)
    {
        PlayerVo playerInfo = getPlayerInfo(session);
        String roleIndex = session.getParam().getString(0);
        RoleSimpleVo simpleRole = playerInfo.getRoleList().get(Integer.parseInt(roleIndex));
        RoleVo role = new RoleVo(simpleRole);
        simpleRole.setOnlineTime(TimeUtils.new2String());
        simpleRole.writeDB(db);
        role.setOnlineTime(new Date().getTime());//设置上线时间(单位:毫秒)
        //返回进入游戏
        rspdClient(session);

        //推送角色信息
        JSONObject sendJson = new JSONObject();
        sendJson.put(FieldName.ROLE_INFO, role.toJson());
        pushClient(session, sendJson, Action.PUSH_ROLE_INFO);
    }

    //根据会话获取玩家信息
    private PlayerVo getPlayerInfo(Session session)
    {
        PlayerVo playerInfo = ((PlayerMonitor) base.getSubMonitor(ModuleNames.Player)).getPlayerInfo(session.getChannelId());
        return playerInfo;
    }
}
