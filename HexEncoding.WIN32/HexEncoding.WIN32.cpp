// HexEncoding.WIN32.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <windows.h>



//根据缓冲区的长度得到所需字符串的容量
int GetBufferSize(int buflen)
{
    return ((buflen / 16) + 1) * (10 + 3 * 16 + 16 + 2);
}


bool DumpBuf(char* buf, int count, char* retBuffer, int size)
{
    const char HexChars[] = "0123456789abcdef";
    const int ValuesPerLine = 16;//每行字符个数
    const int AddressChars = 8;//地址栏的字符个数
    int rowIndex = 0;
    int colIndex = 0;

    int sizeRequired = GetBufferSize(count);
    if (sizeRequired > size)
    {
        return false;
    }

    memset(retBuffer, 0, size);
    int retIndex = 0;


    for (rowIndex = 0; rowIndex * ValuesPerLine < count; rowIndex++)//i表示行数
    {
        for (colIndex = AddressChars - 2; colIndex >= 0; colIndex--)
        {
            retBuffer[retIndex++] = HexChars[(rowIndex >> (colIndex * 4)) & 15];

            //cout << HexChars[(rowIndex >> (colIndex * 4)) & 15];
        }

        retBuffer[retIndex++] = '0';
        retBuffer[retIndex++] = ':';
        retBuffer[retIndex++] = ' ';
        //cout << "0: ";

        for (colIndex = 0; colIndex < ValuesPerLine; colIndex++)//j表示列数
        {
            if (rowIndex * ValuesPerLine + colIndex < count)//
            {
                unsigned char c = buf[rowIndex * ValuesPerLine + colIndex];


                retBuffer[retIndex++] = HexChars[(c >> 4) & 15];
                retBuffer[retIndex++] = HexChars[c & 15];
                retBuffer[retIndex++] = ' ';

                //cout << HexChars[(c >> 4) & 15] << HexChars[c & 15] << " ";
            }
            else
            {
                retBuffer[retIndex++] = ' ';
                retBuffer[retIndex++] = ' ';
                retBuffer[retIndex++] = ' ';

                //cout << "   ";
            }
        }

        for (colIndex = 0; colIndex < ValuesPerLine; colIndex++)
        {
            if (rowIndex * ValuesPerLine + colIndex < count)
            {
                unsigned char c = max(buf[rowIndex * ValuesPerLine + colIndex], 32);

                retBuffer[retIndex++] = c;
                //cout << c;
            }
            else
            {
                retBuffer[retIndex++] = ' ';
                //cout << " ";
            }
        }

        retBuffer[retIndex++] = 0x0D;
        retBuffer[retIndex++] = 0x0A;
        //cout << endl;
    }

    return true;

}
int main()
{
    char buffer[] = {"0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()_+"};
    int size=GetBufferSize(strlen(buffer));
    char* retBuffer = new char[size];
    
    DumpBuf(buffer, strlen(buffer), retBuffer, size);

    printf(retBuffer);



    return 0;
}

