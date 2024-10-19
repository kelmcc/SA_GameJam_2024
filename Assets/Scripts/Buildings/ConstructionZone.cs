using Framework;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ConstructionZone : MonoBehaviour
{
   public LayerMask BuildTriggerMask;
   public float Radius;
   public SphereCollider Coll;
   public InputActionReference InteractReference;
   
   public UnityEvent<GameObject> OnBuilt;

   public GameObject Holograph;

   public GameObject Building;

   private bool _canBuild;
   private bool _built;
   private Collider _builder;

   private void Start()
   {
      if (Holograph != null)
      {
         Holograph.SetActive(false);
      }
   }

   public void OnTriggerEnter(Collider other)
   {
      if (BuildTriggerMask.Contains(other.gameObject.layer))
      {
         _builder = other;
         if (Holograph)
         {
            Holograph.SetActive(true);
         }
         _canBuild = true;
      }
   }

   public void OnTriggerExit(Collider other)
   {
      if (other == _builder)
      {
         if (Holograph)
         {
            Holograph?.SetActive(false);
         }
         _canBuild = false;
      }
   }

   public void Update()
   {
      Coll.radius = Radius;
      if (InteractReference.ToInputAction().IsPressed())
      {
         if (!_built)
         {
            if (Holograph)
            {
               Holograph?.SetActive(false);
            }
            Building.SetActive(true);
            OnBuilt?.Invoke(Building);
            _canBuild = false;
            _built = true;
         }
         else
         {
            //Destroy. Get back coins
         }
      }
   }

   public void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position, Radius);
   }
}
