package utils;

import java.net.InetAddress;
import java.net.UnknownHostException;

/**
 * @ClassName: NetUtils
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/9 14:50
 */
public class NetUtils
{
    //获取本地ip地址
    public static String GetLocalHostAddress()
    {
        try {
            InetAddress address = InetAddress.getLocalHost();//获取的是本地的IP地址 //PC-20140317PXKX/192.168.0.121
            String hostAddress = address.getHostAddress();// 192.168.*.*
            return hostAddress;
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }
        return null;
    }
}
