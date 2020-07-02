using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ServiceProcess;

namespace 系统维护
{
    public class PlugManager
    {
        Thread autoRunThread;
        //自动检查过程中控件的更新
        public delegate void UpdateUI(string msg);
        public UpdateUI refreshUI;
        //创建插件代理
        delegate void OpenMethod();
        private GroupBox groupbox;
        private ImageList imageList;
        MainFrm mainFrm;
        PlugFrm plugFrm;
        bool isRunning = false;
        public PlugManager(MainFrm mainFrm, GroupBox groupbox, ImageList imageList)
        {
            this.mainFrm = mainFrm;
            this.groupbox = groupbox;
            this.imageList = imageList;
        }
        //先建立一个委托，用来invoke
        delegate void AsynUpdateUI();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="showName">名称</param>
        /// <param name="xl">序号</param>
        /// <param name="p">字体偏移量</param>
        public void drawItem(string type, string showName, int xl, int p)
        {
            if (xl < 1) xl = 1;
            Image image = null;
            OpenMethod openMethod = null;

            if (type == "radminInstallAndActive")
            {
                openMethod = new OpenMethod(radminInstallAndActive);
                image = this.imageList.Images["radmin.png"];
            }
            else if (type == "systemUserSafePwd")
            {
                openMethod = new OpenMethod(systemUserSafePwd);
                image = this.imageList.Images["usersafe.png"];
            }
            else if (type == "cancelScreenLockPassword")
            {
                openMethod = new OpenMethod(cancelScreenLockPassword);
                image = this.imageList.Images["unlocksystem.png"];
            }
            else if (type == "lockScreen")
            {
                openMethod = new OpenMethod(lockScreen);
                image = this.imageList.Images["locksystem.png"];
            }
            else if (type == "clearCDisk")
            {
                openMethod = new OpenMethod(clearCDisk);
                image = this.imageList.Images["clearCDisk.png"];
            }
            else if (type == "winUpdate")
            {
                openMethod = new OpenMethod(winUpdate);
                image = this.imageList.Images["winUpdate.png"];
            }
            else if (type == "printShare")
            {
                openMethod = new OpenMethod(printShare);
                image = this.imageList.Images["printShare.png"];
            }



            PictureBox p1 = new PictureBox();
            p1.Image = image;
            p1.Parent = this.groupbox;
            p1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            p1.Location = new System.Drawing.Point(24, 20 + (xl - 1) * 70);
            p1.Size = new System.Drawing.Size(36, 36);
            p1.Name = "picBox" + "_" + xl;

            p1.Click += new System.EventHandler((sender, e) =>
            {
                if (isRunning)
                {
                    plugFrm.Show();
                    return;
                }
                isRunning = true;
                plugFrm = new PlugFrm();
                this.refreshUI += plugFrm.refreshUI;
                plugFrm.Show();
                autoRunThread = new Thread(new ThreadStart(openMethod));
                autoRunThread.Start();
            });
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new System.Drawing.Font("微软雅黑", 7, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label.ForeColor = System.Drawing.Color.Black;
            label.Text = showName;
            label.Location = new System.Drawing.Point(2 + p, 20 + (xl - 1) * 70 + 36);
            label.Parent = this.groupbox;
            label.Name = "lab" + "_" + xl;
            //label.DoubleClick += new System.EventHandler((sender, e) => {
            //    openMethod(urlDic[type],user,pwd);
            //});
            xl++;

        }


        //远程工具radmin的安装与激活
        void radminInstallAndActive()
        {
            string win32Path = @"C:\Windows\System32\rserver30\";
            string win64Path = @"C:\Windows\SysWOW64\rserver30\";
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;

            refreshUI("检查是否已经安装Radmin:");
            Thread.Sleep(1000);
            string path = "";
            if (File.Exists(win64Path+"rserver3.exe"))
            {
                path = win64Path;
            }
            else if (File.Exists(win32Path + "rserver3.exe"))
            {
                path = win32Path;
            }
            if (path != "")
            {
                refreshUI("已安装\r\n");
                Thread.Sleep(1000);
                refreshUI("开始重新激活Radmin\r\n（大约需要10秒，请勿关闭窗口，如弹出英文窗口，则是需要重启，请在保存好资料后选择【yes】）：");
                Thread.Sleep(5000);
                p.StartInfo.WorkingDirectory = Tools.appStartPath() + @"\radmin\jihuo\";
                p.StartInfo.FileName ="install.bat";
                p.Start();
                Thread.Sleep(8000);
                refreshUI("激活完毕\r\n");
                isRunning = false;
            }
            else
            {
                refreshUI("未安装\r\n");
                refreshUI("开始安装Radmin 3.5(如没安装成功请重启电脑后再安装) ...\r\n");
                Thread.Sleep(3000);
                //运行静默安装
                p.StartInfo.WorkingDirectory = Tools.appStartPath() + @"\radmin\";
                p.StartInfo.FileName = "rserv35cn.msi";
                p.StartInfo.Arguments = "/passive";
                p.Start();
                //如果都找不到文件，就继续找
                while (!File.Exists(win32Path + "rserver3.exe") && !File.Exists(win64Path + "rserver3.exe")) { }
                path = File.Exists(win32Path + "rserver3.exe") ? win32Path: win64Path;
                //找到后线程再沉睡5秒，确保安装完毕
                Thread.Sleep(5000);
                p.Close();
                refreshUI("Radmin 3.5安装完成。\r\n");
                Thread.Sleep(1000);
                refreshUI("开始注入基础账号，请在弹出的选择框上选择【是】。\r\n");
                Thread.Sleep(4000);
                p.StartInfo.WorkingDirectory = Tools.appStartPath() + @"\radmin\zhanghao\";
                p.StartInfo.FileName = "userReg.bat";
                p.Start();
                Thread.Sleep(4000);
                refreshUI("基础账号注入完成。。\r\n");
            }


            refreshUI("检测Radmin Server运行状态\r\n");
            Thread.Sleep(2000);
            if (Process.GetProcessesByName("rserver3").Length != 0)
            {
                refreshUI("Radmin Server正在运行");
            }
            else
            {
                refreshUI("Radmin Server未运行，正在启动..\r\n");
                try
                {
                    Process.Start(path + "rserver3.exe /start");
                    Thread.Sleep(3000);
                    refreshUI("Radmin Server已启动");
                }
                catch(Exception e)
                {
                    refreshUI("Radmin Server启动失败，原因是："+e.Message);
                } 
            }

            refreshUI("检测设置完毕\r\n");


            p.Close();
        }
        //为系统账户设置安全密码
        void systemUserSafePwd()
        {
            isRunning = true;
            string[] users = Tools.getUsers();
  
            refreshUI("检查系统中存在的账户:");
            Thread.Sleep(2000);
            
            refreshUI("已找到"+users.Length+"个账户，分别为\r\n");
            foreach (string user in users)
            {
                   refreshUI(user+"\r\n");
            }
            Thread.Sleep(3000);
            refreshUI("当前使用账户名为："+users[0]+"\r\n");
            Thread.Sleep(3000);
            refreshUI("将其余账户自动设置强密码：");
            for (int i = 1; i < users.Length; i++)
            {
                string command = "net user " + users[i]+ "  Hyzx@12306" ;
                string reStr = "";
                RunCmd.run(command, out reStr);         
            }
            Thread.Sleep(2000);
            refreshUI("设置完毕");
            isRunning = false;

        }

        //开启系统自动升级
        void winUpdate()
        {
            isRunning = true;
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;
            refreshUI("正在运行 开启系统自动更新.bat 脚本");
            Thread.Sleep(3000);
            //运行静默安装
            p.StartInfo.WorkingDirectory = Tools.appStartPath() + @"\bat\";
            p.StartInfo.FileName = "开启系统自动更新.bat";
            p.StartInfo.Arguments = "/passive";
            p.Start();
            Thread.Sleep(7000);
            p.Close();
            isRunning = false;
        }

        //打印机共享
        void printShare()
        {
            isRunning = true;




            ServiceController service = new ServiceController("Print Spooler");

            if (service.Status == ServiceControllerStatus.Running)
            {
                refreshUI("正在停止 Print Spooler 服务..\r\n");
                try
                {
                    service.Stop();
                    Thread.Sleep(2000);
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    refreshUI("服务已停止\r\n");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    refreshUI("停止服务失败，原因是：" + ex.Message + "..\r\n");
                }
            }
            refreshUI("正在清除C:\\Windows\\System32\\spool\\PRINTERS中的打印缓存文件..\r\n");
            Thread.Sleep(2000);
            Tools.deletePath(@"C:\Windows\System32\spool\PRINTERS");
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                refreshUI("正在重启 Print Spooler 服务..\r\n");
                Thread.Sleep(2000);
                try
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    refreshUI("服务已重启\r\n");
                }
                catch (Exception ex)
                {
                    refreshUI("停止服务失败，原因是：" + ex.Message + "..\r\n");
                }
            }
            Thread.Sleep(1000);

            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;
            refreshUI("开始运行 打印机共享修复.bat 脚本\r\n");
            Thread.Sleep(3000);
            //运行静默安装
            p.StartInfo.WorkingDirectory = Tools.appStartPath() + @"\bat\";
            p.StartInfo.FileName = "打印机共享修复.bat";
            p.StartInfo.Arguments = "/passive";
            p.Start();
            Thread.Sleep(7000);
            refreshUI("脚本运行完毕！请手动关闭。该脚本需要重启后才能生效，请手动重启计算机\r\n");
            p.Close();
            isRunning = false;
        }

