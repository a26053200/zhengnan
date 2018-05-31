package utils;

import java.io.UnsupportedEncodingException;

import config.ServerConfig;

/**    
 * @FileName: TypeUtil.java  
 * @Package:utils  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月9日 下午4:25:42
 */
public class TypeUtil {

	public static <T> String getType(T t) {
		if (t instanceof String) {
			return "string";
		} else if (t instanceof Integer) {
			return "int";
		} else {
			return " do not know";
		}
	}
	public static int sizeOf(Object... args)
	{
		int len = 0;
		for(Object temp:args)
		{
			if (temp instanceof String) {
				try {
					len += ((String)temp).getBytes(ServerConfig.CHARSET_UTF_8).length + 2;
				} catch (UnsupportedEncodingException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			} else if (temp instanceof Integer) {
				len += 4;
			} else if (temp instanceof Long) {
				len += 8;
			} else if (temp instanceof Double) {
				len += 8;
			} else if (temp instanceof Short) {
				len += 2;
			} else if (temp instanceof Float) {
				len += 4;
			} else if (temp instanceof Byte) {
				len += 1;
			} else if (temp instanceof Boolean) {
				len += 1;
			} else {
				//" do not know type";
				throw new IllegalArgumentException("do not know type" + temp);
			}
		}
		return len;
	}
}
