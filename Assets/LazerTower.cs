using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTower : MonoBehaviour
{
    public float MoveToDestinationTime = 0.2f;
    public float MaxRange = 60;

    public bool RandomTest = false;

    public GameObject LazerRoot;

    public Transform Pivot;
    public float PivotSpeed = 4;

    public ParticleSystem afterFiring;

    private Coroutine test = null;
    private void Update()
    {
        if (RandomTest && test == null)
        {
            test = StartCoroutine(Test());
            IEnumerator Test()
            {
                while (RandomTest)
                {
                    yield return new WaitForSeconds(1);
                    Vector3 r = Random.onUnitSphere;
                    Trigger(new Vector3(r.x * 1f, Mathf.Abs(r.y * 0.2f), r.z * 1f).normalized);
                }

                test = null;
            }
        }
    }

    public void Trigger(Vector3 direction)
    {
        CoroutineUtils.StartCoroutine(Effect());
        IEnumerator Effect()
        {
            Quaternion destination = Quaternion.LookRotation(direction, Vector3.up);
            float t = 0;

            while (t < MoveToDestinationTime)
            {
                t += Time.deltaTime * PivotSpeed;
                Pivot.rotation = Quaternion.Slerp(Pivot.rotation, destination, t);

                yield return null;
            }
            
            Pivot.rotation = destination;

            yield return new WaitForSeconds(0.1f);
            
            LazerRoot.SetActive(true);

            yield return new WaitForSeconds(0.2f);
            
            LazerRoot.SetActive(false);
            
            afterFiring.Play();
        }
    }
}
