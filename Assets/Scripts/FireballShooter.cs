using System.Collections;
using UnityEngine;

public class FireballShooter : MonoBehaviour
{
    [SerializeField] private FireballLogic fireballPrefab;

    const float SHOOT_INTERVAL = 3f;

    private IEnumerator Start()
    {
        while (true)
        {
            var fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);
            fireball.playerId = PlayerId._Neutral;
            yield return new WaitForSeconds(SHOOT_INTERVAL);
        }
    }
}
