using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NaughtyDogLocalizationTool
{
    public static class Helpers
    {
        public static string ReadNullTerminatedString(this System.IO.BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return StringClear(str);
        }
        public static byte[] WriteNullTerminatedString(string str)
        {
            return Encoding.UTF8.GetBytes(StringDeclear(str) + "\0");
        }
        public static byte[] ToByteArray(this System.IO.BinaryWriter a)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                a.BaseStream.Position = 0;
                a.BaseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static string StringClear(string str)
        {
            str = str.Replace("\r\n", "<cf>");
            str = str.Replace("\r", "<cr>");
            str = str.Replace("\n", "<lf>");
            if (str == "") str = "[EmptyString]";
            return str;
        }
        public static string StringDeclear(string str)
        {
            str = str.Replace("<cf>", "\r\n");
            str = str.Replace("<cr>", "\r");
            str = str.Replace("<lf>", "\n");
            if (str == "[EmptyString]") str = "";
            return str;
        }

        public static int ReadBEInt32(this BinaryReader br)
        {
            var data = br.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public static Int16 ReadBEInt16(this BinaryReader br)
        {
            var data = br.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public static Int64 ReadBEInt64(this BinaryReader br)
        {
            var data = br.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public static UInt32 ReadBEUInt32(this BinaryReader br)
        {
            var data = br.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }
        public static void WriteBE(this BinaryWriter bw, int value)
        {
             bw.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        public static void WriteBE(this BinaryWriter bw, uint value)
        {
            bw.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        public static void WriteBE(this BinaryWriter bw, long value)
        {
            bw.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }


    }
}
