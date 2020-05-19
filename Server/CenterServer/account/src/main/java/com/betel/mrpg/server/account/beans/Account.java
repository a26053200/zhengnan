package com.betel.mrpg.server.account.beans;

import com.betel.asd.BaseVo;

/**
 * @ClassName: Account
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/12/5 22:57
 */
public class Account extends BaseVo
{
    private String username;
    private String password;
    private String registerTime;
    private String registerIp;
    private String lastLoginIp;
    private String lastLoginTime;

    @Override
    public String getVid() {
        return this.username;
    }

    @Override
    public void setVid(String vid) {
        this.username = vid;
    }

    public String getUsername()
    {
        return username;
    }

    public void setUsername(String username)
    {
        this.username = username;
    }

    public String getPassword()
    {
        return password;
    }

    public void setPassword(String password)
    {
        this.password = password;
    }

    public String getRegisterTime()
    {
        return registerTime;
    }

    public void setRegisterTime(String registerTime)
    {
        this.registerTime = registerTime;
    }

    public String getRegisterIp()
    {
        return registerIp;
    }

    public void setRegisterIp(String registerIp)
    {
        this.registerIp = registerIp;
    }

    public String getLastLoginIp()
    {
        return lastLoginIp;
    }

    public void setLastLoginIp(String lastLoginIp)
    {
        this.lastLoginIp = lastLoginIp;
    }

    public String getLastLoginTime()
    {
        return lastLoginTime;
    }

    public void setLastLoginTime(String lastLoginTime)
    {
        this.lastLoginTime = lastLoginTime;
    }
}
