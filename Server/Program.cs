using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FelicaLib;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                System.Net.HttpListener server = new System.Net.HttpListener();
                // Address, Port を指定
                server.Prefixes.Add("http://localhost:8000/");
                // サーバ起動
                server.Start();

                while (true)
                {
                    // 接続待ち
                    System.Net.HttpListenerContext context = server.GetContext();

                    // 応答作成
                    System.Net.HttpListenerResponse response = context.Response;
                    // JavaScriptエラー回避
                    response.Headers.Add("Access-Control-Allow-Origin", "*");

                    bool readSuccess = false;
                    byte[] idm = null;

                    if (System.Text.RegularExpressions.Regex.IsMatch(context.Request.Url.ToString(),"http://localhost:8000/getIDmDummy\\?seed=.*"))
                    {
                        // 疑似IDm生成
                        idm = IDmDummy(context.Request.QueryString.Get("seed"));
                        readSuccess = true;

                        Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]\t疑似IDm生成");
                    }
                    else
                    {
                        try
                        {
                            // NFC読み込み
                            Felica felica = new Felica();
                            felica.Polling((int)SystemCode.Common);
                            idm = felica.IDm();
                            felica.Dispose();

                            readSuccess = true;

                            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]\tカード読み取り成功");
                        }
                        catch
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]\tカード読み取り失敗");
                        }
                    }

                    if (readSuccess)
                    {
                        // 読み取り成功
                        string idm_str = BitConverter.ToString(idm).Replace("-", "").ToLower();
                        idm = Encoding.UTF8.GetBytes(idm_str);

                        response.StatusCode = 200;
                        response.OutputStream.Write(idm, 0, idm.Length);
                        response.Close();
                    }
                    else
                    {
                        // 読み取り失敗
                        response.StatusCode = 500;
                        response.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 引数に指定した文字列から擬似IDmを生成します。
        /// アルゴリズム:引数のMD5の前から8バイト(16進数16桁)
        /// </summary>
        /// <param name="seed">種となる文字列</param>
        /// <returns>疑似IDm</returns>
        private static byte[] IDmDummy(string seed)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));

            return Enumerable.Range(0, 8).Select(x => hash[x]).ToArray();
        }
    }
}