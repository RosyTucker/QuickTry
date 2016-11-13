using Windows.Kinect;
using Assets.KinectView.Lib;
using UnityEngine;

namespace Assets.KinectView.Scripts
{
    public class ColorSourceManager : MonoBehaviour
    {
        public Texture2D ColorFrameTexture { get; private set; }

        private Kinect _kinect;
        private uint _frameDataSize;


        void Start()
        {
            _kinect = new Kinect();
            var frameDescription = _kinect.Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorFrameTexture = new Texture2D(frameDescription.Width, frameDescription.Height, TextureFormat.RGBA32, false);
            _frameDataSize = frameDescription.BytesPerPixel*frameDescription.LengthInPixels;
        }

        void Update()
        {
            if (_kinect.Reader == null) return;
            LoadFrameIntoTexture(_kinect.Reader, ColorFrameTexture);
        }

        void OnApplicationQuit()
        {
            _kinect.Dispose();
        }

        private void LoadFrameIntoTexture(MultiSourceFrameReader reader, Texture2D texture)
        {

            var multiSourceFrame = reader.AcquireLatestFrame();
            if (multiSourceFrame == null) return;

            using (var colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame == null) return;
                var frameData = new byte[_frameDataSize];
                colorFrame.CopyConvertedFrameDataToArray(frameData, ColorImageFormat.Rgba);
                texture.LoadRawTextureData(frameData);
                texture.Apply();
            }
        }
    }
}