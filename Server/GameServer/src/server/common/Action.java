package server.common;

/**
 * @ClassName: Action
 * @Description: Action
 * @Author: zhengnan
 * @Date: 2018/6/6 20:03
 */
public class Action
{
    public final static String NAME = "action";

    public final static String NONE = "none";
    //游戏服务器和网关服务器握手
    public final static String HANDSHAKE_GAME2GATE = "handshake_game2gate";
    //登陆帐号服务器
    public final static String LOGIN_ACCOUNT = "login_account";
    //登陆游戏服务器
    public final static String LOGIN_GAME_SEVER = "login_game_server";
    //登出游戏服务器
    public final static String LOGOUT_GAME_SEVER = "logout_game_server";

    /**
     * Player 玩家
     */
    //游戏玩家信息
    public final static String PUSH_PLAYER_INFO = "push@player_info";

    /**
     * Role 游戏角色
     */
    //获取随机角色名
    public final static String ROLE_RANDOM_NAME = "role_random_name";
    //角色创建
    public final static String ROLE_CREATE = "role_create";
    //选择角色并进入游戏
    public final static String ROLE_SELECT_ENTER_GAME = "select_role_enter_game";
    /**
     * Game 游戏
     */
    public final static String PUSH_ROLE_INFO = "push@role_info";
}
