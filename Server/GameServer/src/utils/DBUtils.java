package utils;

import java.util.ArrayList;
import java.util.List;

/**
 * @ClassName: DBUtils
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 22:40
 */
public class DBUtils
{
    public static List<Long> String2Long(List<String> strList)
    {
        List<Long> list = new ArrayList<Long>();
        for (int i = 0; i < strList.size(); i++)
        {
            list.add(Long.parseLong(strList.get(i)));
        }
        return list;
    }

    public static List<Integer> String2Int(List<String> strList)
    {
        List<Integer> list = new ArrayList<Integer>();
        for (int i = 0; i < strList.size(); i++)
        {
            list.add(Integer.parseInt(strList.get(i)));
        }
        return list;
    }
}
