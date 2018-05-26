package business.vo;


import java.io.Serializable;

import business.rspd.LoginRspd;
import io.netty.channel.Channel;

/**    
 * @FileName: UserVo.java  
 * @Package:business.vo  
 * @Description: TODO 
 * 在序列化时，有几点要注意的：
　	<li>１：当一个对象被序列化时，只保存对象的非静态成员变量，不能保存任何的成员方法和静态的成员变量。
　	<li>２：如果一个对象的成员变量是一个对象，那么这个对象的数据成员也会被保存。
　	<li>３：如果一个可序列化的对象包含对某个不可序列化的对象的引用，那么整个序列化操作将会失败，并且会抛出一个NotSerializableException。我们可以将这个引用标记为transient，那么对象仍然可以序列化
 * @author: zhengnan   
 * @date:2018年5月10日 下午2:51:54
 */
public class UserVo extends VoBase implements Serializable{

	/**
	 * serialVersionUID:TODO用一句话描述这个变量表示什么
	 */
	private static final long serialVersionUID = 2L;
	
	private static int id_counter = 1;
	//服务端用数据
	//public
	public String ip;
	public int port;
	
	//业务逻辑数据
	public long userId;
	public int tempId;
	public String userName;
	public String password;
	public int type;
	
	public void valueFromRspd(LoginRspd rspd,Channel ch)
	{
		userId = tempId = id_counter++;
		userName = rspd.userName;
		password = rspd.password;
		type = rspd.type;
		ip = ch.remoteAddress().toString();
		
		channelId = ch.id();
	}
}
