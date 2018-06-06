package server.common;

/**
 * @ClassName: House
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/6/6 23:51
 */
public class House
{
    //测试
    public static void main(String[] args)
    {
        float houseSize = 100;
        float destPrice = 50000;
        float depositPerMon = 20000;
        float srcPrice = 40000;
        float rise = 0.01f;
        float repaymentPerMon = 0.00333333f;//每个月还0.003;
        float decoration = 300000;//装修
        cal(100, 50000, 20000, 40000, 0.01f, 1000000, 0.0033333333f, 300000);
    }

    /**
     * @param houseSize 目标房子面积
     * @param destPrice 目标房价
     * @param depositPerMon 平均每月存款
     * @param srcPrice 房子现价
     * @param rise 房价每月涨幅
     * @param loan 贷款额度
     * @param repaymentPerMon 每月还款
     * @param decoration 装修所需
     */
    public static void cal(float houseSize, float destPrice,
                           float depositPerMon, float srcPrice,
                           float rise, float loan, float repaymentPerMon,
                           float decoration)
    {
        float destTotal = houseSize * destPrice;
        float depositTotal = 0;
        float lastPrice = srcPrice;
        float lastSell = 0;
        float residueRepayment = 0;
        float total = 0;
        float residue = 0;
        int count = 1;
        System.out.println(String.format("目标房子:%f 贷款:%f 需要存款:%f", destTotal, loan, destTotal - loan));
        do
        {
            residueRepayment = 904001f - 1000000f * repaymentPerMon * count;
            lastPrice = lastPrice * (1f + rise);
            lastSell = 89f * lastPrice;
            depositTotal = count * depositPerMon;
            total = depositTotal + lastSell;
            System.out.println(String.format("第%d个月,存款总额:%f 卖房所得:%f 总计:%f 剩余贷款:%f 实际存款:%f",
                    count, depositTotal, lastSell, total, residueRepayment, total - residueRepayment));
            residue = (total - residueRepayment) - (destTotal - loan);
            count++;
        }
        while (total - residueRepayment < destTotal - loan || residue < decoration);
        System.out.println(String.format("买房后剩余用来装修的存款:%f 所需房价:%f", residue, lastSell / 89f));
    }
}
