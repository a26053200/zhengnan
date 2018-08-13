package server.redis;

public class RedisKeys
{
    /**
     * Common
     */
    //public static final String MSG = "msg";

    /**
     * Account
     */
    public static final String account_id = "account_id";
    public static final String account_username = "account_username";
    public static final String account_password = "account_password";
    public static final String account_ip = "account_ip";
    public static final String account_reg_time = "account_reg_time";

    /**
     * Player 玩家
     */
    public static final String player = "player";
    public static final String player_id = "player_id";
    public static final String player_account_id = "player_account_id";
    public static final String player_register_time = "player_register_time";
    public static final String player_login_time = "player_login_time";
    public static final String player_logout_time = "player_logout_time";

    /**
     * Role 玩家游戏角色
     */
    public static final String role = "role";
    public static final String role_id = "role_id";
    public static final String role_player_id = "role_player_id";
    public static final String role_name = "role_name";
    public static final String role_create_time = "role_create_time";
    public static final String role_online_time = "role_online_time";
    public static final String role_offline_time = "role_offline_time";

    public static final String role_random_name1 = "role_random_name1";
    public static final String role_random_name2 = "role_random_name2";
    public static final String role_random_name3 = "role_random_name3";
}
