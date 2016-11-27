using Windows.Kinect;
using UnityEngine;

namespace Assets.Lib.Models
{
    public class ClothingAttachmentPoint
    {
        public Vector3 Position { get; private set; }
        public JointType Type { get; private set; }

        public ClothingAttachmentPoint(JointType type, Vector3 position)
        {
            Type = type;
            Position = position;
        }
    }
}