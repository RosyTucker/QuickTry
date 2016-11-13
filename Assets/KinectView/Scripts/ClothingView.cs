using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.KinectView.Lib;
using UnityEngine;

namespace Assets.KinectView.Scripts
{
    public class ClothingView : MonoBehaviour
    {
        public GameObject ClothingManager;
        public GameObject BodySourceManager;

        private readonly Dictionary<ulong, ClothingForBody> _clothingForBodies = new Dictionary<ulong, ClothingForBody>();

        void Update()
        {
            var bodyData = GameObjectUtils.GetUpdatedBodyData(BodySourceManager);
            var assignedClothing = GameObjectUtils.GetAssignedClothing(ClothingManager);

            if (bodyData == null) return;
            var trackedBodyData = bodyData.Where(body => body.IsTracked).ToArray();

            var knownBodyIds = new List<ulong>(_clothingForBodies.Keys);
            var freshBodyIds = trackedBodyData.Select(trackedBody => trackedBody.TrackingId).ToList();
            RemoveDeadBodies(knownBodyIds, freshBodyIds);

            foreach (var body in trackedBodyData)
            {
                if (!_clothingForBodies.ContainsKey(body.TrackingId))
                {
                    _clothingForBodies[body.TrackingId] = CreateClothingBody(body);
                }
                UpdateClothesForBody(body, _clothingForBodies[body.TrackingId], assignedClothing);
            }
        }

        private ClothingForBody CreateClothingBody(BodyViewModel body)
        {
            var bodyClothingObject = new GameObject("BodyClothing:" + body.TrackingId);
            bodyClothingObject.transform.parent = gameObject.transform;
            return new ClothingForBody(bodyClothingObject, body.TrackingId);
        }

        private void UpdateClothesForBody(BodyViewModel body, ClothingForBody clothingForBody,
            Dictionary<string, ClothingItem> newClothes)
        {
            RemoveDeadClothes(new List<string>(newClothes.Keys), clothingForBody);
            foreach (var clothingItem in newClothes.Values)
            {
                if (!clothingForBody.Clothes.ContainsKey(clothingItem.Id))
                {
                    var clothingObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    clothingObject.transform.parent = clothingForBody.BodyObject.transform;
                    clothingObject.transform.localScale = new Vector3(12, 15, 12);
                    clothingObject.name = clothingItem.Id;
                    clothingForBody.Clothes.Add(clothingItem.Id, clothingObject);
                }
                clothingForBody.Clothes[clothingItem.Id].transform.localPosition =
                    transform.LocalPositionFromColorSourcePosition(body.Joints[JointType.SpineBase].Position);
            }
        }

        private void RemoveDeadClothes(List<string> newClothingIds, ClothingForBody clothingForBody)
        {
            var existingClothingIds = new List<string>(clothingForBody.Clothes.Keys);
            foreach (var knownClothingId in existingClothingIds)
            {
                if (newClothingIds.Contains(knownClothingId)) continue;

                var objectToRemove = clothingForBody.Clothes[knownClothingId];

                Destroy(objectToRemove);

                clothingForBody.Clothes.Remove(knownClothingId);
            }
        }

        private void RemoveDeadBodies(List<ulong> knownBodyIds, List<ulong> freshBodyIds)
        {
            foreach (var knownBodyId in knownBodyIds)
            {
                if (freshBodyIds.Contains(knownBodyId)) continue;

                var wrapper = _clothingForBodies[knownBodyId];
                foreach (var clothingItem in wrapper.Clothes.Values)
                {
                    Destroy(clothingItem);
                }

                Destroy(wrapper.BodyObject);

                _clothingForBodies.Remove(knownBodyId);
            }
        }
    }
}