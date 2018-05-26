package business.vo;

import java.io.Serializable;


/**    
 * @FileName: VoBase.java  
 * @Package:business.vo  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月10日 下午3:23:03
 */
public abstract class VoBase implements Serializable{

	/**
	 * serialVersionUID:TODO用一句话描述这个变量表示什么
	 */
	private static final long serialVersionUID = 1L;
	
	public int channelId;
	
	private int mainCmd;
	private int subCmd;

	public void setCmd(int cmd) {
		mainCmd = cmd/256;
		subCmd = cmd%256;
	}

	public int getMainCmd() {
		return mainCmd;
	}

	public int getSubCmd() {
		return subCmd;
	}
}
