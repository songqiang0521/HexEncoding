using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace HexEncoding
{
    /// <summary>
    /// 此类将byte缓冲区转换为winhex样式字符串
    /// 输出格式为：
    /// 8个地址字符:16个十六进制字符 16个字符的ascii码
    /// </summary>
    public static class HexConverter
    {
        private const string HexChars = "0123456789abcdef";
        private const int ValuesPerLine = 16;
        private const int AddressCharCount = 8;

        /// <summary>
        /// 将源文件中的byte数组转换为winhex样式的字符串并存储到目标文件中
        /// </summary>
        /// <param name="srcFile">待转换的源文件名称</param>
        /// <param name="dstFile">存储转换结果的目标文件名称</param>
        public static void HexFile(string srcFile, string dstFile)
        {
            using (FileStream src = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream dst = new FileStream(dstFile, FileMode.Create, FileAccess.Write))
                {
                    HexStream(src, dst);
                }
            }
        }

        /// <summary>
        /// 将byte数组转换为winhex样式的字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ByteBufferToHexString(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            int size = GetStringSize(buffer.Length);
            byte[] result = new byte[size];

            MemoryStream src = new MemoryStream(buffer);
            MemoryStream dst = new MemoryStream(result);

            HexStream(src, dst);

            string text = Encoding.UTF8.GetString(result);
            return text;
        }

        private static void HexStream(Stream src, Stream dst)
        {
            if (!src.CanRead)
            {
                throw new ArgumentException("stream must be readable", "src");
            }
            if (!src.CanSeek)
            {
                throw new ArgumentException("stream must be seekable", "src");
            }

            if (!dst.CanWrite)
            {
                throw new ArgumentException("stream must be writeable", "dst");
            }

            using (BinaryReader br = new BinaryReader(src, Encoding.UTF8, true))
            {
                using (BinaryWriter bw = new BinaryWriter(dst, Encoding.UTF8, true))
                {
                    int bytesCount = (int)src.Length;
                    int rowIndex = 0;
                    int colIndex = 0;

                    //一行一行打印
                    for (rowIndex = 0; rowIndex * ValuesPerLine < bytesCount; rowIndex++)
                    {
                        //打印地址列
                        for (colIndex = AddressCharCount - 2; colIndex >= 0; colIndex--)
                        {
                            bw.Write(HexChars[(rowIndex >> (colIndex * 4)) & 15]);
                            Debug.Write(HexChars[(rowIndex >> (colIndex * 4)) & 15]);
                        }

                        bw.Write('0');
                        bw.Write(':');
                        bw.Write(' ');
                        Debug.Write("0: ");

                        //打印十六进制列
                        for (colIndex = 0; colIndex < ValuesPerLine; colIndex++)
                        {
                            if (rowIndex * ValuesPerLine + colIndex < bytesCount)
                            {
                                byte value = br.ReadByte();
                                bw.Write(HexChars[(value >> 4) & 15]);
                                bw.Write(HexChars[value & 15]);
                                bw.Write(' ');

                                Debug.Write(HexChars[(value >> 4) & 15]);
                                Debug.Write(HexChars[value & 15]);
                                Debug.Write(" ");
                            }
                            else
                            {
                                bw.Write(' ');
                                bw.Write(' ');
                                bw.Write(' ');
                                Debug.Write("   ");
                            }
                        }

                        //打印ascii之前，需要调整流的位置
                        long current = br.BaseStream.Position;
                        if (current % ValuesPerLine == 0)
                        {
                            br.BaseStream.Seek(-ValuesPerLine, SeekOrigin.Current);
                        }
                        else
                        {
                            br.BaseStream.Seek(0 - current % ValuesPerLine, SeekOrigin.Current);
                        }

                        //打印ascii
                        for (colIndex = 0; colIndex < ValuesPerLine; colIndex++)
                        {
                            if (rowIndex * ValuesPerLine + colIndex < bytesCount)
                            {
                                byte value = br.ReadByte();
                                byte c = Math.Max(value, (byte)32);
                                bw.Write((char)c);

                                Debug.Write((char)c);
                            }
                            else
                            {
                                bw.Write(' ');
                                Debug.Write(" ");
                            }
                        }

                        //打印回车换行
                        foreach (var item in Environment.NewLine)
                        {
                            bw.Write(item);
                        }
                        Debug.WriteLine("");
                    }
                }
            }
        }

        /// <summary>
        /// 根据byte数组的长度得到容纳结果字符串所需的存储空间
        /// </summary>
        /// <param name="bytesLength">待转换的byte数组的长度</param>
        /// <returns>所需存储空间大小</returns>
        private static int GetStringSize(int bytesLength)
        {
            int addressLength = AddressCharCount + 1 + 1;//地址字符加冒号加空格
            int valuesLength = ValuesPerLine * 3;//两个字符加一个空格
            int asciiLength = ValuesPerLine * 2;//ascii码值

            int sizePerLine = addressLength + valuesLength + asciiLength + Environment.NewLine.Length;

            int result = ((bytesLength / ValuesPerLine) + ((bytesLength % ValuesPerLine) == 0 ? 0 : 1)) * sizePerLine;
            return result;
        }
    }
}
