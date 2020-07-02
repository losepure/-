using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
namespace 系统维护
{
    public partial class MainFrm : Form
    {

        private Point mPoint;
        //数据库对象

        DbManager db;
        //ip设置面板对象
        public PanelManager pm;
        //计划对象
        PlanManager plm;


        //加载左侧ip面板控件
        Dictionary<string, TextBox[]> tbsDic = new Dictionary<string, TextBox[]>();
        //加载右侧控件
        Dictionary<string, Label[]> labDic = new Dictionary<string, Label[]>();
        Dictionary<string, PictureBox[]> picDic = new Dictionary<string, PictureBox[]>();

        //
        public bool isJudgeFirstUse = false;
        //系统启动时的参数
        string[] args;
        //界面是否可见

        //创建诊断对象
        public NetworkDoctor networkDoctor;

        public MainFrm()
        {
            InitializeComponent();
        }

        public MainFrm(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }



        public void Form1_Load(object sender, EventArgs e)
        {


            //判断是否第一次使用本软件
            if (!isJudgeFirstUse && Tools.isFirstUse())
            {
                this.Hide();
                FirstUseFrm firstUseFrm = new FirstUseFrm(this);
                firstUseFrm.ShowDialog();
                return;
            }


            //创建数据库对象
            db = new DbManager();
            db.init();



            //加载配置项
            Dictionary<string, string> sysConfig = db.getRow2Dic("sysConfig");
            bool autoRun = Convert.ToBoolean(sysConfig["autoRun"]);
            bool autoConnect = Convert.ToBoolean(sysConfig["autoConnect"]);


            company_lab.Text = sysConfig["company"].ToString();
            tel_lab.Text = sysConfig["phone"].ToString();


            //加载各类图片
            pic_icon.Image = imageList1.Images["icon.png"];
            pic_tubiao.Image = imageList1.Images["tubiao.png"];
            pic_check.Image = imageList1.Images["check.png"];
            pic_min.Image = imageList1.Images["min.png"];
            pic_close.Image = imageList1.Images["close.png"];
            //加载左侧ip面板控件
            tbsDic.Add("ip", new TextBox[4] { tbIp0, tbIp1, tbIp2, tbIp3 });
            tbsDic.Add("mask", new TextBox[4] { tbMask0, tbMask1, tbMask2, tbMask3 });
            tbsDic.Add("gateway", new TextBox[4] { tbGateway0, tbGateway1, tbGateway2, tbGateway3 });
            tbsDic.Add("dns1", new TextBox[4] { tbZDns0, tbZDns1, tbZDns2, tbZDns3 });
            tbsDic.Add("dns2", new TextBox[4] { tbFdns0, tbFdns1, tbFdns2, tbFdns3 });
            tbsDic.Add("iNode", new TextBox[1] { tbiNode });
            //加载右侧控件
            labDic.Add("taskTip", new Label[5] { lab_jc0, lab_jc1, lab_jc2, lab_jc3, lab_jc4 });
            labDic.Add("msg", new Label[5] { lab_suggest0, lab_suggest1, lab_suggest2, lab_suggest3, lab_suggest4 });
            picDic.Add("loading", new PictureBox[5] { pic_jc0, pic_jc1, pic_jc2, pic_jc3, pic_jc4 });




            //创建ip面板管理对象
            pm = new PanelManager(comboBox1, tbsDic);


            //创建诊断对象
            networkDoctor = new NetworkDoctor(pm.getSelectedNetCard(), db);
            networkDoctor.TaskStart += taskStart;
            networkDoctor.refreshUI += refreshUI;
            networkDoctor.statusUI += statusUI;
            networkDoctor.TaskStepCallBack += taskStepCallBack;
            networkDoctor.TaskFinish += taskFinish;



            //创建插件面板对象
            PlugManager plugManager = new PlugManager(this, plug_gb, imageList2);
            plugManager.drawItem("radminInstallAndActive", "Radmin配置", 1, 6);
            plugManager.drawItem("systemUserSafePwd", "账户密码安全", 2, 6);
            plugManager.drawItem("winUpdate", "系统自动升级", 3, 6);
            plugManager.drawItem("printShare", "打印共享修复", 4, 6);
            plugManager.drawItem("clearCDisk", "清除C盘垃圾", 5, 7);

            //如果面板不可用，则说明就没有可用的网卡，给出提示
            if (!pm.IsEnable)
            {
                MessageBox.Show("未找到可用的网卡，请开启诊断助手进行诊断！");
                return;
            }
            else
            {
                if (pm.getSelectedNetCard().isEnableIp())
                {
                    //保存网卡信息
                    string ipInfoPath = Application.StartupPath + "\\IpInfo.json";
                    if (!File.Exists(ipInfoPath))
                    {
                        NetCard selectedNetcard = pm.getSelectedNetCard();
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("ip", selectedNetcard.Ip);
                        dic.Add("mask", selectedNetcard.Mask);
                        dic.Add("gateway", selectedNetcard.Gateway);
                        dic.Add("dns1", "10.208.4.100");
                        dic.Add("dns2", "10.208.4.200");
                        Tools.saveJsonFile(ipInfoPath, dic);
                    }
                }
            }

            //更改托盘图片提示
            notifyIcon1.Text = "本机IP：" + pm.getEnableCard().Ip;

            //加载桌面悬浮信息
            SkinFrm skinfrm = new SkinFrm(pm.getSelectedNetCard(), db);
            skinfrm.Show();



            //创建计划类对象
            plm = new PlanManager(db, this);
            plm.netConnectUI += netConnectUI;
            if (autoConnect_ck.Checked)
            {
                plm.addPlan("网络连接状态");
            }
            else
            {
                plm.removePlan("网络连接状态");
            }
            //这个必须放在plm后面，因为调用了plm
            if (autoRun)
                autoRun_ck.Checked = true;
            else
                autoRun_ck.Checked = false;

            if (autoConnect)
                autoConnect_ck.Checked = true;
            else
                autoConnect_ck.Checked = false;
            plm.start();



            //绑定用户注册信息更新事件
            Tools.userInfoUI += getUserInfo;

        }





