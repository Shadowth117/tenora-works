﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace psu_generic_parser
{
    public class ItemSuitParamFile : PsuFile
    {
        public class outfit
        {
            ushort appearance;
            byte category;
            byte manufacturer;
            byte overlap;
            byte restrict;
            byte color0;
            byte color1;
            byte color2;
            byte color3;
            byte color4;
            byte color5;
            byte color6;
            byte color7;
            byte color8;
            byte color9;
            byte orString;

            public ushort Appearance
            {
                get { return appearance; }
                set { appearance = value; }
            }

            public byte Category
            {
                get { return category; }
                set { category = value; }
            }

            public byte Manufacturer
            {
                get { return manufacturer; }
                set { manufacturer = value; }
            }

            public byte Overlap
            {
                get { return overlap; }
                set { overlap = value; }
            }

            public byte Restrict
            {
                get { return restrict; }
                set { restrict = value; }
            }

            public byte OrString
            {
                get { return orString; }
                set { orString = value; }
            }

            public byte Color0
            {
                get { return color0; }
                set { color0 = value; }
            }

            public byte Color1
            {
                get { return color1; }
                set { color1 = value; }
            }

            public byte Color2
            {
                get { return color2; }
                set { color2 = value; }
            }

            public byte Color3
            {
                get { return color3; }
                set { color3 = value; }
            }

            public byte Color4
            {
                get { return color4; }
                set { color4 = value; }
            }

            public byte Color5
            {
                get { return color5; }
                set { color5 = value; }
            }

            public byte Color6
            {
                get { return color6; }
                set { color6 = value; }
            }

            public byte Color7
            {
                get { return color7; }
                set { color7 = value; }
            }

            public byte Color8
            {
                get { return color8; }
                set { color8 = value; }
            }

            public byte Color9
            {
                get { return color9; }
                set { color9 = value; }
            }
        }

        public ArrayList[] clothes = new ArrayList[6];

        public ItemSuitParamFile(string inFilename, byte[] rawData, byte[] subHeader, int[] ptrs, int baseAddr)
        {
            filename = inFilename;
            header = subHeader;
            calculatedPointers = ptrs;
            MemoryStream inputStream = new MemoryStream(rawData);
            BinaryReader inputReader = new BinaryReader(inputStream);
            inputStream.Seek(4, SeekOrigin.Begin);
            int origFilesize = inputReader.ReadInt32();
            int headerLoc = inputReader.ReadInt32();
            inputStream.Seek(headerLoc, SeekOrigin.Begin);
            int[] listLocs = new int[6];
            int[] listCounts = new int[6];
            for (int i = 0; i < 6; i++)
            {
                listLocs[i] = inputReader.ReadInt32() - baseAddr;
                inputReader.ReadInt32();
                inputReader.ReadInt16();
                listCounts[i] = inputReader.ReadInt16();
            }

            for (int i = 0; i < 6; i++)
            {
                clothes[i] = new ArrayList();
                if (listCounts[i] > 0)
                {
                    inputStream.Seek(listLocs[i], SeekOrigin.Begin);
                    for (int j = 0; j < listCounts[i]; j++)
                    {
                        outfit temp = new outfit();
                        temp.Appearance = inputReader.ReadUInt16();
                        byte catManu = inputReader.ReadByte();
                        temp.Category = (byte)(catManu >> 4);
                        temp.Manufacturer = (byte)(catManu & 0xF);
                        temp.Overlap = inputReader.ReadByte();
                        temp.Restrict = inputReader.ReadByte();
                        temp.Color0 = inputReader.ReadByte();
                        temp.Color1 = inputReader.ReadByte();
                        temp.Color2 = inputReader.ReadByte();
                        temp.Color3 = inputReader.ReadByte();
                        temp.Color4 = inputReader.ReadByte();
                        temp.Color5 = inputReader.ReadByte();
                        temp.Color6 = inputReader.ReadByte();
                        temp.Color7 = inputReader.ReadByte();
                        temp.Color8 = inputReader.ReadByte();
                        temp.Color9 = inputReader.ReadByte();
                        temp.OrString = inputReader.ReadByte();
                        clothes[i].Add(temp);
                    }
                }
            }
        }

        public override byte[] ToRaw()
        {
            calculatedPointers = new int[6];
            MemoryStream outStream = new MemoryStream();
            BinaryWriter outWriter = new BinaryWriter(outStream);
            outWriter.Write((int)(0x0052584E));
            outStream.Seek(0x10, SeekOrigin.Begin);

            outWriter.Write((int)1);
            outWriter.Write((int)-1);
            outWriter.Write((int)-1);

            int[] listLocs = new int[6];
            for (int i = 0; i < 6; i++)
            {
                listLocs[i] = (int)outStream.Position;
                for (int j = 0; j < clothes[i].Count; j++)
                {
                    outfit temp = (outfit)clothes[i][j];
                    outWriter.Write(temp.Appearance);
                    outWriter.Write((byte)(temp.Category << 4 | temp.Manufacturer));
                    outWriter.Write(temp.Overlap);
                    outWriter.Write(temp.Restrict);
                    outWriter.Write(temp.Color0);
                    outWriter.Write(temp.Color1);
                    outWriter.Write(temp.Color2);
                    outWriter.Write(temp.Color3);
                    outWriter.Write(temp.Color4);
                    outWriter.Write(temp.Color5);
                    outWriter.Write(temp.Color6);
                    outWriter.Write(temp.Color7);
                    outWriter.Write(temp.Color8);
                    outWriter.Write(temp.Color9);
                    outWriter.Write(temp.OrString);
                }
            }
            int headerLoc = (int)outStream.Position;
            for (int i = 0; i < 6; i++)
            {
                calculatedPointers[i] = (int)outStream.Position;
                outWriter.Write(listLocs[i]);
                outWriter.Write((int)0);
                outWriter.Write((short)0);
                outWriter.Write((short)clothes[i].Count);
            }
            int fileSize = (int)((outStream.Position + 0xF) & 0xFFFFFF0);
            outStream.Seek(0x4, SeekOrigin.Begin);
            outWriter.Write(fileSize);
            outWriter.Write(headerLoc);
            return outStream.ToArray();
        }

        public void saveTextFile(Stream outStream)
        {
            StreamWriter textOut = new StreamWriter(outStream);
            textOut.WriteLine("//cat\tappear\torval\tmanuf\tarrow\trace\tcolor0\tcolor1\tcolor2\tcolor3\tcolor4\tcolor5\tcolor6\tcolor7\tcolor8\tcolor9");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < clothes[i].Count; j++)
                {
                    textOut.Write(i + 1 + "\t");
                    textOut.Write(BitConverter.ToString(BitConverter.GetBytes(((outfit)clothes[i][j]).Appearance)).Replace("-", "") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).OrString.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Manufacturer + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Overlap.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Restrict.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color0.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color1.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color2.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color3.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color4.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color5.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color6.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color7.ToString("X2") + "\t");
                    textOut.Write(((outfit)clothes[i][j]).Color8.ToString("X2") + "\t");
                    textOut.WriteLine(((outfit)clothes[i][j]).Color9.ToString("X2") + "\t");
                }
                textOut.WriteLine();
            }
            textOut.Close();
        }

        public void loadTextFile(Stream inStream)
        {
            StreamReader fileReader = new StreamReader(inStream);
            for (int i = 0; i < 6; i++)
                clothes[i] = new ArrayList();
            while (!fileReader.EndOfStream)
            {
                string currLine = fileReader.ReadLine();
                if (!currLine.StartsWith("//") && currLine.Trim() != "")
                {
                    string[] splitLine = currLine.Split('\t');
                    {
                        byte category = Convert.ToByte(splitLine[0]);
                        outfit temp = new outfit();
                        temp.Appearance = (ushort)(byte.Parse(splitLine[1].Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier) | (byte.Parse(splitLine[1].Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier) << 8));
                        temp.OrString = byte.Parse(splitLine[2], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Manufacturer = byte.Parse(splitLine[3], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Overlap = byte.Parse(splitLine[4], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Restrict = byte.Parse(splitLine[5], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color0 = byte.Parse(splitLine[6], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color1 = byte.Parse(splitLine[7], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color2 = byte.Parse(splitLine[8], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color3 = byte.Parse(splitLine[9], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color4 = byte.Parse(splitLine[10], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color5 = byte.Parse(splitLine[11], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color6 = byte.Parse(splitLine[12], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color7 = byte.Parse(splitLine[13], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color8 = byte.Parse(splitLine[14], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Color9 = byte.Parse(splitLine[15], System.Globalization.NumberStyles.AllowHexSpecifier);
                        temp.Category = category;
                        clothes[category - 1].Add(temp);
                    }
                }
            }
        }
    }
}
