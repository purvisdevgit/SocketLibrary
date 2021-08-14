using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLibrary
{
    public class SokClient
    {
        private Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IAsyncResult asyncResult = null;
        Thread th = null;
        Timer timer = null;
        /// <summary>
        /// 连接Socket服务端
        /// </summary>
        /// <param name="sIp">IP地址</param>
        /// <param name="sPort">端口号</param>
        /// <returns></returns>
        public bool Connect(string sIp, string sPort)
        {
            try
            {
                if (!IsConnected(sk))//验证是否可以连接
                {
                    return true;
                }
                IPAddress IpAddress = IPAddress.Parse(sIp);
                IPEndPoint IpEndPoint = new IPEndPoint(IpAddress, Convert.ToInt32(sPort));
                asyncResult = sk.BeginConnect(IpEndPoint, null, null);
                asyncResult.AsyncWaitHandle.WaitOne(1500, true);  //等待1.5秒
                if (!asyncResult.IsCompleted)
                {
                    sk.Close();
                    sk.Dispose();
                    return false;
                }
                timer = new Timer(new TimerCallback(Heart), null, Timeout.Infinite, 3000);//每3秒发送一次心跳💓
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="strMsg">消息内容</param>
        /// <param name="type">消息类型：1.HEX 2.ASCII</param>
        /// <returns></returns>
        public int SendMessage(string strMsg, int type)
        {
            try
            {
                byte[] buf = null;
                if (type == 1) //Hex
                {
                    buf = HexStringToArray(strMsg, 0, strMsg.Length);
                }
                else
                {
                    buf = Encoding.UTF8.GetBytes(strMsg);
                }
                //sk.SendTimeout = 1500;
                return sk.Send(buf);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="type">消息类型：1.HEX 2.ASCII</param>
        /// <returns></returns>
        public string ReciveMessage(int type)
        {
            try
            {
                byte[] buf = new byte[256];
                string sRecive = "";
                //实际接收到的字节数
                //sk.ReceiveTimeout = 1500;
                int r = sk.Receive(buf);
                if (r == 0)
                {
                    return sRecive;
                }
                if (type == 1)
                {
                    sRecive = ArrayToHexString(buf, 0, r);
                }
                else
                {
                    sRecive = Encoding.UTF8.GetString(buf, 0, r);
                }
                return sRecive;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 断开Socket连接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            try
            {
                timer.Dispose();
                sk.Close();
                sk.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 心跳用来保证连接不断掉
        /// </summary>
        /// <param name="val"></param>
        public void Heart(object val)
        {
            try
            {
                if (IsConnected(sk))
                {
                    sk.Close();//关闭socket
                    sk.Dispose();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // 检查一个Socket是否可连接
        public bool IsConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return false;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                if (e.NativeErrorCode.Equals(10035))
                    return false;
                else
                    return true;
            }
            finally
            {
                client.Blocking = blockingState;    // 恢复状态
            }
        }

        /// <summary>
        /// 数组转十六进制字符串
        /// </summary>
        /// <param name="value">要转换的数组</param>
        /// <param name="startIndex">数组起始位置</param>
        /// <param name="length">要转换的长度</param>
        /// <returns></returns>
        public static string ArrayToHexString(byte[] value, int startIndex, int length)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    int b = value[startIndex + i];
                    sb.AppendFormat("{0:X2} ", b);
                }
                return sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 十进制字符串转字节数组
        /// </summary>
        /// <param name="value">要转换的数组</param>
        /// <param name="startIndex">数组起始位置</param>
        /// <param name="length">要转换的长度</param>
        /// <returns></returns>
        public static byte[] HexStringToArray(string value, int startIndex, int length)
        {
            try
            {
                List<byte> list = new List<byte>();
                value = value.Replace(" ", "");
                if ((value.Length % 2) != 0)
                    value += " ";
                byte[] returnBytes = new byte[value.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    list.Add(Convert.ToByte(value.Substring(i * 2, 2).Replace(" ", ""), 16));
                return list.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
