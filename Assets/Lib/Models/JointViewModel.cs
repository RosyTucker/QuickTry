using Windows.Kinect;
using UnityEngine;
using Vector4 = Windows.Kinect.Vector4;

namespace Assets.Lib.Models
{
    public class JointViewModel
    {
        public TrackingState TrackingState { get; private set; }
        public Vector2 Position { get; private set; }
        public JointType JointJointType { get; private set; }
        public Vector4 Orientation { get; private set; }

        public JointViewModel(JointType jointJointType, TrackingState trackingState, Vector2 position, Vector4 orientation)
        {
            TrackingState = trackingState;
            Position = position;
            JointJointType = jointJointType;
            Orientation = orientation;
        }

    }
}