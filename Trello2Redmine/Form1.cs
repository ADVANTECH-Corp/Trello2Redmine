using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
using System.IO;
using Microsoft.Internal.Performance;
using Trello2Redmine.Properties;
using System.Threading;
using System.Text.RegularExpressions;
using TrelloNet;
using System.Net;
using Dongzr.MidiLite;

namespace Trello2Redmine
{    
    
    public partial class Form1 : Form
    {
        #region public para
        public static RedmineManager manager;
        public static ITrello trello;
        public static ConfigInfo ConfigPara = new ConfigInfo();  
      
        #endregion

        #region private para
        private string host = "http://172.20.0.60/redmines/AXDC-Common";
        private Thread Create_or_Update;
        private Thread AutoRun;
        private Board Selected_borad;
        private Object thisLock = new object();       
        private RedmineFun Redmine = new RedmineFun();
        private TrelloFun Trello = new TrelloFun();
        public static string FilePath;
        static int BoardListNum = 1000;      
        #endregion        

        public delegate void DispMSGDelegate(string MSG);
        public void DispMsg(string strMsg)
        {
            //如果调用该函数的线程和控件lstMain位于同一个线程内
            if (this.process_lable.InvokeRequired == false)
            {
                //直接将内容添加到窗体的控件上
                this.process_lable.Text = strMsg;               
            }
            //如果调用该函数的线程和控件lstMain不在同一个线程
            else                                                        
            {
                //通过使用Invoke的方法，让子线程告诉窗体线程来完成相应的控件操作
                DispMSGDelegate DMSGD = new DispMSGDelegate(DispMsg);
                //使用控件lstMain的Invoke方法执行DMSGD代理(其类型是DispMSGDelegate)
                this.process_lable.Invoke(DMSGD, strMsg);
            }
        }        

        public Form1(string[] args)
        {          
            InitializeComponent();
            FilePath = PublicFun.GetCurrentExePath() + "\\log_" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            ConfigPara.IniFilePara = new Dictionary<string, string>();
            ConfigPara.BoardList = new Dictionary<string, string>();
            if (args.Length > 0 && args[0] == "Auto")
            {
                if (AutoRun != null)
                    AutoRun.Abort();
                AutoRun = new Thread(new ThreadStart(ParaFunEntry));                
                AutoRun.IsBackground = true;
            }          
        }       
        private void trello_board_combobox_DropDown(object sender, EventArgs e)
        {
            trello_board_combobox.Items.Clear();
            if (trello_org_combobox.SelectedItem == null)
                return;           
            
            IEnumerable<Board> AllBoard = trello.Boards.ForOrganization((Organization)trello_org_combobox.SelectedItem, BoardFilter.Open);
            foreach (Board Board in AllBoard)
            {
                trello_board_combobox.Items.Add(Board);
            }
        }
 
        private void create_or_update_btn_Click(object sender, EventArgs e)
        {
            //Timers_Timer.Stop();
            if (!Init())
                return;           
            if (trello_board_combobox.Text == "" || trello_org_combobox.Text == "")
            {                
                DispMsg(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + 
                    "Trello org or board is not selected!");                
                return;
            }           
            create_or_update_btn.Enabled = false;
            if (Create_or_Update != null)
                Create_or_Update.Abort();
            Selected_borad = (Board)trello_board_combobox.SelectedItem;
            CreateOrUpdataThread();
        }
        private bool Init()
        {                        
            if (ConfigPara.IniFilePara.Count != 0 || ConfigPara.BoardList.Count != 0)
            {
                ConfigPara.IniFilePara.Clear();
                ConfigPara.BoardList.Clear();
            }
            //Read ini file            
            if (!ReadIniFile())
            {
                PublicFun.WriteMyLog(FilePath, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") +
                    "Read ini file error!");               
                return false;
            }                
            //Connet to redmine and trello
            if (!Connet())
                return false;
            //Get Redmine info
            if (!Redmine.GetRedmineMap())
                return false;
            return true;
        }
        private void ParaFunEntry()
        {
            if (!Init())
                return;           
            foreach (var item in ConfigPara.BoardList)
            {
                string[] str_board = item.Value.Split(new char[] { ',' });
                IEnumerable<Organization> AllOrg = trello.Organizations.ForMe(OrganizationFilter.All);
                foreach (Organization org in AllOrg)
                {
                    if (Regex.Replace(item.Key, @"\s", "").ToLower() !=
                        Regex.Replace(org.DisplayName, @"\s", "").ToLower())
                        continue;
                    foreach (string itemboard in str_board)
                    {
                        IEnumerable<Board> AllBoard = trello.Boards.ForOrganization(org);
                        foreach (Board board in AllBoard)
                        {                           
                            if (Regex.Replace(itemboard, @"\s", "").ToLower() != 
                                Regex.Replace(board.Name, @"\s", "").ToLower() || board.Closed)
                                continue;                           
                            CreateOrUpdatePro(board);
                        }
                    }
                }
            }
            System.Environment.Exit(0);
        }
        private void CreateOrUpdatePro(Board board)
        {
            lock (thisLock)
            {
                int parent_project_id = Redmine.GetParentRedmineProjectId();
                if (parent_project_id == -1)
                    return;
                //create project
                int current_project_id;
                IList<Project> board_project = Redmine.GetProjectsByName(board.Name, true);
                if (board_project.Count == 0)
                    current_project_id = Redmine.Create_Projects(board.Name, parent_project_id);
                else
                    current_project_id = board_project[0].Id;

                List<TrelloNet.List> allList = Trello.GetTrelloList(board);
                foreach (TrelloNet.List list in allList)
                {
                    string listname = Regex.Replace(list.Name, @"\s", "").ToLower();
                    if (listname.length > 2)
                    {
                        if (listname.Substring(0, 3) == "!r-" || listname.Substring(0, 3) == "！r-")
                            continue;                        
                    }
                    
                    int list_task_id = Redmine.CreateListTask(list, current_project_id);
                    if (list_task_id == -1)
                        continue;
                    List<Card> allCard = Trello.GetTrelloCard(list);
                    foreach (Card card in allCard)
                    {                       
                        //get redmine member list
                        Dictionary<string, int> redmine_member = Redmine.GetRedmineProjectMember(current_project_id);
                        string result = Redmine.CreateCardTask(list, card, redmine_member, current_project_id, list_task_id);
                        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + 
                            board.Name + ">" + list.Name + ">" + card.Name + "--" + result;
                        DispMsg(str);
                        PublicFun.WriteMyLog(FilePath, str);
                    }
                }
            }
        }
        private void CreateOrUpdateEntry()
        {
            CreateOrUpdatePro(Selected_borad);            
            create_or_update_btn.Enabled = true;
            DispMsg("");
        }
        private void CreateOrUpdataThread()
        {
            Create_or_Update = new Thread(new ThreadStart(CreateOrUpdateEntry));
            Create_or_Update.Start();
            Create_or_Update.IsBackground = true;
        }

