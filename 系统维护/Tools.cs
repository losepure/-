using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Management;
using System.Data.OleDb;
using System.Xml;
using System.Runtime.InteropServices;
using System.Drawing;

namespace 系统维护
{
    class Tools
    {
        //获取桌面某点颜色所需要的api
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetDC(int hwnd);
        [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetPixel(int hdc, int x, int y);
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int ReleaseDC(int hwnd, int hdc);
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WindowFromPoint(int x, int y);
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int ScreenToClient(int hwnd, ref POINT lppoint);

        //窗口前置api
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        public static bool saveJsonFile(string path, Dictionary<string, string> dic)
        {
            try
            {
                StringWriter sw = new StringWriter();
                JsonWriter jw = new JsonTextWriter(sw);
                jw.WriteStartObject();
                foreach (KeyValuePair<string, string> kv in dic)
                {
                    jw.WritePropertyName(kv.Key);
                    jw.WriteValue(kv.Value);
                }
                //jw.WritePropertyName("date");
                //jw.WriteValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                jw.Flush();
                string jsonStr = sw.GetStringBuilder().ToString();
                using (FileStream file = File.Open(path, FileMode.OpenOrCreate))
                {
                    byte[] b = System.Text.Encoding.UTF8.GetBytes(jsonStr);
                    file.Write(b, 0, b.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        /// <summary>
        /// 获取json文件
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> readJsonFile(string path)
        {

            if (File.Exists(path))
            {
                using (FileStream file = File.OpenRead(path))
                {
                    int fsLen = (int)file.Length;
                    byte[] b = new byte[fsLen];
                    int r = file.Read(b, 0, b.Length);
                    string jsonStr = System.Text.Encoding.UTF8.GetString(b);
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                }
            }
            else
            {
                return null;
            }

        }


        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(ipInfoPath);
        //bf.Serialize(file, netCard);
        //file.Close();


        /// <summary>
        /// 返回程序的运行路径
        /// </summary>
        /// <returns></returns>
        public static string appStartPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// ping命令
        /// </summary>
        /// <param name="ip">要ping的ip地址</param>
        /// <returns></returns>
        public static string pingC(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ip);
                IPStatus ipStatus = reply.Status;
                if (ipStatus == IPStatus.Success)
                {
                    return "成功";
                }
                else if (ipStatus == IPStatus.TimedOut)
                {
                    return "超时";
                }
                else
                {
                    return "失败";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }



        /// <summary>
        /// 获取系统版本号
        /// </summary>
        /// <returns></returns>
        public static string getOSVersion()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";
            if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }



        /// <summary>
        /// 获取本系统所有用户，且把当前用户放在最前面
        /// </summary>
        /// <returns></returns>
        public static string[] getUsers()
        {
            string currentUserName = Environment.UserName.ToString();
            string[] users = getSoftwareInfo("Win32_UserAccount");
            int pos = Array.IndexOf(users, currentUserName);
            if (pos != 0)
            {
                users[pos] = users[0];
                users[0] = currentUserName;
            }
            return users;
        }

        /// <summary>
        /// 获取系统默认安装文件路径
        /// </summary>
        /// <returns></returns>
        public static string getSystemDefaultDirectory()
        {
            string osVersion = getOSVersion();
            string path = null;
            if (osVersion.Contains("Windows XP"))
            {
                path = @"C:\Program Files (x86)\";
            }
            else if (osVersion.Contains("Windows 7"))
            {
                path = @"C:\Program Files (x86)\";
            }
            else
            {
                path = @"C:\Program Files (x86)\";
            }
            return path;
        }




        public static string getiNodeUsername()
        {
            string path = @"C:\Program Files (x86)\iNode\iNode Client\Data\5020\1cfg.xml";
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(path, settings);
                doc.Load(reader);
                string username = doc.GetElementsByTagName("userName")[0].InnerText;
                return username;
            }
            return null;


        }






        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string[] getSoftwareInfo(string item)
        {
            //Win32_OperatingSystem   操作系统信息
            //Win32_ComputerSystem   计算机信息概要
            //Win32_UserAccount       操作系统所有账户
            List<string> softwareInfoList = new List<string>();
            string query = string.Format("select * from {0}", item);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj in searcher.Get())
            {
                softwareInfoList.Add(obj["Name"].ToString());
                //MessageBox.Show(obj["Name"].ToString());
            }
            return softwareInfoList.ToArray();
        }

        /// <summary>
        /// 是否第一次使用本软件
        /// </summary>
        /// <returns></returns>
        public static bool isFirstUse()
        {
            string path = Application.StartupPath + "\\.lock";
            if (File.Exists(path))
                return false;
            else
                return true;
        }
        /// <summary>
        /// 重新初始化程序
        /// </summary>
        /// <returns></returns>
        public static bool reInit()
        {
            string path = Application.StartupPath + "\\.lock";
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">更新数据库pc的ip信息</param>
        /// <param name="netcard">可用网卡实例</param>
        public static void updatePcInfo(DbManager db, NetCard netcard) 
        {
            db.updateConfigField<string>("pcInfo", "netCardName", netcard.Name);
            db.updateConfigField<string>("pcInfo", "ip", netcard.Ip);
            db.updateConfigField<string>("pcInfo", "mask", netcard.Mask);
            db.updateConfigField<string>("pcInfo", "gateway", netcard.Gateway);
            db.updateConfigField<string>("pcInfo", "dns1", netcard.Dns1);
            db.updateConfigField<string>("pcInfo", "dns2", netcard.Dns2);
            db.updateConfigField<string>("pcInfo", "iNode", Tools.getiNodeUsername());
            db.updateConfigField<string>("pcInfo", "sysUser", Tools.getUsers()[0]);
        }

        /// <summary>
        /// 更新数据库pc的ip信息
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="netcard">可用网卡实例</param>
        /// <param name="iNode">iNode账号</param>
        /// <param name="systemPwd">系统密码</param>
        public static void updatePcInfo(DbManager db, NetCard netcard, string iNode, string systemPwd)
        {
            updatePcInfo(db,netcard);
            db.updateConfigField<string>("pcInfo", "iNodePwd", iNode);
            db.updateConfigField<string>("pcInfo", "sysPwd", systemPwd);
        }


        /// <summary>
        /// 获取屏幕指定坐标点的颜色
        /// </summary>
        private struct POINT
        {
            private int x;
            private int y;
        }
        static POINT point;
        /// <summary>
        /// 获取桌面某点颜色
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        public static Color getDeskTopPixelColor(int x, int y)
        {
            int h = WindowFromPoint(x, y);
            int hdc = GetDC(h);
            ScreenToClient(h, ref point);
            int c = GetPixel(hdc, x, y);
            return Color.FromArgb(c);

        }
        static bool isOpenUpdateDialog = false;
        /// <summary>
        /// 检查更新
        /// </summary>
        public static void checkUpdate(DbManager db, MainFrm mainFrm, bool isDefault)
        {

            Dictionary<string, string> sysConfig = db.getRow2Dic("sysConfig");
            string version = sysConfig["version"];
            string updateUrl = sysConfig["updateUrl"];
            string localUpdateExeName = sysConfig["localUpdateExeName"];


            try
            {
                string urlStr = updateUrl + @"?type=version&isTest=0&version=" + version;
                string html = HttpSend.GetSend(urlStr); //获取页面报文
                if (html != "false" && html != "")
                {
                    //如果对话框已经打开，就不要再提示了
                    if (isOpenUpdateDialog) return;
                    DialogResult result = MessageBox.Show("发现系统更新，要现在升级吗？", "系统更新", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    isOpenUpdateDialog = true;
                    if (result == DialogResult.Yes)
                    {
                        isOpenUpdateDialog = false;
                        string defaultPath = getSystemDefaultDirectory();
                        db.close();
                        Process p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.FileName = localUpdateExeName;
                        p.StartInfo.Arguments = "mainCallBack";
                        p.Start();
                        mainFrm.Dispose();
                        mainFrm.Close();
                    }
                    else if (result == DialogResult.No)
                    {
                        isOpenUpdateDialog = false;
                    }
                }
                else
                {
                    if (!isDefault)
                    {
                        MessageBox.Show("当前已是最新版本【" + version + "】");
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("检查升级失败！原因为：" + ex.Message); ;
            }
        }
        /// <summary>
        /// 获得用户注册信息
        /// </summary>
        /// <param name="db">数据库实例</param>
        /// <param name="mainFrm"></param>
        /// 先建立一个委托，用来invoke
        public delegate bool UpdateUserInfoUI(string html);
        public static UpdateUserInfoUI userInfoUI;
        public static bool getUserRegInfo(string ip, DbManager db)
        {

            Dictionary<string, string> sysConfig = db.getRow2Dic("sysConfig");
            string userInfoUrl = sysConfig["userInfoUrl"];
            try
            {
                string urlStr = userInfoUrl + @"?ip=" + ip + "&inode=" + getiNodeUsername();
                string html = HttpSend.GetSend(urlStr); //获取页面报文
                return userInfoUI(html);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("获取注册信息失败！原因为：" + ex.Message);
                return false;
            }
        }







        /// <summary>
        /// 删除制定目录下的文件和文件夹
        /// </summary>
        /// <param name="path">指定目录</param>
        public static void deletePath(string path)
        {

            DirectoryInfo dir = new DirectoryInfo(path);
            dir.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            File.SetAttributes(path, FileAttributes.Normal);

            if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(f))
                    {
                        File.Delete(f);
                    }
                    else
                    {
                        deletePath(f);
                        Directory.Delete(f);
                    }

                }
            }
        }


    }
}
