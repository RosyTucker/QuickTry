using System.Collections.Generic;
using Assets.Lib;
using Assets.Lib.Models;
using Assets.Lib.Parser;
using UnityEngine;

namespace Assets.Scripts
{
    public class ClothingManager : MonoBehaviour
    {
        public string ClothingFilePath = "Clothing";
        public Dictionary<string, ClothingItem> AssignedClothing { get; private set; }
        public Dictionary<string, ClothingItem> AllClothing { get; private set; }

        void Start()
        {
            AllClothing = ClothingParser.Parse(ClothingFilePath);
            AssignedClothing = AllClothing;
        }      
    }
}