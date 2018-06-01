package server.common;

import common.log.Debug;
import io.netty.buffer.ByteBuf;
import utils.ZipUtil;

/**
 * @ClassName: ClientRequest
 * @Description: 客户端请求
 * @Author: zhengnan
 * @Date: 2018/6/1 23:34
 */
public class ClientRequest
{
    protected static final long COMPERSS = 0x40000000;
    /**
     * 是否起用压缩
     */
    private Boolean useCompress = true;
    public ResponderBase clientReceive(ByteBuf bytes) throws Exception
    {
        currRspd = null;
        int msgLen = bytes.readableBytes();
        Debug.info("收到包长 msgLen:"+msgLen);
        long packHead = bytes.readUnsignedInt();
        Boolean hasCompress = (packHead & COMPERSS) > 0;
        hasCompress = hasCompress && useCompress;
        //包头 代表 包长(无压缩)
        long packLen = packHead;
        if(hasCompress)//如果压缩过的
            packLen = (int)(packHead ^ COMPERSS);
        if(hasCompress)
        {//解压缩
            byte[] tempBytes = bytes.readBytes(msgLen-4).array();
            tempBytes = ZipUtil.unZLib(tempBytes);
            bytes.clear();
            bytes.writeBytes(tempBytes);
        }
        int mainCmd 	= bytes.readUnsignedByte();	//大指令
        int subCmd  	= bytes.readUnsignedByte();	//小指令
        int rspdTime 	= bytes.readInt();				//校验时间
        Debug.info("收到指令 code:"+mainCmd+"-"+subCmd+" len:"+packLen+" time:"+rspdTime);
        String clsName = _rspdMap.get(mainCmd*256+subCmd);
        ResponderBase rspd = null;
        if(clsName!=null)
        {
            try{
                rspd = (ResponderBase)Class.forName(clsName).newInstance();
            }catch(Exception newClassE){
                Debug.exception("无法创建消息结构实例："+clsName+"");
            }
        }else{
            Debug.error("找不到相关消息结构："+mainCmd+"-"+subCmd);
        }
        rspd.setPackHead(mainCmd, subCmd,rspdTime);
        rspd.receive(bytes);
        currRspd = rspd;
        return rspd;
    }
}
