using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ZiplinePole : MonoBehaviour
{
   public Transform LineConnector;
   public DecalProjector Decal;
   public Collider Trigger;

   public LayerMask PlayerMask;

   private Collider _interacter;

   public InputActionReference JumpReference;

   public Zipline Parent;

   private bool _active;

   public void Start()
   {
      DisableUse();
   }

   public void ActivateUse()
   {
      _active = true;
      Trigger.enabled = true;
      Decal.gameObject.SetActive(true);
   }
   
   public void DisableUse()
   {
      _active = false;
      Trigger.enabled = false;
      Decal.gameObject.SetActive(false);
   }
   
   public void OnTriggerEnter(Collider other)
   {
      Debug.Log(other.gameObject.name);
      if (PlayerMask.Contains(other.gameObject.layer))
      {
         Debug.Log("SETTING INTERACTER");
         _interacter = other;
      }
   }

   public void OnTriggerExit(Collider other)
   {
      if (other == _interacter)
      {
         Debug.Log("UNSETTING INTERACTER");
         _interacter = null;
      }
   }

   public void Update()
   {
      if (_interacter != null && JumpReference.ToInputAction().IsPressed())
      {
         Player player = _interacter.GetComponentInParent<Player>();

         if (!player.IsZipping)
         {
            Parent.StartZip(this, player);
            _interacter = null;
         }
      }
   }
}
