using UnityEngine;

public class GoblinLogic : PlayerLogic
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;

    private void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1" + playerId.ToString()))
        {
            animator.SetTrigger("Fireball");
        }
    }

    public void Fireball()
    {
        var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
    }

}
