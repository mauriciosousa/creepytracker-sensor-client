using System;
using System.IO;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class NetworkConfigFile
    {
        private string _port = "33333";

        public NetworkConfigFile(string filename)
        {
            if (File.Exists(filename))
            {
                foreach (string line in File.ReadAllLines(filename))
                {
                    string [] s = line.Split('=');

                    if (s[0] == "udp.port") _port = s[1];
                }
            }
        }

        public string Port { get { return _port; } internal set { _port = value; } }
    }
}