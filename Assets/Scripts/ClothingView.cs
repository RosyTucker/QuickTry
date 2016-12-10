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

        private readonly Dictionary<ulong, ClothingForBody> _clothingForBodies =
            new Dictionary<ulong, ClothingForBody>();

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
                    var clothingObject = CreateClothingGameObject(clothingItem);
                    clothingObject.transform.parent = clothingForBody.BodyObject.transform;
                    clothingObject.transform.Rotate(0, clothingItem.BaseYRotation, 0);
                    clothingForBody.Clothes.Add(clothingItem.Id, clothingObject);
                }
                MoveClothingObjectToBody(clothingForBody, clothingItem, body);
            }
        }

        private static GameObject CreateClothingGameObject(ClothingItem clothingItem)
        {
            var clothingObject = Instantiate(Resources.Load<GameObject>(clothingItem.Mesh));
            var skinnedMeshRenderer = clothingObject.transform.Find(clothingItem.RootMeshName).GetComponent<SkinnedMeshRenderer>();
            var texture = (Texture2D)Resources.Load(clothingItem.Texture, typeof(Texture2D));
            skinnedMeshRenderer.material = (Material)Resources.Load(clothingItem.Material, typeof(Material));
            skinnedMeshRenderer.material.mainTexture = texture;
            clothingObject.name = clothingItem.Id;

            return clothingObject;
        }

        private void MoveClothingObjectToBody(ClothingForBody clothingForBody, ClothingItem clothingItem,
            BodyViewModel bodyViewModel)
        {
            var clothingGameObject = clothingForBody.Clothes[clothingItem.Id];

            clothingGameObject.transform.position =
                transform.LocalPositionFromColorSourcePosition(bodyViewModel.Joints[JointType.SpineMid].Position);
            clothingGameObject.transform.localScale = GetClothingObjectScale(bodyViewModel, clothingItem);
            foreach (var joint in Rig.JointMapping)
            {
                var bone = clothingGameObject.transform.Find(joint.Value);
                var jointModel = bodyViewModel.Joints[joint.Key];
                bone.transform.rotation = new Quaternion(
                    jointModel.Orientation.X,
                    -jointModel.Orientation.Y, // Mirror Y
                    -jointModel.Orientation.Z, // Mirror X
                    jointModel.Orientation.W);
            }
        }

        private Vector3 GetClothingObjectScale(BodyViewModel bodyViewModel, ClothingItem clothingItem)
        {
            var headY = transform.LocalPositionFromColorSourcePosition(bodyViewModel.Joints[JointType.Head].Position).y;
            var footY = Mathf.Min(
                transform.LocalPositionFromColorSourcePosition(bodyViewModel.Joints[JointType.FootLeft].Position).y,
                transform.LocalPositionFromColorSourcePosition(bodyViewModel.Joints[JointType.FootRight].Position).y);

            var bodyYDistance = Mathf.Abs(footY - headY);
            var newScale = bodyYDistance / clothingItem.BaseScale;
            return new Vector3(newScale, newScale, newScale);
        }

        private static void RemoveDeadClothes(ICollection<string> newClothingIds, ClothingForBody clothingForBody)
        {
            var existingClothingIds = new List<string>(clothingForBody.Clothes.Keys);
            foreach (var knownClothingId in existingClothingIds)
            {
                if (newClothingIds.Contains(knownClothingId)) continue;

                var clothingToRemove = clothingForBody.Clothes[knownClothingId];

                Destroy(clothingToRemove);

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