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
        public static string UnknownString = "UNKNOWN STRING!!!";
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                Console.WriteLine("Using this tool, you can edit the localization files of all Naughty Dog games (which contain the extensions .subtitles and .common).");
                Console.WriteLine("For this, just drag the localization file into the tool to convert it into a text file");
                Console.WriteLine("To convert it into a localization file, just drag the text file into the tool to create a new localization file");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Naughty Dog Localization Tool");
                Console.WriteLine("By NoobInCoding");
                if (LocalizationExtensions.Contains(Path.GetExtension(args[0])))
                {
                    BinaryReader Read = new BinaryReader(new MemoryStream(File.ReadAllBytes(args[0])));
                    int ver = CheckVersion(File.ReadAllBytes(args[0]));
                    List<long> offsets = new List<long>();
                    List<string> ids = new List<string>();
                    List<string> strings = new List<string>();
                    ids.Add(ver + "|" + Path.GetExtension(args[0]).Replace(".", ""));
                    int CountOfStrings = Read.ReadInt32();
                    for (int i = 0; i < CountOfStrings; i++)
                    {
                        if (ver == 0)
                        {
                            ids.Add(Read.ReadUInt32().ToString());
                            offsets.Add(Read.ReadInt32());
                        }
                        if (ver == 1)
                        {
                            ids.Add(Read.ReadUInt64().ToString());
                            offsets.Add(Read.ReadInt64());
                        }

                    }
                    long StringTablePos = Read.BaseStream.Position;
                    for (int i = 0; i < CountOfStrings; i++)
                    {
                        Read.BaseStream.Position = StringTablePos + offsets[i];
                        string str = Read.ReadNullTerminatedString();
                        strings.Add(str);

                    }
                    File.WriteAllLines(args[0] + ".txt", strings);
                    File.WriteAllLines(args[0] + ".ids", ids);
                    Console.WriteLine("Done!");

                }
                else if (Path.GetExtension(args[0]) == ".txt")
                {
                    BinaryWriter Write = new BinaryWriter(new MemoryStream());
                    List<string> ids = new List<string>(File.ReadAllLines(Path.ChangeExtension(args[0], ".ids")));
                    List<string> strings = new List<string>(File.ReadAllLines(args[0]));
                    List<byte[]> stringsInbyte = new List<byte[]>();
                    string FileExtension = ids[0].Split('|')[1];
                    int ver = Convert.ToInt32(ids[0].Split('|')[0]);
                    ids.RemoveAt(0);
                    int CountOfStrings = strings.Count;
                    Write.Write(CountOfStrings);
                    int TempOffsetv0 = UnknownString.Length;
                    long TempOffsetv1 = 0;
                    for (int i = 0; i < CountOfStrings; i++)
                    {
                        stringsInbyte.Add(Helpers.WriteNullTerminatedString(strings[i]));
                        if (ver == 0)
                        {
                            Write.Write(Convert.ToUInt32(ids[i]));
                            Write.Write(TempOffsetv0);
                            TempOffsetv0 += stringsInbyte[i].Length;
                        }
                        else
                        {
                            Write.Write(Convert.ToUInt64(ids[i]));
                            Write.Write(TempOffsetv1);
                            TempOffsetv1 += stringsInbyte[i].Length;
                        }

                    }
                    if (ver == 0) Write.Write(Encoding.UTF8.GetBytes(UnknownString + "\0"));
                    for (int i = 0; i < CountOfStrings; i++)
                    {
                        Write.Write(stringsInbyte[i]);
                    }
                    File.WriteAllBytes(Path.ChangeExtension(args[0], FileExtension) + ".new", Write.ToByteArray());
                    Console.WriteLine("Done!");
                }
            }
        }
        static int CheckVersion(byte[] Data)
        {
            string tmp = Encoding.UTF8.GetString(Data);
            if (tmp.Contains(UnknownString))
            {
                //TLOU 1 (PS4) - Uncharted (4 or newer)
                return 0;
            }
            else
            {
                //TLOU 2 - TLOU (PC)
                return 1;
            }
        }

    }
}
