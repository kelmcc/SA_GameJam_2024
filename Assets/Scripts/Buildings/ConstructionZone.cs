using Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ConstructionZone : MonoBehaviour
{
   public int Cost;
   
   public LayerMask BuildTriggerMask;
   public float Radius;
   public SphereCollider Coll;
   public InputActionReference InteractReference;
   
   public UnityEvent<Building> OnBuilt;

   private List<Material> _mats = new List<Material>();
   private Renderer[] _renderers;
   public Building Building;
   public Material HologramMaterial;

   public GameObject CanAffordPrompt;
   public GameObject CantAffordPrompt;

   public TextMeshProUGUI CostText;
   public TextMeshProUGUI CostText2;
   
   private bool _canBuild;
   private bool _built;
   private Collider _builder;

   private void Start()
   {
      _renderers =  Building.GetComponentsInChildren<Renderer>();
      SetHologram();
      Building.gameObject.SetActive(false);

      CostText.text = "" + Cost;
      CostText2.text = "" + Cost;
      CanAffordPrompt.SetActive(false);
      CantAffordPrompt.SetActive(false);
   }

   private bool CanAfford()
   {
      return Player.Instance.Coinz > Cost;
   }

   private void SetHologram()
   {
      Building.SetPassive();
      _mats.Clear();
      foreach (Renderer r in _renderers)
      {
         _mats.Add(r.sharedMaterial);
         r.sharedMaterial = HologramMaterial;
      }
   }

   private void SetReal()
   {
      for (int i = 0; i < _renderers.Length; i++)
      {
         _renderers[i].sharedMaterial = _mats[i];
      }
      Building.SetInteractable();
   }

   public void OnTriggerEnter(Collider other)
   {
      if (BuildTriggerMask.Contains(other.gameObject.layer))
      {
         _builder = other;
         Building.gameObject.SetActive(true); 
         _canBuild = true;

         if (CanAfford())
         {
            CanAffordPrompt.SetActive(true);
            CantAffordPrompt.SetActive(false);
         }
         else
         {
            CanAffordPrompt.SetActive(false);
            CantAffordPrompt.SetActive(true);
         }
      }
   }

   public void OnTriggerExit(Collider other)
   {
      if (other == _builder)
      {
         if (!_built)
         {
            Building.gameObject.SetActive(false); 
         }
         
         CanAffordPrompt.SetActive(false);
         CantAffordPrompt.SetActive(false);
       
         _canBuild = false;
      }
   }

   public void Update()
   {
      Coll.radius = Radius;

      if (!_canBuild)
      {
         return;
      }
      
      if (InteractReference.ToInputAction().IsPressed())
      {
         if (!_built && CanAfford())
         {
            CanAffordPrompt.SetActive(false);
            CantAffordPrompt.SetActive(false);
            Player.Instance.Buy(Cost);
            
            Building.gameObject.SetActive(true);
            SetReal();
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
