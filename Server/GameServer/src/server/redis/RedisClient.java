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
    private static RedisClient s_instance = null;

    public static RedisClient getInstance()
    {
        if(s_instance == null)
            s_instance = new RedisClient();
        return s_instance;
    }

    private Jedis jedis;
    public RedisClient()
    {
    }

    public Jedis createDB(String host)
    {
        if(jedis == null)
            jedis = createDB(host,6379);
        return jedis;
    }
    public Jedis createDB(String host,int port)
    {
        if(jedis == null)
            jedis = new Jedis(host,port);
        return jedis;
    }

    public Jedis getDB(int index)
    {
        jedis.select(index);
        return  jedis;
    }
    //测试
    public static void main(String[] args)
    {
        //连接本地的 Redis 服务
        Jedis jedis = new Jedis("localhost");
        System.out.println("连接成功");
        //设置 redis 字符串数据
        jedis.set("runoobkey", "www.runoob.com");
        // 获取存储的数据并输出
        System.out.println("redis 存储的字符串为: "+ jedis.get("runoobkey"));
    }
}