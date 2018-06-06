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

    public static String readString(ByteBuf buff, int len)
    {
        String res = "decode fail string";//转码失败
//		try{
        byte[] bytes = new byte[len];
        buff.readBytes(bytes);
        res = new String(bytes, Charset.forName(ServerConfig.CHARSET_UTF_8));
//		}catch(UnsupportedEncodingException e){
//			e.printStackTrace();
//		}
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
}
