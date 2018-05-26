package server.dict;
/**    
 * @FileName: GateHubCmdDict.java  
 * @Package:server.dict  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月15日 下午8:17:28
 */
public class GateHubCmdDict {

	/**
	 * 中转指令:客户端  --> 登陆服务 
	 */
	public static final int HUB_CLIENT_TO_LOGIN = 256*255+1;
	
	/**
	 * 中转指令:客户端  <-- 登陆服务
	 */
	public static final int HUB_LOGIN_TO_CLIENT = 256*255+2;
}
