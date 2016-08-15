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
    class RedmineFun
    {
        #region para
        Project softwareServicePrj = null;
        char[] removeChar = new char[] { '&', '|', ':', ';', ',', '.', '\t', '\n', ' ', '(', ')',
            '+', '/', '*', '#', '\'', '[', ']', '【', '】', '：', '；', '，', '（', '）'};
        private Dictionary<string, int> CustomFieldMaps = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineTracker = new Dictionary<string, int>();
        private Dictionary<string, int> RedminePriority = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineStatus = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineVersion = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineJournalId = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineUser = new Dictionary<string, int>();
        private Dictionary<string, int> RedmineRole = new Dictionary<string, int>();
        private int MinRandom = int.MinValue;
        private int MaxRandom = int.MaxValue;
        private TrelloFun Trello_fun = new TrelloFun();
        private bool NeedUpdateProjectList = true;
        IList<Project> ProjectCache;
        #endregion       
        
        public bool CheckStringChineseReg(string str)
        {
            bool res = false;
            for (int i = 0; i < str.Length; i++)
            {
                if ((int)str[i] > 127)             
                   res = true;                
            }              
            return res;
        }

        public int IsTaskAlreadExist(string task_id, int current_project_id, bool is_card)
        {
            int issue_id = -1;
            string custom_name = string.Empty;
            if (is_card)
                custom_name = "Trello_Card_ID";
            else
                custom_name = "Trello_List_ID";
            //Get Custom fields  
            NameValueCollection mem = new NameValueCollection { { "project_id", current_project_id.ToString() } };
            int num = 0;
            foreach (Issue item in GetAllObjectList<Issue>(mem, out num))
            {
                if (item.CustomFields != null)
                {
                    foreach (IssueCustomField fielditem in item.CustomFields)
                    {
                        if (fielditem.Name == custom_name)
                        {
                            IList<CustomFieldValue> value = fielditem.Values;
                            if (value[0].Info == task_id)
                            {
                                issue_id = item.Id;
                                break;
                            }
                        }
                    }
                }
                if (issue_id != -1)
                    break;
            }
            return issue_id;
        }
        
        public int IsIssueNameExistInProject(int current_project_id, string list_task_name)
        {
            NameValueCollection mem = new NameValueCollection { { "project_id", current_project_id.ToString() } };
            int num = 0;
            foreach (var item in GetAllObjectList<Issue>(mem, out num))
            {
                if (item.Subject == list_task_name)
                {
                    return item.Id;
                }
            }
            return -1;
        }

        

        #region GetFun
        public int GetRedmineEmailName(string user_name, Dictionary<string, int> redmine_member)
        {
            foreach (var item in redmine_member)
            {
                if (item.Key.ToLower().Contains(user_name))
                    return item.Value;
            }
            return -1;
        }

        public bool GetParentProjectIdAndName(ref int id, ref string name)
        {
            IList<Project> parentProject = GetProjectsByName("Auto Project Sync with Trello");
            if (parentProject.Count != 0)
            {
                id = parentProject[0].Id;
                name = parentProject[0].Name;
                return true;
            }
            else
            {
                Console.WriteLine("Auto Project Sync with Trello not exist!");
                return false;
            }
        }

        public int GetRedmineStatus(string list_name, ref bool IsFinish)
        {            
            try
            {
                string temp = string.Empty;
                string str = Regex.Replace(list_name, @"\s", "").ToLower().
                    Replace("（", "(").Replace("）", ")");                
                string[] str_1 = str.Split(new char[] { '(' });
                if (str_1.Length == 2)
                {
                    string[] str_2 = str_1[1].Split(new char[] { ')' });
                    temp = str_2[0];
                }
                if (temp == "todo")
                {
                    if (Form1.ConfigPara.IniFilePara.ContainsKey("Todo"))
                        return RedmineStatus[Form1.ConfigPara.IniFilePara["Todo"]];
                }
                else if (temp == "doing")
                {
                    if (Form1.ConfigPara.IniFilePara.ContainsKey("Doing"))
                        return RedmineStatus[Form1.ConfigPara.IniFilePara["Doing"]];
                }
                else if (temp == "done")
                {
                    IsFinish = true;
                    if (Form1.ConfigPara.IniFilePara.ContainsKey("Done"))
                        return RedmineStatus[Form1.ConfigPara.IniFilePara["Done"]];
                }
                             
            }
            catch
            {
                return -1;
            }
            return -1;
        }

        public string GetRedminePriority(TrelloNet.Card card)
        {
            string str = string.Empty;
            List<TrelloNet.Label> label_list = card.Labels;
            foreach (TrelloNet.Label item in label_list)
            {
                str = Regex.Replace(item.Name, @"\s", "").ToLower();
                if (str.Substring(0, 2) == "p-")
                {
                    return str.Substring(2);
                }
            }
            return str;
        }
        public bool GetRedmineMap()
        {
            try
            {
                RedmineTracker.Clear();
                foreach (var item in GetAllObjectList<Tracker>())
                {
                    if(!RedmineTracker.ContainsKey(item.Name.ToLower()))
                        RedmineTracker.Add(item.Name.ToLower(), item.Id);
                }
                RedminePriority.Clear();
                foreach (var item in GetAllObjectList<IssuePriority>())
                {
                    if (!RedminePriority.ContainsKey(item.Name.ToLower()))
                        RedminePriority.Add(item.Name.ToLower(), item.Id);
                }
                RedmineStatus.Clear();
                foreach (var item in GetAllObjectList<IssueStatus>())
                {
                    if (!RedmineStatus.ContainsKey(item.Name.ToLower()))
                        RedmineStatus.Add(item.Name.ToLower(), item.Id);
                }
                RedmineUser.Clear();
                foreach (var item in GetAllObjectList<User>())
                {
                    if (!RedmineUser.ContainsKey(item.Email.ToLower()))
                        RedmineUser.Add(item.Email.ToLower(), item.Id);
                }
                RedmineRole.Clear();
                foreach (var item in GetAllObjectList<Role>())
                {
                    if (!RedmineRole.ContainsKey(item.Name.ToLower()))
                        RedmineRole.Add(item.Name.ToLower(), item.Id);
                }
                //Get Custom fields
                CustomFieldMaps.Clear();
                Issue firstIssue = GetFirstObject<Issue>();
                if (firstIssue == null || firstIssue.CustomFields == null)
                {
                    PublicFun.WriteMyLog(Form1.FilePath, "Can't find any issue for get Custom fields list");                    
                }
                else
                {
                    if (CustomFieldMaps.Count == 0)
                    {
                        foreach (IssueCustomField customFields in firstIssue.CustomFields)
                        {
                            if (!CustomFieldMaps.ContainsKey(customFields.Name.ToLower()))
                                CustomFieldMaps.Add(customFields.Name.ToLower(), customFields.Id);
                        }
                    }
                }                
            }
            catch (Exception e)
            {
                PublicFun.WriteMyLog(Form1.FilePath, "Get redmine map error:" + e.Message);
                return false;
            }
            return true;

        }
        public int GetParentRedmineProjectId()
        {
            //get parent project id and name
            int parent_project_id = -1;
            string parent_project_name = string.Empty;
            if (!GetParentProjectIdAndName(ref parent_project_id, ref parent_project_name))
                return -1;
            return parent_project_id;
        }

        public string GetRedmindeUserName(string card_name)
        {
            string redmine_assign_name = "None";
            string[] str = Regex.Replace(card_name, @"\s", "").ToLower().Split(new char[] { '.' });
            if (str.Length == 3)
                redmine_assign_name = str[0] + "." + str[1];
            return redmine_assign_name;
        }

        public Dictionary<string, int> GetRedmineProjectMember(int project_id)
        {
            //get redmine member list
            Dictionary<string, int> redmine_member = new Dictionary<string, int>();
            NameValueCollection mem = new NameValueCollection { { "project_id", project_id.ToString() } };
            int num = 0;
            foreach (var item in GetAllObjectList<ProjectMembership>(mem, out num))
            {
                redmine_member.Add(item.User.Name, item.User.Id);
            }
            return redmine_member;
        }

        public IList<Project> GetProjectsByName(string name, bool beUpper)
        {
            IList<Project> items = new List<Project>();

            //Use Cache to patch REST API search project doesn't have filter issue
            if (NeedUpdateProjectList == true)
            {
                ProjectCache = GetAllObjectList<Project>();
                NeedUpdateProjectList = false;
            }
            foreach (Project project in ProjectCache)
            {
                if (beUpper == true)
                {
                    if (Regex.Replace(project.Name, @"\s", "").ToUpper() ==
                        Regex.Replace(name, @"\s", "").ToUpper())
                        items.Add(project);
                }
                else
                {
                    if (Regex.Replace(project.Name, @"\s", "") ==
                        Regex.Replace(name, @"\s", ""))
                        items.Add(project);
                }
            }
            return items;
        }

        public IList<T> GetAllObjectList<T>() where T : class, new()
        {
            int totalCount;
            return GetAllObjectList<T>(out totalCount);
        }

        public IList<T> GetAllObjectList<T>(out int totalCount) where T : class, new()
        {
            NameValueCollection parameters = new NameValueCollection { { "status_id", "*" } };

            return GetAllObjectList<T>(parameters, out totalCount);
        }

        public IList<T> GetAllObjectList<T>(NameValueCollection newParameters, out int totalCount) where T : class, new()
        {
            NameValueCollection parameters = new NameValueCollection { { "limit", "100" } };
            parameters.Add(newParameters);
            List<T> objects;

            objects = Form1.manager.GetObjectList<T>(parameters, out totalCount).ToList();

            if (totalCount > 100)
            {
                int steps = (totalCount % 100 > 0 ? (totalCount / 100) + 1 : totalCount / 100);
                for (int step = 1; step < steps; step++)
                {
                    parameters = new NameValueCollection { { "limit", "100" }, { "offset", (step * 100).ToString() } };
                    parameters.Add(newParameters);
                    objects.AddRange(Form1.manager.GetObjectList<T>(parameters).ToList());
                }
            }

            return objects;
        }

        public IList<ProjectMembership> GetProjectMembershipsByName(string identifier)
        {
            softwareServicePrj = Form1.manager.GetObject<Project>(identifier, null);

            NameValueCollection parameters = new NameValueCollection { { "project_id", softwareServicePrj.Id.ToString() } };

            return Form1.manager.GetTotalObjectList<ProjectMembership>(parameters);
        }

        public T GetFirstObject<T>() where T : class, new()
        {
            NameValueCollection parameters = new NameValueCollection { { "status_id", "*" }, { "sort", "created_on" }, { "limit", "1" } };
            List<T> objects;

            objects = Form1.manager.GetObjectList<T>(parameters).ToList();
            if (objects.Count <= 0)
            {
                return null;
            }

            return objects[0];
        }

        public IList<Project> GetProjectsByName(string name)
        {
            return GetProjectsByName(name, false);
        }

        public int GetTrackerIDByName(string name, bool beUpper)
        {
            int id = -1;
            IList<Tracker> items = GetAllObjectList<Tracker>();

            foreach (Tracker tracker in items)
            {
                if (beUpper == true)
                {
                    if (tracker.Name.ToUpper() == name.ToUpper())
                    {
                        id = tracker.Id;
                        break;
                    }
                }
                else
                {
                    if (tracker.Name == name)
                    {
                        id = tracker.Id;
                        break;
                    }
                }
            }
            return id;
        }

        #endregion

        #region CreateFun
        public int CreateListTask(TrelloNet.List list, int current_project_id)
        {
            //get list name
            string list_task_name = string.Empty;
            string str = Regex.Replace(list.Name, @"\s", "").Replace("（", "(").Replace("）", ")");
            if(str.Contains("("))
            {
                string[] str_temp = str.Split(new char[]{'('});
                list_task_name = str_temp[0];
            }
            else
            {
                list_task_name = str;
            }           
            //list task is exsit
            int list_task_id = IsIssueNameExistInProject(current_project_id, list_task_name);
            if (list_task_id == -1)
            {
                Issue issue = new Issue();
                issue.Tracker = new IdentifiableName() { Id = RedmineTracker["task"] };
                issue.Subject = list_task_name;
                issue.Status = new IdentifiableName() { Id = RedmineStatus["new"] };
                issue.Priority = new IdentifiableName() { Id = RedminePriority["normal"] };
                issue.Project = new IdentifiableName() { Id = current_project_id };
                Form1.manager.CreateObject(issue);
                list_task_id = IsIssueNameExistInProject(current_project_id, list_task_name);
            }
            return list_task_id;
        }

        public IssueCustomField CreateCustomField(int Id, string Value)
        {
            IssueCustomField ret = new IssueCustomField();
            CustomFieldValue tmpValue = new CustomFieldValue();

            ret.Id = Id;
            tmpValue.Info = Value;
            ret.Values = new List<CustomFieldValue>() { tmpValue };

            return ret;
        }

        public ProjectTracker CreateProjectTracker(int Id, string Value)
        {
            ProjectTracker ret = new ProjectTracker();
            ret.Id = Id;
            ret.Name = Value;
            return ret;
        }

        public string CreateCardTask(TrelloNet.List list, TrelloNet.Card card, Dictionary<string, int> redmine_member,
           int current_project_id, int parent_task_id)
        {
            try
            {
                string shortlink = card.ShortLink;
                string shroturl = card.ShortUrl;
                Issue issue = new Issue();
                //Get Trello First Member
                IEnumerable<Member> AllMember = Form1.trello.Members.ForCard(card);
                if (AllMember.Count() != 0)
                {
                    string cardmember_name = AllMember.First<Member>().FullName;
                    string user_name = GetRedmindeUserName(cardmember_name);
                    //get redmine AssignMember
                    int userid = GetRedmineEmailName(user_name, redmine_member);
                    if (userid != -1)
                        issue.AssignedTo = new IdentifiableName() { Id = userid };
                    else
                    {
                        int id = AddMemberShip(user_name, current_project_id);
                        if (id != -1)
                            issue.AssignedTo = new IdentifiableName() { Id = id };
                    }
                }
                issue.Tracker = new IdentifiableName() { Id = RedmineTracker["task"] };
                issue.Project = new IdentifiableName() { Id = current_project_id };
                issue.Subject = card.Name;
                issue.ParentIssue = new IdentifiableName() { Id = parent_task_id };
                //get redmine status
                bool IsFinish = false;
                int status = GetRedmineStatus(list.Name, ref IsFinish);
                if (status != -1)             
                    issue.Status = new IdentifiableName() { Id = status };                                 
                else
                    issue.Status = new IdentifiableName() { Id = RedmineStatus["new"] };
                //get DoneRatio
                if (IsFinish)
                    issue.DoneRatio = 100.0f;
                //Prioritys
                string str_priority = GetRedminePriority(card);
                if (!RedminePriority.ContainsKey(str_priority))
                    str_priority = "normal";
                issue.Priority = new IdentifiableName() { Id = RedminePriority[str_priority] };
                //Due date           
                issue.DueDate = card.Due;
                if (DateTime.Now > card.Due)
                    issue.StartDate = card.Due;
                //Trello card id
                issue.CustomFields = new List<IssueCustomField>() { CreateCustomField(CustomFieldMaps["trello_card_id"], card.Id) };
                //Comment  
                issue.Description = card.Desc + "<br><br>";
                issue.Description += Trello_fun.GetCardComment(card);
                issue.Description += Trello_fun.GetCheckList(card);
                //Task is Already exist                 
                int issue_id = IsTaskAlreadExist(card.Id, current_project_id, true);
                //Attachment  
                //issue.Uploads = Trello_fun.GetCardAttachment(card, issue_id);
                IList<Upload> attach = new List<Upload>();
                Thread AttachThread = new Thread(() => GetAttach(card, issue_id, ref attach));
                AttachThread.IsBackground = true;
                AttachThread.Start();                
                if (!AttachThread.Join(1000*60*5))
                {
                    AttachThread.Abort();
                    PublicFun.WriteMyLog(Form1.FilePath,
                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Abort attachments thread");
                }
                issue.Uploads = attach;
                if (issue_id != -1)
                    Form1.manager.UpdateObject(issue_id.ToString(), issue);
                else
                    Form1.manager.CreateObject(issue);
                return "Pass";
            }
            catch (Exception e)
            {          
                return "Fail,ErrorMessage:" + e.Message;
            }
        }
        public void GetAttach(Card card, int issue_id, ref IList<Upload> attachments)
        {
            attachments = Trello_fun.GetCardAttachment(card, issue_id);
        }

        public int Create_Projects(string projectName, int parentId)
        {
            //Create new project
            Project project = new Project();
            project.Name = projectName;
            //Length between 1 and 100 characters. Only lower case letters (a-z), numbers, dashes and underscores are allowed, must start with a lower case letter.Once saved, the identifier cannot be changed.
            string projectIdentifier = projectName.ToLower();
            //Replace all space & specific symbol to _
            string[] sArr = projectIdentifier.Split(removeChar, StringSplitOptions.RemoveEmptyEntries);
            projectIdentifier = String.Join("_", sArr);
            project.Identifier = projectIdentifier;
            //project名字有中文Identifier随机生成一个数字，为确保数字不是重复的，添加此逻辑进行判断
            bool HaveChinese = CheckStringChineseReg(projectIdentifier);            
            while (true)
            {
                try
                {
                    if(HaveChinese)
                        project.Identifier = PublicFun.GetRandomNum(MinRandom, MaxRandom).ToString();
                    Form1.manager.CreateObject(project);
                    break;
                }
                catch
                {
                    continue;
                }
            }
            
            //Update new project Parent
            NeedUpdateProjectList = true;
            project = GetProjectsByName(projectName, true)[0];
            project.Parent = new IdentifiableName() { Id = parentId };
            Form1.manager.UpdateObject<Project>(project.Id.ToString(), project);

            return project.Id;
        }
        public int AddMemberShip(string user_name, int project_id)
        {
            string email = string.Empty;
            ProjectMembership pm = new ProjectMembership();
            foreach (var item in RedmineUser)
            {
                string[] str = item.Key.Split(new char[] { '@' });
                if (str[0] == user_name)
                {
                    email = item.Key;
                    pm.User = new IdentifiableName { Id = item.Value };
                    pm.Roles = new List<MembershipRole>();
                    pm.Roles.Add(new MembershipRole { Id = RedmineRole["qa"] });
                    Form1.manager.CreateObject<ProjectMembership>(pm, project_id.ToString());
                    break;
                }
            }
            Dictionary<string, int> member = GetRedmineProjectMember(project_id);
            foreach (var item in member)
            {
                if (item.Key == email)
                    return item.Value;
            }
            return -1;
        }
        #endregion
    }
}
