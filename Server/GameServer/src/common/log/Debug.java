package common.log;

import org.apache.log4j.PropertyConfigurator;
import org.apache.log4j.Logger;

public class Debug
{
    public static void initLog()
    {
        PropertyConfigurator.configure("log4j.properties");
    }
    public static Logger CreateLog(String className)
    {
        Logger log = Logger.getLogger(className);
        return log;
    }

    private static Logger s_instance = null;

    public static Logger getInstance()
    {
        if (s_instance == null)
            s_instance = Logger.getLogger(Debug.class.getName());
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
