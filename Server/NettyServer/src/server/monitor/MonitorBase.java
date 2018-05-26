package server.monitor;

import io.netty.buffer.ByteBuf;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelOutboundHandlerAdapter;
import io.netty.channel.group.ChannelGroup;

import java.util.HashMap;
import java.util.Vector;

import business.vo.VoBase;
import core.interfaces.INotifyHandler;
import core.interfaces.IVoNotifHandler;
import server.rspd.ResponderBase;
import server.rt.ReturnBase;
import utils.ZipUtil;
import debug.Debug;

/**    
 * @FileName: MonitorBase.java  
 * @Package:server.monitor  
 * @Description: TODO 
 * @author: zhengnan   
 * @date:2018年5月8日 下午7:47:46
 */
public abstract class MonitorBase {

	protected static final long COMPERSS = 0x40000000;
	
	/**
	 * 所有客户端Respond Class的映射
	 */
	private HashMap<Integer, String> _rspdMap;
	/**
	 * 所有客户端Respond Notify的映射
	 */
	private HashMap<Integer, Vector<INotifyHandler>> _rspdNotifyMap;
	/**
	 * 所有服务端value object的映射
	 */
	private HashMap<Integer, Class<? extends VoBase>> _voMap;
	/**
	 * 所有服务端value object handler的映射
	 */
	private HashMap<Integer, Vector<IVoNotifHandler>> _voNotifyMap;
	/**
	 * 是否起用压缩
	 */
	private Boolean useCompress = true;
	
	/**
	 * 多个客户端链接通道(用户的tempID索引)
	 */
	protected HashMap<Integer, Channel> userChannelMap;

	/**
	 * 多个客户端链接通道key(用户的tempID索引)
	 */
	protected HashMap<Integer, Integer> userChannelKeyMap;
	
	/**
	 * 是否起用压缩
	 */
	protected ResponderBase currRspd = null;
	
	/**
	 * 多个客户端链接通道
	 */
	protected ChannelGroup channelGroup;
	
	/**
	 * 服务作为客户端时与其他服务连接的通道
	 */
	protected Channel clientChannel;

	/**
	 * 服务作为客户端时与其他服务连接的上下文
	 */
	protected ChannelHandlerContext clientCtx;
	
	/**
	 * 服务作为客户端时与其他服务连接的 适配器
	 */
	protected ChannelOutboundHandlerAdapter clientAdapter;
	
	protected MonitorBase(ChannelGroup channelGroup) 
	{
		this.channelGroup = channelGroup;
		init();
	}
	protected MonitorBase(Channel clientChannel) 
	{
		this.clientChannel = clientChannel;
		init();
	}
	protected MonitorBase(ChannelHandlerContext clientCtx) 
	{
		this.clientCtx = clientCtx;
		init();
	}
	protected MonitorBase(ChannelOutboundHandlerAdapter clientAdapter) 
	{
		this.clientAdapter = clientAdapter;
		init();
	}
	
	protected void init()
	{
		_rspdMap		= new HashMap<Integer, String>();
		_rspdNotifyMap 	= new HashMap<Integer, Vector<INotifyHandler>>();
		_voMap			= new HashMap<Integer, Class<? extends VoBase>>();
		_voNotifyMap	= new HashMap<Integer, Vector<IVoNotifHandler>>();
		userChannelMap	= new HashMap<Integer, Channel>();
		userChannelKeyMap	= new HashMap<Integer, Integer>();
		initHandler();
		registNotify();
	}
	
	protected abstract void initHandler();
	
	protected abstract void registNotify();
	
