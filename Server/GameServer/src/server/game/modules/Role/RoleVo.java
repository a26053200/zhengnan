package server.game.modules.Role;

import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;
import server.common.BaseVo;
import server.common.interfaces.IDataBaseVo;
import server.redis.RedisKeys;


/**
 * @ClassName: RoleVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:51
 */
public class RoleVo extends BaseVo implements IDataBaseVo
{
    private final static String ID = "id";
    private final static String PLAYER_ID = "playerId";
    private final static String ROLE_NAME = "roleName";
    private final static String ONLINE_TIME = "onlineTime";

    private String id;
    private String playerId;
    private String roleName;
    private long onlineTime;  //最后一次上线时间

    public RoleVo(RoleSimpleVo simpleRole)
    {
        id = simpleRole.getId();
        playerId = simpleRole.getPlayerId();
        roleName = simpleRole.getRoleName();
    }
    public String getId()
    {
        return id;
    }

    public String getPlayerId()
    {
        return playerId;
    }

    public String getRoleName()
    {
        return roleName;
    }

    public long getOnlineTime()
    {
        return onlineTime;
    }

    public void setOnlineTime(long onlineTime)
    {
        this.onlineTime = onlineTime;
    }

    @Override
    public void fromDB(Jedis db)
    {

    }

    @Override
    public void writeDB(Jedis db)
    {

    }

    @Override
    public JSONObject toJson()
    {
        JSONObject json = new JSONObject();
        json.put(ID, id);
        json.put(PLAYER_ID, playerId);
        json.put(ROLE_NAME, roleName);
        json.put(ONLINE_TIME, onlineTime);
        return json;
    }
}
