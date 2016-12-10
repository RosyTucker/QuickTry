using System.Collections.Generic;
using Windows.Kinect;

namespace Assets.Lib
{
    public class Rig
    {
        public static readonly Dictionary<JointType, string> JointMapping = new Dictionary<JointType, string>
            {
                {JointType.Head, "skeleton/Hips/LowerBack/Spine/Spine1/Neck/Neck1/Head"},
                {JointType.Neck, "skeleton/Hips/LowerBack/Spine/Spine1/Neck"},

                {JointType.SpineShoulder, "skeleton/Hips/LowerBack/Spine"},
                {JointType.SpineMid, "skeleton/Hips/LowerBack/Spine"},
                {JointType.SpineBase, "skeleton/Hips/LowerBack/Spine"},

                {JointType.ShoulderLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder"},
                {JointType.ElbowLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm"},
                {JointType.WristLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm"},
                {JointType.HandLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand"},
                {JointType.HandTipLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftFingerBase"},
                {JointType.ThumbLeft, "skeleton/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LThumb"},


                {JointType.ShoulderRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder"},
                {JointType.ElbowRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm"},
                {JointType.WristRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm"},
                {JointType.HandRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand"},
                {JointType.HandTipRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand/RightFingerBase"},
                {JointType.ThumbRight, "skeleton/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand/RThumb"},


                {JointType.HipLeft,"skeleton/Hips/LHipJoint" },
                {JointType.KneeLeft, "skeleton/Hips/LHipJoint/LeftUpLeg" },
                {JointType.AnkleLeft, "skeleton/Hips/LHipJoint/LeftUpLeg/LeftLeg" },
                {JointType.FootLeft, "skeleton/Hips/LHipJoint/LeftUpLeg/LeftLeg/LeftFoot" },

                {JointType.HipRight,"skeleton/Hips/RHipJoint" },
                {JointType.KneeRight,"skeleton/Hips/RHipJoint/RightUpLeg" },
                {JointType.AnkleRight,"skeleton/Hips/RHipJoint/RightUpLeg/RightLeg" },
                {JointType.FootRight,"skeleton/Hips/RHipJoint/RightUpLeg/RightLeg/RightFoot" }
            };
    }
}
