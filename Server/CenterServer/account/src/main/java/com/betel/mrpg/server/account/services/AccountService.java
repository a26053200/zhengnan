package com.betel.mrpg.server.account.services;

import com.betel.asd.BaseVo;
import com.betel.asd.RedisDao;
import com.betel.center.core.sdr.IUserDao;
import com.betel.mrpg.server.account.beans.Account;
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
    private IUserDao userDao;

    @Override
    public RedisDao<Account> getDao()
    {
        return null;
    }

    @Override
    public void setDao(RedisDao<Account> redisDao)
    {

    }

    @Override
    public boolean addEntity(Account account)
    {
        return false;
    }

    @Override
    public boolean batchAddEntity(List<Account> list)
    {
        return false;
    }

    @Override
    public Account getEntity(String s)
    {
        return null;
    }

    @Override
    public List<Account> getViceEntities(String s)
    {
        return null;
    }

    @Override
    public boolean updateEntity(Account account)
    {
        return false;
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
