package server.rt;


import io.netty.channel.Channel;
import io.netty.util.concurrent.Future;
import io.netty.util.concurrent.GenericFutureListener;
import config.ServerConfig;
import server.buff.ByteArray;
import core.interfaces.IReturn;
import debug.Debug;

/**    
 * @FileName: ReturnBase.java  
 * @Package:server.rt  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月9日 上午9:19:24
 */
public abstract class ReturnBase implements IReturn {

	protected int cmd;
	protected long packLen;
	
	protected ByteArray bytes;
	
	public ReturnBase(int cmd)
	{
		this.cmd =cmd;
		bytes = new ByteArray();
	}
	
	protected abstract void packageData();
	
	protected void packageLen()
	{
		packLen = packLen + ServerConfig.PACK_HEAD_LEN;
		//ByteBuf buf = Unpooled.buffer(4);
		bytes = new ByteArray((int)packLen + 4);
		bytes.writeUnsignedInt(packLen);
	}
	protected void calculate(int len)
	{
		packLen += len;
	}
	public void send(final Channel ch)
	{
		//计算数据长度
		packageLen();
		//数据头
		bytes.writeUnsignedByte((short)(cmd/256));
		bytes.writeUnsignedByte((short)(cmd%256));
		bytes.writeUnsignedInt(2222222222L);
		//数据组装
		packageData();
		try {
			ch.write(bytes.array()).sync().addListener(new GenericFutureListener<Future<? super Void>>() {

				@Override
				public void operationComplete(Future<? super Void> future)
						throws Exception {
					// TODO Auto-generated method stub
					if(future.isSuccess())
					{
						Debug.log("发送字节到客户端成功");
					}else{
						Debug.log("发送字节到客户端失败");
					}
				}
			});
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public ByteArray getBytes() {
		return bytes;
	}
	
	
}
