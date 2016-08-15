using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Trello2Redmine
{
    public class IniFile
    {
        public string path;             //INI文件名  

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                    string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                    StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string section, string key, int dafault, string filePath);
                
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(5000);
            int i = GetPrivateProfileString(Section, Key, "", temp, 5000, this.path);
            string str = temp.ToString();
            int endPos = str.LastIndexOf(";");
            if (-1 != endPos)
            {
                str = str.Substring(0, endPos);
            } 
            return str.Trim();
        }

        public int IniReadIntValue(string Section, string Key)
        {
            int i = GetPrivateProfileInt(Section, Key, -1, this.path);     
            return i;
        } 


    }
}  
