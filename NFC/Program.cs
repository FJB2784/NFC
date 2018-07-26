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
            do
            {
                try
                {
                    Felica felica = new Felica();

                    felica.Polling((int)SystemCode.Suica);
                    Console.WriteLine("IDm : " + BitConverter.ToString(felica.IDm()).Replace("-", ""));
                    Console.WriteLine("PMm : " + BitConverter.ToString(felica.PMm()).Replace("-", ""));
                    Console.WriteLine();
                    //PrintSuicaInfo(felica);
                    PrintSuicaInfo(felica, SuicaServiceCode.History);

                    felica.Dispose();
                }
                catch
                {
                    Console.WriteLine("読み取り失敗");
                }
            } while (Console.ReadLine() == "");
        }

        enum SuicaServiceCode : int
        {
            History = 0x090f
        }

        private static void PrintSuicaInfo(this Felica felica, SuicaServiceCode code)
        {
            if (code == SuicaServiceCode.History)
            {
                for (int i = 0; ; i++)
                {
                    byte[] data = felica.ReadWithoutEncryption(0x090f, i);
                    if (data == null) { break; }

                    Console.WriteLine("機器種別 : {0}", GetKikiShubetsu(data[0]));
                    Console.WriteLine("利用種別 : {0}", GetRiyouShubetsu(data[1]));
                    Console.WriteLine("支払種別 : {0}", GetShiharaiShubetsu(data[2]));
                    Console.WriteLine("入出場種別 : {0}", GetNyushutsujoShubetsu(data[3]));
                    //Console.WriteLine("処理日付 : {0}", );
                    BinaryToDate(new byte[] { data[4], data[5] });
                    //Console.WriteLine(" : {0:X2}", data[0]);
                    //Console.WriteLine("機器種別 : {0:X2}", data[0]);
                    //Console.WriteLine("機器種別 : {0:X2}", data[0]);
                    //Console.WriteLine("機器種別 : {0:X2}", data[0]);
                    //Console.WriteLine("機器種別 : {0:X2}", data[0]);
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }

        private static string GetKikiShubetsu(byte data)
        {
            switch (data)
            {
                case 0x03:
                    return "のりこし精算機";
                case 0x04:
                    return "携帯端末";
                case 0x05:
                    return "バス等車載機";
                case 0x07:
                    return "カード販売機";
                case 0x08:
                    return "自動券売機";
                case 0x09:
                    return "チャージ機";
                case 0x12:
                    return "自動券売機";
                case 0x14:
                    return "駅務機器";
                case 0x15:
                    return "定期券発売機";
                case 0x16:
                    return "自動改札機";
                case 0x17:
                    return "簡易改札機";
                case 0x18:
                    return "駅務機器";
                case 0x19:
                    return "窓口処理機(みどりの窓口)";
                case 0x1A:
                    return "窓口処理機(有人改札)";
                case 0x1B:
                    return "モバイルFeliCa";
                case 0x1C:
                    return "入場券券売機";
                case 0x1D:
                    return "他社乗換自動改札機";
                case 0x1F:
                    return "入金機";
                case 0x20:
                    return "発行機";
                case 0x22:
                    return "簡易改札機";
                case 0x34:
                    return "バス等車載機";
                case 0x36:
                    return "バス等車載機";
                default:
                    return "判別不能";
            }
        }

        private static string GetRiyouShubetsu(byte data)
        {
            switch (data)
            {
                case 0x01:
                    return "自動改札機出場";
                case 0x02:
                    return "SFチャージ";
                case 0x03:
                    return "きっぷ購入";
                case 0x04:
                    return "磁気券精算";
                case 0x05:
                    return "乗越精算";
                case 0x06:
                    return "窓口出場";
                case 0x07:
                    return "新規";
                case 0x08:
                    return "控除";
                case 0x0D:
                    return "バス等均一運賃";
                case 0x0F:
                    return "バス等";
                case 0x11:
                    return "再発行";
                case 0x13:
                    return "料金出場";
                case 0x14:
                    return "オートチャージ";
                case 0x1F:
                    return "バス等チャージ";
                case 0x46:
                    return "物販";
                case 0x48:
                    return "ポイントチャージ";
                case 0x4B:
                    return "入場・物販";
                default:
                    return "判別不能";
            }
        }

        private static string GetShiharaiShubetsu(byte data)
        {
            switch (data)
            {
                case 0x00:
                    return "現金/なし";
                case 0x02:
                    return "VIEW";
                case 0x0B:
                    return "PiPaPa";
                case 0x0D:
                    return "オートチャージ対応PASMO";
                case 0x3F:
                    return "モバイルSuica";
                default:
                    return "判別不能";
            }
        }

        private static string GetNyushutsujoShubetsu(byte data)
        {
            switch (data)
            {
                case 0x00:
                    return "なし";
                case 0x01:
                    return "入場";
                case 0x02:
                    return "入場・出場";
                case 0x03:
                    return "定期入場・出場";
                case 0x04:
                    return "入場・定期出場";
                case 0x0E:
                    return "窓口出場";
                case 0x0F:
                    return "入場・出場(バス等)";
                case 0x12:
                    return "料金定期入場・料金出場";
                case 0x17:
                    return "入場・出場(乗継割引)";
                case 0x21:
                    return "入場・出場(バス等・乗継割引)";
                default:
                    return "判別不能";
            }
        }

        private static void BinaryToDate(byte[] data)
        {
            if (data.Length != 2)
            {
                throw new Exception("dataの長さが2ではありません。");
            }

            Console.WriteLine("{0}{1}", data[0], data[1]);

            // 2進数16桁を1ビットずつbyte配列に格納
            byte[] data_bit = new byte[16];
            int temp = data[0];
            for (int i = 0, j = data[0]; 0 < j / 2; i++, j /= 2)
            {
                data_bit[i] = (byte)(j % 2);

                Console.Write(data_bit[i]);
            }
            temp = data[1];
            for (int i = 8, j = data[1]; 0 < j / 2; i++, j /= 2)
            {
                data_bit[i] = (byte)(j % 2);

                Console.Write(data_bit[i]);
            }

            // 7, 4, 5ビットごとに10進数に変換
            Console.WriteLine("年" + BinaryToInt32(data_bit, 6, 7));
            Console.WriteLine("月" + BinaryToInt32(data_bit, 10, 4));
            Console.WriteLine("日" + BinaryToInt32(data_bit, 15, 5));
        }

        private static int BinaryToInt32(byte[] data, int StartIndex, int Length)
        {
            int result = 0;
            for (int i = StartIndex, j = 0; StartIndex - Length < i; i--, j++)
            {
                result += data[i] * (int)Math.Pow(2, j);
            }

            return result;
        }

        private static void PrintSuicaInfo(this FelicaLib.Felica f)
        {
            // システムコード: 0003 (Suicaなどの領域)
            f.Polling((int)SystemCode.Suica);

            Console.WriteLine("IDm=" + BitConverter.ToString(f.IDm()));
            Console.WriteLine("PMm=" + BitConverter.ToString(f.PMm()));

            // Suica 各サービスコード
            for (int i = 0; ; i++)
            {
                // サービスコード　乗降履歴情報
                byte[] data = f.ReadWithoutEncryption(0x090f, i);
                if (data == null) break;
                Console.WriteLine("Suica 乗降履歴情報 [" + i + "]  " + BitConverter.ToString(data));
            }
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
