package server.common;

import com.alibaba.fastjson.JSONObject;
import redis.clients.jedis.Jedis;

/**
 * @ClassName: BaseVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 21:55
 */
public abstract class BaseVo
{
    protected boolean isEmpty = false;

    public boolean isEmpty()
    {
        return isEmpty;
    }

    public abstract void fromDB(Jedis db, String key);

    public abstract void writeDB(Jedis db);

    public abstract JSONObject toJson();
}
