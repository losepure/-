using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 系统维护
{
    public class PanelManager
    {
        NetCards netCards;
        Dictionary<string, NetCard> netCardDic;
        Dictionary<string, TextBox[]> tbsDic;
        ComboBox comboBox;
        bool isEnable;
        delegate void cmdM();
        //当前选中的网卡名称
        private string selectedNetCardName;

        public string SelectedNetCardName
        {
            get
            {
                return selectedNetCardName;
            }
            set
            {
                selectedNetCardName = value;
                NetCard netcard = getSelectedNetCard();
                if (netcard != null)
                {
                    writeIpPanel(getSelectedNetCard());
                }
            }
        }

        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; }
        }
        /// <summary>
        /// 无控件构造方法
        /// </summary>
        public PanelManager()
        {
            //创建获取网卡对象
            this.netCards = new NetCards();
            //获取所有网卡信息
            this.netCardDic = this.netCards.getAllNetCard();
        }
        /// <summary>
        /// 继承无控件构造的有控件构造方法
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="tbsDic"></param>
        public PanelManager(ComboBox comboBox, Dictionary<string, TextBox[]> tbsDic)
            : this()
        {
            //渲染控件
            this.tbsDic = tbsDic;
            this.comboBox = comboBox;
            //初始化面板
            initPanel();
        }



        private void initPanel()
        {
            //遍历网卡填入下拉列表，同时判断是否有设置合适的网卡
            foreach (KeyValuePair<string, NetCard> kv in this.netCardDic)
            {
                this.comboBox.Items.Add(kv.Key);
                NetCard netcard = kv.Value;
                //if (this.isEnable) continue;
                if (!netcard.isDrop() && netcard.isEnableIp())
                {
                    this.isEnable = true;
                    this.selectedNetCardName = netcard.Name;
                    writeIpPanel();

                }
            }
            //如果没找到符合规定的网卡，那么就选择默认第一个
            if (!this.isEnable && comboBox.Items.Count>0)
            {
                this.selectedNetCardName = comboBox.Items[0].ToString();
                writeIpPanel();
            }
           


        }


        /// <summary>
        /// 获得第一个可用的网卡
        /// </summary>
        /// <returns></returns>
        public NetCard getEnableCard()
        {
            this.netCardDic = this.netCards.getAllNetCard();
            //遍历网卡写入
            foreach (KeyValuePair<string, NetCard> kv in this.netCardDic)
            {
                NetCard netcard = kv.Value;
                if (!netcard.isDrop() && netcard.isEnableIp())
                {
                    return netcard;
                }
            }
            return null;
        }


        public NetCard getSelectedNetCard()
        {
            if (this.selectedNetCardName != "" && this.selectedNetCardName != null && this.netCardDic.ContainsKey(this.selectedNetCardName))
            {
                return this.netCardDic[this.selectedNetCardName];
            }
            else
            {
                return null;
            }
        }


        private void fillPanel(TextBox[] tb, string[] s)
        {
            for (int i = 0; i < tb.Length; i++)
            {
                tb[i].Text = s[i];
            }
        }

        public void writeIpPanel()
        {
            NetCard netcard = this.getSelectedNetCard();
            writeIpPanel(netcard);
        }


        public void writeIpPanel(NetCard netcard)
        {
            if (netcard.Name != "")
            {
                comboBox.Text = netcard.Name;
            }
            if (netcard.Ip != "")
            {

                string[] ipStrs = netcard.Ip.Split('.');
                fillPanel(this.tbsDic["ip"], ipStrs);
            }
            if (netcard.Mask != "")
            {
                string[] maskStrs = netcard.Mask.Split('.');
                fillPanel(this.tbsDic["mask"], maskStrs);
            }
            if (netcard.Gateway != "")
            {
                string[] gatewayStrs = netcard.Gateway.Split('.');
                fillPanel(this.tbsDic["gateway"], gatewayStrs);
            }

            if (netcard.Dns1 != "" && !netcard.Dns1.Contains(":"))
            {

                string[] dns1Strs = netcard.Dns1.Split('.');
                fillPanel(this.tbsDic["dns1"], dns1Strs);
            }
            if (netcard.Dns2 != "" && !netcard.Dns2.Contains(":"))
            {
                string[] dns2Strs = netcard.Dns2.Split('.');
                fillPanel(this.tbsDic["dns2"], dns2Strs);
            }
            string iNodeName = Tools.getiNodeUsername();
            if (iNodeName != "")
            {
                this.tbsDic["iNode"][0].Text = iNodeName;
            }
        }
        //读取面板网卡信息
        public NetCard readIpPanel()
        {
            ComboBox comboBox = this.comboBox;
            TextBox[] ip = this.tbsDic["ip"];
            TextBox[] mask = this.tbsDic["mask"];
            TextBox[] gateway = this.tbsDic["gateway"];
            TextBox[] dns1 = this.tbsDic["dns1"];
            TextBox[] dns2 = this.tbsDic["dns2"];

            string ipStr = ip[0].Text + "." + ip[1].Text + "." + ip[2].Text + "." + ip[3].Text;
            string maskStr = mask[0].Text + "." + mask[1].Text + "." + mask[2].Text + "." + mask[3].Text;
            string gatewayStr = gateway[0].Text + "." + gateway[1].Text + "." + gateway[2].Text + "." + gateway[3].Text;
            string dns1Str;
            if (dns1[0].Text == "" && dns1[1].Text == "" && dns1[2].Text == "" && dns1[3].Text == "")
            {
                dns1Str = "none";
            }
            else
            {
                dns1Str = dns1[0].Text + "." + dns1[1].Text + "." + dns1[2].Text + "." + dns1[3].Text;
            }
            string dns2Str;
            if (dns2[0].Text == "" && dns2[1].Text == "" && dns2[2].Text == "" && dns2[3].Text == "")
            {
                dns2Str = "none";
            }
            else
            {
                dns2Str = dns2[0].Text + "." + dns2[1].Text + "." + dns2[2].Text + "." + dns2[3].Text;
            }

            NetCard netcard = new NetCard(comboBox.Text, ipStr, maskStr, gatewayStr, dns1Str, dns2Str, null);
            return netcard;
        }




        /// <summary>
        /// 根据面板值修改IP
        /// </summary>
        public NetCard changeIp()
        {
            NetCard netcard = readIpPanel();
            if (netcard == null) return null;
            string reStr = "";

            string setIp = "netsh interface ip set address name=" + netcard.Name + " source=static addr=" + netcard.Ip + " mask=" + netcard.Mask + " gateway=" + netcard.Gateway + " gwmetric=1";
            string command1 = "echo 正在修改IP配置..&&" + setIp;
            RunCmd.run(command1, out reStr);
            //MessageBox.Show(reStr);


            string setDns1 = "netsh interface ip set dns name=\"" + netcard.Name + "\" source=static addr=" + netcard.Dns1 + " register=primary";
            string setDns2 = "netsh interface ip add dns name=\"" + netcard.Name + "\" " + netcard.Dns2 + " index=2";
            string command2 = "echo 正在修改DNS.." + " && " + setDns1 + " && " + setDns2;
            reStr = "";
            RunCmd.run(command2, out reStr);
            return netcard;
            //MessageBox.Show(reStr);
        }


    }
}
