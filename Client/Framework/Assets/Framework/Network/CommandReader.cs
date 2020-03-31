using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MiscUtil.Conversion;
using MiscUtil.IO;


public class CommandReader
{
    public delegate void Response(EndianBinaryReader reader, int dataLen);
    private MemoryStream memStream;
    private EndianBinaryReader reader;

    private int packLen;
    public CommandReader()
    {
		memStream = new MemoryStream();
		reader = new EndianBinaryReader(EndianBitConverter.Big, memStream);
	}
    public void decode(byte[] receiveData, int length, Response response)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(receiveData, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (remainingBytesLen > 4)
        {
            //packLen = IPAddress.NetworkToHostOrder(reader.ReadInt32());//包长
            packLen = reader.ReadInt32();//包长
            if(remainingBytesLen >= packLen)
            {
                response(reader, packLen);
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
