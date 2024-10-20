using Framework;
using UnityEngine;

[ExecuteAlways]
public class Pickup : MonoBehaviour
{
    public Transform bounceRoot;
    public float BounceSpeed;
    public FloatRange RotateSpeed;
    public float BounceSize = 1;
    
    public Vector3 StartBouncePosition;

    private float _startRotOffset;
    private float _rotateSpeed;
    
    void Start()
    {
        bounceRoot.transform.localPosition = StartBouncePosition;
        _startRotOffset = Random.value * 360;
        _rotateSpeed = RotateSpeed.ChooseRandom();
    }
    
    void Update()
    {
        bounceRoot.transform.localPosition = bounceRoot.transform.localPosition.WithY(StartBouncePosition.y  + Mathf.Sin(Time.time * BounceSpeed) * BounceSize);
        bounceRoot.transform.localRotation = Quaternion.AngleAxis(_startRotOffset + Time.time * _rotateSpeed, Vector3.up);
    }
}
