using UnityEngine;
using System.Collections.Generic;
using Assets.KinectView.Scripts;
using Assets.KinectView.Util;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        var bodyData = _BodyManager.BodyViewModels;
        if (bodyData == null)
        {
            return;
        }
        
        var trackedIds = new List<ulong>();
        foreach(var body in bodyData)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        var knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(var trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in bodyData)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        var body = new GameObject("Body:" + id);
        body.transform.parent = gameObject.transform;

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            var jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void RefreshBodyObject(BodyViewModel body, GameObject bodyObject)
    {
        for (var joint = Kinect.JointType.SpineBase; joint <= Kinect.JointType.ThumbRight; joint++)
        {
            var sourceJoint = body.Joints[joint];
            JointViewModel targetJoint = null;
            
            if(_BoneMap.ContainsKey(joint))
            {
                targetJoint = body.Joints[_BoneMap[joint]];
            }
            
            var jointObj = bodyObject.transform.FindChild(joint.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            if(targetJoint != null)
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


    private static float MapToUiCoordinates(float numberToMap, float minInput, float maxInput, float minOutput, float maxOutput)
    {
    return (numberToMap - minInput) * (maxOutput - minOutput) / (maxInput - minInput) + minOutput;
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
}
