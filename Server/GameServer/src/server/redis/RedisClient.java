/**
 * @ClassName: RedisClient
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/5 1:07
 */
package server.redis;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import redis.clients.jedis.Jedis;
import redis.clients.jedis.JedisPool;
import redis.clients.jedis.JedisPoolConfig;
import redis.clients.jedis.JedisShardInfo;
import redis.clients.jedis.ShardedJedis;
import redis.clients.jedis.ShardedJedisPool;
import redis.clients.jedis.SortingParams;



public class RedisClient
{
    private static  Jedis jedis;

    public static void main(String[] args)
    {
        jedis = new Jedis("localhost");
        jedis.select(0);
    }
}