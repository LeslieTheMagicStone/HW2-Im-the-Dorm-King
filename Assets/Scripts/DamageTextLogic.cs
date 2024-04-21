using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageTextLogic : MonoBehaviour
{
    private PlayerLogic master;
    private TMP_Text tmpText;
    const float YOFFSET = 2f;

    private void Start()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        UpdateText(0);
    }

    private void Update()
    {
        if (master)
            transform.position = master.transform.position + YOFFSET * Vector3.up;
        else
            Destroy(gameObject);
    }

    public void SetMaster(PlayerLogic master)
    {
        if (master) master.OnDamaged.RemoveListener(UpdateText);
        this.master = master;
        master.OnDamaged.AddListener(UpdateText);
    }

    private void UpdateText(float totalDamage)
    {
        tmpText.text = totalDamage.ToString("F1") + "%";
        tmpText.transform.DOShakePosition(0.5f, 0.7f, 1000).SetEase(Ease.Linear);
        tmpText.DOFade(1, 0.01f).OnComplete(() => tmpText.DOFade(1, 1f).OnComplete(() => tmpText.DOFade(0, 1)));
    }


}
