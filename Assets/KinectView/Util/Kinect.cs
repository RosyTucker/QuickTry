using System;
using Windows.Kinect;

namespace Assets.KinectView.Util
{
    public class Kinect : IDisposable
    {
        public KinectSensor Sensor { get; private set; }
        public MultiSourceFrameReader Reader { get; private set; }

        public Kinect(FrameSourceTypes frameSourceTypes)
        {
            Sensor = KinectSensor.GetDefault();
            if (Sensor == null) return;

            Sensor.Open();
            Reader = Sensor.OpenMultiSourceFrameReader(frameSourceTypes);
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