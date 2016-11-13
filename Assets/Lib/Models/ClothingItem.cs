using System.Collections.Generic;
using Assets.Scripts;
using Windows.Kinect;
using Assets.Lib.Parser;

namespace Assets.Lib
{
    public class ClothingItem
    {
        public string Texture { get; private set; }
        public Dictionary<JointType, ClothingAttachmentPoint> AttachmentPoints { get; private set; }
        public ClothingType Type { get; private set; }
        public string Id { get; private set; }

        public ClothingItem(string id, ClothingType type, string texture, Dictionary<JointType, ClothingAttachmentPoint> attachmentPoints)
        {
            Id = id;
            Type = type;
            Texture = texture;
            AttachmentPoints = attachmentPoints;
        }
    }
}