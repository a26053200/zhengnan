package server.common;

/**
 * @ClassName: Code
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/7 0:17
 */
public class ReturnCode
{
    static String[] code;

    public enum Code
    {
        UNKNOWN_ERROR,
        WRONG_PASSWORD,
        FETCH_GAME_SERVER_LIST_ERROR,

        MAX_CODE_NUM
    }

    static
    {
        code = new String[Code.MAX_CODE_NUM.ordinal()];

        code[Code.UNKNOWN_ERROR.ordinal()] = "未知错误";
        code[Code.WRONG_PASSWORD.ordinal()] = "密码错误";
        code[Code.FETCH_GAME_SERVER_LIST_ERROR.ordinal()] = "获取游戏服务器列表错误";
    }

    public static String getMsg(Code codeId)
    {
        return code[codeId.ordinal()];
    }
}


