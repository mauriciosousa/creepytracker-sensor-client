using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class CloudMessage
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public IPAddress ReplyIpAddress;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public int Port;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public int Mode;

        public CloudMessage(string m)
        {
            string[] msg = m.Split(MessageSeparators.L1);

            ReplyIpAddress = IPAddress.Parse(msg[0]);
            Mode = int.Parse(msg[1]); 
            Port = int.Parse(msg[2]);
        }

        // ReSharper disable once UnusedMember.Global
        public static string CreateMessage(string cloudInfo, uint id)
        {
            return "CloudMessage" + MessageSeparators.L0 + Environment.MachineName + MessageSeparators.L1 + id+ MessageSeparators.L1 +cloudInfo; 
        }
    }
}
