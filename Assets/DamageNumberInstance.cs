using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class DamageNumberInstance : MonoBehaviour
{
   public TMP_Text _number;
   private ObjectPool<DamageNumberInstance> _pool;
   public SpriteRenderer Renderer;

   public void SetDamage(int damage, bool showCoinIcon, ObjectPool<DamageNumberInstance> pool)
   {
      _pool = pool;
      _number.text = $"-{damage}";

      Renderer.enabled = showCoinIcon;

      transform.DOMove(transform.position + Vector3.up * 5, 2).onComplete += () =>
      {
         _pool.Release(this);
      };
   }
}
