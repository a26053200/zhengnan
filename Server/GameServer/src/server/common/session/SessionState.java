package server.common.session;

/**
 * @ClassName: SessionState
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/12 21:42
 */
public enum SessionState
{
    Success,
    Fail,
    UnknowError,

    //角色名已经存在
    RoleNameHasAlreadyExists,
}
