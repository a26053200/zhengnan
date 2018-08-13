package server.game.modules.Role;

import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;
import server.common.BaseVo;
import server.common.interfaces.IDataBaseVo;
import server.redis.RedisKeys;


/**
 * @ClassName: RoleSimpleVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:51
 */
public class RoleSimpleVo extends BaseVo implements IDataBaseVo
{
    private final static String ID = "id";
    private final static String PLAYER_ID = "playerId";
    private final static String ROLE_NAME = "roleName";

    private String id;
    private String playerId;
    private String roleName;
    private String createTime;  //角色创建时间
    private String onlineTime;  //最后一次上线时间
    private String offlineTime; //最后一次下线时间

    public String getId()
    {
        return id;
    }

    public void setId(String id)
    {
        this.id = id;
    }

    public String getPlayerId()
    {
        return playerId;
    }

    public void setPlayerId(String playerId)
    {
        this.playerId = playerId;
    }

    public String getRoleName()
    {
        return roleName;
    }

    public void setRoleName(String roleName)
    {
        this.roleName = roleName;
    }

    public String getCreateTime()
    {
        return createTime;
    }

    public void setCreateTime(String createTime)
    {
        this.createTime = createTime;
    }

    public String getOnlineTime()
    {
        return onlineTime;
    }

    public void setOnlineTime(String onlineTime)
    {
        this.onlineTime = onlineTime;
    }

    public String getOfflineTime()
    {
        return offlineTime;
    }

    public void setOfflineTime(String offlineTime)
    {
        this.offlineTime = offlineTime;
    }

    @Override
    public void fromDB(Jedis db)
    {
        id          = db.hget(primaryKey, RedisKeys.role_id);
        playerId    = db.hget(primaryKey, RedisKeys.role_player_id);
        roleName    = db.hget(primaryKey, RedisKeys.role_name);
        createTime  = db.hget(primaryKey, RedisKeys.role_create_time);
        onlineTime  = db.hget(primaryKey, RedisKeys.role_online_time);
        offlineTime = db.hget(primaryKey, RedisKeys.role_offline_time);
    }

    @Override
    public void writeDB(Jedis db)
    {
        db.hset(primaryKey, RedisKeys.role_id,          id);
        db.hset(primaryKey, RedisKeys.role_player_id,   playerId);
        db.hset(primaryKey, RedisKeys.role_name,        roleName);
        db.hset(primaryKey, RedisKeys.role_create_time, createTime);
        if(onlineTime != null)
            db.hset(primaryKey, RedisKeys.role_online_time, onlineTime);
        if(offlineTime != null)
            db.hset(primaryKey, RedisKeys.role_offline_time,offlineTime);
    }

    @Override
    public JSONObject toJson()
    {
        JSONObject json = new JSONObject();
        json.put(ID, id);
        json.put(PLAYER_ID, playerId);
        json.put(ROLE_NAME, roleName);
        return json;
    }
}
