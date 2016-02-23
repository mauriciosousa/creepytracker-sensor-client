using System;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class Skeleton
    {

        public Skeleton(Microsoft.Kinect.Body body)
        {
            Message = ""
            + BodyPropertiesTypes.UID.ToString() + MessageSeparators.SET + body.TrackingId
            + MessageSeparators.L2 + BodyPropertiesTypes.Confidence.ToString() + MessageSeparators.SET + BodyConfidence(body)
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftState.ToString() + MessageSeparators.SET + body.HandLeftState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftConfidence.ToString() + MessageSeparators.SET + body.HandLeftConfidence
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightState.ToString() + MessageSeparators.SET + body.HandRightState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightConfidence.ToString() + MessageSeparators.SET + body.HandRightConfidence;

            foreach (Microsoft.Kinect.JointType j in Enum.GetValues(typeof(Microsoft.Kinect.JointType)))
            {
                Message += "" + MessageSeparators.L2 + j.ToString() + MessageSeparators.SET + convertVectorToStringRPC(body.Joints[j].Position);
            }
        }

        public string Message { get; internal set; }   

        private int BodyConfidence(Microsoft.Kinect.Body body)
        {
            int confidence = 0;

            foreach (Microsoft.Kinect.Joint j in body.Joints.Values)
            {
                if (j.TrackingState == Microsoft.Kinect.TrackingState.Tracked)
                    confidence += 1;
            }

            return confidence;
        }

        internal static string convertVectorToStringRPC(Microsoft.Kinect.CameraSpacePoint v)
        {
            return "" + Math.Round(v.X, 3) + MessageSeparators.L3 + Math.Round(v.Y, 3) + MessageSeparators.L3 + Math.Round(v.Z, 3);
        }
    }
}