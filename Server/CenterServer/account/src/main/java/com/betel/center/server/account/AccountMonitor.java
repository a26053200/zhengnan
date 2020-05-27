package com.betel.center.server.account;

import com.betel.center.core.consts.Bean;
import com.betel.center.server.account.beans.Account;
import com.betel.config.ServerConfigVo;
import com.betel.center.server.account.services.AccountService;
import com.betel.servers.action.ImplAction;
import com.betel.servers.node.NodeServerMonitor;
import org.springframework.context.ApplicationContext;
import org.springframework.context.support.ClassPathXmlApplicationContext;

/**
 * @ClassName: AccountMonitor
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/12/3 22:20
 */
public class AccountMonitor extends NodeServerMonitor
{
    public AccountMonitor(ServerConfigVo serverCfgInfo)
    {
        super(serverCfgInfo);
        ApplicationContext applicationContext = new ClassPathXmlApplicationContext("applicationContext.xml");
        AccountService accountService = (AccountService) applicationContext.getBean("accountService");

        pushService(Account.class, new AccountBusiness(), accountService);
    }
}
