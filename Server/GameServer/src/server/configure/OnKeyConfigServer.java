package server.configure;

import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import common.log.Debug;
import redis.clients.jedis.Jedis;
import server.redis.RedisClient;

/**
 * @ClassName: OnKeyConfigServer
 * @Description: 一键配置服务器
 * @Author: zhengnan
 * @Date: 2018/6/7 23:59
 */
public class OnKeyConfigServer
{
    public static void main(String[] args)
    {
        Debug.initLog("[OnKeyConfigServer]");
        //连接数据库
        RedisClient.getInstance().connectDB("127.0.0.1");

        Jedis db = RedisClient.getInstance().getDB(0);

        JSONObject gameServerJson = new JSONObject();
        JSONArray ja = new JSONArray();

        //1服
        JSONObject gameServer1 = new JSONObject();
        gameServer1.put("name","zhengnan 1");
        gameServer1.put("host","127.0.0.1");
        gameServer1.put("port",8081);
        ja.add(gameServer1);
        //2服
        JSONObject gameServer2 = new JSONObject();
        gameServer2.put("name","zhengnan 2");
        gameServer2.put("host","127.0.0.1");
        gameServer2.put("port",8082);
        ja.add(gameServer2);

        gameServerJson.put("list",ja);
        db.set("GameServer",gameServerJson.toJSONString());
    }
}
