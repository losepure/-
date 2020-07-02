using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 系统维护
{
    public class NetworkDoctorBackObj
    {
        private List<string> msgList = new List<string>();
        private List<string> sugestList = new List<string>();

        //检查名称
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        //状态值
        private bool status;
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        //错误消息
        private string[] msg;
        public string[] Msg
        {
            get { return msgList.ToArray(); }
            set { msg = value; }
        }

        //错误建议
        private string[] suggest;
        public string[] Suggest
        {
            get { return suggest.ToArray(); }
            set { suggest = value; }
        }


        public NetworkDoctorBackObj()
        {

        }
        public NetworkDoctorBackObj(string name)
        {
            this.name = name;
            this.msg = new string[0];
            this.suggest = new string[0];
        }
        public void addMsg(string str)
        {
            this.msgList.Add(str);
        }
        public void addSugest(string str)
        {
            this.sugestList.Add(str);
        }

        public string getSugest()
        {
            string str = "";
            foreach (string s in sugestList)
            {
                str += s;
            }
            return str;
        }
        public string getMsg()
        {
            string str = "";
            foreach (string s in msgList)
            {
                str += s;
            }
            return str;
        }

    }
}
