import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import server.game.GameServer;
import server.gate.GateServer;
import server.login.LoginServer;

/**    
 * @FileName: NettyServer.java  
 * @Package:server.login  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 上午10:48:06
 */
public class NettyServer {

	public static void main(String args[])
	{
		Thread th1= new Thread(new Runnable() {
			
			@Override
			public void run() {
				new LoginServer(8889).service();
			}
		},"LoginServer");
		Thread th2= new Thread(new Runnable() {
			
			@Override
			public void run() {
				new GateServer(8888).service();
			}
		},"GateServer");
		Thread th3= new Thread(new Runnable() {
			
			@Override
			public void run() {
				new GameServer(8887).service();
			}
		},"GameServer");
		initLog();
		th1.start();
		th2.start();
		th3.start();
	}
	public static void initLog()
	{
       PropertyConfigurator.configure("log4j.properties");//加载.properties文件
       Logger log = Logger.getLogger(NettyServer.class.getName());
       log.info("Log4j 日志服务 启动");
   }
}
