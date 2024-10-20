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

    public static CoinUIController Instance;

    private Tween t;

    private void Awake()
    {
        Instance = this;
    }

    public void PopObjective()
    {
        t?.Kill(true);
        t= youNeedText.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f);
    }
    
    // Update is called once per frame
    void Update()
    {
        Player p = Player.Instance;
        text.text = ""+p.Coinz;
        fillImage.fillAmount = Mathf.Clamp01(p.Coinz / (float)p.WinningCoinCount);

        youNeedText.text = $"You need <size=50>{p.WinningCoinCount-p.Coinz}</size> Coinz <sup>(tm)</sup> to pay rent!";
        //spiderText.text = "The spiders are angry!";
    }
}
