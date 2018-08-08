package server.game.modules.Player;

import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;
import server.common.BaseVo;
import server.redis.RedisKeys;

import java.util.ArrayList;
import java.util.List;

/**
 * @ClassName: PlayerVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:47
 */
public class PlayerVo extends BaseVo
{
    private String id;
    private String accountId;
    private List<String> roleIdList; //玩家所创建的角色列表
    private String registerTime; //玩家第一次登陆游戏时间,即玩家在该服务器的注册时间
    private String lastLoginTime;//最后一次登陆时间
    private String lastLogoutTime;//最后一次登出时间

    public PlayerVo()
    {
        roleIdList = new ArrayList<String>();
    }

    public String getId()
    {
        return id;
    }

    public void setId(String id)
    {
        this.id = id;
    }

    public String getAccountId()
    {
        return accountId;
    }

    public void setAccountId(String accountId)
    {
        this.accountId = accountId;
    }

    public List<String> getRoleIdList()
    {
        return roleIdList;
    }

    public void addRoleId(String roleId)
    {
        this.roleIdList.add(roleId);
    }

    public String getRegisterTime()
    {
        return registerTime;
    }

    public void setRegisterTime(String registerTime)
    {
        this.registerTime = registerTime;
    }

    public String getLastLoginTime()
    {
        return lastLoginTime;
    }

    public void setLastLoginTime(String lastLoginTime)
    {
        this.lastLoginTime = lastLoginTime;
    }

    public String getLastLogoutTime()
    {
        return lastLogoutTime;
    }

    public void setLastLogoutTime(String lastLogoutTime)
    {
        this.lastLogoutTime = lastLogoutTime;
    }

    @Override
    public void fromDB(Jedis db, String key)
    {
        if (db.hgetAll(key).isEmpty())
            isEmpty = true;
        else
        {
            accountId = key;
            id = db.hget(key, RedisKeys.player_id);
            registerTime = db.hget(key, RedisKeys.player_register_time);
            lastLoginTime = db.hget(key, RedisKeys.player_login_time);
            lastLogoutTime = db.hget(key, RedisKeys.player_logout_time);
            roleIdList = db.lrange(id,0, -1);
        }
    }

    @Override
    public void writeDB(Jedis db)
    {
        isEmpty = false;
        db.hset(accountId, RedisKeys.player_id,             id);
        db.hset(accountId, RedisKeys.player_account_id,     accountId);
        db.hset(accountId, RedisKeys.player_register_time,  registerTime);
        db.hset(accountId, RedisKeys.player_login_time,     lastLoginTime);
        if(lastLogoutTime != null)
            db.hset(accountId, RedisKeys.player_logout_time, lastLogoutTime);
        for (int i = 0; i < roleIdList.size(); i++)
            db.lpush(id,roleIdList.get(i));
    }

    @Override
    public JSONObject toJson()
    {
        JSONObject json = new JSONObject();
        json.put(RedisKeys.player_id, id);
        //json.put("aid", account_id);
        //json.put("token", JwtHelper.createJWT(account_id));
        JSONArray srvList = new JSONArray();
        for (int i = 0; i < roleIdList.size(); i++)
        {
            srvList.add(roleIdList.get(i));
        }
        json.put("srvList", srvList);
        return json;
    }
}
