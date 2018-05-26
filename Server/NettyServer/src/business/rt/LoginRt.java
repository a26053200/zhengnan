package business.rt;

import server.rt.ReturnBase;
import config.ServerConfig;

/**    
 * @FileName: LoginRt.java  
 * @Package:business.rt  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 上午10:24:51
 */
public class LoginRt extends ReturnBase
{

	protected String str;
	protected int unShort;
	protected boolean bool;
	public LoginRt(int cmd) {
		super(cmd);
		
		str = "登陆成功";
		packLen += ServerConfig.BYTE_SIZE_NAME;
		unShort = 55555;
		packLen += 4;
		bool = true;
		packLen += 1;
	}

	protected void packageData() 
	{
		bytes.writeUTF_8(str,ServerConfig.BYTE_SIZE_NAME);
		bytes.writeUnsignedShort(unShort);
		bytes.writeBoolean(bool);
	}
}
