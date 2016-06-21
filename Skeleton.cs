using System;
using System.Collections.Generic;

namespace Microsoft.Samples.Kinect.BodyBasics
{

    internal class Skeleton
    {

        private List<Microsoft.Kinect.JointType> BodyConfidenceAcceptedJoints = new List<Microsoft.Kinect.JointType>()
        {
            Microsoft.Kinect.JointType.Head,
            Microsoft.Kinect.JointType.ShoulderLeft,
            Microsoft.Kinect.JointType.ElbowLeft,
            Microsoft.Kinect.JointType.HandLeft,
            Microsoft.Kinect.JointType.HipLeft,
            Microsoft.Kinect.JointType.KneeLeft,
            Microsoft.Kinect.JointType.AnkleLeft,
            Microsoft.Kinect.JointType.ShoulderRight,
            Microsoft.Kinect.JointType.ElbowRight,
            Microsoft.Kinect.JointType.HandRight,
            Microsoft.Kinect.JointType.HipRight,
            Microsoft.Kinect.JointType.KneeRight,
            Microsoft.Kinect.JointType.AnkleRight
        };

        private Dictionary<string, int> JointsConfidenceWeight;

        public Skeleton(Microsoft.Kinect.Body body, Dictionary<string, int> jointsConfidenceWeight)
        {
            this.JointsConfidenceWeight = jointsConfidenceWeight;

            Message = ""
            + BodyPropertiesTypes.UID.ToString() + MessageSeparators.SET + body.TrackingId
            + MessageSeparators.L2 + BodyPropertiesTypes.Confidence.ToString() + MessageSeparators.SET + BodyConfidence(body)
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftState.ToString() + MessageSeparators.SET + body.HandLeftState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftConfidence.ToString() + MessageSeparators.SET + body.HandLeftConfidence
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightState.ToString() + MessageSeparators.SET + body.HandRightState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightConfidence.ToString() + MessageSeparators.SET + body.HandRightConfidence;

            foreach (Microsoft.Kinect.JointType j in Enum.GetValues(typeof(Microsoft.Kinect.JointType)))
            {
                Message += "" + MessageSeparators.L2 + j.ToString() + MessageSeparators.SET + ConvertVectorToStringRpc(body.Joints[j].Position);
            }
        }

        public string Message { get; internal set; }   

        private int BodyConfidence(Microsoft.Kinect.Body body)
        {
            int confidence = 0;

            foreach (Microsoft.Kinect.Joint j in body.Joints.Values)
            {
                if (BodyConfidenceAcceptedJoints.Contains(j.JointType) && j.TrackingState == Microsoft.Kinect.TrackingState.Tracked)
                {

                    //if (j.JointType == Microsoft.Kinect.JointType.KneeLeft || j.JointType == Microsoft.Kinect.JointType.KneeRight || j.JointType == Microsoft.Kinect.JointType.Head || j.JointType == Microsoft.Kinect.JointType.SpineMid)
                    //    confidence += 3;

                    //    if (j.JointType == Microsoft.Kinect.JointType.HandLeft || j.JointType == Microsoft.Kinect.JointType.HandRight)
                    //        confidence += 3;
                    //    else
                    //        confidence += 1;

                    if (JointsConfidenceWeight.ContainsKey(j.JointType.ToString()))
                    {
                        confidence += JointsConfidenceWeight[j.JointType.ToString()];
                    }

                    else
                        confidence += 1;
                }
            }
            return confidence;
        }

        internal static string ConvertVectorToStringRpc(Microsoft.Kinect.CameraSpacePoint v)
        {
            return "" + Math.Round(v.X, 3) + MessageSeparators.L3 + Math.Round(v.Y, 3) + MessageSeparators.L3 + Math.Round(v.Z, 3);
        }
    }
}