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
        static void Main(string[] args)
        {
            Felica felica = new Felica();
            felica.Polling(0x88b4);
            Console.WriteLine("IDm : " + BitConverter.ToString(felica.IDm()));
            Console.WriteLine("PMm : " + BitConverter.ToString(felica.PMm()));

            string text = "Hello World!";
            byte[] text_bytes = Encoding.UTF8.GetBytes(text);

            for (int i = 0; i * 16 < text_bytes.Length; i++)
            {
                break;
                byte[] data = new byte[16];
                for (int j = 0; j < 16; j++)
                {
                    if ((i + 1) * j < text_bytes.Length)
                    {
                        data[j] = text_bytes[(i + 1) * j];
                    }
                    else
                    {
                        data[j] = 0x00;
                    }
                }

                felica.WriteWithoutEncryption(0x0009, i, data);
            }

            for (int i = 0; i < 0x0e; i++)
            {
                break;
                //byte[] data = new byte[16];
                //felica.WriteWithoutEncryption(0x0009, i, data);
            }

            for (int i = 0; i < 0x0e; i++)
            {
                Console.WriteLine("READ {0:X2} : " + BitConverter.ToString(felica.ReadWithoutEncryption(0x0009, i)), i);
            }
        
            for (int i = 0; i < 0x0e; i++)
            {
                Console.WriteLine("READ {0:X2} : " + Encoding.UTF8.GetString(felica.ReadWithoutEncryption(0x0009, i)), i);
            }

            felica.Dispose();
        }
    }
}
