// public domain

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public static class Zip
{
    private static readonly uint[] crc32_table = new Func<uint[]>(() =>
    {
        var ret = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var crc = i;
            for (int j = 0; j < 8; j++)
            {
                var crc2 = crc >> 1;
                crc = (crc & 1) == 0 ? crc2 : crc2 ^ 0xedb88320u;
            }
        }
        return ret;
    })();

    public static uint Crc32(byte[] buf)
    {
        var crc = ~0u;
        foreach (var b in buf)
            crc = (crc >> 8) ^ crc32_table[(crc ^ b) & 0xff];
        return ~crc;
    }

    public static ZipDirHeader[] GetFiles(BinaryReader br, Func<ZipDirHeader, bool> f)
    {
        var list = new List<ZipDirHeader>();

        var fs = br.BaseStream;
        if (fs.Length < 22)
            throw new Exception("ファイルが小さ過ぎます。");

        fs.Position = fs.Length - 22;
        if (br.ReadInt32() != 0x06054b50)
            throw new Exception("ヘッダが見付かりません。");

        fs.Position += 6;
        int count = br.ReadUInt16();
        var dir_len = br.ReadUInt32();
        var dir_start = br.ReadUInt32();

        fs.Position = dir_start;
        for (int i = 0; i < count; i++)
        {
            if (br.ReadInt32() != 0x02014b50)
                throw new Exception("ファイルが壊れています。");
            var zipdh = new ZipDirHeader(br);
            if (f(zipdh)) list.Add(zipdh);
        }

        return list.ToArray();
    }
}
