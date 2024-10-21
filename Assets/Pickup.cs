using Framework;
using System;
using UnityEngine;
using UnityEngine.Pool;
using Easing = UnityEngine.UIElements.Experimental.Easing;
using Random = UnityEngine.Random;

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


    private bool _pickedUp;
    private Rigidbody rb;
    private float t = 0;
    
    void Start()
    {
        bounceRoot.transform.localPosition = StartBouncePosition;
        _startRotOffset = Random.value * 360;
        _rotateSpeed = RotateSpeed.ChooseRandom();
        rb =  GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (rb == null)
        {
            rb =  GetComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        _pickedUp = false;
        t = 0;
    }

    void Update()
    {
       

        if (_pickedUp)
        {
            bounceRoot.transform.localPosition = bounceRoot.transform.localPosition.WithY(StartBouncePosition.y  + (Mathf.Sin(Time.time * BounceSpeed) * BounceSize) * Mathf.Clamp01(1-t));
            bounceRoot.transform.localRotation = Quaternion.AngleAxis(_startRotOffset + Time.time * _rotateSpeed, Vector3.up);
            
            t += Time.deltaTime;
            Vector3 pos = transform.position;
            transform.position = (Vector3.Lerp(pos, Player.Instance.transform.position, Easing.InCubic(t)));

            if (Vector3.Distance(Player.Instance.transform.position, pos) < 1.5)
            {
                _pickedUp = false;
                t = 0;
                CoinPickupSpawner.Instance.PickedUp(this);
            }
        }
        else
        {
            bounceRoot.transform.localPosition = bounceRoot.transform.localPosition.WithY(StartBouncePosition.y  + Mathf.Sin(Time.time * BounceSpeed) * BounceSize);
            bounceRoot.transform.localRotation = Quaternion.AngleAxis(_startRotOffset + Time.time * _rotateSpeed, Vector3.up);
        }
    }

    public void PickedUp()
    {
        rb.isKinematic = true;
       _pickedUp = true;
       t = 0;
    }
}
