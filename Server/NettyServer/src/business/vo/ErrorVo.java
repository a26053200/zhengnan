package business.vo;

import java.io.Serializable;

/**    
 * @FileName: ErrorVo.java  
 * @Package:business.vo  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月12日 下午3:35:35
 */
public class ErrorVo extends VoBase implements Serializable {

	/**
	 * serialVersionUID:TODO用一句话描述这个变量表示什么
	 */
	
	private static final long serialVersionUID = 1L;
	
	public int mainCmd;
	public int subCmd;
	
	public String msg;
	
	public ErrorVo(String msg)
	{
		this.msg = msg;
	}
	@Override
	public void setCmd(int cmd) {
		// TODO Auto-generated method stub
		mainCmd = cmd/256;
		subCmd = cmd%256;
	}
	@Override
	public int getMainCmd() {
		// TODO Auto-generated method stub
		return mainCmd;
	}

	@Override
	public int getSubCmd() {
		// TODO Auto-generated method stub
		return subCmd;
	}
}
