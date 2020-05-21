package com.betel.center.server.balance;

import com.betel.center.core.consts.ServerName;
import com.betel.config.ServerConfigVo;
import com.betel.servers.center.CenterServer;
import com.betel.servers.http.HttpServer;
import com.betel.utils.ServerTools;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * @ClassName: BalanceServer
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/12/2 23:52
 */
public class BalanceServer extends CenterServer
{

    final static Logger logger = LogManager.getLogger(HttpServer.class);

    public BalanceServer(ServerConfigVo serverConfig)
    {
        super(serverConfig, new BalanceMonitor(serverConfig));
    }

    public static void main(String[] args) throws Exception
    {
        new BalanceServer(ServerTools.createServerConfig(args, ServerName.BALANCE_SERVER)).run();
    }
}
