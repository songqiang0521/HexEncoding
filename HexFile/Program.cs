using System;

namespace HexFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string src = args[0];
                string dst = src + ".hex";

                HexEncoding.HexConverter.HexFile(src, dst, false);
            }
            else if (args.Length == 2)
            {
                string src = args[0];
                string dst = src + ".hex";

                if (args[1] == "ascii")
                {
                    HexEncoding.HexConverter.HexFile(src, dst, true);
                }
            }

            Console.WriteLine("success");
        }
    }
}