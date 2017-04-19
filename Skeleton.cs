using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class Skeleton
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<JointType> _bodyConfidenceAcceptedJoints = new List<JointType>
        {
            JointType.Head,
            
            JointType.ShoulderLeft,
            JointType.ElbowLeft,
            JointType.HandLeft,
            JointType.HipLeft,
            JointType.KneeLeft,
            JointType.AnkleLeft,
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.HandRight,
            JointType.HipRight,
            JointType.KneeRight,
            JointType.AnkleRight,

            JointType.SpineMid,
            JointType.SpineShoulder
        };
        
        private Dictionary<JointType, TrackingState> BodyTrackingStateJoints = new Dictionary<JointType,TrackingState>();

        private Dictionary<string, int> _jointsConfidenceWeight;

        public Skeleton(Body body, Dictionary<string, int> jointsConfidenceWeight)
        {
            SetTrackingStateJoints(body);

            _jointsConfidenceWeight = jointsConfidenceWeight;

            // get the coordinate mapper
            CoordinateMapper coordinateMapper = KinectSensor.GetDefault().CoordinateMapper;

            Message = ""  
            + BodyPropertiesTypes.UID + MessageSeparators.SET + body.TrackingId

            + MessageSeparators.L2 + BodyPropertiesTypes.Confidence          + MessageSeparators.SET + BodyConfidence(body)
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftState       + MessageSeparators.SET + body.HandLeftState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftConfidence  + MessageSeparators.SET + body.HandLeftConfidence
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightState      + MessageSeparators.SET + body.HandRightState
            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightConfidence + MessageSeparators.SET + body.HandRightConfidence //;

            + MessageSeparators.L2 + HandScreenSpace.HandLeftPosition        + MessageSeparators.SET + ConvertCameraDepthPointToStringRpc(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position))
            + MessageSeparators.L2 + HandScreenSpace.HandRightPosition       + MessageSeparators.SET + ConvertCameraDepthPointToStringRpc(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position));

            //body.HandLeftState = HandState.Closed;
            //body.HandLeftState = HandState.Open;
            //body.HandLeftState = HandState.Unknown;

            foreach (JointType j in Enum.GetValues(typeof(JointType)))
            {
                Message += "" + MessageSeparators.L2 + j + MessageSeparators.SET + ConvertVectorToStringRpc(body.Joints[j].Position);
            }
            
            // AddImportantTrackingStateToMessage();
            // AddTrackingStateToMessage();
        }

        private void SetTrackingStateJoints(Body body)
        {
            foreach (var joint in Enum.GetValues(typeof(JointType)))
            {
                BodyTrackingStateJoints.Add((JointType)joint, body.Joints[(JointType)joint].TrackingState);
            }
        }

        public string Message { get; internal set; }

        private int BodyConfidence(Body body)
        {
            int confidence = 0;

            foreach (Joint j in body.Joints.Values)
            {
                if (_bodyConfidenceAcceptedJoints.Contains(j.JointType) && j.TrackingState == TrackingState.Tracked)
                {
                    
                    //if (j.JointType == Microsoft.Kinect.JointType.KneeLeft || j.JointType == Microsoft.Kinect.JointType.KneeRight || j.JointType == Microsoft.Kinect.JointType.Head || j.JointType == Microsoft.Kinect.JointType.SpineMid)
                    //    confidence += 3;

                    //    if (j.JointType == Microsoft.Kinect.JointType.HandLeft || j.JointType == Microsoft.Kinect.JointType.HandRight)
                    //        confidence += 3;
                    //    else
                    //        confidence += 1;

                    if (_jointsConfidenceWeight.ContainsKey(j.JointType.ToString()))
                    {
                        confidence += _jointsConfidenceWeight[j.JointType.ToString()];
                    }

                    else
                        confidence += 1;
                }
            }
            return confidence;
        }

        // internal static string convertVectorToStringRPC(CameraSpacePoint v)

        internal static string ConvertVectorToStringRpc(CameraSpacePoint v)
        {
            return "" + Math.Round(v.X, 3) + MessageSeparators.L3 + Math.Round(v.Y, 3) + MessageSeparators.L3 + Math.Round(v.Z, 3);
        }

        internal static string ConvertCameraDepthPointToStringRpc(DepthSpacePoint p)
        {
            return "" + Math.Round(p.X, 3) + MessageSeparators.L3 + Math.Round(p.Y, 3) + MessageSeparators.L3 + 0.0;
        }

       
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       

        private void AddTrackingStateToMessage()
        {
            Message += MessageSeparators.L2;
            foreach (var joint in Enum.GetValues(typeof(JointType)))
            {
                Message += "Tracking" + joint + MessageSeparators.SET +
                           BodyTrackingStateJoints[(JointType) joint] + MessageSeparators.L2;
            }
        }

        private void AddImportantTrackingStateToMessage()
        {
            Message += MessageSeparators.L2 +
                       "Tracking_" + JointType.KneeRight + MessageSeparators.SET + BodyTrackingStateJoints[JointType.KneeRight] + 
                       MessageSeparators.L2 +
                       "Tracking_" + JointType.KneeLeft  + MessageSeparators.SET + BodyTrackingStateJoints[JointType.KneeLeft];    
        }
    }
}
////////////////////////////////////////////////////////////////////////////////////
/*
    

     internal static string convertVectorToStringRPC(CameraSpacePoint v)
=======
    }
}
////////////////////////////////////////////////////////////////////////////////////
/*
      //private List<JointType> BodyConfidenceAcceptedJoints = new List<JointType>()
        //{
        //    JointType.Head,

        //    JointType.ShoulderLeft,
        //    JointType.ElbowLeft,
        //    JointType.HandLeft,
        //    JointType.HipLeft,
        //    JointType.KneeLeft,
        //    JointType.AnkleLeft,

        //    JointType.ShoulderRight,
        //    JointType.ElbowRight,
        //    JointType.HandRight,
        //    JointType.HipRight,
        //    JointType.KneeRight,
        //    JointType.AnkleRight,

        //    JointType.SpineMid,
        //    JointType.SpineShoulder


    ////////////////////////////////////////////////////////////////////////////////////
            //body.HandLeftState = HandState.Closed;
            //body.HandLeftState = HandState.Open;
            //body.HandLeftState = HandState.Unknown;

            //foreach (Microsoft.Kinect.JointType j in Enum.GetValues(typeof(Microsoft.Kinect.JointType)))
            //=======
            //            + MessageSeparators.L2 + BodyPropertiesTypes.Confidence.ToString() + MessageSeparators.SET + BodyConfidence(body)
            //            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftState.ToString() + MessageSeparators.SET + body.HandLeftState
            //            + MessageSeparators.L2 + BodyPropertiesTypes.HandLeftConfidence.ToString() + MessageSeparators.SET + body.HandLeftConfidence
            //            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightState.ToString() + MessageSeparators.SET + body.HandRightState
            //            + MessageSeparators.L2 + BodyPropertiesTypes.HandRightConfidence.ToString() + MessageSeparators.SET + body.HandRightConfidence
            //            + MessageSeparators.L2 + HandScreenSpace.HandLeftPosition.ToString() + MessageSeparators.SET + convertCameraDepthPointToStringRPC(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position))
            //            + MessageSeparators.L2 + HandScreenSpace.HandRightPosition.ToString() + MessageSeparators.SET + convertCameraDepthPointToStringRPC(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position));










>>>>>>> 4746fee32d607b7c8f8989137e92d34ffd88ae18

    ////////////////////////////////////////////////////////////////////////////////////
    //private  List<TrackingState> BodyTrackingStateJoints = new List<TrackingState>()
    //{
    //     Microsoft.Kinect.TrackingState.Inferred,
    //     Microsoft.Kinect.TrackingState.NotTracked,
    //     Microsoft.Kinect.TrackingState.Tracked,
    //};
    ////////////////////////////////////////////////////////////////////////////////////
    JointType.Head
    JointType.Neck
    JointType.SpineShoulder
    JointType.SpineMid
    JointType.SpineBase

    JointType.ShoulderRight
    JointType.ShoulderLeft

    JointType.HipRight
    JointType.HipLeft

    JointType.ElbowRight
    JointType.ElbowLeft

    JointType.WristRight
    JointType.WristLeft

    JointType.HandRight
    JointType.HandLeft

    JointType.HandTipRight
    JointType.HandTipLeft

    JointType.ThumbRight
    JointType.ThumbLeft

    JointType.KneeRight
    JointType.KneeLeft

    JointType.AnkleRight
    JointType.AnkleLeft

    JointType.FootRight          
    JointType.FootLeft 
    ////////////////////////////////////////////////////////////////////////////////////

    BodyTrackingStateJoints.Add(body.Joints[JointType.Neck].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.SpineShoulder].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.SpineBase].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.SpineMid].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.ShoulderRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.ShoulderLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.ElbowRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.ElbowLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.HipRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.HipLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.KneeRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.KneeLeft].TrackingState);
    ////////////////////////////////////////////////////////////////////////////////////
    BodyTrackingStateJoints.Add(body.Joints[JointType.ElbowRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.ElbowLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.WristRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.WristLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.HandRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.HandLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.HandTipRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.HandTipLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.AnkleRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.AnkleLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.FootRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.FootLeft].TrackingState);

    BodyTrackingStateJoints.Add(body.Joints[JointType.ThumbRight].TrackingState);
    BodyTrackingStateJoints.Add(body.Joints[JointType.ThumbLeft].TrackingState);
*/

