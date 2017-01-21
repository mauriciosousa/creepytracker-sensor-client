
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Collections; // Tomas: To include ArrayList
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class UdpListener
    {
        public List<CloudMessage> PendingRequests;

        private UdpClient _udpClient = null;
        private IPEndPoint _anyIP;
        private KinectSensor _kinectSensor;

        private int _port;

        uint messageCount;

        int limit; // TMA: To keep track of the number of bytes sent.

        byte[] final_bytes; // TMA: To point to the bytes that will be send.


        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        
        public UdpListener(int port)
        {
            _port = port;
            PendingRequests = new List<CloudMessage>();
        }

        public void UdpRestart()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
            }

            PendingRequests = new List<CloudMessage>();

            _anyIP = new IPEndPoint(IPAddress.Any, _port);

            _udpClient = new UdpClient(_anyIP);

            _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);

            Console.WriteLine("[UDPListener] Receiving in port: " + _port);
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            Console.WriteLine("[UDPListener] Received request: " + _port);
            try { 
                Byte[] receiveBytes = _udpClient.EndReceive(ar, ref _anyIP);
                string request = Encoding.ASCII.GetString(receiveBytes);
               
                string[] msg = request.Split(MessageSeparators.L0);
                if (msg[0] == "CloudMessage")
                {
                    PendingRequests.Add(new CloudMessage(msg[1]));
                }
                _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on callback" + e.Message);
            }
            
        }

        public void ProcessRequests(ArrayList byte_list)
        {
            List<CloudMessage> todelete = new List<CloudMessage>();
            for (int i = 0; i < PendingRequests.Count; i++)
            {
                CloudMessage cm = PendingRequests.ElementAt(i);
                if (cm.Mode == 2)
                {
                    foreach (CloudMessage cm2 in PendingRequests)
                    {
                        if (cm.ReplyIpAddress.ToString() == cm2.ReplyIpAddress.ToString() &&
                            cm.Port == cm2.Port)
                            todelete.Add(cm2);
                    }
                    continue;
                }
                if (todelete.Contains(cm))
                {
                    continue;
                }
                // TMA: Get the bytes from the ArrayList
                byte[] points_bytes = byte_list.OfType<byte>().ToArray();
                // This is the heading for every package.
                string msg = "CloudMessage" + MessageSeparators.L0 + Environment.MachineName + MessageSeparators.L1 + messageCount + MessageSeparators.L1; // String to tag the sensor
                // Get the heading bytes.
                int remainder =4- (msg.Length % 4);
                while (remainder-- > 0) msg = 'C' + msg; 

                byte[] msg_bytes = Encoding.ASCII.GetBytes(msg); // Convert to bytes

                IPEndPoint ep = new IPEndPoint(cm.ReplyIpAddress, cm.Port);

                for (limit = 0; limit < points_bytes.Length; limit += 8000) // Each packet has 500 points (16 * 500 = 8000 bytes)
                {
                    if (limit + 8000 > points_bytes.Length) // If there are less points than 500
                    {
                        final_bytes = new byte[msg_bytes.Length + points_bytes.Length - limit];
                        Array.Copy(msg_bytes, 0, final_bytes, 0, msg_bytes.Length);
                        Array.Copy(points_bytes, limit, final_bytes, msg_bytes.Length, points_bytes.Length - limit);
                    }
                    else // If there are more or 500 points to send
                    {
                        final_bytes = new byte[msg_bytes.Length + 8000];
                        Array.Copy(msg_bytes, 0, final_bytes, 0, msg_bytes.Length);
                        Array.Copy(points_bytes, limit, final_bytes, msg_bytes.Length, 8000);
                    }
                    
                   //IPEndPoint ep = new IPEndPoint(cm.ReplyIpAddress, cm.Port);

                    try
                    {
                       // System.Threading.Thread.Sleep(1);
                        _udpClient.Send(final_bytes, final_bytes.Length, ep); // Send the bytes
                        if (cm.Mode == 0)
                        {
                            todelete.Add(cm);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error sending data to " + cm.ReplyIpAddress.ToString() + " " + e.Message);
                    }
                }
                msg_bytes = Encoding.ASCII.GetBytes(msg); // Convert to bytes
             //   System.Threading.Thread.Sleep(1);
                _udpClient.Send(msg_bytes, msg_bytes.Length, ep); // Send the bytes
            }
            foreach (CloudMessage cm in todelete)
            {
                PendingRequests.Remove(cm);
            }
            messageCount++;
        }

        public void OnApplicationQuit()
        {
            if (_udpClient != null) _udpClient.Close();
        }

    }
}