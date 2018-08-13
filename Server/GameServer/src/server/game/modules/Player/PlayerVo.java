package server.game.modules.Player;

import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;
import server.common.BaseVo;
import server.common.interfaces.IDataBaseVo;
import server.game.modules.Role.RoleSimpleVo;
import server.redis.RedisKeys;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

/**
 * @ClassName: PlayerVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:47
 */
public class PlayerVo extends BaseVo implements IDataBaseVo
{
    private final static String ID = "id";
    private final static String ROLE_LIST = "roleList";

    private String id;
    private String accountId;
    private List<RoleSimpleVo> roleList; //玩家所创建的角色列表
    private String registerTime; //玩家第一次登陆游戏时间,即玩家在该服务器的注册时间
    private String lastLoginTime;//最后一次登陆时间
    private String lastLogoutTime;//最后一次登出时间

    public PlayerVo(String key)
    {
        super(key);
        roleList = new ArrayList<>();
        primaryKey = RedisKeys.player + ":" + key;
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

    public List<RoleSimpleVo> getRoleList()
    {
        return roleList;
    }

    @Override
    public void fromDB(Jedis db)
    {
        this.db = db;
        if (db.hgetAll(primaryKey).isEmpty())
            isEmpty = true;
        else
        {
            id              = db.hget(primaryKey, RedisKeys.player_id);
            registerTime    = db.hget(primaryKey, RedisKeys.player_register_time);
            lastLoginTime   = db.hget(primaryKey, RedisKeys.player_login_time);
            lastLogoutTime  = db.hget(primaryKey, RedisKeys.player_logout_time);
            //已经创建好的角色
            String rolePrimaryKey = RedisKeys.role + ":" + id+"*";
            Set<String> keys = db.keys(rolePrimaryKey);
            Iterator<String> it = keys.iterator() ;
            while(it.hasNext())
            {
                String key = it.next();
                System.out.println(key);
                RoleSimpleVo role = new RoleSimpleVo();
                role.setPrimaryKey(key);
                role.fromDB(db);
                roleList.add(role);
            }
        }
    }

    @Override
    public void writeDB(Jedis db)
    {
        isEmpty = false;
        db.hset(primaryKey, RedisKeys.player_id,             id);
        db.hset(primaryKey, RedisKeys.player_account_id,     accountId);
        db.hset(primaryKey, RedisKeys.player_register_time,  registerTime);
        db.hset(primaryKey, RedisKeys.player_login_time,     lastLoginTime);
        if(lastLogoutTime != null)
            db.hset(primaryKey, RedisKeys.player_logout_time, lastLogoutTime);
    }

    @Override
    public JSONObject toJson()
    {
        JSONObject json = new JSONObject();
        json.put(ID, id);
        json.put(ROLE_LIST, list2jsonArray(roleList));
        return json;
    }
}
