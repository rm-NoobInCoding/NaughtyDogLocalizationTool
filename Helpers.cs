using System.IO;
using System;
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
            str = str.Replace("<cf>","\r\n");
            str = str.Replace("<cr>","\r");
            str = str.Replace("<lf>","\n");
            if (str == "[EmptyString]") str = "";
            return str;
        }
        
    }
}
