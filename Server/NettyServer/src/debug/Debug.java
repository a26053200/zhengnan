package debug;

public class Debug
{
    public Debug()
    {

    }

    private static final String OPERATION_MEMORY = "[Memory]:";
    private static final String OPERATION_ERROR = "[Error]:";
    private static final String OPERATION_LOG = "[Log]:";
    private static final String OPERATION_WARNING = "[Warning]:";
    private static final String OPERATION_EXCEPTION = "[Exception]:";

    public static Boolean memory()
    {
        return send(OPERATION_MEMORY, "", null);
    }

    public static Boolean error(String err)
    {
        return send(OPERATION_ERROR, err, null);
    }

    public static Boolean trace(String log)
    {
        return print(log);
    }

    public static Boolean log(String log)
    {
        return send(OPERATION_LOG, log, null);
    }

    public static Boolean exception(String exception)
    {
        return send(OPERATION_EXCEPTION, exception, null);
    }

    public static Boolean warning(String warning)
    {
        return send(OPERATION_WARNING, warning, null);
    }

    private static Boolean send(String operation, String value, String param)
    {
        System.err.println(operation + value);
        return true;
    }

    private static Boolean print(String value)
    {
        System.err.print(value);
        return true;
    }
}
