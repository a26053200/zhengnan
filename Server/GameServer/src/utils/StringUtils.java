package utils;

/**
 * @ClassName: StringUtils
 * @Description: 字符串工具
 * @Author: zhengnan
 * @Date: 2018/5/31 22:54
 */
public class StringUtils
{
    public static boolean isNullOrEmpty(String s)
    {
        return s == null || s.length() <= 0;
    }

    public static String RemoveAllEmpty(String resource)
    {
        StringBuffer src = new StringBuffer(resource);
        src = RemoveAllChar(src, ' ');
        return src.toString();
    }

    public static StringBuffer RemoveAllChar(StringBuffer src, char ch)
    {
        StringBuffer buffer = new StringBuffer();
        int position = 0;
        char currentChar;

        while (position > 0)
        {
            currentChar = src.charAt(position++);
            if (currentChar != ch)
                buffer.append(currentChar);
        }
        return buffer;
    }
}
