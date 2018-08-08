package server.redis;

public class RedisKeys
{
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
    public static final String player_id = "player_id";
    public static final String player_account_id = "player_account_id";
    public static final String player_register_time = "player_register_time";
    public static final String player_login_time = "player_login_time";
    public static final String player_logout_time = "player_logout_time";

    /**
     * Role 玩家游戏角色
     */
    public static final String role_id = "role_id";
    public static final String role_player_id = "role_player_id";
    public static final String role_name = "role_name";
}
