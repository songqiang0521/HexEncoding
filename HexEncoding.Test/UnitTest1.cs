using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace HexEncoding.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var dstFile=Path.GetTempFileName();
            var file=Process.GetCurrentProcess().MainModule.FileName;
            File.Copy(file, dstFile,true);

            HexEncoding.HexConverter.HexFile(dstFile, dstFile+".dat");
            Assert.True(1==1);

        }



    }
}
