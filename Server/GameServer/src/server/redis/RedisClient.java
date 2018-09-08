/**
 * @ClassName: RedisClient
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/5 1:07
 */
package server.redis;

import org.apache.log4j.Logger;
import redis.clients.jedis.Jedis;
import server.gate.GateClientHandler;


public class RedisClient
{
    private static final Logger logger = Logger.getLogger(RedisClient.class);

    private static RedisClient s_instance = null;

    public static RedisClient getInstance()
    {
        if (s_instance == null)
            s_instance = new RedisClient();
        return s_instance;
    }

    private Jedis jedis;

    public RedisClient()
    {
    }

    public Jedis connectDB(String host)
    {
        if (jedis == null)
            jedis = connectDB(host, 6379);
        return jedis;
    }

    public Jedis connectDB(String host, int port)
    {
        if (jedis == null)
            jedis = new Jedis(host, port);
        logger.info(String.format("Redis Database connection success! connection info:%s:%d", host, port));
        return jedis;
    }

    public Jedis getDB(int index)
    {
        jedis.select(index);
        return jedis;
    }

    //测试
    public static void main(String[] args)
    {
        /*
        //连接本地的 Redis 服务
        Jedis jedis = new Jedis("localhost");
        System.out.println("连接成功");
        //设置 redis 字符串数据
        jedis.set("runoobkey", "www.runoob.com");
        // 获取存储的数据并输出
        System.out.println("redis 存储的字符串为: " + jedis.get("runoobkey"));
        */
        float houseSize = 100;
        float destPrice = 50000;
        float depositPerMon = 20000;
        float srcPrice = 40000;
        float rise = 0.01f;
        float repaymentPerMon = 0.003f;//每个月还0.003;
        float decoration = 300000;//装修
        cal(100, 50000, 20000, 40000, 0.01f, 1000000, 0.0033333333f, 300000);
    }

    public static void cal(float houseSize, float destPrice,
                           float depositPerMon, float srcPrice,
                           float rise, float loan, float repaymentPerMon,
                           float decoration)
    {
        float destTotal = houseSize * destPrice;
        float depositTotal = 0;
        float lastPrice = srcPrice;
        float lastSell = 0;
        float residueRepayment = 0;
        float total = 0;
        float residue = 0;
        int count = 1;
        System.out.println(String.format("目标房子:%f 贷款:%f 需要存款:%f", destTotal, loan, destTotal - loan));
        do
        {
            residueRepayment = 904000f - 1000000f * repaymentPerMon * count;
            lastPrice = lastPrice * (1f + rise);
            lastSell = 89f * lastPrice;
            depositTotal = count * depositPerMon;
            total = depositTotal + lastSell;
            System.out.println(String.format("第%d个月,存款总额:%f 卖房所得:%f 总计:%f 剩余贷款:%f 实际存款:%f",
                    count, depositTotal, lastSell, total, residueRepayment, total - residueRepayment));
            residue = (total - residueRepayment) - (destTotal - loan);
            count++;
        }
        while (total - residueRepayment < destTotal - loan || residue < decoration);
        System.out.println(String.format("买房后剩余用来装修的存款:%f 所需房价:%f", residue, lastSell / 89f));
    }
}