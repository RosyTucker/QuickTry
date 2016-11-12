using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor _kinectSensor;
    private BodyFrameReader _reader;
    public Body[] BodyData { get; private set; }

    void Start () 
    {
        _kinectSensor = KinectSensor.GetDefault();

        if (_kinectSensor == null) return;

        _reader = _kinectSensor.BodyFrameSource.OpenReader();
            
        if (!_kinectSensor.IsOpen)
        {
            _kinectSensor.Open();
        }
    }
    
    void Update () 
    {
        if (_reader == null) return;
        BodyData = GetUpdatedBodyData();
    }
    
    void OnApplicationQuit()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }

        if (_kinectSensor == null) return;

        if (_kinectSensor.IsOpen)
        {
            _kinectSensor.Close();
        }
            
        _kinectSensor = null;
    }

    private Body[] GetUpdatedBodyData()
    {
        using (var frame = _reader.AcquireLatestFrame())
        {
            if (frame == null) return null;

            var bodyData = new Body[_kinectSensor.BodyFrameSource.BodyCount];

            frame.GetAndRefreshBodyData(bodyData);
            return bodyData;
        }
    }
}
