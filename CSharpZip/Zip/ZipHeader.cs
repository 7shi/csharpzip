// public domain

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ZipHeader
{
    public ushort Version, Flags, Compression, DosTime, DosDate;
    public uint Crc32, CompressedSize, UncompressedSize;
    public ushort FilenameLength, ExtraFieldLength;

    public ZipHeader(BinaryReader br)
    {
        Version = br.ReadUInt16();
        Flags = br.ReadUInt16();
        Compression = br.ReadUInt16();
        DosTime = br.ReadUInt16();
        DosDate = br.ReadUInt16();
        Crc32 = br.ReadUInt32();
        CompressedSize = br.ReadUInt32();
        UncompressedSize = br.ReadUInt32();
        FilenameLength = br.ReadUInt16();
        ExtraFieldLength = br.ReadUInt16();
    }
}
