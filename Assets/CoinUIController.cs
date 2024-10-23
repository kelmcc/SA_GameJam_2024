using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinUIController : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI text;
    public TextMeshProUGUI youNeedText;
    public TextMeshProUGUI spiderText;

    public Transform cribsHealthRoot;
    public Image cribsFill;

    public static CoinUIController Instance;

    private Tween t;

    private float _lastSpiderTime = 0;
    private float _spiderUpdateInterval = 60 * 5;

    private void Awake()
    {
        Instance = this;
        _lastSpiderTime = Time.time;
    }

    public void PopObjective()
    {
        t?.Kill(true);
        t= youNeedText.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f);
    }
    
    // Update is called once per frame
    void Update()
    {
        float t = Time.time - _lastSpiderTime;
        if (t > _spiderUpdateInterval)
        {
            _lastSpiderTime = Time.time;
            t = 0;
            Player.Instance.SpidersHaveIncreased();
        }

        float tInverse = _spiderUpdateInterval - t;
        int seconds = (int) (tInverse % 60f);
        int minutes = (int)(tInverse / 60f);

        spiderText.text = $"Spiders increasing in: {minutes:0}:{seconds:00}";

        float scale = Mathf.Max(0.01f,Mathf.Clamp01(t / _spiderUpdateInterval));
        spiderText.transform.parent.localScale = new Vector3(scale, scale, scale);

        if (Tower.Crib != null)
        {
            if (Tower.Crib.IsBought)
            {
                cribsHealthRoot.gameObject.SetActive(true);
            }
            else
            {
                cribsHealthRoot.gameObject.SetActive(false);
            }
            cribsFill.fillAmount = Mathf.Clamp01(Tower.Crib.CurrentHealth / (float)Tower.Crib.Health);
        }
        else
        {
            cribsHealthRoot.gameObject.SetActive(false);
        }
        
        
        Player p = Player.Instance;
        text.text = ""+p.Coinz;
        fillImage.fillAmount = Mathf.Clamp01(p.Coinz / (float)p.WinningCoinCount);
        youNeedText.text =  $"You're<size=50><color=red>-{p.WinningCoinCount-p.Coinz}</color></size> Coinz<sup>(tm)</sup> in debt!";
        //$"You need <size=50></size> Coinz <sup>(tm)</sup> to pay rent!";
        //spiderText.text = "The spiders are angry!";
    }
}
