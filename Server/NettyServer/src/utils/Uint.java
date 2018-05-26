package utils;

import java.math.BigInteger;

/**    
 * @FileName: Uint.java  
 * @Package:utils  
 * @Description: TODO 无符号int型数据 4个字节
 * @author: zhengnan   
 * @date:2018年5月9日 下午7:18:29
 */
public class Uint {

	private static final long UINT32_MAX = 4294967295L;
	
	private static final long UINT16_MAX = 65535;
	
	private static final long UINT8_MAX = 255;
	
	public static byte[] uint32(long value)
	{
		if(value > UINT32_MAX || value < 0)
		{
			throw new IllegalArgumentException("参数错误,不能为负数且 value:"+value+"  UINT32_MAX:"+UINT32_MAX);
		}
		byte[] b = new byte[4];
        b[3] = (byte) (value & 0xff);
        b[2] = (byte) (value >> 8 & 0xff);
        b[1] = (byte) (value >> 16 & 0xff);
        b[0] = (byte) (value >> 24 & 0xff);
		return b;
	}
	public static byte[] uint16(int value)
	{
		if(value > UINT16_MAX || value < 0)
		{
			throw new IllegalArgumentException("参数错误,不能为负数且 value:"+value+"  UINT16_MAX:"+UINT16_MAX);
		}
		byte[] b = new byte[2];
        b[1] = (byte) (value & 0xff);
        b[0] = (byte) (value >> 8 & 0xff);
		return b;
	}
	public static byte[] uint8(short value)
	{
		if(value > UINT8_MAX || value < 0)
		{
			throw new IllegalArgumentException("参数错误,不能为负数且 value:"+value+"  UINT8_MAX:"+UINT8_MAX);
		}
		byte[] b = new byte[1];
        b[0] = (byte) (value & 0xff);
		return b;
	}
	public static final long readUnsignedInt(byte[] byte32)
    {
    	BigInteger bigInt = readUnsignedInt32(byte32);
    	return new Long(bigInt.toString());
    }
	private static final BigInteger readUnsignedInt32(byte[] readBuffer) {
        if (readBuffer == null || readBuffer.length < 4)
            return new BigInteger("0");
        // 处理成无符号数
        byte[] uint32 = new byte[5];
        System.arraycopy(readBuffer, 0, uint32, 1, 4);
        uint32[0] = 0;
        return new BigInteger(uint32);
    }
	/**
     * 逆转字节数组
     * 
     * @param b
     * @return
     */
//    private static byte[] reverse(byte[] b) {
//
//        byte[] temp = new byte[b.length];
//        for (int i = 0; i < b.length; i++) {
//            temp[i] = b[b.length - 1 - i];
//        }
//        return temp;
//    }
}
