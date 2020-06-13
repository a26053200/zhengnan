package com.betel.center.server.account.services;

import com.alibaba.fastjson.JSONObject;
import com.betel.asd.BaseService;
import com.betel.asd.RedisDao;
import com.betel.center.core.consts.ReturnCode;
import com.betel.center.server.account.beans.Account;
import com.betel.center.server.account.dao.AccountDao;
import com.betel.consts.FieldName;
import com.betel.session.Session;
import com.betel.session.SessionState;
import com.betel.utils.IdGenerator;
import com.betel.utils.JSONArrayUtils;
import com.betel.utils.JwtHelper;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.List;

/**
 * @Description
 * @Author zhengnan
 * @Date 2020/5/20
 */
public class AccountService extends BaseService<Account>
{
    final static Logger logger = LogManager.getLogger(AccountService.class);

    @Autowired
    protected AccountDao accountDao;
    @Override
    public RedisDao<Account> getDao()
    {
        return accountDao;
    }
    //明文Token,用于测试 以后有时间再做加密的
    //Token 过期时间
    public final static int expiresSecond = 5 * 60 * 60 * 1000;
    //Token 密钥
    public final static String tokenSecretKey = "emhlbmduYW50YW5naHVpanVhbnpoZW5neWk==";

    class Field
    {
        static final String USERNAME = "username";
        static final String PASSWORD = "password";
    }


    private void accountLogin(Session session)
    {
        String username = session.getRecvJson().getString(Field.USERNAME);
        String password = session.getRecvJson().getString(Field.PASSWORD);

        List<Account> allAccount = accountDao.getViceEntities(username);

        if (allAccount.size() > 0)
        {//已经注册过
            Account account = allAccount.get(0);
            if (account.getPassword().equals(password)) {//密码正确，登陆成功
                logger.info(String.format("用户:%s 登陆成功", username));
                //rspdMessage(session,ReturnCode.Login_success);
                onLoginSuccess(session, account.getId());
                updateAccount(session,account.getId());
            } else {//密码错误，登陆失败
                logger.info(String.format("用户:%s 登陆失败", username));
                session.setState(SessionState.Fail);
                rspdMessage(session, ReturnCode.Wrong_password);
            }
        }else{//还未注册过
            logger.info(String.format("用户:%s 登陆失败", username));
            session.setState(SessionState.Fail);
            rspdMessage(session,ReturnCode.Register_not_yet);
        }
    }

    private void onLoginSuccess(Session session, String account_id)
    {
        //游戏服务器的网关地址列表 json
        JSONObject gameServerJson = getGameServers();
        JSONObject rspdJson = new JSONObject();
        rspdJson.put("aid", account_id);
        rspdJson.put("token", JwtHelper.createJWT(account_id, tokenSecretKey,expiresSecond,false));
        rspdJson.put("srvList", gameServerJson);
        rspdClient(session, rspdJson);
    }

    public JSONObject getGameServers()
    {
        String path = AccountService.class.getResource("/GameServer.json").getPath();
        return JSONArrayUtils.getJsonObject(path);
    }

    private void accountRegister(Session session)
    {
        String username = session.getRecvJson().getString(Field.USERNAME);
        String password = session.getRecvJson().getString(Field.PASSWORD);
        String clientIpAddress = session.getRecvJson().getString(FieldName.CLIENT_IP);
        List<Account> allAccount = accountDao.getViceEntities(username);
        if (allAccount.size() > 0)
        {//已经注册过
            //Account account = allAccount.get(0);
            session.setState(SessionState.Fail);
            rspdMessage(session,ReturnCode.Error_already_exits);
        }else{
            String nowTime = now();
            Account account = new Account();
            account.setId(Long.toString(IdGenerator.getInstance().nextId()));
            account.setUsername(username);
            account.setPassword(password);
            account.setRegisterTime(nowTime);
            account.setRegisterIp(clientIpAddress);
            account.setLastLoginTime(nowTime);
            account.setLastLoginIp(clientIpAddress);
            accountDao.addEntity(account);
            rspdMessage(session,ReturnCode.Register_success);
        }
    }

    public void updateAccount(Session session, String accountId)
    {
        Account account = accountDao.getEntity(accountId);
        if(account != null)
        {
            String clientIpAddress = session.getRecvJson().getString(FieldName.CLIENT_IP);
            account.setLastLoginTime(now());
            account.setLastLoginIp(clientIpAddress);
            accountDao.updateEntity(account);
        }else{
            logger.error("There is no account tha id = " + accountId);
        }
    }

}
