package utils;

/**
 * @ClassName: MathUtils
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/8/11 23:55
 */
public class MathUtils
{
    public static int randomInt(int min, int max)
    {
        int num = min + (int) (Math.random() * (max - min));        //返回大于等于min小于max（不包括max）之间的随机数
        return num;
    }
}
