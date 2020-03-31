using System.Collections.Generic;
using MiscUtil.IO;

namespace Framework
{
    public interface IResponse
    {
        void Decode(EndianBinaryReader reader, int dataLen, bool isCompress);
    }
}