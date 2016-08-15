using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Trello2Redmine
{
    class PublicFun
    {
        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static int GetRandomNum(int minValue, int maxValue)
        {
            Random ra = new Random(GetRandomSeed());
            int tmp = ra.Next(minValue, maxValue);
            return tmp;
        }
        public static string GetCurrentExePath()
        {
            string str = System.Windows.Forms.Application.ExecutablePath;
            return str.Substring(0, str.LastIndexOf('\\'));

        }
        public static void WriteMyLog(string fileName, string str)
        {
            FileStream file = null;
            StreamWriter sw = null;
            try
            {
                file = new FileStream(fileName, FileMode.Append);
                sw = new StreamWriter(file);
                sw.WriteLine(str);
            }
            catch
            {
            }

            if (sw != null)
                sw.Close();

            if (file != null)
                file.Close();
        }
    }
}
