using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FelicaLib;

namespace NFC
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Felica felica = new Felica();
            try {
                felica.Polling((int)SystemCode.FelicaLiteS);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("IDm : " + BitConverter.ToString(felica.IDm()));
            Console.WriteLine();

            if (args.Length == 1 && args[0].Equals("DUMP", StringComparison.CurrentCultureIgnoreCase))
            {
                Dump(ref felica);
            }
            else if (args.Length == 1 && args[0].Equals("READ", StringComparison.CurrentCultureIgnoreCase))
            {
                Read(ref felica);
            }
            else if (args.Length == 2 && args[0].Equals("WRITE", StringComparison.CurrentCultureIgnoreCase))
            {
                Write(ref felica, args[1]);
            }
            else
            {
                PrintHelp();
            }

            felica.Dispose();
        }

        private static void Write(ref Felica felica, string text)
        {
            byte[] text_bytes = GetBytes(text);

            for (int i = 0; i < FELICA_LITE_S_DATA_LENGTH / 0x10; i++)
            {
                byte[] data = new byte[0x10];
                for (int j = 0; j < 0x10; j++)
                {
                    if ((i << 4) + j < text_bytes.Length)
                    {
                        data[j] = text_bytes[(i << 4) + j];
                    }
                    else
                    {
                        data[j] = 0x00;
                    }
                }

                felica.WriteWithoutEncryption(0x0009, i, data);
            }

        }

        private static void Read(ref Felica felica)
        {
            string str = "";
            for (int i = 0; i < 0x0e; i++)
            {
                str += ToString(felica.ReadWithoutEncryption(0x0009, i));
            }
            Console.WriteLine(str);
        }

        private static void Dump(ref Felica felica)
        {
            Console.Write("addr  ");
            for (int i = 0; i < 0x10; i++)
            {
                Console.Write("+{0:X1} ", i);
            }
            Console.WriteLine();
            for (int i = 0; i < 0x0e; i++)
            {
                Console.WriteLine("00{0:X2}  " + BitConverter.ToString(felica.ReadWithoutEncryption(0x0009, i)).Replace('-', ' '), i);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Felica Lite-Sに文字列を書き込み、読み込むプログラムです。");
            Console.WriteLine();
            Console.WriteLine("NFC.exe DUMP");
            Console.WriteLine("Felica Lite-Sの全ブロックをダンプします。");
            Console.WriteLine();
            Console.WriteLine("NFC.exe READ");
            Console.WriteLine("Felica Lite-Sに保存されたテキストを読み込みます。");
            Console.WriteLine();
            Console.WriteLine("NFC.exe WRITE \"TEXT\"");
            Console.WriteLine("Felica Lite-Sにテキストを書き込みます。");
            Console.WriteLine("JIS X 0201 に準拠した文字列を入力してください。");
            Console.WriteLine("文字列内にスペースが含まれる場合ダブルクォーテーションで囲ってください。");
        }

        private const int FELICA_LITE_S_DATA_LENGTH = 0xe0;
        private const char NULL = '\0';
        private static char[,] CHARCODE_TABLE =
        {
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '\n', NULL, NULL, NULL, NULL, NULL }, // 0x00
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }, // 0x10
            { ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/' }, // 0x20
            { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?' }, // 0x30
            { '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O' }, // 0x40
            { 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_' }, // 0x50
            { '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o' }, // 0x60
            { 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', NULL }, // 0x70
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }, // 0x80
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }, // 0x90
            { NULL, '｡', '｢', '｣', '､', '･', 'ｦ', 'ｧ', 'ｨ', 'ｩ', 'ｪ', 'ｫ', 'ｬ', 'ｭ', 'ｮ', 'ｯ' }, // 0xA0
            { 'ｰ', 'ｱ', 'ｲ', 'ｳ', 'ｴ', 'ｵ', 'ｶ', 'ｷ', 'ｸ', 'ｹ', 'ｺ', 'ｻ', 'ｼ', 'ｽ', 'ｾ', 'ｿ' }, // 0xB0
            { 'ﾀ', 'ﾁ', 'ﾂ', 'ﾃ', 'ﾄ', 'ﾅ', 'ﾆ', 'ﾇ', 'ﾈ', 'ﾉ', 'ﾊ', 'ﾋ', 'ﾌ', 'ﾍ', 'ﾎ', 'ﾏ' }, // 0xC0
            { 'ﾐ', 'ﾑ', 'ﾒ', 'ﾓ', 'ﾔ', 'ﾕ', 'ﾖ', 'ﾗ', 'ﾘ', 'ﾙ', 'ﾚ', 'ﾛ', 'ﾜ', 'ﾝ', 'ﾞ', 'ﾟ' }, // 0xD0
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }, // 0xE0
            { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL } // 0xF0
        };

        private static byte[] GetBytes(string str)
        {
            byte[] data = new byte[FELICA_LITE_S_DATA_LENGTH];
            int count = 0;
            foreach (char c in str.ToCharArray())
            {
                bool flg = true;
                for (byte i = 0; i < 0x10; i++)
                {
                    for (byte j = 0; j < 0x10; j++)
                    {
                        if (c == CHARCODE_TABLE[i, j] && count < FELICA_LITE_S_DATA_LENGTH)
                        {
                            data[count] = (byte)((byte)(i << 4) + j);
                            count++;
                            break;
                        }
                        //else
                        //{
                        //    byte[] daku = GetDakutenHandakuten(c);
                        //    if (daku != null && count < FELICA_LITE_S_DATA_LENGTH - 1)
                        //    {
                        //        flg = false;
                        //        data[count] = daku[0];
                        //        count++;
                        //        data[count] = daku[1];
                        //        count++;
                        //        break;
                        //    }
                        //}

                        if (count == FELICA_LITE_S_DATA_LENGTH)
                        {
                            break;
                        }
                    }

                    if (count == FELICA_LITE_S_DATA_LENGTH || !flg)
                    {
                        break;
                    }
                }

                if (count == FELICA_LITE_S_DATA_LENGTH)
                {
                    break;
                }
            }

            return data;
        }

        //private static byte[] GetDakutenHandakuten(char c)
        //{
        //    byte[] data = new byte[2];

        //    switch (c)
        //    {
        //        case 'ガ':
        //            data[0] = 0xb6;
        //            break;
        //        case 'ギ':
        //            data[0] = 0xb7;
        //            break;
        //        case 'グ':
        //            data[0] = 0xb8;
        //            break;
        //        case 'ゲ':
        //            data[0] = 0xb9;
        //            break;
        //        case 'ゴ':
        //            data[0] = 0xba;
        //            break;
        //        case 'ザ':
        //            data[0] = 0xbb;
        //            break;
        //        case 'ジ':
        //            data[0] = 0xbc;
        //            break;
        //        case 'ズ':
        //            data[0] = 0xbd;
        //            break;
        //        case 'ゼ':
        //            data[0] = 0xbe;
        //            break;
        //        case 'ゾ':
        //            data[0] = 0xbf;
        //            break;
        //        case 'ダ':
        //            data[0] = 0xc0;
        //            break;
        //        case 'ヂ':
        //            data[0] = 0xc1;
        //            break;
        //        case 'ヅ':
        //            data[0] = 0xc2;
        //            break;
        //        case 'デ':
        //            data[0] = 0xc3;
        //            break;
        //        case 'ド':
        //            data[0] = 0xc4;
        //            break;
        //        case 'バ':
        //        case 'パ':
        //            data[0] = 0xca;
        //            break;
        //        case 'ビ':
        //        case 'ピ':
        //            data[0] = 0xcb;
        //            break;
        //        case 'ブ':
        //        case 'プ':
        //            data[0] = 0xcc;
        //            break;
        //        case 'ベ':
        //        case 'ペ':
        //            data[0] = 0xcd;
        //            break;
        //        case 'ボ':
        //        case 'ポ':
        //            data[0] = 0xce;
        //            break;
        //        default:
        //            data[0] = 0x00;
        //            break;
        //    }

        //    switch (c)
        //    {
        //        case 'ガ':
        //        case 'ギ':
        //        case 'グ':
        //        case 'ゲ':
        //        case 'ゴ':
        //        case 'ザ':
        //        case 'ジ':
        //        case 'ズ':
        //        case 'ゼ':
        //        case 'ゾ':
        //        case 'ダ':
        //        case 'ヂ':
        //        case 'ヅ':
        //        case 'デ':
        //        case 'ド':
        //        case 'バ':
        //        case 'ビ':
        //        case 'ブ':
        //        case 'ベ':
        //        case 'ボ':
        //            data[1] = 0xde;
        //            break;
        //        case 'パ':
        //        case 'ピ':
        //        case 'プ':
        //        case 'ペ':
        //        case 'ポ':
        //            data[1] = 0xdf;
        //            break;
        //        default:
        //            data[1] = 0x00;
        //            break;
        //    }

        //    if (data[0] == 0x00 && data[1] == 0x00)
        //    {
        //        return null;
        //    }

        //    return data;
        //}

        private static string ToString(byte[] data)
        {
            string str = "";
            foreach (byte b in data)
            {
                if (b == 0x00)
                {
                    break;
                }

                byte x = (byte)(b >> 4);
                byte y = (byte)(b - (x << 4));

                str += CHARCODE_TABLE[x, y];
            }
            return str;
        }
    }
}
