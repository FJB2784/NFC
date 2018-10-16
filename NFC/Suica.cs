using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FelicaLib;

namespace NFC
{
    class Suica
    {
        private Felica felica;

        public enum SuicaServiceCode : int
        {
            History = 0x090f
        }

        public enum InOut : int
        {
            In = 0,
            Out = 1
        }

        public void PrintSuicaInfo(SuicaServiceCode code)
        {
            felica.Polling((int)SystemCode.Suica);

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
                    Console.WriteLine("処理日付 : {0}", BinaryToDate(data[4], data[5]).ToString("yyyy/MM/dd"));
                    Console.WriteLine("詳細情報 : {0:X2}{1:X2}{2:X2}{3:X2}", data[6], data[7], data[8], data[9]);
                    Console.WriteLine("残額 : \\ {0}", BitConverter.ToUInt16(data, 10));
                    Console.WriteLine("不明 : {0:X2}", data[12]);
                    Console.WriteLine("取引通番 : {0}", BitConverter.ToUInt16(new byte[] { data[14], data[13] }, 0));
                    Console.WriteLine("地域(入場) : {0}", GetChiikiCode(data[15], InOut.In));
                    Console.WriteLine("地域(出場) : {0}", GetChiikiCode(data[15], InOut.Out));
                    Console.WriteLine("不明 : {0:X2}", BinaryToInt32(ByteToBinary(data[15]), 7, 4));
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
                    return $"判別不能({data:x2})";
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
                    return $"判別不能{data:x2}";
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
                    return $"判別不能({data:x2})";
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
                    return $"判別不能({data:x2})";
            }
        }

        private static string GetChiikiCode(byte data, InOut io)
        {
            byte[] data_bit = ByteToBinary(data);
            int code;
            if (io == InOut.In)
            {
                code = BinaryToInt32(data_bit, 1, 2);
            }
            else
            {
                code = BinaryToInt32(data_bit, 3, 2);
            }

            switch (code)
            {
                case 0:
                    return "首都圏";
                case 2:
                    return "関西圏";
                case 3:
                    return "地方";
                default:
                    return $"不明({code:X2})";
            }
        }

        private static DateTime BinaryToDate(byte data_0, byte data_1)
        {
            // 2進数16桁を1ビットずつbyte配列に格納
            byte[] data_bit = new byte[16];
            for (int i = 7, temp = data_0; 0 <= i; i--, temp /= 2)
            {
                data_bit[i] = (byte)(temp % 2);
            }
            for (int i = 15, temp = data_1; 8 <= i; i--, temp /= 2)
            {
                data_bit[i] = (byte)(temp % 2);
            }

            // 7, 4, 5ビットごとに10進数に変換しDateTimeとして返す
            return new DateTime(BinaryToInt32(data_bit, 6, 7) + 2000, BinaryToInt32(data_bit, 10, 4), BinaryToInt32(data_bit, 15, 5));
        }

        private static byte[] ByteToBinary(byte data)
        {
            byte[] bit = new byte[8];
            for (int i = 7; 0 <= i; i--, data /= 2)
            {
                bit[i] = (byte)(data % 2);
            }

            return bit;
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
    }
}
