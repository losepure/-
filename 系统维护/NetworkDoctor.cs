using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 系统维护
{
    public class NetworkDoctor
    {
        //检查完毕后返回委托，更新ui
        public delegate void AccomplishStepTask(NetworkDoctorBackObj reObj);
        public AccomplishStepTask TaskStepCallBack;


        //设置后测试网络
        Dictionary<string, string> resultDic;
        //网卡组
        NetCards netCards;
        //网卡字典
        Dictionary<string, NetCard> netCardDic;
        NetCard netCard;
        //系统配置字典
        Dictionary<string, string> sysConfigDic;
        //要ping的ip地址
        Dictionary<string, string> pingIpDic;
        //本地保存的ip配置字典
        Dictionary<string, string> localIpDic;
        //本地数据库
        DbManager db;
        //创建诊断线程
        Thread autoCheckThread;//new Thread(new ThreadStart(delegate() { }));
        NetCards netcards = new NetCards();
        //自动检查过程中控件的更新
        public delegate void UpdateUI(int index, string type, string msg);
        public delegate void UpdateUI1(int index, string type, bool msg);
        public UpdateUI refreshUI;
        public UpdateUI1 statusUI;
        //获取系统默认安装路径c:/programe
        public static string defaultPath = Tools.getSystemDefaultDirectory();

        public NetworkDoctor(NetCard netCard, DbManager Db)
        {

            //创建网卡对象
            this.netCards = new NetCards();
            //获取所有网卡字典
            this.netCardDic = this.netCards.getAllNetCard();
            //设置当前要诊断的网卡
            this.netCard = netCard;
            //设置数据库对象
            this.db = Db;
            //获取对应设置参数
            this.localIpDic = this.db.getRow2Dic("pcInfo");
            this.pingIpDic = this.db.getDic<string, string>("pingIp", "webName", "ip");
            this.sysConfigDic = this.db.getRow2Dic("sysConfig");
        }

        /// <summary>
        /// 检查网卡是否被禁用或者不存在
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkHandware()
        {
            statusUI(0, "start", true);
            refreshUI(0, "msg", "开始检查");
            Thread.Sleep(1000);
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("handware");
            backObj.Status = true;
            //如果没有找到网卡
            if (this.netCardDic.Count == 0)
            {
                refreshUI(0, "msg", "未发现物理网卡,请检查交换机或重装iNode");
                backObj.addMsg("未发现物理网卡\r\n");
                backObj.addSugest("此种故障一般由iNode引起，请先卸载iNode，卸载密码www.h3c.com。卸载后重启，再重新安装iNode后，再次运行本工具..");
                statusUI(0, "end", false);
                backObj.Status = false;
                //如果网卡被禁用===========================
            }
            else
            {
                backObj.addMsg("网卡正常\r\n");
                refreshUI(0, "msg", "正常");
                statusUI(0, "end", true);
                backObj.Status = true;
            }

            TaskStepCallBack(backObj);
            return backObj;
        }


        /// <summary>
        /// 检查IP地址是否设置正确
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkIp()
        {
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("ip");
            backObj.Status = true;
            statusUI(1, "start", true);
            refreshUI(1, "msg", "开始检查");
            Thread.Sleep(1000);

            //判断是否有NetCard符合ip规则
            string enableNetCardName = "";
            NetCard enableNetCard = null;
            foreach (KeyValuePair<string, NetCard> kv in netCardDic)
            {
                NetCard netCard = kv.Value;
                if (netCard.isEnableIp())
                {
                    enableNetCardName = kv.Key;
                    enableNetCard = kv.Value;
                }
            }

            //检查IP地址是否可用
            refreshUI(1, "msg", "检查IP地址是否可用");
            if (enableNetCardName == "")
            {
                refreshUI(1, "msg", "发现本地ip地址设置错误..");
                backObj.addMsg("发现本地ip地址设置错误..\r\n");
                backObj.Status = false;
                if (this.localIpDic != null && this.localIpDic["netCardName"] != "")
                {
                    refreshUI(1, "msg", "尝试从本地恢复正确的IP配置..");
                    backObj.addMsg("尝试从本地恢复正确的IP配置..\r\n");
                    if (this.netCardDic.ContainsKey(this.localIpDic["netCardName"]))
                    {
                        this.netCard = this.netCardDic[this.localIpDic["netCardName"]];
                        this.netCard.setIp(this.localIpDic["ip"], this.localIpDic["mask"], this.localIpDic["gateway"]);
                        refreshUI(1, "msg", "检查IP是否设置成功..");
                        backObj.addMsg("检查IP是否设置成功..\r\n");
                        this.netCard = refreshNetCard(this.netCard.Name);
                        if (!this.netCard.isEnableIp())
                        {
                            refreshUI(1, "msg", "IP地址设置失败..");
                            backObj.addMsg("IP地址设置失败..\r\n");
                            backObj.addSugest("请尝试手动设置IP地址\r\n");
                            statusUI(1, "end", false);
                        }
                        else
                        {
                            refreshUI(1, "msg", "IP地址设置成功！");
                            statusUI(1, "end", true);
                            backObj.addMsg("IP地址正常\r\n");
                            backObj.Status = true;
                        }
                    }
                    else
                    {
                        refreshUI(1, "msg", "本地IP配置未存储，无法恢复");
                        Thread.Sleep(1000);
                        refreshUI(1, "msg", "失败");
                        statusUI(1, "end", false);
                    }
                }

            }
            //检查子网掩码是否可用
            //refreshUI(1, "msg", "检查子网掩码是否可用");
            //if (enableNetCard.isEnableMask()) 
            //{
            //    refreshUI(1, "msg", "子网掩码设置错误，尝试重新设置子网掩码..");
            //    backObj.addMsg("子网掩码设置错误，尝试重新设置子网掩码..\r\n"); 
            //    enableNetCard.setIp(enableNetCard.Ip, "255.255.255.0",enableNetCard.Gateway);
            //    refreshUI(1, "msg", "检查子网掩码是否设置成功..");
            //    backObj.addMsg("检查子网掩码是否设置成功..\r\n");
            //    this.netCard = refreshNetCard(enableNetCardName);
            //    if (!this.netCard.isEnableIp())
            //    {
            //        refreshUI(1, "msg", "子网掩码设置失败..");
            //        backObj.addMsg("子网掩码设置失败..\r\n");
            //        backObj.addSugest("请尝试手动设置子网掩码\r\n");
            //        statusUI(1, "end", false);
            //        return backObj;
            //    }
            //    else
            //    {
            //        refreshUI(1, "msg", "子网掩码设置成功！");
            //        statusUI(1, "end", true);
            //        backObj.addMsg("子网掩码正常\r\n");
            //        backObj.Status = true;
            //    }
            //}
            //检查默认网关是否可用
            refreshUI(1, "msg", "检查默认网关是否正确");
            if (enableNetCard != null)
            {
                string[] ipArr = enableNetCard.Ip.Split(new char[] { '.' });
                ipArr[3] = "1";
                string theoryGateway = string.Join(".", ipArr);
                if (enableNetCard.isEnableGateway())
                {
                    refreshUI(1, "msg", "网关设置错误，尝试重新设置网关..");
                    backObj.addMsg("网关设置错误，尝试重新设置网关..\r\n");
                    enableNetCard.setIp(enableNetCard.Ip,enableNetCard.Mask, theoryGateway);
                    refreshUI(1, "msg", "检查网关是否设置成功..");
                    backObj.addMsg("检查网关是否设置成功..\r\n");
                    this.netCard = refreshNetCard(enableNetCardName);
                    if (!this.netCard.isEnableGateway())
                    {
                        refreshUI(1, "msg", "网关地址设置失败..");
                        backObj.addMsg("网关地址设置失败..\r\n");
                        backObj.addSugest("请尝试手动设置网关地址\r\n");
                        statusUI(1, "end", false);
                        return backObj;
                    }
                    else
                    {
                        refreshUI(1, "msg", "网关地址设置成功！");
                        statusUI(1, "end", true);
                        backObj.addMsg("网关地址正常\r\n");
                        backObj.Status = true;
                    }
                }
            }

            refreshUI(1, "msg", "正常");
            statusUI(1, "end", true);
            backObj.addMsg("IP地址正常\r\n");
            //任务完成了界面的返回
            TaskStepCallBack(backObj);
            return backObj;
        }

        /// <summary>
        /// 检查DNS是否设置正确
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkDNS()
        {
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("dns");
            backObj.Status = true;
            statusUI(2, "start", true);
            refreshUI(2, "msg", "开始检查");
            Thread.Sleep(1000);
            if (!this.netCard.isEnableDns(sysConfigDic["defaultDNS1"], sysConfigDic["defaultDNS2"]))
            {
                refreshUI(2, "msg", "发现本地dns地址设置错误..");
                backObj.addMsg("发现本地dns地址设置错误..\r\n");
                backObj.Status = false;
                refreshUI(2, "msg", "尝试从本地恢复正确的DNS配置..");
                backObj.addMsg("尝试从本地恢复正确的DNS配置..\r\n");
                this.netCard.setDns(sysConfigDic["defaultDNS1"], sysConfigDic["defaultDNS2"]);
                refreshUI(2, "msg", "检查DNS是否设置成功..");
                backObj.addMsg("检查DNS是否设置成功..\r\n");
                this.netCard = refreshNetCard(this.netCard.Name);
                if (!this.netCard.isEnableDns(sysConfigDic["defaultDNS1"], sysConfigDic["defaultDNS2"]))
                {
                    refreshUI(2, "msg", "DNS地址设置失败..");
                    backObj.addMsg("DNS地址设置失败..\r\n");
                    backObj.addSugest("请尝试手动设置DNS地址\r\n");
                    refreshUI(2, "msg", "失败");
                    statusUI(2, "end", false);
                }
                else
                {
                    refreshUI(2, "msg", "DNS地址设置成功！");
                    statusUI(2, "end", true);
                    refreshUI(2, "msg", "正常");
                    backObj.addMsg("DNS地址正常\r\n");
                    backObj.Status = true;
                }

            }
            else
            {
                backObj.addMsg("DNS地址正常\r\n");
                refreshUI(2, "msg", "正常");
                statusUI(2, "end", true);
            }
            TaskStepCallBack(backObj);
            return backObj;
        }

        /// <summary>
        /// 检查iNode状态
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkiNode()
        {

            refreshUI(3, "msg", "开始检查");
            statusUI(3, "start", true);
            Thread.Sleep(1000);
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("iNode");
            backObj.Status = true;
            refreshUI(3, "msg", "判断iNode运行状态");
            if (Process.GetProcessesByName("iNode Client").Length != 0)
            {
                refreshUI(3, "msg", "iNode正在运行，判断网络是否畅通");
                if (checkNetwork().Status)
                {
                    refreshUI(3, "msg", "正常");
                    statusUI(3, "end", true);
                }
                else
                {
                    refreshUI(3, "msg", "iNode状态可能异常，尝试重启iNode连接..");
                    Process.GetProcessesByName("iNode Client")[0].Kill();
                    Process.Start(defaultPath + @"iNode\iNode Client\iNode Client.exe");
                    refreshUI(3, "msg", "iNode已重启，等待返回网络通畅情况检测结果..");
                    Thread.Sleep(5000);
                    if (checkNetwork().Status)
                    {
                        refreshUI(3, "msg", "正常");
                        statusUI(3, "end", true);
                    }
                    else
                    {
                        refreshUI(3, "msg", "失败，请确认iNode账号密码是否正确");
                        statusUI(3, "end", false);

                    }
                }
            }
            else
            {
                Process.Start(defaultPath + @"iNode\iNode Client\iNode Client.exe");
                refreshUI(3, "msg", "iNode未运行，重新启动iNode..");
                Thread.Sleep(5000);
                refreshUI(3, "msg", "iNode已重启，等待返回网络通畅情况检测结果..");
                if (checkNetwork().Status)
                {
                    refreshUI(3, "msg", "正常");
                    statusUI(3, "end", true);
                }
                else
                {
                    refreshUI(3, "msg", "失败，请确认iNode账号密码是否正确");
                    statusUI(3, "end", false);

                }
            }

            //
            TaskStepCallBack(backObj);
            return backObj;
        }

        public NetworkDoctorBackObj checkVRV()
        {

            refreshUI(4, "msg", "开始检查");
            statusUI(4, "start", true);
            Thread.Sleep(1000);
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("iNode");
            backObj.Status = true;
            refreshUI(4, "msg", "判断北信源运行状态");
            if (Process.GetProcessesByName("vpengine").Length != 0 || Process.GetProcessesByName("vpengine64").Length != 0)
            {
                refreshUI(4, "msg", "北信源正在运行，判断网络是否畅通");
                if (checkNetwork().Status)
                {
                    refreshUI(4, "msg", "正常");
                    statusUI(4, "end", true);
                }
                else
                {
                    refreshUI(4, "msg", "失败，北信源状态异常，请联系管理员解决。");
                    statusUI(4, "end", false);

                }
            }
            else
            {
                Process.Start(defaultPath + @"VRV\CEMS\vpengine.exe");
                refreshUI(4, "msg", "北信源未运行，重新启动北信源..");
                Thread.Sleep(5000);
                refreshUI(4, "msg", "北信源已重启，等待返回网络通畅情况检测结果..");
                if (checkNetwork().Status)
                {
                    refreshUI(4, "msg", "正常");
                    statusUI(4, "end", true);
                }
                else
                {
                    refreshUI(4, "msg", "失败，北信源状态异常，请联系管理员解决。");
                    statusUI(4, "end", false);

                }
            }

            //
            TaskStepCallBack(backObj);
            return backObj;
        }



        /// <summary>
        /// 检查网络
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkNetwork(NetCard netcard)
        {
            NetworkDoctorBackObj backObj = new NetworkDoctorBackObj("network");
            backObj.Status = true;
            Dictionary<string, string> connectNetTestResultDic = netCard.connectNetTest(this.pingIpDic);
            foreach (KeyValuePair<string, string> kv in connectNetTestResultDic)
            {
                if (kv.Value != "成功")
                {
                    backObj.Status = false;
                    backObj.addMsg("网络状态异常\r\n");
                    break;
                }
            }
            TaskStepCallBack(backObj);
            return backObj;
        }


        /// <summary>
        /// 检查网络
        /// </summary>
        /// <returns></returns>
        public NetworkDoctorBackObj checkNetwork()
        {
            return checkNetwork(this.netCard);
        }

        public NetCard refreshNetCard(string name)
        {
            netcards = new NetCards();
            Dictionary<string, NetCard> allNetCardDic = netcards.getAllNetCard();
            if (allNetCardDic.ContainsKey(name))
            {
                return allNetCardDic[name];
            }
            return null;
        }

        public delegate void AccomplishTask();
        public AccomplishTask TaskStart;
        public AccomplishTask TaskFinish;
        /// <summary>
        /// 执行自动检查程序
        /// </summary>
        public void autoCheck()
        {
            NetworkDoctorBackObj reObj = null;
            //开始时需要渲染的控件
            TaskStart();
            //进行逐个检查

            //1.先检查硬件连接是否正确
            reObj = checkHandware();
            if (!reObj.Status) return;
            //2.检查ip地址是否正确
            reObj = checkIp();
            if (!reObj.Status) return;
            //3.检查dns是否正确
            reObj = checkDNS();
            if (!reObj.Status) return;
            //4.检查iNode是否正常
            reObj = checkiNode();
            if (!reObj.Status) return;
            //5.检查北信源是否正常
            reObj = checkVRV();
            if (!reObj.Status) return;
            //结束时要渲染的控件
            TaskFinish();
        }
        //开始执行自动检查
        public void start()
        {
            autoCheckThread = new Thread(new ThreadStart(autoCheck));
            autoCheckThread.Start();
        }
        //中断自动检查
        public void abort()
        {
            autoCheckThread.Abort();
        }
    }
}
