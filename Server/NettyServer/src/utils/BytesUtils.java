package utils;

import java.io.UnsupportedEncodingException;

import config.ServerConfig;
import io.netty.buffer.ByteBuf;

/**    
 * @FileName: BytesUtils.java  
 * @Package:utils  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午4:55:16
 */
public class BytesUtils {

	public static String readString(ByteBuf buff,int len)
	{
		String res = "decode fail string";//转码失败
		try{
			res = new String(buff.readBytes(len).array(),ServerConfig.CHARSET_UTF_8);
		}catch(UnsupportedEncodingException e){
			e.printStackTrace();
		}
		return res.trim();
	}
}
