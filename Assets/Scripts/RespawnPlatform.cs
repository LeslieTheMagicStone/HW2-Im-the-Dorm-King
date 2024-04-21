using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RespawnPlatform : MonoBehaviour
{
    const float MAX_WAIT_TIME = 2f;
    const float SHIFT_TIME = 1f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(MAX_WAIT_TIME);

        transform.DOMoveX(transform.position.x > 0 ? 12f : -12f, SHIFT_TIME).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}
