package server.game.modules.Role;

import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;
import server.common.BaseVo;
import server.redis.RedisKeys;


/**
 * @ClassName: RoleVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:51
 */
public class RoleVo extends BaseVo
{
    private String id;
    private String playerId;
    private String roleName;

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

    @Override
    public void fromDB(Jedis db, String key)
    {
        if(db.hgetAll(key) == null)
            isEmpty = true;
        else
        {
            id = db.hget(key, RedisKeys.role_id);
            playerId = key;
            roleName = db.hget(key,RedisKeys.role_name);
        }
    }

    @Override
    public void writeDB(Jedis db)
    {
        db.hset(playerId, RedisKeys.role_id, id);
        db.hset(playerId, RedisKeys.role_player_id, playerId);
        isEmpty = false;
    }

    @Override
    public JSONObject toJson()
    {
        return null;
    }
}
