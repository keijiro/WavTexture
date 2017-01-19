using UnityEngine;

class FastForward : MonoBehaviour
{
    [SerializeField] float _time;

    void Start()
    {
        GetComponent<AudioSource>().time = _time;
    }
}
