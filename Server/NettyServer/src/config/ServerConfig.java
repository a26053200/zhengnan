package config;
/**    
 * @FileName: ServerConfig.java  
 * @Package:  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午4:51:40
 */
public class ServerConfig {

	public static final String CHARSET_UTF_8 = "utf-8";
	
	public static final int BUFFER_MAX_SIZE = 2048;
	
	
	public static final int PACK_HEAD_LEN = 6;
	
	/**
	 * 名字字节长度
	 */
	public static final int BYTE_SIZE_NAME = 32;
	/**
	 * 密码字节长度
	 */
	public static final int BYTE_SIZE_PASS = 16;
	/**
	 * 长文本字节长度
	 */
	public static final int BYTE_SIZE_LONG_TEXT = 256;
	/**
	 * 中等文本字节长度
	 */
	public static final int BYTE_SIZE_NORMAL_TEXT = 128;
}
