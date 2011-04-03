// public domain

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ZipDirHeader
{
    public ushort Version;
    public ZipHeader Header;
    public ushort FileCommentLength, DiskNumberStart, InternalFileAttrs;
    public uint Attrs, Position;
    public byte[] Filename;

    public ZipDirHeader(BinaryReader br)
    {
        Version = br.ReadUInt16();
        Header = new ZipHeader(br);
        FileCommentLength = br.ReadUInt16();
        DiskNumberStart = br.ReadUInt16();
        InternalFileAttrs = br.ReadUInt16();
        Attrs = br.ReadUInt32();
        Position = br.ReadUInt32();
        Filename = br.ReadBytes(Header.FilenameLength);
        var exlen = Header.ExtraFieldLength + FileCommentLength;
        br.BaseStream.Seek(exlen, SeekOrigin.Current);
    }

    public SubStream GetSubStream(BinaryReader br)
    {
        var fs = br.BaseStream;
        fs.Position = Position;
        if (br.ReadInt32() != 0x04034b50)
            throw new Exception("ファイルが壊れています。");

        var ziph = new ZipHeader(br);
        fs.Position += ziph.FilenameLength + ziph.ExtraFieldLength;
        return new SubStream(fs, Header.CompressedSize);
    }
}