	/**
	 * regRspd 注册指令对应的 Respond
	 * 注册指令对应的 Respond
	 */
	protected void regRspd(int cmd,String rspdClass)
	{
		_rspdMap.put(cmd, rspdClass);
	}
	protected void regVO (int cmd,Class<? extends VoBase> voClass)
	{
		_voMap.put(cmd, voClass);
	}
	protected void addRspdNotify(int cmd,INotifyHandler rspdNotify)
	{
		if(_rspdNotifyMap.get(cmd) == null)
			_rspdNotifyMap.put(cmd, new Vector<INotifyHandler>());
		Vector<INotifyHandler> vec = _rspdNotifyMap.get(cmd);
		vec.add(rspdNotify);
	}
	protected void delRspdNotify(int cmd,INotifyHandler rspdNotify)
	{
		Vector<INotifyHandler> vec = _rspdNotifyMap.get(cmd);
		if(vec != null&&vec.contains(rspdNotify))
			vec.remove(rspdNotify);
	}
	protected void addVoNotify(int cmd,IVoNotifHandler voNotify)
	{
		if(_voNotifyMap.get(cmd) == null)
			_voNotifyMap.put(cmd, new Vector<IVoNotifHandler>());
		Vector<IVoNotifHandler> vec = _voNotifyMap.get(cmd);
		vec.add(voNotify);
	}
	protected void delVoNotify(int cmd,IVoNotifHandler voNotify)
	{
		Vector<IVoNotifHandler> vec = _voNotifyMap.get(cmd);
		if(vec != null&&vec.contains(voNotify))
			vec.remove(voNotify);
	}
	/**
	 * clientReceive 处理客户端发来的数据
	 * 解析数据，并且分发到各个通知处理器里面
	 */
	public ResponderBase clientReceive(ByteBuf bytes) throws Exception
	{
		currRspd = null;
		int msgLen = bytes.readableBytes();
    	Debug.log("收到包长 msgLen:"+msgLen);
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
		Debug.log("收到指令 code:"+mainCmd+"-"+subCmd+" len:"+packLen+" time:"+rspdTime);
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
	//读取客户端数据结束操作
	protected byte[] ending(ResponderBase rspd)
	{
		//解析消息
		//...
		//分发通知到各个消息处理器
		Vector<INotifyHandler> vec = _rspdNotifyMap.get(rspd.mainCmd*256+rspd.subCmd);
		if(vec == null)
		{	
			Debug.error("找不到相关消息处理器："+rspd.mainCmd+"-"+rspd.subCmd);
			return null;
		}
		INotifyHandler notify = null;
		ReturnBase rt = null;
		for(int i=0;i<vec.size();i++)
		{
			notify = vec.get(i);
			if(notify!=null)
			{
				rt = notify.handler(rspd);
			}
		}
		if(rt != null)
		{
			return rt.getBytes().array();
		}else{
			return null;
		}
	}
	
	public abstract byte[] notifyRspd(Channel ch);
	
	/**
	 * 
	 * voReceive 接收序列化VO的类<br>
	 * 接收序列化VO的类<br>
	 */
	public VoBase voReceive(Object obj,ChannelHandlerContext ctx)
	{
		VoBase tempObj = null;
		try{
			tempObj = VoBase.class.cast(obj);
		}catch (Exception castClassE){
			Debug.exception("无法转换VO结构："+obj.getClass().getName()+"");
			return null;
		}
		int mainCmd 	= tempObj.getMainCmd();	//大指令
		int subCmd  	= tempObj.getSubCmd();	//小指令
		Class<? extends VoBase> cls = _voMap.get(mainCmd*256+subCmd);
		if(cls != null)
		{
			try{
				tempObj = cls.cast(obj);
			}catch(Exception newClassE){
				Debug.exception("无法转换VO结构："+cls.getName()+"");
			}
		}else{
			Debug.error("找不到相关Vo结构："+mainCmd+"-"+subCmd);
		}
		//分发通知
		Vector<IVoNotifHandler> vec = _voNotifyMap.get(mainCmd*256+subCmd);
		if(vec == null)
		{	
			Debug.error("找不到相关Vo消息处理器："+mainCmd+"-"+subCmd);
			return null;
		}
		IVoNotifHandler notify = null;
		for(int i=0;i<vec.size();i++)
		{
			notify = vec.get(i);
			if(notify!=null)
			{
				notify.handler(tempObj,ctx);
			}
		}
		return tempObj;
	}
	
	public void setChannelGroup(ChannelGroup channelGroup) {
		this.channelGroup = channelGroup;
	}

	public ChannelGroup getChannelGroup() {
		return channelGroup;
	}
	
	public Boolean removeChannel(int channelId)
	{
		if(channelGroup != null)
		{
			Channel ch = channelGroup.find(channelId);
			if(ch!=null)
			{
				if(userChannelMap.containsValue(ch))
				{
					userChannelMap.remove(ch);
				}
				return channelGroup.remove(ch);
			}
		}
		return false;
	}
	protected void send(Channel ch, Object obj)
	{
		ch.write(obj);
	}
	protected void send(Channel ch, byte[] bytes)
	{
		ch.write(bytes);
	}
	
	/**
	 * 
	 * 设置通道映射<br>
	 * 用来区分数据是来源是谁<br>
	 */
	protected int setChannelMap(Channel ch,int key)
	{
		if(!userChannelMap.containsValue(ch))
		{//同一通道的请求
			userChannelMap.put(key, ch);
			userChannelKeyMap.put(ch.id(), key);
			return key;
		}else{
			return userChannelKeyMap.get(ch.id());
		}
	}
}
