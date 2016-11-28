using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.Lib;
using Assets.Lib.Models;
using UnityEngine;

namespace Assets.Scripts
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
            bodyClothingObject.AddComponent<BoxCollider>();
            bodyClothingObject.transform.parent = gameObject.transform;            
            bodyClothingObject.transform.localPosition = new Vector3(0, -0.5f, 0);
            bodyClothingObject.transform.localScale = new Vector3(1, 1, 1);
            return new ClothingForBody(bodyClothingObject, body.TrackingId);
        }

        private static void UpdateClothesForBody(BodyViewModel body, ClothingForBody clothingForBody,
            Dictionary<string, ClothingItem> newClothes)
        {
            RemoveDeadClothes(new List<string>(newClothes.Keys), clothingForBody);
            foreach (var clothingItem in newClothes.Values)
            {
                if (!clothingForBody.Clothes.ContainsKey(clothingItem.Id))
                {
                    var clothingObject = CreateClothingGameObject(clothingItem, body);
                    clothingObject.transform.parent = clothingForBody.BodyObject.transform;
                    clothingObject.transform.localPosition = new Vector3(0, 0, 0);
                    clothingForBody.Clothes.Add(clothingItem.Id, clothingObject);
                }
                MapClothingObjectPositionsFromBody(clothingForBody.BodyObject.transform, clothingForBody.Clothes[clothingItem.Id], body);
            }
        }

        private static GameObject CreateClothingGameObject(ClothingItem clothingItem, BodyViewModel body)
        {
            var clothingObject = Instantiate(Resources.Load<GameObject>(clothingItem.Mesh));
            clothingObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            clothingObject.name = clothingItem.Id;
            return clothingObject;
        }

        private static void RemoveDeadClothes(ICollection<string> newClothingIds, ClothingForBody clothingForBody)
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

        private void RemoveDeadBodies(IEnumerable<ulong> knownBodyIds, ICollection<ulong> freshBodyIds)
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

        private static void MapClothingObjectPositionsFromBody(Transform transform, GameObject clothingGameObject, BodyViewModel bodyViewModel)
        {
            var jointMapping = new Dictionary<JointType, string>
            {
                {JointType.ElbowRight, "rig/root/MCH-shoulder_rh_ns_ch.L/DEF-upper_arm.01.L" },
                {JointType.ElbowLeft, "rig/root/MCH-shoulder_rh_ns_ch.R/DEF-upper_arm.01.R" },
            };

            var bone = clothingGameObject.transform.Find(jointMapping[JointType.ElbowLeft]);
            var jointPosition = bodyViewModel.Joints[JointType.ElbowLeft].Position;
            bone.transform.localPosition = transform.LocalPositionFromColorSourcePosition(jointPosition);

            var bone2 = clothingGameObject.transform.Find(jointMapping[JointType.ElbowRight]);
            var jointPosition2 = bodyViewModel.Joints[JointType.ElbowRight].Position;
            bone2.transform.localPosition = transform.LocalPositionFromColorSourcePosition(jointPosition2);
        }
    }
}