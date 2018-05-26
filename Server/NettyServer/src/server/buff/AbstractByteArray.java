package server.buff;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.channels.GatheringByteChannel;
import java.nio.channels.ScatteringByteChannel;

import io.netty.buffer.AbstractByteBuf;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.ByteBufAllocator;

/**    
 * @FileName: AbstractByteArray.java  
 * @Package:server.buff  
 * @Description: TODO 定长的字节数组
 * @author: zhengnan   
 * @date:2018年5月9日 下午12:18:04
 */
public abstract class AbstractByteArray extends AbstractByteBuf {

	protected byte[] bytes;
	
	protected AbstractByteArray(int maxCapacity) {
		super(maxCapacity);
		bytes = new byte[maxCapacity];
	}

	@Override
	public ByteBufAllocator alloc() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public byte[] array() {
		return bytes;
	}

	@Override
	public int arrayOffset() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int capacity() {
		return bytes.length;
	}

	@Override
	public ByteBuf capacity(int newCapacity) {
		int size = capacity();
		if(newCapacity != size)
		{//扩容 或者 截断
			byte[] destBytes = new byte[newCapacity];
			System.arraycopy(bytes, 0, destBytes, 0, newCapacity);
			setBytes(0, destBytes);
		}
		return this;
	}


	@Override
	public ByteBuf getBytes(int beginIndex, ByteBuffer arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ByteBuf getBytes(int arg0, OutputStream arg1, int arg2)
			throws IOException {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int getBytes(int arg0, GatheringByteChannel arg1, int arg2)
			throws IOException {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public ByteBuf getBytes(int index, ByteBuf dest, int destIndex, int length) {
		dest.setBytes(destIndex, this, index, length);
		return null;
	}

	@Override
	public ByteBuf getBytes(int index, byte[] dest, int destIndex, int length) {
		System.arraycopy(bytes, index, dest, destIndex, length);
		return this;
	}

	@Override
	public boolean hasArray() {
		try{
			return array().length > 0;
		}catch(Exception ex)
		{
			return false;
		}
	}

	@Override
	public boolean hasMemoryAddress() {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public ByteBuffer internalNioBuffer(int arg0, int arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean isDirect() {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public long memoryAddress() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int nioBufferCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public ByteBuffer[] nioBuffers(int arg0, int arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ByteOrder order() {
		return ByteOrder.nativeOrder();
	}

	@Override
	public ByteBuf retain() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ByteBuf retain(int arg0) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ByteBuf setBytes(int arg0, ByteBuffer arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int setBytes(int arg0, InputStream arg1, int arg2)
			throws IOException {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int setBytes(int arg0, ScatteringByteChannel arg1, int arg2)
			throws IOException {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public ByteBuf setBytes(int index, ByteBuf buf, int srcIndex, int length) {
		int size = capacity();
		if(index + length > size)
		{
			throw new IllegalArgumentException("设置的ByteBuf容量必须<=");
		}else{
			byte[] srcBytes = buf.array();
			System.arraycopy(srcBytes, srcIndex, bytes, index, length);
		}
		return this;
	}

	@Override
	public ByteBuf setBytes(int index, byte[] src, int srcIndex, int length) {
		int size = capacity();
		if(index + length > size)
		{
			throw new IllegalArgumentException("设置的byte[]容量必须<=");
		}else{
			System.arraycopy(src, srcIndex, bytes, index, length);
		}
		return this;
	}

	@Override
	public ByteBuf unwrap() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int refCnt() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public boolean release() {
		bytes = null;
		return true;
	}

	@Override
	public boolean release(int arg0) {
		bytes = null;
		return true;
	}

	@Override
	protected byte _getByte(int index) {
		return bytes[index];
	}

	@Override
	protected int _getInt(int index) {
		int val =(
				((bytes[index] & 0xff) << 24) |
				((bytes[index+1] & 0xff) << 16) |
				((bytes[index+2] & 0xff) << 8) | 
				(bytes[index+3] & 0xff));
		return val;
	}

	@Override
	protected long _getLong(int index) {
		int val = (
				((bytes[index] & 0xff) << 56) |
				((bytes[index+1] & 0xff) << 48) |
				((bytes[index+2] & 0xff) << 40) |
				((bytes[index+3] & 0xff) << 32) |
				((bytes[index+4] & 0xff) << 24) |
				((bytes[index+5] & 0xff) << 16) |
				((bytes[index+6] & 0xff) << 8) | 
				(bytes[index+7] & 0xff));
		return val;
	}

	@Override
	protected short _getShort(int index) {
		short val = (short)((bytes[index] << 8) | (bytes[index+1] & 0xff));
		return val;
	}

	@Override
	protected int _getUnsignedMedium(int arg0) {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	protected void _setByte(int index, int n) {
		bytes[index] = (byte)n;
	}

	@Override
	protected void _setInt(int index, int n) {
		bytes[index+3] = (byte) (n & 0xff);
		bytes[index+2] = (byte) (n >> 8 & 0xff);
		bytes[index+1] = (byte) (n >> 16 & 0xff);
		bytes[index+0] = (byte) (n >> 24 & 0xff);
	}

	@Override
	protected void _setLong(int index, long n) {
		bytes[index+7] = (byte) (n & 0xff);
		bytes[index+6] = (byte) (n >> 8 & 0xff);
		bytes[index+5] = (byte) (n >> 16 & 0xff);
		bytes[index+4] = (byte) (n >> 24 & 0xff);
		bytes[index+3] = (byte) (n >> 32 & 0xff);
		bytes[index+2] = (byte) (n >> 40 & 0xff);
		bytes[index+1] = (byte) (n >> 48 & 0xff);
		bytes[index+0] = (byte) (n >> 56 & 0xff);
	}

	@Override
	protected void _setMedium(int arg0, int arg1) {
		// TODO Auto-generated method stub

	}

	@Override
	protected void _setShort(int index, int n) {
		bytes[index+1] = (byte) (n & 0xff);
		bytes[index+0] = (byte) (n >> 8 & 0xff);
	}

}
