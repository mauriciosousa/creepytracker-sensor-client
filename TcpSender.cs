using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class TcpSender
    {
        private TcpClient _client;

        private Stream _stream;

        private ASCIIEncoding _encoder;

        private byte[] sendHeaderBuffer;

        private int _port;

        public string _address;

        private bool _connected;

        public bool Connected
        {
            get
            {
                return _connected; 
                
            }
        }

        public TcpSender()
        {
            _connected = false;
            sendHeaderBuffer = new byte[8];
        }

        public void Connect(string address, int port)
        {
            _address = address;
            _port = port;

            _encoder = new ASCIIEncoding();

            _client = new TcpClient();
            try
            {
                _client.Connect(address, port);

                _stream = _client.GetStream();

                _connected = true;

                this.write("k/" + Environment.MachineName + "/");
            }
            catch (Exception e)
            {
                _connected = false;
                const string unableToConnect = "Unable to connect";
                Console.WriteLine(unableToConnect);
            }
        }

        public void SendCloud(byte[] frame, uint messageCount)
        {
            byte[] id    = BitConverter.GetBytes(messageCount);
            byte[] count = BitConverter.GetBytes(frame.Length);

            Array.Copy(  id,  0, sendHeaderBuffer, 0, 4);
            Array.Copy(count, 0, sendHeaderBuffer, 4, 4);

            write(sendHeaderBuffer);
            write(frame);
        }

        public void write(string line)
        {
            byte[] ba = _encoder.GetBytes(line);
            this.write(ba);
        }

        public void write(byte[] frame)
        {
            if (_connected)
            {
                try
                {
                    _stream.Write(frame, 0, frame.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Close();
                    _connected = false;
                }
            }
        }

        public void Close()
        {
            _client.Close();
        }
    }
}
