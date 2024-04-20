using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageTextLogic : MonoBehaviour
{
    private PlayerLogic master;
    private TMP_Text tmpText;

    private void Start()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void SetMaster(PlayerLogic master)
    {
        if(master) master.OnDamaged.RemoveListener(UpdateText);
        this.master = master;
        master.OnDamaged.AddListener(UpdateText);
        var portrait = GetComponentInChildren<RawImage>();
        portrait.texture = master.portrait;
    }

    private void UpdateText(float totalDamage)
    {
        tmpText.text = totalDamage.ToString("F1") + "%";
    }


}