        delegate void cmdM();
        private void btn_changeIp_Click(object sender, EventArgs e)
        {

            cmdM cmdm = () =>pm.changeIp();
            this.BeginInvoke(cmdm);
            MessageBox.Show("修改成功！");
            //重新读取面板
            NetCard netcard = pm.readIpPanel();
            //判断更改后的IP能否ping通，如果能，则保存新的ip配置
            System.Threading.Timer threadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(delegate {
                NetworkDoctorBackObj backObj = networkDoctor.checkNetwork(netcard);
                if (netcard.isEnableIp() && backObj.Status)
                {
                    Tools.updatePcInfo(db, netcard);
                }
            }), null, 8000, 0);

   
        }



        private Dictionary<string, bool> iNodeTest()
        {

            Dictionary<string, bool> iNodeProcesses = new Dictionary<string, bool>();
            iNodeProcesses.Add("iNode Client", false);
            iNodeProcesses.Add("iNodeCmn", false);
            iNodeProcesses.Add("iNodePortal", false);
            foreach (KeyValuePair<string, bool> kv in iNodeProcesses)
            {
                if (Process.GetProcessesByName(kv.Key).Length > 0)
                {
                    iNodeProcesses[kv.Key] = true;
                }
                else
                {
                    iNodeProcesses[kv.Key] = false;
                }
            }
            return iNodeProcesses;
        }

        bool isChecking = false;
        string checkPicName = "check";
        //进行全面检查
        private void pic_check_Click(object sender, EventArgs e)
        {
            if (checkPicName == "back")
            {
                checkAbort();
                checkPicName = "check";
                return;
            }
            //是否正在检查
            isChecking = isChecking ? false : true;
            //如果正在检查，则停止
            if (!isChecking)
            {
                pic_check.Image = imageList1.Images["check.png"];
                checkPicName = "check";
                networkDoctor.abort();
                checkAbort();
            }
            else
            {
                pic_check.Image = imageList1.Images["cancel.png"];
                checkPicName = "cancel";
                networkDoctor.start();
            }


        }


        //中断诊断检查
        private void checkAbort()
        {
            for (int i = 0; i < 5; i++)
            {
                labDic["taskTip"][i].Text = "待检查";
                labDic["msg"][i].Text = "";
                picDic["loading"][i].Visible = false;
            }
        }

