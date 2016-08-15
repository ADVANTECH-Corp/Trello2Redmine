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
    class TrelloFun
    {
        public string GetCheckList(TrelloNet.Card card)
        {
            bool flag = false;
            string action_info = string.Empty;
            action_info = "CheckList:";
            action_info += "<table border=\"1\">";
            foreach (var item in Form1.trello.Checklists.ForCard(card))
            {                
                foreach (TrelloNet.CheckItem checkItem in item.CheckItems)
                {
                    action_info += GetHtmlString(item.Name + "," + checkItem.Name);
                    flag = true;
                }              
            }
            action_info += "</table>";
            if (flag)
                return action_info;
            else
                return "";
        }
        public string GetCardComment(TrelloNet.Card card)
        {
            bool flag = false;
            List<ActionType> filter = new List<ActionType>() { ActionType.CommentCard };
            IEnumerable<TrelloNet.Action> actions = Form1.trello.Actions.ForCard(card, filter);
            string action_info = string.Empty;            
            action_info += "CardCommnet:";
            action_info += "<table border=\"1\">";
            foreach (TrelloNet.Action itemaction in actions)
            {
                string str = GetCommentTypeStr(itemaction);
                if (str != "")
                {
                    action_info += GetHtmlString(str);
                    flag = true;
                }                    
            }
            action_info += "</table>";
            if (flag)
                return action_info;
            else
                return "";            
        }
        private string GetHtmlString(string content)
        {
            string result = string.Empty;
            result = "<tr>";
            string[] str = content.Split(new char[] { ',' });
            for (int i = 0; i < str.Length; i++)
            {
                result += "<td>" + str[i] + "</td>";
            }
            result += "</tr>";
            return result;
        }
        public string GetCommentTypeStr(TrelloNet.Action action)
        {
            string str = string.Empty;
            switch (action.GetType().Name)
            {
                case "CommentCardAction":
                    TrelloNet.CommentCardAction comment = (TrelloNet.CommentCardAction)action;
                    str = comment.Date + "," + comment.MemberCreator.FullName + "," + comment.Data.Text;
                    break;                
            }
            return str;
        }
        public IList<Upload> GetCardAttachment(TrelloNet.Card card, int issue_id)
        {
            IList<Upload> attachments = new List<Upload>();
            try
            {
                //Is the attachement already exist with the same name
                //Dictionary<string, string> card_attachement = new Dictionary<string, string>();
                //foreach (Card.Attachment itematch in card.Attachments)
                //    card_attachement.Add(itematch.Name, itematch.Url);
                //if (issue_id != -1)
                //{
                //    Issue issue_attachment = Form1.manager.GetObject<Issue>(issue_id.ToString(),
                //        new NameValueCollection { { "include", "attachments" } });
                //    foreach (Attachment itemissue in issue_attachment.Attachments)
                //    {
                //        if (card_attachement.ContainsKey(itemissue.FileName))
                //            card_attachement.Remove(itemissue.FileName);
                //    }
                //}
                foreach (Card.Attachment item_attachment in card.Attachments)
                {                    
                    if(item_attachment.Date > DateTime.Now.AddDays(-1))
                    {
                        Upload attachment = Form1.manager.UploadFile(GetUrlFileContent(item_attachment.Url));
                        attachment.FileName = item_attachment.Name;
                        attachment.ContentType = "text/plain";                       
                        attachments.Add(attachment);
                    }                   
                }
                return attachments;
            }
            catch
            {
                return attachments;
            }    
        }

        public byte[] GetUrlFileContent(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                WebRequest req = WebRequest.Create(uri);
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(sr.ReadToEnd());                
                sr.Close();
                sr.Dispose();
                stream.Close();
                resp.Close();
                return byteArray;
            }
            catch
            {
                return System.Text.Encoding.Default.GetBytes("");
            }
            
        }

        public List<Organization> GetTrelloOrg()
        {
            List<Organization> org_list = new List<Organization>();
            IEnumerable<Organization> AllOrg = Form1.trello.Organizations.ForMe(OrganizationFilter.All);
            foreach (Organization org in AllOrg)
            {
                org_list.Add(org);
            }
            return org_list;
        }

        public List<Board> GetTrelloBoard(Organization org)
        {
            List<Board> board_list = new List<Board>();
            IEnumerable<Board> AllBoard = Form1.trello.Boards.ForOrganization(org, BoardFilter.Open);
            foreach (Board Board in AllBoard)
            {
                board_list.Add(Board);
            }
            return board_list;
        }

        public List<TrelloNet.List> GetTrelloList(Board board)
        {
            List<TrelloNet.List> list_list = new List<TrelloNet.List>();
            IEnumerable<TrelloNet.List> AllList = Form1.trello.Lists.ForBoard(board, ListFilter.Open);
            foreach (TrelloNet.List List in AllList)
            {
                list_list.Add(List);
            }
            return list_list;
        }

        public List<Card> GetTrelloCard(TrelloNet.List list)
        {
            List<Card> card_list = new List<Card>();
            IEnumerable<Card> AllCard = Form1.trello.Cards.ForList(list, CardFilter.All);
            foreach (Card Card in AllCard)
            {
                card_list.Add(Card);
            }
            return card_list;
        }

        public List<Member> GetTrelloMember(Board board)
        {
            List<Member> member_list = new List<Member>();
            IEnumerable<Member> AllMember = Form1.trello.Members.ForBoard(board, MemberFilter.All);
            foreach (Member Member in AllMember)
            {
                member_list.Add(Member);
            }
            return member_list;
        }

        public List<TrelloNet.Label> GetTrelloLable(Board board)
        {
            List<TrelloNet.Label> label_list = new List<TrelloNet.Label>();
            IEnumerable<TrelloNet.Label> AllLabel = Form1.trello.Labels.ForBoard(board);
            foreach (TrelloNet.Label Label in AllLabel)
            {
                label_list.Add(Label);
            }
            return label_list;
        }       
    }
}
