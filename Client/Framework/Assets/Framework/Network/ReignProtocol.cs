using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using LuaInterface;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Framework
{
    internal class ReignRequestFactory
    {
        private static ReignRequestFactory s_instance;

        internal static ReignRequestFactory GetInstance()
        {
            if(s_instance == null)
                s_instance = new ReignRequestFactory();
            return s_instance;
        }

        private MemoryStream stream;
        private EndianBinaryWriter writer;

        public ReignRequestFactory()
        {
            stream = new MemoryStream ();
            writer = new EndianBinaryWriter (EndianBitConverter.Big, stream);
        }
        
        internal byte[] GetBytes (ReignRequest rqst)
        {
#if REIGN_MSG_PROTOCOL_CLUSTER
			writer.Write (rqst.packageType);
            writer.Write (rqst.serverType);
            writer.Write (rqst.serverId);
#endif
            stream.Seek(0, SeekOrigin.End);
            byte[] commandBytes = Encoding.UTF8.GetBytes (rqst.command);
            byte[] bytes = new byte[32];
            Array.Copy (commandBytes, bytes, commandBytes.Length);
            writer.Write (bytes);

            writer.Write (rqst.requestId);

            bytes = Encoding.UTF8.GetBytes (rqst.content);
            writer.Write (bytes);
            bytes = stream.ToArray ();
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);

            // 先写入长度，再写入内容
            stream.Seek(0, SeekOrigin.End);
            writer.Write (bytes.Length);
            writer.Write (bytes);
            bytes = stream.ToArray ();
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            
            return bytes;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ReignRequest : IRequest
    {
        // 包的类型 [ 1:请求包 ]
        public byte packageType = 1;
        // 服务器类型
        public int serverType;
        // 服务器 ID
        public int serverId;
        // 命令
        public string command;
        // 请求 ID
        public int requestId;
        // 内容
        public string content;

		
        public ReignRequest (int serverType, int serverId, string command, string content, int requestId)
        {
            this.serverType = serverType;
            this.serverId = serverId;
            this.command = command;
            this.content = content;
            this.requestId = requestId;
        }


        [NoToLua]
        public byte[] GetBytes ()
        {
            return ReignRequestFactory.GetInstance().GetBytes(this);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class ReignResponse : IResponse
    {
        // 命令
        public string command;
        // 请求 ID
        public int requestId;
        // 内容
        public string content;

        private MemoryStream _output;
        private Inflater _inflater;
        private byte[] _buff;
        private int _len;
        private byte[] _bytes;
        public ReignResponse()
        { 
            _output = new MemoryStream();
            _inflater = new Inflater ();
            _buff = new byte[2048];
        }

        [NoToLua]
        public void Decode(EndianBinaryReader reader, int dataLen, bool isCompress)
        {
#if REIGN_MSG_PROTOCOL_CLUSTER
			reader.ReadByte ();
#endif
            _bytes = reader.ReadBytes (32);
            command = Encoding.UTF8.GetString (_bytes);
            command = command.Trim ('\0');

            requestId = reader.ReadInt32 ();

#if REIGN_MSG_PROTOCOL_CLUSTER
			bytes = reader.ReadBytes (dataLen - 37);
#else
            _bytes = reader.ReadBytes (dataLen - 36);
#endif
            if (isCompress) {
                try
                {
                    _output.Seek(0, SeekOrigin.End);
                    _inflater.SetInput (_bytes);
                    while (!_inflater.IsFinished) {
                        _len = _inflater.Inflate (_buff);
                        _output.Write (_buff, 0, _len);
                    }
                    content = Encoding.UTF8.GetString (_output.ToArray ());
                    _output.Seek(0, SeekOrigin.Begin);
                    _output.SetLength(0);
                    _inflater.Reset();
                } catch (Exception e) {
                    content = "decompress error:" + e.ToString ();
                }

            } else {
                content = Encoding.UTF8.GetString (_bytes);
            }
        }
    }
}

