﻿using System.Collections;
using System.Collections.Generic;
using Kinect = Microsoft.Kinect;
using System;
using System.Linq;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public static class MessageSeparators
    {
        public const char L0 = '$';
        public const char L1 = '#'; // top level separator -> bodies
        public const char L2 = '/'; // -> body attributes
        public const char L3 = ':'; // -> 3D values
        public const char L4 = '?'; // -> Extra
        public const char SET = '=';
    }

    public enum BodyPropertiesTypes
    {
        UID,
        HandLeftState,
        HandLeftConfidence,
        HandRightState,
        HandRightConfidence,
        Confidence
    }
    
    public class BodiesMessage
    {
        private object _bodies;

        public BodiesMessage(Microsoft.Kinect.Body[] listOfBodies, Dictionary<string, int> jointsConfidenceWeight)
        {
            Message = "BodiesMessage" + MessageSeparators.L0 + Environment.MachineName;
            if (listOfBodies.Length == 0) Message += "" + MessageSeparators.L1 + "None";
            else
            {
                foreach (var b in listOfBodies)
                {
                    
                    var newBody = new Skeleton(b, jointsConfidenceWeight);
                    Message += "" + MessageSeparators.L1 + newBody.Message;
                }
            }
        }

        public string Message { get; private set; }
    }
}