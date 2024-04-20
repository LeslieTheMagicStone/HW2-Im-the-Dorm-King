using UnityEngine;

public class GoblinLogic : PlayerLogic
{
    [SerializeField] private FireballLogic fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;

    protected override void Update()
    {
        base.Update();
        if (!isControllable) return;
        HandleMovement();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1" + playerId.ToString()) && !isBusy)
        {
            animator.SetTrigger("Fireball");
        }
    }

    public void Fireball()
    {
        var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
        fireball.playerId = playerId;
    }

}
