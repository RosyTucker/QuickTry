using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.KinectView.Lib;
using UnityEngine;

namespace Assets.KinectView.Scripts
{
    public class ClothingWrapper
    {
        public GameObject WrapperObject { get; set; }
        public Dictionary<string, GameObject> Clothes { get; set; }
        public readonly ulong TrackingId;

        public ClothingWrapper(GameObject wrapperObject, ulong trackingId)
        {
            WrapperObject = wrapperObject;
            Clothes = new Dictionary<string, GameObject>();
            TrackingId = trackingId;
        }
    }

    public class ClothingView : MonoBehaviour
    {
        public GameObject ClothingManager;
        public GameObject BodySourceManager;

        private readonly Dictionary<ulong, ClothingWrapper> _clothing = new Dictionary<ulong, ClothingWrapper>();

        void Update()
        {
            var bodyData = GameObjectUtils.GetUpdatedBodyData(BodySourceManager);
            if (bodyData == null) return;

            var assignedClothing = GameObjectUtils.GetAssignedClothing(ClothingManager);
            var trackedBodyData = bodyData.Where(body => body.IsTracked).ToArray();

            var knownBodyIds = new List<ulong>(_clothing.Keys);
            var freshBodyIds = trackedBodyData.Select(trackedBody => trackedBody.TrackingId).ToList();
            RemoveDeadBodies(knownBodyIds, freshBodyIds);

            foreach (var body in trackedBodyData)
            {
                if (!_clothing.ContainsKey(body.TrackingId))
                {
                    _clothing[body.TrackingId] = CreateClothingBody(body);
                }
                UpdateClothesForBody(body, _clothing[body.TrackingId], assignedClothing);
            }
        }

        private ClothingWrapper CreateClothingBody(BodyViewModel body)
        {
            var bodyClothingObject = new GameObject("BodyClothing:" + body.TrackingId);
            bodyClothingObject.transform.parent = gameObject.transform;
            return new ClothingWrapper(bodyClothingObject, body.TrackingId);
        }

        private void UpdateClothesForBody(BodyViewModel body, ClothingWrapper clothingWrapper,
            Dictionary<string, ClothingItem> newClothes)
        {
            RemoveDeadClothes(new List<string>(newClothes.Keys), clothingWrapper);
            foreach (var clothingItem in newClothes.Values)
            {
                if (!clothingWrapper.Clothes.ContainsKey(clothingItem.Id))
                {
                    var clothingObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    clothingObject.transform.parent = clothingWrapper.WrapperObject.transform;
                    clothingObject.transform.localScale = new Vector3(12, 15, 12);
                    clothingObject.name = clothingItem.Id;
                    clothingWrapper.Clothes.Add(clothingItem.Id, clothingObject);
                }
                clothingWrapper.Clothes[clothingItem.Id].transform.localPosition =
                    transform.LocalPositionFromColorSourcePosition(body.Joints[JointType.SpineBase].Position);
            }
        }

        private void RemoveDeadClothes(List<string> newClothingIds, ClothingWrapper clothingWrapper)
        {
            var existingClothingIds = new List<string>(clothingWrapper.Clothes.Keys);
            foreach (var knownClothingId in existingClothingIds)
            {
                if (newClothingIds.Contains(knownClothingId)) continue;

                var objectToRemove = clothingWrapper.Clothes[knownClothingId];

                Destroy(objectToRemove);

                clothingWrapper.Clothes.Remove(knownClothingId);
            }
        }

        private void RemoveDeadBodies(List<ulong> knownBodyIds, List<ulong> freshBodyIds)
        {
            foreach (var knownBodyId in knownBodyIds)
            {
                if (freshBodyIds.Contains(knownBodyId)) continue;

                var wrapper = _clothing[knownBodyId];
                foreach (var clothingItem in wrapper.Clothes.Values)
                {
                    Destroy(clothingItem);
                }

                Destroy(wrapper.WrapperObject);

                _clothing.Remove(knownBodyId);
            }
        }
    }
}