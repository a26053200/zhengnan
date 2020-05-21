package com.betel.center.server.account.services;

import com.betel.asd.RedisDao;
import com.betel.center.server.account.beans.Account;
import com.betel.center.server.account.dao.AccountDao;
import com.betel.spring.IRedisService;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.List;

/**
 * @Description
 * @Author zhengnan
 * @Date 2020/5/20
 */
public class AccountService implements IRedisService<Account>
{
    @Autowired
    protected AccountDao accountDao;

    @Override
    public void setTableName(String s)
    {
        accountDao.setTableName(s);
    }

    @Override
    public boolean addEntity(Account account)
    {
        return accountDao.addEntity(account);
    }

    @Override
    public boolean batchAddEntity(List<Account> list)
    {
        return accountDao.batchAddEntity(list);
    }

    @Override
    public Account getEntity(String s)
    {
        return accountDao.getEntity(s);
    }

    @Override
    public List<Account> getViceEntities(String s)
    {
        return accountDao.getViceEntities(s);
    }

    @Override
    public boolean updateEntity(Account account)
    {
        return accountDao.updateEntity(account);
    }

    @Override
    public void deleteEntity(List<String> list)
    {

    }

    @Override
    public void deleteEntity(String s)
    {

    }
}
