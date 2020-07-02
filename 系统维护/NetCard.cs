using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace 系统维护
{
    public class NetCard
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        private string mask;

        public string Mask
        {
            get { return mask; }
            set { mask = value; }
        }
        private string gateway;

        public string Gateway
        {
            get { return gateway; }
            set { gateway = value; }
        }
        private string dns1;

        public string Dns1
        {
            get { return dns1; }
            set { dns1 = value; }
        }
        private string dns2;

        public string Dns2
        {
            get { return dns2; }
            set { dns2 = value; }
        }
        private NetworkInterface adapter;

        public NetworkInterface Adapter
        {
            get { return adapter; }
            set { adapter = value; }
        }
        public NetCard(string name, string ip, string mask, string gateway, string dns1, string dns2, NetworkInterface adapter)
        {
            this.name = name;
            this.ip = ip;
            this.mask = mask;
            this.gateway = gateway;
            this.dns1 = dns1;
            this.dns2 = dns2;
            this.adapter = adapter;

        }
        /// <summary>
        /// 判断是否为物理断开
        /// </summary>
        /// <returns></returns>
        public bool isDrop()
        {
            return this.adapter.OperationalStatus.ToString() == "down" ? true : false;
        }
        /// <summary>
        /// 判断ip地址是否可用
        /// </summary>
        /// <returns></returns>
        public bool isEnableIp()
        {
            if (this.ip.IndexOf("10.") == 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断dns是否可用
        /// </summary>
        /// <returns></returns>
        public bool isEnableDns(string defaultDns1,string defaultDns2)
        {
            if (this.dns1 != defaultDns1 && this.dns2!=defaultDns2)
            {
                return false;
            }
            else 
            {
                return true;
            }
           
        }
        /// <summary>
        /// 判断子网掩码是否可用
        /// </summary>
        /// <returns></returns>
        public bool isEnableMask()
        {
            if (this.Mask != "255.255.255.0" || this.Mask != "255.0.0.0")
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        /// <summary>
        /// 判断网关是否可用
        /// </summary>
        /// <returns></returns>
        public bool isEnableGateway()
        {
            if (this.gateway != "")
            {
                string[] ipStrs = this.ip.Split('.');
                string[] gatewayStrs = this.gateway.Split('.');
                if (gatewayStrs.Length == 4)
                {
                    if (gatewayStrs[3] == "1")
                    {
                        if (gatewayStrs[0] == ipStrs[0] && gatewayStrs[1] == ipStrs[1] && gatewayStrs[2] == ipStrs[2])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 修改ip地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <param name="gateway"></param>
        public void setIp(string ip, string mask, string gateway)
        {
            string setIp = "netsh interface ip set address name=" + this.name + " source=static addr=" + ip + " mask=" + mask + " gateway=" + gateway + " gwmetric=1";
            string command = "echo 正在修改IP配置..&&" + setIp;
            string reStr = "";
            RunCmd.run(command, out reStr);
        }


        /// <summary>
        /// 修改dns
        /// </summary>
        /// <param name="dns1">主要dns</param>
        /// <param name="dns2">备用dns</param>
        public void setDns(string dns1, string dns2)
        {
            string setDns1 = "netsh interface ip set dns name=\"" + this.Name  + "\" source=static addr=" + dns1 + " register=primary";
            string setDns2 = "netsh interface ip add dns name=\"" + this.Name + "\" " + dns2 + " index=2";
            string command2 = "echo 正在修改DNS.." + " && " + setDns1 + " && " + setDns2;

            string reStr = "";
            RunCmd.run(command2, out reStr);
        }



        /// <summary>
        /// 判断网络是否通畅
        /// </summary>
        /// <param name="pingIpDic">要ping的ip字典</param>
        /// <returns></returns>
        public Dictionary<string, string> connectNetTest(Dictionary<string, string> pingIpDic)
        {
            Dictionary<string, string> connectStatusDic = new Dictionary<string, string>();

            connectStatusDic.Add("网关" ,Tools.pingC(this.gateway));
            foreach (KeyValuePair<string, string> kv in pingIpDic)
            {
                connectStatusDic.Add(kv.Key, Tools.pingC(kv.Value));
            }
            return connectStatusDic;
        }
    }
}
