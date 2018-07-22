using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FelicaLib;
//using NfcStarterKitWrap;

namespace NFC
{
    static class Program
    {
        static void Main(string[] args)
        {
            Felica felica = new Felica();

            felica.Polling((int)SystemCode.Any);
            Console.WriteLine("IDm : " + BitConverter.ToString(felica.IDm()).Replace("-", ""));
            Console.WriteLine("PMm : " + BitConverter.ToString(felica.PMm()).Replace("-", ""));

            felica.Dispose();
        }

        private static void PrintSuicaNo(this FelicaLib.Felica f)
        {
            // システムコード: 0003 (Suicaなどの領域)
            f.Polling((int)SystemCode.Suica);

            Console.WriteLine("IDm=" + BitConverter.ToString(f.IDm()));
            Console.WriteLine("PMm=" + BitConverter.ToString(f.PMm()));

            // Suica 各サービスコード
            //for (int i = 0; ; i++)
            //{
            //    // サービスコード　乗降履歴情報
            //    byte[] data = f.ReadWithoutEncryption(0x090f, i);
            //    if (data == null) break;
            //    Console.WriteLine("Suica 乗降履歴情報 [" + i + "]  " + BitConverter.ToString(data));
            //}
            //for (int i = 0; ; i++)
            //{
            //    Console.WriteLine("Suica カード種別およびカード残額情報 " + i);
            //    // サービスコード　カード種別およびカード残額情報
            //    byte[] data = f.ReadWithoutEncryption(0x008B, i);
            //    if (data == null) break;
            //}
            //for (int i = 0; ; i++)
            //{
            //    Console.WriteLine("Suica 改札入出場履歴情報 " + i);
            //    // サービスコード　改札入出場履歴情報
            //    byte[] data = f.ReadWithoutEncryption(0x108F, i);
            //    if (data == null) break;
            //}
            //for (int i = 0; ; i++)
            //{
            //    Console.WriteLine("Suica SF入場情報 " + i);
            //    // サービスコード　SF入場情報
            //    byte[] data = f.ReadWithoutEncryption(0x10CB, i);
            //    if (data == null) break;
            //}
            //for (int i = 0; ; i++)
            //{
            //    Console.WriteLine("Suica 料金券情報 " + i);
            //    // サービスコード　料金券情報
            //    byte[] data = f.ReadWithoutEncryption(0x184B, i);
            //    if (data == null) break;
            //}

            //for (int i = 0; ; i++)
            //{
            //    byte[] data = f.ReadWithoutEncryption(0x008B, i);
            //    if (data == null) break;
            //}

        }
    }
}
