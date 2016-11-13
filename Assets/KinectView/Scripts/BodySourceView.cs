﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.KinectView.Util;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Kinect = Windows.Kinect;

namespace Assets.KinectView.Scripts
{
    public class BodySourceView : MonoBehaviour
    {
        public Material BoneMaterial;
        public GameObject BodySourceManager = null;

        private readonly Dictionary<ulong, GameObject> _bodyGameObjects = new Dictionary<ulong, GameObject>();

        void Update()
        {
            var updatedBodyData = GetUpdatedBodyData(BodySourceManager);

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
                lr.SetWidth(0.05f, 0.05f);

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
                jointObj.localPosition = GetVector3FromJoint(sourceJoint);

                if (targetJoint != null)
                {
                    DrawBone(jointObj, sourceJoint, targetJoint);
                }
            }
        }

        private Vector3 GetVector3FromJoint(JointViewModel joint)
        {
            var bounds = gameObject.GetComponent<Collider>().bounds;
            var xCoord = MapToUiCoordinates(joint.Position.x, 0, 1920, bounds.min.x, bounds.max.x);
            var yCoord = MapToUiCoordinates(joint.Position.y, 0, 1080, bounds.min.y, bounds.max.y);
            return new Vector3(-xCoord, -yCoord, 0);
        }

        private void DrawBone(Transform jointObj, JointViewModel sourceJoint, JointViewModel targetJoint)
        {
            var lineRenderer = jointObj.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, jointObj.localPosition);
            lineRenderer.SetPosition(1, GetVector3FromJoint(targetJoint));
            lineRenderer.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.TrackingState));
        }

        private static BodyViewModel[] GetUpdatedBodyData(GameObject bodySourceManager)
        {
            Debug.Assert(bodySourceManager != null, "BodySourceManager is null");

            var bodySourceManagerScript = bodySourceManager.GetComponent<BodySourceManager>();

            Debug.Assert(bodySourceManagerScript != null, "BodySourceManager does not have the required script");

            return bodySourceManagerScript.BodyViewModels;
        }

        private static float MapToUiCoordinates(float numberToMap, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            var output = (numberToMap - minInput)*(maxOutput - minOutput)/(maxInput - minInput) + minOutput;
            return LimitInclusive(output, minOutput, maxOutput);
        }

        public static float LimitInclusive(float value, float min, float max)
        {
            return Math.Min(max, Math.Max(value, min));
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

        private readonly Dictionary<Kinect.JointType, Kinect.JointType> _boneMap = new Dictionary<Kinect.JointType, Kinect.JointType>
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