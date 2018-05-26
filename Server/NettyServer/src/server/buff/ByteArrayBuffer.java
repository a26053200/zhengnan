package server.buff;

import io.netty.buffer.ByteBuf;

/**    
 * @FileName: ByteArrayBuffer.java  
 * @Package:server.buff  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月9日 下午4:01:29
 */
public class ByteArrayBuffer extends AbstractByteArray {

	protected ByteArrayBuffer(int maxCapacity) {
		super(maxCapacity);
		// TODO Auto-generated constructor stub
	}

	public ByteArrayBuffer()
	{
		this(0);
	}
	@Override
	public ByteBuf copy(int beginIndex, int endIndex) {
		int len = endIndex - beginIndex + 1;
		if(len <= 0)
		{
			throw new IllegalArgumentException("endIndex: " + endIndex + "必须 >= beginIndex:"+beginIndex);
		}
		byte[] destBytes = new byte[len];
		System.arraycopy(bytes, 0, destBytes, 0, len);
		ByteArrayBuffer newObj = new ByteArrayBuffer();
		newObj.capacity(len);
		newObj.setBytes(0, destBytes);
		return newObj;
	}
}
