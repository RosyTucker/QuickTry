using System.Collections.Generic;
using Assets.Lib.Models;
using Assets.Scripts;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Lib
{
    class GameObjectUtils
    {
        public static BodyViewModel[] GetUpdatedBodyData(GameObject bodySourceManager)
        {
            Debug.Assert(bodySourceManager != null, "BodySourceManager is null");

            var bodySourceManagerScript = bodySourceManager.GetComponent<BodySourceManager>();

            Debug.Assert(bodySourceManagerScript != null, "BodySourceManager does not have the required script");

            return bodySourceManagerScript.BodyViewModels;
        }

        public static Dictionary<string, ClothingItem> GetAssignedClothing(GameObject clothingManager)
        {
            Debug.Assert(clothingManager != null, "ClothingManager is null");

            var clothingManagerScript = clothingManager.GetComponent<ClothingManager>();

            Debug.Assert(clothingManagerScript != null, "ClothingManager does not have the required script");

            return clothingManagerScript.AssignedClothing;
        }
    }
}
