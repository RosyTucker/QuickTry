using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.Lib;
using Assets.Lib.Models;
using UnityEngine;

namespace Assets.Scripts
{
    public class BodySourceManager : MonoBehaviour
    {
        private Kinect _kinect;
        public BodyViewModel[] BodyViewModels { get; private set; }

        void Start()
        {
            _kinect = new Kinect();
        }

        void Update()
        {
            var updatedBodyData = GetUpdatedBodyData();
            if (updatedBodyData == null) return;
            BodyViewModels =  updatedBodyData
                .Select(body => new BodyViewModel(body.IsTracked, body.TrackingId, GetJoints(body))).ToArray();
        }

        void OnApplicationQuit()
        {
            _kinect.Dispose();
            _kinect = null;
        }

        private Body[] GetUpdatedBodyData()
        {
            if (_kinect.Reader == null) return null;

            var multiSourceFrame = _kinect.Reader.AcquireLatestFrame();

            if (multiSourceFrame == null) return null;

            using (var frame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (frame == null) return null;

                var bodyData = new Body[_kinect.Sensor.BodyFrameSource.BodyCount];

                frame.GetAndRefreshBodyData(bodyData);
                return bodyData;
            }
        }

        private Dictionary<JointType, JointViewModel> GetJoints(Body body)
        {
            var joints = new Dictionary<JointType, JointViewModel>();
            foreach (var joint in body.Joints.Values)
            {
                var jointPosition = joint.Position;
                var colorPoint = _kinect.Sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
                var orientation = body.JointOrientations[joint.JointType];
                joints[joint.JointType] = new JointViewModel(
                    joint.JointType,
                    joint.TrackingState,
                    new Vector2(colorPoint.X, colorPoint.Y), orientation.Orientation);
            }
            return joints;
        }
    }
}