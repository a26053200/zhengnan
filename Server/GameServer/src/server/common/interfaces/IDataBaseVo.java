package server.common.interfaces;

import redis.clients.jedis.Jedis;

/**
 * @ClassName: IDataBaseVo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/14 0:24
 */
public interface IDataBaseVo
{
    void fromDB(Jedis db);
    void writeDB(Jedis db);
}
