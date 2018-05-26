package business.rspd;

import server.rspd.ResponderBase;
import utils.BytesUtils;
import config.ServerConfig;

/**    
 * @FileName: LoginRspd.java  
 * @Package:server.rspd  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午10:14:13
 */
public class LoginRspd  extends ResponderBase{

	public String userName;
	public String password;
	public int type;
	@Override
	protected void respond() {
		// TODO Auto-generated method stub
		userName = BytesUtils.readString(bytes, ServerConfig.BYTE_SIZE_NAME);
		password = BytesUtils.readString(bytes, ServerConfig.BYTE_SIZE_PASS);
		type = bytes.readByte();
	}
}
