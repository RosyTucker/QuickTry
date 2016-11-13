using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.KinectView.Lib
{
    public static class TransformExtensions
    {
        public static Vector3 LocalPositionFromColorSourcePosition(this Transform transform, Vector2 colorSourcePosition)
        {
            var bounds = transform.GetComponent<Collider>().bounds;
            var xCoord = MapToUiCoordinates(colorSourcePosition.x, 0, 1920, bounds.min.x, bounds.max.x);
            var yCoord = MapToUiCoordinates(colorSourcePosition.y, 0, 1080, bounds.min.y, bounds.max.y);
            return new Vector3(-xCoord, -yCoord, -1);
        }

        private static float MapToUiCoordinates(float numberToMap, float minInput, float maxInput, float minOutput,
            float maxOutput)
        {
            var output = (numberToMap - minInput)*(maxOutput - minOutput)/(maxInput - minInput) + minOutput;
            return LimitInclusive(output, minOutput, maxOutput);
        }

        public static float LimitInclusive(float value, float min, float max)
        {
            return Math.Min(max, Math.Max(value, min));
        }
    }
}