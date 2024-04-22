using UnityEngine;
using DG.Tweening;
using System.Collections;

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
            StartCoroutine(CollectCoinCoroutine());
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
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

    IEnumerator CollectCoinCoroutine()
    {
        collider.enabled = false;
        transform.DOMoveY(4, COLLECTED_ANIMATION_TIME).SetRelative(true).SetEase(Ease.InBack);
        transform.DORotate(new(0, 0, COLLECTED_ROTATION_SPEED * COLLECTED_ANIMATION_TIME), COLLECTED_ANIMATION_TIME, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(COLLECTED_ANIMATION_TIME);

        SetCoinState(CoinState.Inactive);
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
            meshRenderer.enabled = false;
            collider.enabled = false;
        }
    }

}
