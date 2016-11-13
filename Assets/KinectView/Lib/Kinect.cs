using System;
using Windows.Kinect;

namespace Assets.KinectView.Lib
{
    public class Kinect : IDisposable
    {
        public KinectSensor Sensor { get; private set; }
        public MultiSourceFrameReader Reader { get; private set; }
        public FrameDescription ColorFrameDescription { get; private set; }

        public Kinect()
        {
            const FrameSourceTypes frameSourceTypes = FrameSourceTypes.Color | FrameSourceTypes.Body;
            Sensor = KinectSensor.GetDefault();
            if (Sensor == null) return;

            Sensor.Open();
            Reader = Sensor.OpenMultiSourceFrameReader(frameSourceTypes);
            ColorFrameDescription = Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
        }

        public void Dispose()
        {
            if (Reader != null) Reader.Dispose();
            if (Sensor != null) Sensor.Close();

            Reader = null;
            Sensor = null;
        }
    }
}