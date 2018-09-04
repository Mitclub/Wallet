﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIT.Eth.Common
{
    public class ClsCommon
    {
        static ReaderWriterLockSlim m_LogWriteLock = new ReaderWriterLockSlim();

        /// <summary>
        /// write logs
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="logtype"></param>
        static public void WriteLog(string strMessage, string logtype)
        {
            m_LogWriteLock.EnterWriteLock();

            StreamWriter sw = new StreamWriter(Application.StartupPath + "\\" + DateTime.Today.ToString("yyyyMM") + ".log", true);
            //sw.WriteLine("============================================================================");
            sw.WriteLine(string.Format("[{0}][{1}]:{2}", logtype, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strMessage));
            sw.Close();

            m_LogWriteLock.ExitWriteLock();
        }
        /// <summary>
        /// Open URL with IE
        /// </summary>
        /// <param name="strURL">the url need to be opened</param>
        static public void OpenURLwithIE(string strURL)
        {
            System.Diagnostics.Process.Start("IEXPLORE.EXE", strURL);
        }

        public static class ClintInfor
        {
            public static readonly string M_UserName = Environment.UserDomainName + "." + Environment.UserName;
            public static readonly string M_MachineName = Environment.MachineName;
            public static readonly string M_Domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;

            /// <summary>
            /// IP
            /// </summary>
            private static string strIP4Address = string.Empty;
            public static string M_IP
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(strIP4Address))
                    {
                        getIp1();

                    }

                    return strIP4Address;
                }

            }

            private static void getIp1()
            {
                var ips = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress IPA in ips)
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        strIP4Address = IPA.ToString();

                    }
                }
            }

            private static void getIp2()
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection nics = mc.GetInstances();
                foreach (ManagementObject nic in nics)
                {
                    if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                    {
                        string[] dnses = (string[])nic["DNSServerSearchOrder"];  //DNS
                        foreach (var dns in dnses)
                        {
                            if (dns == "0.0.0.0")
                            {
                                strIP4Address = (nic["IPAddress"] as String[])[0];
                                break;
                            }
                        }
                        //string mac = nic["MacAddress"].ToString();//Mac地址
                        //string ip = (nic["IPAddress"] as String[])[0];//IP地址
                        //string ipsubnet = (nic["IPSubnet"] as String[])[0];//子网掩码
                        //string ipgateway = (nic["DefaultIPGateway"] as String[])[0];//默认网关
                    }
                }
            }

        }

        /// <summary>
        /// Mail window pop
        /// </summary>
        /// <param name="strTo">Email Address</param>
        /// <param name="strCC">CC</param>
        /// <param name="strBCC">BCC</param>
        /// <param name="strSubject">subject of mail</param>
        /// <param name="strBody">body of mail</param>
        static public void MailTo(string strTo, string strSubject = " ", string strBody = " ", string strCC = " ", string strBCC = " ")
        {
            // Body=Here is the shipping manifest %0Asecond .%0A%0A third.
            System.Diagnostics.Process.Start(string.Format("mailto:{0}?cc={1}&bcc={2}&Subject={3}&Body={4}", strTo, strCC, strBCC, strSubject, strBody));
        }

    }
}