//}
///////////////////////////////////////////////////////////////////////

/*



 =======
    private List<JointType> BodyConfidenceAcceptedJoints = new List<JointType>()
    {
        JointType.Head,

        JointType.ShoulderLeft,
        JointType.ElbowLeft,
        JointType.HandLeft,
        JointType.HipLeft,
        JointType.KneeLeft,
        JointType.AnkleLeft,

        JointType.ShoulderRight,
        JointType.ElbowRight,
        JointType.HandRight,
        JointType.HipRight,
        JointType.KneeRight,
        JointType.AnkleRight,

        JointType.SpineMid,
        JointType.SpineShoulder
>>>>>>> 40e375685f06e881e0c3bb03939e019d61178a5a


 */
/*
 
     
            //+ MessageSeparators.L2 + BodyPropertiesTypes.Confidence.ToString() + MessageSeparators.SET + BodyConfidence(body)
            //+ MessageSeparators.L2 + BodyPropertiesTypes.HandLeftState.ToString() + MessageSeparators.SET + body.HandLeftState
            //+ MessageSeparators.L2 + BodyPropertiesTypes.HandLeftConfidence.ToString() + MessageSeparators.SET + body.HandLeftConfidence
            //+ MessageSeparators.L2 + BodyPropertiesTypes.HandRightState.ToString() + MessageSeparators.SET + body.HandRightState
            //+ MessageSeparators.L2 + BodyPropertiesTypes.HandRightConfidence.ToString() + MessageSeparators.SET + body.HandRightConfidence
        
            //foreach (JointType j in Enum.GetValues(typeof(JointType)))

//=======

//            + MessageSeparators.L2 + HandScreenSpace.HandLeftPosition.ToString()        + MessageSeparators.SET + ConvertCameraDepthPointToStringRpc(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position ))
//            + MessageSeparators.L2 + HandScreenSpace.HandRightPosition.ToString()       + MessageSeparators.SET + ConvertCameraDepthPointToStringRpc(coordinateMapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position));

//            foreach (JointType j in Enum.GetValues(typeof(JointType)))
     
     */
