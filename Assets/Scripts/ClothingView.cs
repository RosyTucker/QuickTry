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
                    var clothingObject = CreateClothingGameObject(clothingItem, body);
                    clothingObject.transform.parent = clothingForBody.BodyObject.transform;
                    clothingForBody.Clothes.Add(clothingItem.Id, clothingObject);
                }
               clothingForBody.Clothes[clothingItem.Id].transform.localPosition = GetClothingItemPosition(transform, clothingItem, body);
            }
        }

        private static Vector3 GetClothingItemPosition(Transform transform, ClothingItem clothingItem, BodyViewModel body)
        {
            return transform.LocalPositionFromColorSourcePosition(body.Joints[JointType.SpineMid].Position);
        }

        private static GameObject CreateClothingGameObject(ClothingItem clothingItem, BodyViewModel body)
        {
            var clothingObject = new GameObject(clothingItem.Id + body.TrackingId);
            var skinnedMeshRenderer = clothingObject.AddComponent<SkinnedMeshRenderer>();
            var animator = clothingObject.AddComponent<Animator>();
            animator.avatar = Resources.Load<Avatar>(clothingItem.Mesh);
            var mesh = Resources.Load<Mesh>(clothingItem.Mesh);
            skinnedMeshRenderer.sharedMesh = mesh;
            var texture = (Texture2D)Resources.Load(clothingItem.Texture, typeof(Texture2D));
            skinnedMeshRenderer.material = (Material)Resources.Load(clothingItem.Material, typeof(Material));
            skinnedMeshRenderer.material.mainTexture = texture;
            clothingObject.transform.Rotate(-90, 0, 0);
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
    }
}