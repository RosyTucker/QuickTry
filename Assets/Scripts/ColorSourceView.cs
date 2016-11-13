using UnityEngine;

namespace Assets.Scripts
{
    public class ColorSourceView : MonoBehaviour
    {
        public GameObject ColorSourceManager;
        private ColorSourceManager _colorManager;
    
        void Update()
        {
            if (ColorSourceManager == null) return;
        
            _colorManager = ColorSourceManager.GetComponent<ColorSourceManager>();

            if (_colorManager == null) return;
        
            gameObject.GetComponent<Renderer>().material.mainTexture = _colorManager.ColorFrameTexture;
        }
    }
}
