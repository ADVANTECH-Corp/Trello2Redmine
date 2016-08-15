using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trello2Redmine
{
    public struct ConfigInfo
    {
        public Dictionary<string, string> IniFilePara;
        public Dictionary<string, string> BoardList;
        public string ImportTime;
        public string RemineKey;
        public string TrelloKey;
        public string TrelloToken;        
    };
}
