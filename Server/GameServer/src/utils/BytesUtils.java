package utils;

import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;

import config.ServerConfig;
import io.netty.buffer.ByteBuf;

/**
 * @FileName: BytesUtils.java
 * @Package:utils
 * @Description: TODO
 * @author: zhengnan
 * @date:2018年5月8日 下午4:55:16
 */
public class BytesUtils
{
    //所有数据转换为字符串
    public static String readString(ByteBuf buff)
    {
        return readString(buff, buff.readableBytes());
    }
    public static String readString(ByteBuf buff, int len)
    {
        byte[] bytes = new byte[len];
        buff.readBytes(bytes);
        String res = readString(bytes);
        return res.trim();
    }
    public static String readString(byte[] bytes)
    {
        String res = new String(bytes, Charset.forName(ServerConfig.CHARSET_UTF_8));
        return res.trim();
    }
    public static byte[] string2Bytes(String str)
    {
        str = str + "\0";
        byte[] bytes = null;
        try
        {
            bytes = str.getBytes(ServerConfig.CHARSET_UTF_8);
        }
        catch (UnsupportedEncodingException e)
        {
            e.printStackTrace();
        }
        return bytes;
    }

    public static byte[] string2Bytes(String str, int length)
    {
        byte[] bytes = new byte[length];
        try
        {
            byte[] byteStr = str.getBytes(ServerConfig.CHARSET_UTF_8);
            System.arraycopy(byteStr, 0, bytes, 0, byteStr.length);
        }
        catch (UnsupportedEncodingException e)
        {
            e.printStackTrace();
        }
        return bytes;
    }

    /**
     * 将int数值转换为占四个字节的byte数组，本方法适用于(低位在前，高位在后)的顺序。 和bytesToInt（）配套使用
     * @param value
     *            要转换的int值
     * @return byte数组
     */
    public static byte[] intToBytes( int value )
    {
        byte[] src = new byte[4];
        src[3] =  (byte) ((value>>24) & 0xFF);
        src[2] =  (byte) ((value>>16) & 0xFF);
        src[1] =  (byte) ((value>>8) & 0xFF);
        src[0] =  (byte) (value & 0xFF);
        return src;
    }
    /**
     * 将int数值转换为占四个字节的byte数组，本方法适用于(高位在前，低位在后)的顺序。  和bytesToInt2（）配套使用
     */
    public static byte[] intToBytes2(int value)
    {
        byte[] src = new byte[4];
        src[0] = (byte) ((value>>24) & 0xFF);
        src[1] = (byte) ((value>>16)& 0xFF);
        src[2] = (byte) ((value>>8)&0xFF);
        src[3] = (byte) (value & 0xFF);
        return src;
    }

    //打包
    public static byte[] packBytes(byte[] bytes)
    {
        byte[] header = intToBytes2(bytes.length);
        byte[] allBytes = new byte[bytes.length + 4];
        System.arraycopy(header, 0, allBytes, 0, header.length);
        System.arraycopy(bytes, 0, allBytes, 4, bytes.length);
        return allBytes;
    }
}
