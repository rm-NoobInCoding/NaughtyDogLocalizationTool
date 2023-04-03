using Be.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NaughtyDogLocalizationTool
{
    internal class Program
    {
        public static string[] LocalizationExtensions = { ".subtitles", ".common", ".subtitles-systemic" };
        public static byte[] UnknownString = Encoding.UTF8.GetBytes("UNKNOWN STRING!!!\0");
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                Console.WriteLine("Using this tool, you can edit the localization files of all Naughty Dog games (which contain the extensions .subtitles and .common).");
                Console.WriteLine("Usage:");
                Console.WriteLine("\tExport: " + AppDomain.CurrentDomain.FriendlyName + " <localization file> [version (default is 2)]");
                Console.WriteLine("");
                Console.WriteLine("To convert it into a localization file, just drag the text file into the tool to create a new localization file");
                Console.WriteLine("\tImport: " + AppDomain.CurrentDomain.FriendlyName + " <localization file>");
                
                Console.ReadKey();
            }
            else
            {

                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                if (LocalizationExtensions.Contains(Path.GetExtension(args[0])))
                {
                    int ver = 2;
                    if (args.Length == 2) ver = Int32.Parse(args[1]);
                    Console.WriteLine("Selected Ver" + ver);
                    switch (ver)
                    {
                        case 0:
                            Export0(args[0]);
                            break;
                        case 1:
                            Export1(args[0]);
                            break;
                        case 2:
                            Export2(args[0]);
                            break;
                    }

                }
                else if (Path.GetExtension(args[0]) == ".txt")
                {

                    List<string> ids = new List<string>(File.ReadAllLines(Path.ChangeExtension(args[0], ".ids")));
                    List<string> strings = new List<string>(File.ReadAllLines(args[0]));

                    int ver = Convert.ToInt32(ids[0].Split('|')[0]);

                    switch (ver)
                    {
                        case 2:
                            Import2(args[0], ids, strings);
                            break;
                        case 1:
                            Import1(args[0], ids, strings);
                            break;
                        case 0:
                            Import0(args[0], ids, strings);
                            break;
                    }
                }
            }
        }

        static void Export2(string path)
        {
            BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            List<long> offsets = new List<long>();
            List<string> ids = new List<string>();
            List<string> strings = new List<string>();
            ids.Add(3 + "|" + Path.GetExtension(path).Replace(".", ""));
            int CountOfStrings = Read.ReadInt32();
            for (int i = 0; i < CountOfStrings; i++)
            {
                ids.Add(Read.ReadUInt64().ToString());
                offsets.Add(Read.ReadInt64());

            }
            long StringTablePos = Read.BaseStream.Position;
            for (int i = 0; i < CountOfStrings; i++)
            {
                Read.BaseStream.Position = StringTablePos + offsets[i];
                string str = Read.ReadNullTerminatedString();
                strings.Add(str);

            }
            File.WriteAllLines(path + ".txt", strings);
            File.WriteAllLines(path + ".ids", ids);
            Console.WriteLine("Done!");
        }
        static void Export1(string path)
        {
            BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            List<int> offsets = new List<int>();
            List<string> ids = new List<string>();
            List<string> strings = new List<string>();
            ids.Add(path + "|" + Path.GetExtension(path).Replace(".", ""));
            int CountOfStrings = Read.ReadInt32();
            for (int i = 0; i < CountOfStrings; i++)
            {
                ids.Add(Read.ReadUInt32().ToString());
                offsets.Add(Read.ReadInt32());

            }
            long StringTablePos = Read.BaseStream.Position;
            for (int i = 0; i < CountOfStrings; i++)
            {
                Read.BaseStream.Position = StringTablePos + offsets[i];
                string str = Read.ReadNullTerminatedString();
                strings.Add(str);

            }
            File.WriteAllLines(path + ".txt", strings);
            File.WriteAllLines(path + ".ids", ids);
            Console.WriteLine("Done!");
        }
        static void Export0(string path)
        {
            BeBinaryReader Read = new BeBinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            List<int> offsets = new List<int>();
            List<string> ids = new List<string>();
            List<string> strings = new List<string>();
            ids.Add(path + "|" + Path.GetExtension(path).Replace(".", ""));
            int CountOfStrings = Read.ReadInt32();
            for (int i = 0; i < CountOfStrings; i++)
            {
                ids.Add(Read.ReadUInt32().ToString());
                offsets.Add(Read.ReadInt32());

            }
            long baseoff = Read.BaseStream.Position;
            for (int i = 0; i < CountOfStrings; i++)
            {
                Read.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                string str = Read.ReadNullTerminatedString();
                strings.Add(str);
                Read.BaseStream.Seek(baseoff, SeekOrigin.Begin);

            }
            File.WriteAllLines(path + ".txt", strings);
            File.WriteAllLines(path + ".ids", ids);
            Console.WriteLine("Done!");
        }

        static void Import2(string txtpath, List<string> ids, List<string> strings)
        {
            BinaryWriter Write = new BinaryWriter(new MemoryStream());
            List<byte[]> stringsInbyte = new List<byte[]>();
            string FileExtension = ids[0].Split('|')[1];
            ids.RemoveAt(0);
            int CountOfStrings = strings.Count;
            Write.Write(CountOfStrings);
            long TempOffset = 0;
            for (int i = 0; i < CountOfStrings; i++)
            {
                stringsInbyte.Add(Helpers.WriteNullTerminatedString(strings[i]));
                Write.Write(Convert.ToUInt64(ids[i]));
                Write.Write(TempOffset);
                TempOffset += stringsInbyte[i].Length;

            }
            Write.Write(UnknownString);
            for (int i = 0; i < CountOfStrings; i++)
            {
                Write.Write(stringsInbyte[i]);
            }
            File.WriteAllBytes(Path.ChangeExtension(txtpath, FileExtension) + ".new", Write.ToByteArray());
            Console.WriteLine("Done!");
        }
        static void Import1(string txtpath, List<string> ids, List<string> strings)
        {
            BinaryWriter Write = new BinaryWriter(new MemoryStream());
            List<byte[]> stringsInbyte = new List<byte[]>();
            string FileExtension = ids[0].Split('|')[1];
            ids.RemoveAt(0);
            int CountOfStrings = strings.Count;
            Write.Write(CountOfStrings);
            long TempOffset = UnknownString.Length;
            for (int i = 0; i < CountOfStrings; i++)
            {
                stringsInbyte.Add(Helpers.WriteNullTerminatedString(strings[i]));
                Write.Write(Convert.ToUInt32(ids[i]));
                Write.Write(TempOffset);
                TempOffset += stringsInbyte[i].Length;

            }
            Write.Write(UnknownString);
            for (int i = 0; i < CountOfStrings; i++)
            {
                Write.Write(stringsInbyte[i]);
            }
            File.WriteAllBytes(Path.ChangeExtension(txtpath, FileExtension) + ".new", Write.ToByteArray());
            Console.WriteLine("Done!");
        }
        static void Import0(string txtpath, List<string> ids, List<string> strings)
        {
            BeBinaryWriter Write = new BeBinaryWriter(new MemoryStream());
            List<byte[]> stringsInbyte = new List<byte[]>();
            string FileExtension = ids[0].Split('|')[1];
            ids.RemoveAt(0);
            int CountOfStrings = strings.Count;
            Write.Write(CountOfStrings);
            long TempOffset = CountOfStrings * 8 + 4;
            for (int i = 0; i < CountOfStrings; i++)
            {
                stringsInbyte.Add(Helpers.WriteNullTerminatedString(strings[i]));
                Write.Write(Convert.ToUInt32(ids[i]));
                Write.Write(TempOffset);
                TempOffset += stringsInbyte[i].Length;

            }
            Write.Write(UnknownString);
            for (int i = 0; i < CountOfStrings; i++)
            {
                Write.Write(stringsInbyte[i]);
            }
            File.WriteAllBytes(Path.ChangeExtension(txtpath, FileExtension) + ".new", Write.ToByteArray());
            Console.WriteLine("Done!");
        }

    }
}
