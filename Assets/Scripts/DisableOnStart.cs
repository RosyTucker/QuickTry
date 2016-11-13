using UnityEngine;

namespace Assets.Scripts
{
    public class DisableOnStart : MonoBehaviour {

        // Use this for initialization
        void Start () 
        {
            gameObject.SetActive (false);
        }
    }
}
