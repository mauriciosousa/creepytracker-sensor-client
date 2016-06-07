
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class UdpListener
    {

        private UdpClient _udpClient = null;
        private IPEndPoint _anyIP;
        private KinectSensor _kinectSensor;
        private int _port;
        uint messageCount;
        public List<CloudMessage> PendingRequests;
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
        public void udpRestart()
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

        public void processRequests(string cloudInfo)
        {
            
            List<CloudMessage> todelete = new List<CloudMessage>();
            foreach (CloudMessage cm in PendingRequests)
            {
                string[] messageBlocks = cloudInfo.Split(MessageSeparators.L2);
                string msg = "";
                for (int i = 0; i < messageBlocks.Length;i++)
                {
                     if(msg.Length + messageBlocks[i].Length > 1024 || i == messageBlocks.Length - 1){
                         msg = CloudMessage.createMessage(msg,messageCount);
                         byte[] data = Encoding.UTF8.GetBytes(msg);
             
                        IPEndPoint ep = new IPEndPoint(cm.replyIPAddress,cm.port);
                        try{
                            _udpClient.Send(data, data.Length, ep);
                            todelete.Add(cm);
                            msg = "";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error sending data to " + cm.replyIPAddress.ToString() + " " + e.Message);
                        }
                    }
                    msg += messageBlocks[i]+MessageSeparators.L2;
                }
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