        //删除c盘下的垃圾文件
        void clearCDisk()
        {
            isRunning = true;
            ServiceController service = new ServiceController("Windows Modules Installer");

            if (service.Status == ServiceControllerStatus.Running) 
            {
                refreshUI("正在停止 Windows Modules Installer 服务..\r\n");
                try 
                {
                    service.Stop();
                    Thread.Sleep(2000);
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    refreshUI("服务已停止\r\n");
                    Thread.Sleep(1000);
                }
                catch(Exception ex)
                {
                    refreshUI("停止服务失败，原因是："+ex.Message+"..\r\n");
                }
            }
            refreshUI("正在清除C:\\windows\\log中系统更新日志缓存..\r\n");
            Thread.Sleep(2000);
            Tools.deletePath(@"C:\Windows\Logs");
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                refreshUI("正在重启 Windows Modules Installer 服务..\r\n");
                Thread.Sleep(2000);
                try
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    refreshUI("服务已重启\r\n");
                }
                catch (Exception ex)
                {
                    refreshUI("停止服务失败，原因是：" + ex.Message + "..\r\n");
                }
            }
            Thread.Sleep(2000);
            refreshUI("清除完毕");
            isRunning = false;
        }


        //取消开机密码
        void cancelScreenLockPassword()
        {
    
            refreshUI("正在开发");
            isRunning = false;
        }

        //开机1分钟后锁屏
        void lockScreen()
        {
            refreshUI("正在开发");
            isRunning = false;
        }
    }
}
