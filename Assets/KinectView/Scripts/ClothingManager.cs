using System.Collections.Generic;
using Assets.KinectView.Lib;
using UnityEngine;

namespace Assets.KinectView.Scripts
{
    public class ClothingManager : MonoBehaviour
    {

        public Dictionary<string, ClothingItem> AssignedClothing { get; private set; }

        void Start()
        {
            AssignedClothing = new Dictionary<string, ClothingItem>
            {
                {"1", new ClothingItem(ClothingType.Shirt, "Green Shirt")},
                {"2", new ClothingItem(ClothingType.Trousers, "Blue Trousers")}
            };
        }

        void Update()
        {
      
        }

        void OnApplicationQuit()
        {
        }
    }
}