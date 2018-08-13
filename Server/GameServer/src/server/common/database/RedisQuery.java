package server.common.database;

import redis.clients.jedis.Jedis;
import server.common.BaseVo;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

/**
 * @ClassName: RedisQuery
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/12 21:36
 */
public interface RedisQuery
{

    public <T extends BaseVo> List<T> String2Int(Jedis db, String patternKey);
}
