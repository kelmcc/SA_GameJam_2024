using SteppedAnimations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkaterAnimator : MonoBehaviour
{
   public SimpleAnimationClipAnimatorPlayer Anim;
   
   public AnimationClip Skate;
   public AnimationClip Falling;
   public AnimationClip Frewheeling;
   public AnimationClip Grind;
   public AnimationClip Jump;
   public AnimationClip Idle;
   public AnimationClip Break;

   public float Speed
   {
      set
      {
         Anim.SetSpeed(value);
      }
   }

   public void PlayIdle()
   {
      Anim.SetClip(Idle, true, false);
   }
   
   public void PlayBreak()
   {
      Anim.SetClip(Break, true, false);
   }
   
   public void PlaySkate()
   {
      Anim.SetClip(Skate, true, false);
   }

   public void PlayJump()
   {
      Anim.SetClip(Jump, false, true);
   }

   public void PlayFall()
   {
      Anim.SetClip(Falling, true, false);
   }

   public void PlayGrind()
   {
      Anim.SetClip(Grind, true, true);
   }

   public void PlayFreewheel()
   {
      Anim.SetClip(Frewheeling, true, false);
   }
}
