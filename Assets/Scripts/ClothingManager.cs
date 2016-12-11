using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        // Feeling lazy, rather than do the gesture stuff I'm just going to switch it every 10 seconds - basically ignore all the code in this file
        private const float TimeBetweenChanges = 10;
        private float _timeUntilChange = TimeBetweenChanges;
        private Dictionary<string, ClothingItem>.Enumerator _enumerator;

        void Start()
        {
            AllClothing = ClothingParser.Parse(ClothingFilePath);
            AssignedClothing = new Dictionary<string, ClothingItem>();
            if (AllClothing.Count == 0) return;

            _enumerator = AllClothing.GetEnumerator();
        }

        void Update()
        {
            _timeUntilChange -= Time.deltaTime;
            if (_timeUntilChange > 0) return;

            if (!_enumerator.MoveNext())
            {
                _enumerator = AllClothing.GetEnumerator();
                if (!_enumerator.MoveNext()) return;
            }

            _timeUntilChange = TimeBetweenChanges;
            AssignedClothing.Clear();
            var current = _enumerator.Current;
            AssignedClothing.Add(current.Key, current.Value);
        }
    }
}