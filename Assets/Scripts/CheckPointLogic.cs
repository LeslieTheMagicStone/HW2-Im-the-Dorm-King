using UnityEngine;
using DG.Tweening;

public class CheckPointLogic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SaveGame();
            transform.DOJump(transform.position, 1, 1, 0.5f);
        }
    }
}
