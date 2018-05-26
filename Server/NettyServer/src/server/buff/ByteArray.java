package server.buff;

import java.io.UnsupportedEncodingException;

import config.ServerConfig;
import utils.Uint;

/**    
 * @FileName: ByteArray.java  
 * @Package:server.buff  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月9日 下午4:01:29
 */
public class ByteArray{

	protected byte[] bytes;
	
//	private int readerIndex;
    private int writerIndex;
//    private int markedReaderIndex;
//    private int markedWriterIndex;

    private int maxCapacity;


    public ByteArray(int maxCapacity) {
        if (maxCapacity < 0) {
            throw new IllegalArgumentException("maxCapacity: " + maxCapacity + " (expected: >= 0)");
        }
        this.maxCapacity = maxCapacity;
        bytes = new byte[maxCapacity];
    }
    
	public ByteArray()
	{
		this(0);
	}
	public byte[] array()
	{
		return bytes;
	}
	public void write(byte[] src, int index, int length){
		ensureWritable(length);
		System.arraycopy(src, index, bytes, writerIndex, length);
		writerIndex += length;
	}
	
	public void writeBoolean(boolean v){
		ensureWritable(1);
		_setByte(writerIndex, v?1:0);
		writerIndex += 1;
	}
	
	public void writeByte(int b){
		ensureWritable(1);
		_setByte(writerIndex, b);
		writerIndex += 1;
	}
	
	public void writeChar(int c){
		ensureWritable(2);
		_setShort(writerIndex, c);
		writerIndex += 2;
	}
	
	public void writeDouble(double value){
		ensureWritable(8);
		writeLong(Double.doubleToRawLongBits(value));
		writerIndex += 8;
	}
	
	public void writeFloat(float value){
		ensureWritable(8);
		writeInt(Float.floatToRawIntBits(value));
		writerIndex += 8;
	}
	
	public void writeInt(int value){
		ensureWritable(4);
        _setInt(writerIndex, value);
        writerIndex += 4;
	}
	
	public void writeLong(long value){
		ensureWritable(8);
        _setLong(writerIndex, value);
        writerIndex += 8;
	}
	
	public void writeShort(int value){
		ensureWritable(2);
        _setShort(writerIndex, value);
        writerIndex += 2;
	}
	
    public void writeUnsignedByte(short value) {
    	write(Uint.uint8(value),0,1);
    }
	
    public void writeUnsignedInt(long value) {
        write(Uint.uint32(value),0,4);
    }
    
    public void writeUnsignedShort(int value) {
    	write(Uint.uint16(value),0,2);
    }
    
	public void writeUTF(String str){
		str = str+"\0";
		byte[] bytes;
		try {
			bytes = str.getBytes(ServerConfig.CHARSET_UTF_8);
			write(bytes, 0, bytes.length);
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	public void writeUTF_8(String str, int length){
		byte[] bytes = new byte[length];
		byte[] byteStr;
		try {
			byteStr = str.getBytes(ServerConfig.CHARSET_UTF_8);
			System.arraycopy(byteStr, 0, bytes, 0, byteStr.length);
			write(bytes, 0, bytes.length);
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		}
	}
	public int capacity()
	{
		return bytes.length;
	}
	public int writableBytes() {
        return capacity() - writerIndex;
    }
	
	protected void _setByte(int index, int n) {
		bytes[index] = (byte)n;
	}

	protected void _setShort(int index, int n) {
		bytes[index+1] = (byte) (n & 0xff);
		bytes[index+0] = (byte) (n >> 8 & 0xff);
	}
	
	protected void _setInt(int index, int n) {
		bytes[index+3] = (byte) (n & 0xff);
		bytes[index+2] = (byte) (n >> 8 & 0xff);
		bytes[index+1] = (byte) (n >> 16 & 0xff);
		bytes[index+0] = (byte) (n >> 24 & 0xff);
	}

	
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
	
	protected ByteArray ensureWritable(int minWritableBytes) {
        if (minWritableBytes < 0) {
            throw new IllegalArgumentException(String.format(
                    "minWritableBytes: %d (expected: >= 0)", minWritableBytes));
        }

        if (minWritableBytes <= writableBytes()) {
            return this;
        }

        if (minWritableBytes > maxCapacity - writerIndex) {
            throw new IndexOutOfBoundsException(String.format(
                    "writerIndex(%d) + minWritableBytes(%d) exceeds maxCapacity(%d): %s",
                    writerIndex, minWritableBytes, maxCapacity, this));
        }
        return this;
    }
}
