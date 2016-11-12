using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.KinectView.Util;

namespace Assets.KinectView.Scripts
{
    public class BodyViewModel
    {
        public bool IsTracked { get; private set; }
        public ulong TrackingId { get; private set; }
        public Dictionary<JointType, JointViewModel> Joints { get; private set; }

        public BodyViewModel(bool isTracked, ulong trackingId, Dictionary<JointType, JointViewModel> joints)
        {
            IsTracked = isTracked;
            TrackingId = trackingId;
            Joints = joints;
        }
    }
}