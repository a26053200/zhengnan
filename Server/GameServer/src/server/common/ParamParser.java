package server.common;

import com.alibaba.fastjson.JSONObject;

/**
 * @ClassName: ParamParser
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/31 22:13
 */
public class ParamParser
{
    private String[] params;

    public ParamParser(JSONObject jsonObject)
    {
        String dataStr = jsonObject.getString("data");
        params = dataStr.split("&");
    }

    public String getString(int index)
    {
        if (params.length <= index)
            return null;
        else
            return params[index];
    }

    public long getLong(int index)
    {
        return Long.parseLong(getNumString(index));
    }

    public int getInt(int index)
    {
        return Integer.parseInt(getNumString(index));
    }

    public boolean getBool(int index)
    {
        return Boolean.parseBoolean(getString(index));
    }

    private String getNumString(int index)
    {
        if (params.length <= index)
            return "0";
        else
            return params[index];
    }
}
