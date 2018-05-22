import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

public class NettyServer 
{
	public static void main(String args[])
	{
		initLog();
		System.out.println("Hello World");
	}

	private static void initLog() {
		// TODO Auto-generated method stub
		PropertyConfigurator.configure("log4j.properties");
		Logger log = Logger.getLogger(NettyServer.class.getName());
		log.info("Log4j has already start up。。。");
	}
}
