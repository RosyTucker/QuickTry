using System.Collections.Generic;
using Windows.Kinect;

namespace Assets.Lib.Models
{
    public class ClothingItem
    {
        public string Texture { get; private set; }
        public string Mesh { get; private set; }
        public string Material { get; private set; }
        public Dictionary<JointType, ClothingAttachmentPoint> AttachmentPoints { get; private set; }
        public ClothingType Type { get; private set; }
        public string Id { get; private set; }

        public ClothingItem(
            string id,
            ClothingType type,
            string texture,
            string mesh,
            string material,
            Dictionary<JointType,
            ClothingAttachmentPoint> attachmentPoints
            )
        {
            Id = id;
            Type = type;
            Texture = texture;
            Mesh = mesh;
            Material = material;
            AttachmentPoints = attachmentPoints;
        }
    }
}