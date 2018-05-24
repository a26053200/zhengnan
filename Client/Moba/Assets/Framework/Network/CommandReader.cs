using System;
using System.Collections.Generic;
using System.IO;
using System.Net;


public class CommandReader
{
    private byte[] jsonBytes;

    private MemoryStream memStream;
    private BinaryReader reader;

    private int packLen;
    public CommandReader()
    {
        jsonBytes = new byte[JsonSocket.MAX_PACK_LEN];
		memStream = new MemoryStream();
		reader = new BinaryReader(memStream);
	}
    public void decode(byte[] receiveData, int length, List<byte[]> datas)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(receiveData, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (remainingBytesLen > 4)
        {
            packLen = IPAddress.NetworkToHostOrder(reader.ReadInt32());//包长
            if(remainingBytesLen >= packLen)
            {
                byte[] dataBytes = reader.ReadBytes(packLen);
                datas.Add(dataBytes);
            }
            else
            {
                memStream.Position = memStream.Position - 4;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        int leftlen = (int)remainingBytesLen;
        if (leftlen > 0)
        {
            byte[] leftover = reader.ReadBytes(leftlen);
            memStream.SetLength(0);     //Clear
            memStream.Write(leftover, 0, leftover.Length);
        }
        else
        {
            memStream.SetLength(0);
        }
    }
   
    long remainingBytesLen
    {
        get
        {
            return memStream.Length - memStream.Position;
        }
    }
	public void reset()
    {
		memStream.Close();
	}
}
