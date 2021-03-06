package com.betel.center.server.account;

import com.betel.center.core.consts.ServerName;
import com.betel.common.Monitor;
import com.betel.config.ServerConfigVo;
import com.betel.servers.forward.ServerClient;
import com.betel.servers.node.NodeServer;
import com.betel.utils.ServerTools;

/**
 * @ClassName: AccountServer
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/12/3 22:20
 */
public class AccountServer extends NodeServer
{

    public AccountServer(ServerConfigVo serverConfig,  Monitor monitor)
    {
        super(serverConfig, monitor);
    }

    public static void main(String[] args) throws Exception
    {
        ServerConfigVo accountSrvCfg = ServerTools.createServerConfig(args, ServerName.ACCOUNT_SERVER,ServerName.BALANCE_SERVER);
        AccountMonitor mnt = new AccountMonitor(accountSrvCfg);
        AccountServer server = new AccountServer(accountSrvCfg,mnt);
        server.setServerClient(new ServerClient(accountSrvCfg, mnt));
        server.run();
    }
}
