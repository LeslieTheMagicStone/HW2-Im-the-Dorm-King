using UnityEngine;
using DG.Tweening;

public enum CoinState
{
    Inactive,
    Active,
}

public class CoinLogic : MonoBehaviour
{
    const float NORMAL_ROTATION_SPEED = 60f;
    const float COLLECTED_ROTATION_SPEED = 2880f;
    const float COLLECTED_ANIMATION_TIME = 1f;
    CoinState coinState = CoinState.Active;
    MeshRenderer meshRenderer;
    new Collider collider;
    AudioSource audioSource;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (coinState == CoinState.Active)
            transform.Rotate(0, 0, NORMAL_ROTATION_SPEED * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetCoinState(CoinState.Inactive);
            audioSource.Play();
        }
    }

    public void Save(int index)
    {
        PlayerPrefs.SetInt("CoinState" + index, (int)coinState);
    }

    public void ResetSave(int index)
    {
        PlayerPrefs.SetInt("CoinState" + index, (int)CoinState.Active);
    }

    public void Load(int index)
    {
        SetCoinState((CoinState)PlayerPrefs.GetInt("CoinState" + index, 1));
    }

    void SetCoinState(CoinState coinState)
    {
        this.coinState = coinState;

        if (coinState == CoinState.Active)
        {
            meshRenderer.enabled = true;
            collider.enabled = true;
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            transform.DOMoveY(4, COLLECTED_ANIMATION_TIME).SetRelative(true).SetEase(Ease.InBack);
            transform.DORotate(new(0, 0, COLLECTED_ROTATION_SPEED * COLLECTED_ANIMATION_TIME), COLLECTED_ANIMATION_TIME, RotateMode.LocalAxisAdd);
            sequence.AppendInterval(COLLECTED_ANIMATION_TIME);
            sequence.AppendCallback(() => meshRenderer.enabled = false);
            sequence.AppendCallback(() => collider.enabled = false);
        }
    }

}
