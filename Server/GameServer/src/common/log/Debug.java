package common.log;

import org.apache.log4j.PropertyConfigurator;
import org.apache.log4j.Logger;

public class Debug
{
    static String s_serverName;

    public static void initLog(String serverName)
    {
        s_serverName = serverName;
        PropertyConfigurator.configure("log4j.properties");
    }

    private static Logger s_instance = null;

    public static Logger getInstance()
    {
        if (s_instance == null)
            s_instance = Logger.getLogger(s_serverName);
        return  s_instance;
    }

    public static void info(Object msg)
    {
        getInstance().info(msg);
    }

    public static void warn(Object msg)
    {
        getInstance().warn(msg);
    }

    public static void error(Object msg)
    {
        getInstance().error(msg);
    }
}

