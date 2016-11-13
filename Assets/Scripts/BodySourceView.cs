using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Lib;
using UnityEngine;
using Kinect = Windows.Kinect;

namespace Assets.Scripts
{
    public class BodySourceView : MonoBehaviour
    {
        public Material BoneMaterial;
        public GameObject BodySourceManager = null;

        private readonly Dictionary<ulong, GameObject> _bodyGameObjects = new Dictionary<ulong, GameObject>();

        void Update()
        {
            Debug.Log(1/Time.smoothDeltaTime);
            var updatedBodyData = GameObjectUtils.GetUpdatedBodyData(BodySourceManager);

            if (updatedBodyData == null) return;

            var freshTrackedBodyData = updatedBodyData.Where(body => body.IsTracked).ToArray();
            var knownBodyIds = new List<ulong>(_bodyGameObjects.Keys);
            var freshBodyIds = freshTrackedBodyData.Select(trackedBody => trackedBody.TrackingId).ToList();
            RemoveDeadBodies(knownBodyIds, freshBodyIds);

            foreach (var body in freshTrackedBodyData)
            {
                if (!_bodyGameObjects.ContainsKey(body.TrackingId))
                {
                    _bodyGameObjects[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _bodyGameObjects[body.TrackingId]);
            }
        }

        private void RemoveDeadBodies(List<ulong> knownBodyIds, List<ulong> freshBodyIds)
        {
            foreach (var knownBodyId in knownBodyIds)
            {
                if (freshBodyIds.Contains(knownBodyId)) continue;

                Destroy(_bodyGameObjects[knownBodyId]);
                _bodyGameObjects.Remove(knownBodyId);
            }
        }

        private GameObject CreateBodyObject(ulong id)
        {
            var body = new GameObject("Body:" + id);
            body.transform.parent = gameObject.transform;

            foreach (Kinect.JointType joint in Enum.GetValues(typeof(Kinect.JointType)))
            {
                var jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                var lr = jointObj.AddComponent<LineRenderer>();
                lr.SetVertexCount(2);
                lr.material = BoneMaterial;
                lr.SetWidth(0.3f, 0.3f);

                jointObj.name = joint.ToString();
                jointObj.transform.parent = body.transform;
            }

            return body;
        }

        private void RefreshBodyObject(BodyViewModel body, GameObject bodyObject)
        {
            foreach (Kinect.JointType jointType in Enum.GetValues(typeof(Kinect.JointType)))
            {
                var sourceJoint = body.Joints[jointType];
                var targetJoint = _boneMap.ContainsKey(jointType) ? body.Joints[_boneMap[jointType]] : null;

                var jointObj = bodyObject.transform.FindChild(jointType.ToString());
                jointObj.localPosition = transform.LocalPositionFromColorSourcePosition(sourceJoint.Position);

                if (targetJoint != null)
                {
                    DrawBone(jointObj, sourceJoint, targetJoint);
                }
            }
        }

        private void DrawBone(Transform jointObj, JointViewModel sourceJoint, JointViewModel targetJoint)
        {
            var lineRenderer = jointObj.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, jointObj.localPosition);
            lineRenderer.SetPosition(1, transform.LocalPositionFromColorSourcePosition(targetJoint.Position));
            lineRenderer.SetColors(GetColorForState(sourceJoint.TrackingState),
                GetColorForState(targetJoint.TrackingState));
        }

        private static Color GetColorForState(Kinect.TrackingState state)
        {
            switch (state)
            {
                case Kinect.TrackingState.Tracked:
                    return Color.green;

                case Kinect.TrackingState.Inferred:
                    return Color.red;

                default:
                    return Color.black;
            }
        }

        private readonly Dictionary<Kinect.JointType, Kinect.JointType> _boneMap = new Dictionary
            <Kinect.JointType, Kinect.JointType>
            {
                {Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft},
                {Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft},
                {Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft},
                {Kinect.JointType.HipLeft, Kinect.JointType.SpineBase},
                {Kinect.JointType.FootRight, Kinect.JointType.AnkleRight},
                {Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight},
                {Kinect.JointType.KneeRight, Kinect.JointType.HipRight},
                {Kinect.JointType.HipRight, Kinect.JointType.SpineBase},
                {Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft},
                {Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft},
                {Kinect.JointType.HandLeft, Kinect.JointType.WristLeft},
                {Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft},
                {Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft},
                {Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder},
                {Kinect.JointType.HandTipRight, Kinect.JointType.HandRight},
                {Kinect.JointType.ThumbRight, Kinect.JointType.HandRight},
                {Kinect.JointType.HandRight, Kinect.JointType.WristRight},
                {Kinect.JointType.WristRight, Kinect.JointType.ElbowRight},
                {Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight},
                {Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder},
                {Kinect.JointType.SpineBase, Kinect.JointType.SpineMid},
                {Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder},
                {Kinect.JointType.SpineShoulder, Kinect.JointType.Neck},
                {Kinect.JointType.Neck, Kinect.JointType.Head},
            };
    }
}