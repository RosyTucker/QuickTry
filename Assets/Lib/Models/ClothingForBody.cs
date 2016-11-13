using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ClothingForBody
    {
        public GameObject BodyObject { get; set; }
        public Dictionary<string, GameObject> Clothes { get; set; }
        public readonly ulong BodyTrackingId;

        public ClothingForBody(GameObject bodyObject, ulong bodyTrackingId)
        {
            BodyObject = bodyObject;
            Clothes = new Dictionary<string, GameObject>();
            BodyTrackingId = bodyTrackingId;
        }
    }
}