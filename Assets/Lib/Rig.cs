using System.Collections.Generic;
using Windows.Kinect;
using Assets.Lib.Models;
using UnityEngine;

namespace Assets.Lib
{
    public class Rig {

        public static readonly Dictionary<JointType, string> JointMapping = new Dictionary<JointType, string>
            {
                {JointType.Head, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/Neck/Neck1/Head"},
                {JointType.Neck, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/Neck"},

                {JointType.SpineShoulder, "CMU compliant skeleton/Hips/LowerBack/Spine"},
                {JointType.SpineMid, "CMU compliant skeleton/Hips/LowerBack/Spine"},
                {JointType.SpineBase, "CMU compliant skeleton/Hips/LowerBack/Spine"},

                {JointType.ShoulderLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder"},
                {JointType.ElbowLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm"},
                {JointType.WristLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm"},
                {JointType.HandLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand"},
                {JointType.HandTipLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftFingerBase"},
                {JointType.ThumbLeft, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LThumb"},


                {JointType.ShoulderRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder"},
                {JointType.ElbowRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm"},
                {JointType.WristRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm"},
                {JointType.HandRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand"},
                {JointType.HandTipRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand/RightFingerBase"},
                {JointType.ThumbRight, "CMU compliant skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand/RThumb"},


                {JointType.HipLeft,"CMU compliant skeleton/Hips/LHipJoint" },
                {JointType.KneeLeft, "CMU compliant skeleton/Hips/LHipJoint/LeftUpLeg" },
                {JointType.AnkleLeft, "CMU compliant skeleton/Hips/LHipJoint/LeftUpLeg/LeftLeg" },
                {JointType.FootLeft, "CMU compliant skeleton/Hips/LHipJoint/LeftUpLeg/LeftLeg/LeftFoot" },

                {JointType.HipRight,"CMU compliant skeleton/Hips/RHipJoint" },
                {JointType.KneeRight,"CMU compliant skeleton/Hips/RHipJoint/RightUpLeg" },
                {JointType.AnkleRight,"CMU compliant skeleton/Hips/RHipJoint/RightUpLeg/RightLeg" },
                {JointType.FootRight,"CMU compliant skeleton/Hips/RHipJoint/RightUpLeg/RightLeg/RightFoot" }
            };

        public static Vector3 CalculateScale(Dictionary<JointType, JointViewModel> joints, float baseSize)
        {
            var top = joints[JointType.Head].Position;
            var bottom = joints[JointType.Head].Position;
            var y = Vector3.Distance(top, bottom)/ baseSize;

            var left = joints[JointType.ShoulderLeft].Position;
            var right = joints[JointType.ShoulderRight].Position;
            var x = Vector3.Distance(left, right)/baseSize;

            return new Vector3(x, y, baseSize);
        }
    }
}
