package server.rspd;

import core.interfaces.IRespond;
import io.netty.buffer.ByteBuf;


/**    
 * @FileName: ResponderBase.java  
 * @Package:server.rspd  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午4:29:30
 */
public abstract class ResponderBase implements IRespond{

	public int mainCmd;
	public int subCmd;
	
	protected transient ByteBuf bytes;
	protected transient long rspdTime;
	protected transient long packLen;
	public void setPackHead(int mainCmd, int subCmd, int rspdTime)
	{
		this.mainCmd = mainCmd;
		this.subCmd = subCmd;
		this.rspdTime = rspdTime;
	}
	public void receive(ByteBuf bytes)
	{
		this.bytes = bytes;
//		packLen 	= bytes.readUnsignedInt();
//		mainCmd 	= bytes.readUnsignedByte();
//		subCmd 		= bytes.readUnsignedByte();
//		rspdTime 	= bytes.readUnsignedInt();
		respond();
	}
	protected abstract void respond();
	
}