        private bool Connet()
        {
            //connet to redmine           
            try
            {
                manager = new RedmineManager(host, ConfigPara.RemineKey);//apiKey   
                Redmine.GetProjectMembershipsByName("auto-project-sy-n-c-with-trello");                
            }
            catch (Exception ex)
            {
                PublicFun.WriteMyLog(FilePath, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") +
                    " Connect to Redmine failed, error:" + ex.Message);               
                return false;
            }
            //connect to trello
            try
            {
                trello = new Trello(ConfigPara.TrelloKey.Trim());
                trello.Authorize(ConfigPara.TrelloToken.Trim());                
            }
            catch (Exception ex)
            {
                PublicFun.WriteMyLog(FilePath, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") +
                    " Connect to Trello failed, error:" + ex.Message);               
                return false;
            }
            return true;
        }

        private bool ReadIniFile()
        {
            try
            {
                String path1 = System.IO.Directory.GetCurrentDirectory();
                IniFile ini = new IniFile(path1 + "\\Trello2Redmine.ini");
                string str = ini.IniReadValue("Status", "Todo");
                ConfigPara.IniFilePara.Add("Todo", str.ToLower());
                str = ini.IniReadValue("Status", "Doing");
                ConfigPara.IniFilePara.Add("Doing", str.ToLower());
                str = ini.IniReadValue("Status", "Done");
                ConfigPara.IniFilePara.Add("Done", str.ToLower());
                //ConfigPara.ImportTime = ini.IniReadValue("Time", "ImportTime");
                ConfigPara.RemineKey = ini.IniReadValue("Key", "RedmineKey");
                ConfigPara.TrelloKey = ini.IniReadValue("Key", "TrelloKey");
                ConfigPara.TrelloToken = ini.IniReadValue("Key", "TrelloToken");                
                for (int i = 1; i < BoardListNum; i++)
                {
                    str = ini.IniReadValue("Board", "BoardList" + i.ToString());
                    if (str == "")
                        break;
                    string[] str_split = str.Split(new char[] { ':' });
                    ConfigPara.BoardList.Add(str_split[0], str_split[1]);
                }
            }
            catch
            {
                return false;
            }
            return true;

        }

        private void trello_org_combobox_DropDown(object sender, EventArgs e)
        {            
            //get trello org list
            trello_org_combobox.Items.Clear();
            IEnumerable<Organization> AllOrg = trello.Organizations.ForMe(OrganizationFilter.All);
            foreach (Organization org in AllOrg)
            {
                trello_org_combobox.Items.Add(org);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
            if (AutoRun != null)
                AutoRun.Start();             
            //timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (ArgStr == "Auto")
            //{
            //    if (AutoRun != null)
            //        AutoRun.Abort();
            //    AutoRun = new Thread(new ThreadStart(ParaFunEntry));
            //    AutoRun.Start();
            //    AutoRun.IsBackground = true;
            //    ArgStr = "";
            //}

        }       
    }
}
