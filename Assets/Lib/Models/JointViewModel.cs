using Windows.Kinect;
using UnityEngine;

namespace Assets.Lib
{
    public class JointViewModel
    {
        public TrackingState TrackingState { get; private set; }
        public Vector2 Position { get; private set; }
        public JointType JointJointType { get; private set; }

        public JointViewModel(JointType jointJointType, TrackingState trackingState, Vector2 position)
        {
            TrackingState = trackingState;
            Position = position;
            JointJointType = jointJointType;
        }
    }
}