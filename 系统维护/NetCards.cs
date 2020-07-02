using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace 系统维护
{
    class NetCards
    {
        private Dictionary<string, NetCard> netCardDic = new Dictionary<string, NetCard>();
        public NetCards()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID  
                    // 如果前面有 PCI 就是本机的真实网卡 
                    string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                    {
                        IPInterfaceProperties ipProperties = adapter.GetIPProperties();
                        UnicastIPAddressInformationCollection curIPCollection = ipProperties.UnicastAddresses;
                        UnicastIPAddressInformation curIP;
                        //获取IP
                        if (curIPCollection.Count > 1)
                        {
                            curIP = ipProperties.UnicastAddresses[curIPCollection.Count - 1];
                        }
                        else 
                        {
                            curIP = ipProperties.UnicastAddresses[0];
                        }
                        string name = adapter.Name;//获得连接名称
                        string ip = curIP.Address.ToString();// 添加IP地址
                        string mask = curIP.IPv4Mask == null ? "" : curIP.IPv4Mask.ToString();// 添加子网掩码
                        string gateway = ipProperties.GatewayAddresses[0].Address.ToString();//添加默认网关
                        //获取dns
                        IPAddressCollection dnsCollection = ipProperties.DnsAddresses;
                        string dns1 = "";
                        string dns2 = "";
                        if (dnsCollection.Count ==1)
                        {
                            dns1 = dnsCollection[0].ToString();
                          
                        }
                        else  if (dnsCollection.Count ==2)
                        {
                            dns1 = dnsCollection[0].ToString();
                            dns2 = dnsCollection[1].ToString();
                        }
                        NetCard netCard = new NetCard(name, ip, mask, gateway, dns1, dns2, adapter);
                        netCardDic.Add(name, netCard);
                    }
  
                }
            }
        }
        /// <summary>
        /// 获得所有网卡信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, NetCard> getAllNetCard()
        {
            return this.netCardDic;
        }

        //public bool ipTest() 
        //{
        //    foreach (KeyValuePair<string, NetCard> kv in netCardDic)
        //    {
        //        NetCard netcard = kv.Value;
        //        if (isEnableIp) continue;
        //        if (netcard.Ip.IndexOf("10.") == 0)
        //        {
        //            isEnableIp = true;
        //            netCardName = kv.Key;
        //        }
        //    }
        //    return isEnableIp;
        //}

    }

}
