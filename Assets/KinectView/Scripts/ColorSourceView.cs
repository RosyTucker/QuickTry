using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    private ColorSourceManager _colorManager;
    
    void Start ()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    
    void Update()
    {
        if (ColorSourceManager == null) return;
        
        _colorManager = ColorSourceManager.GetComponent<ColorSourceManager>();

        if (_colorManager == null) return;
        
        gameObject.GetComponent<Renderer>().material.mainTexture = _colorManager.GetColorTexture();
    }
}
