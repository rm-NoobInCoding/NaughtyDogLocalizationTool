using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NaughtyDogLocalizationTool
{
    internal class Program
    {
        public static string[] LocalizationExtensions = { ".subtitles", ".common", ".subtitles-systemic", ".web" };
        public static byte[] UnknownString = Encoding.UTF8.GetBytes("UNKNOWN STRING!!!\0");
        public static string[] SupportedGames = { "Uncharted (ALL Games)", "Last Of Us (ALL Games)" };
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                Console.WriteLine("Using this tool, you can edit the localization files of all Naughty Dog games (*.subtitles or *.common).");
                Console.WriteLine("Usage:");
                Console.WriteLine("\tExport: " + AppDomain.CurrentDomain.FriendlyName + " <localization file>");
                Console.WriteLine("\tImport: " + AppDomain.CurrentDomain.FriendlyName + " <Exported txt file>");
                Console.WriteLine("\nSupported versions:\n");
                Console.WriteLine(string.Join("\r\n", SupportedGames));
                Console.ReadKey();
            }
            else
            {

                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                if (LocalizationExtensions.Contains(Path.GetExtension(args[0])))
                {
                    int ver = CheckVer(args[0]);
                    Console.WriteLine("Selected Ver: " + ver);
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

        static int CheckVer(string path)
        {
            BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            byte littleOrBig = Read.ReadByte();
            if (littleOrBig == 0)
            {
                //big
                return 0;
            }
            else
            {
                //lit
                Read.BaseStream.Position = 16;
                if (Read.ReadInt32() == 0)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }

            }

        }

        static void Export2(string path)
        {
            BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            List<long> offsets = new List<long>();
            List<string> ids = new List<string>();
            List<string> strings = new List<string>();
            ids.Add(2 + "|" + Path.GetExtension(path).Replace(".", ""));
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
            ids.Add(1 + "|" + Path.GetExtension(path).Replace(".", ""));
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
            BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            List<int> offsets = new List<int>();
            List<string> ids = new List<string>();
            List<string> strings = new List<string>();
            ids.Add(0 + "|" + Path.GetExtension(path).Replace(".", ""));
            int CountOfStrings = Read.ReadBEInt32();
            for (int i = 0; i < CountOfStrings; i++)
            {
                ids.Add(Read.ReadBEUInt32().ToString());
                offsets.Add(Read.ReadBEInt32());

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
            BinaryWriter Write = new BinaryWriter(new MemoryStream());
            List<byte[]> stringsInbyte = new List<byte[]>();
            string FileExtension = ids[0].Split('|')[1];
            ids.RemoveAt(0);
            int CountOfStrings = strings.Count;
            Write.WriteBE(CountOfStrings);
            long TempOffset = 0;
            for (int i = 0; i < CountOfStrings; i++)
            {
                stringsInbyte.Add(Helpers.WriteNullTerminatedString(strings[i]));
                Write.WriteBE(Convert.ToUInt32(ids[i]));
                Write.WriteBE((int)TempOffset);
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
