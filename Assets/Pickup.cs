using Framework;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Transform bounceRoot;
    public float BounceSpeed;
    public float RotateSpeed;

    private Vector3 _bounceIdle;
    
    // Start is called before the first frame update
    void Start()
    {
        _bounceIdle = bounceRoot.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        bounceRoot.transform.localPosition = bounceRoot.transform.localPosition.WithY(Mathf.Sign(Time.time * BounceSpeed));
        bounceRoot.transform.localRotation = Quaternion.AngleAxis(Time.time * RotateSpeed, Vector3.up);
    }
}