        //先建立一个委托，用来invoke
        delegate void AsynUpdateUI();
        //检查图标更新图标更新
        private void statusUI(int index, string type, bool msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    labDic["taskTip"][index].Text = msg ? "正常" : "失败";
                    if (type == "start")
                    {
                        picDic["loading"][index].Image = Image.FromFile(Application.StartupPath + "\\images\\loading.gif");
                        picDic["loading"][index].Visible = true;
                    }
                    else if (type == "end")
                    {
                        if (msg)
                        {
                            picDic["loading"][index].Image = imageList1.Images["right.png"];
                        }
                        else
                        {
                            picDic["loading"][index].Image = imageList1.Images["error.png"];
                        }
                    }
                }));
            }

        }
        //检查文字状态更新
        private void refreshUI(int index, string type, string msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    labDic["msg"][index].Text = msg;
                }));
            }
        }
        private void taskStepCallBack(NetworkDoctorBackObj backObj)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                    {
                        //如果检查未通过，说明有问题，直接停止下面的检查
                        if (!backObj.Status)
                        {

                        }


                    }));
            }

        }
        //检查任务开始时的方法
        private void taskStart()
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {


                }));
            }

        }
        //检查任务结束时的方法
        private void taskFinish()
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    pic_check.Image = imageList1.Images["back.png"];
                }));
                isChecking = false;
                checkPicName = "back";
            }

        }


        //用于控制窗体拖动
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }







        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string netcardName = comboBox1.SelectedItem.ToString();
            if (pm != null) 
            {
                pm.SelectedNetCardName = netcardName;
            }
          

        }



        private void pic_check_MouseMove(object sender, MouseEventArgs e)
        {
            pic_check.Image = imageList1.Images[checkPicName + "_hover.png"];
            Console.WriteLine(checkPicName);
        }

        private void pic_check_MouseLeave(object sender, EventArgs e)
        {
            pic_check.Image = imageList1.Images[checkPicName + ".png"];
        }

        private void pic_min_MouseMove(object sender, MouseEventArgs e)
        {
            pic_min.Image = imageList1.Images["min_hover.png"];
        }

        private void pic_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pic_min_MouseLeave(object sender, EventArgs e)
        {
            pic_min.Image = imageList1.Images["min.png"];
        }

        private void pic_close_MouseMove(object sender, MouseEventArgs e)
        {
            pic_close.Image = imageList1.Images["close_hover.png"];
        }
        private void pic_close_MouseLeave(object sender, EventArgs e)
        {
            pic_close.Image = imageList1.Images["close.png"];
        }




        /// <summary>
        /// 设置开机自动启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRun_ck_CheckedChanged(object sender, EventArgs e)
        {

            string path = Application.ExecutablePath;
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            if (autoRun_ck.Checked)
            {
                rk2.SetValue("JcShutdown", path);
                db.updateConfigField<bool>("sysConfig", "autoRun", true);
            }
            else
            {
                rk2.DeleteValue("JcShutdown", false);
                db.updateConfigField<bool>("sysConfig", "autoRun", false);
            }
            rk2.Close();
            rk.Close();
        }


        //右下角图标操作
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();

        }
        private void pic_close_Click(object sender, EventArgs e)
        {


            this.Hide();
            //this.Close();
            //this.Dispose();
            //System.Environment.Exit(0);
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            pic_close_Click(sender, e);

        }

        private void exit_Click(object sender, EventArgs e)
        {
            db.close();
            this.notifyIcon1.Visible = false;
            this.Close();
            this.Dispose();
            System.Environment.Exit(0);
        }




        private void reset_Click(object sender, EventArgs e)
        {
            if (Tools.reInit())
            {
                MessageBox.Show("重新初始化成功！请重新运行程序");
                this.Close();
                this.Dispose();
            }
        }


        private void userinfo_url_lab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string regUserInfoUrl = db.getField("sysConfig", "regUserInfoUrl");
            Process.Start(regUserInfoUrl);
        }



        private void autoConnect_ck_CheckedChanged(object sender, EventArgs e)
        {
            if (autoConnect_ck.Checked)
            {
                plm.addPlan("网络连接状态");
                db.updateConfigField<bool>("sysConfig", "autoConnect", true);
            }
            else
            {
                plm.removePlan("网络连接状态");

                db.updateConfigField<bool>("sysConfig", "autoConnect", false);
            }
        }

        private void checkUpdate_Click(object sender, EventArgs e)
        {
            Tools.checkUpdate(db, this, false);
        }

        /// <summary>
        /// 更新网络检查状态UI
        /// </summary>
        private void netConnectUI(string msg) 
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    netStatus_lab.Text = msg;
                   
                }));
            }
        }

        /// <summary>
        /// 获取用户注册信息
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public bool getUserInfo(string html)
        {
            if (html != "false" && html != "")
            {
                string[] items = html.Split(new char[] { '|' });

                if (InvokeRequired)
                {
                    this.Invoke(new AsynUpdateUI(delegate()
                    {
                        foreach (string item in items)
                        {
                            string[] kv = item.Split(new char[] { ':' });
                            this.Invoke(new AsynUpdateUI(delegate()
                            {
                                if (kv[0] == "xingmingStr") userInfo_name_txt.Text = kv[1];
                                if (kv[0] == "gangwei") userInfo_gangwei_txt.Text = kv[1];
                                if (kv[0] == "bumen") userInfo_bumeng_txt.Text = kv[1];
                                if (kv[0] == "bushi") userInfo_bushi_txt.Text = kv[1];
                            }));
                        }
                        userinfo_tip_lab.Hide();
                        userinfo_url_lab.Hide();
                    }));
                }
                return true;
            }
            else
            {
                if (InvokeRequired)
                {
                    this.Invoke(new AsynUpdateUI(delegate()
                    {
                        userinfo_tip_lab.Show();
                        userinfo_url_lab.Show();
                    }));
                }

                return false;
            }
        }

        private void about_Click(object sender, EventArgs e)
        {
            AboutFrm aboutFrm = new AboutFrm(db);
            aboutFrm.ShowDialog();

        }

        private void sysOptions_Click(object sender, EventArgs e)
        {
            SettingFrm settingFrm = new SettingFrm(this,db);
            settingFrm.ShowDialog();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }




    }
}
