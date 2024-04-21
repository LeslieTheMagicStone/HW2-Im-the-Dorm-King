using UnityEngine;

public class GoblinLogic : PlayerLogic
{
    [SerializeField] private FireballLogic fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;

    [SerializeField] private EarthShatterLogic earthShatterPrefab;
    [SerializeField] private Transform earthShatterSpawnPoint;

    protected override void Update()
    {
        base.Update();
        if (!isControllable || isBusy) return;
        HandleMovement();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1" + playerId.ToString()) && verticalInputRaw == -1 && characterController.isGrounded && !isBusy)
        {
            animator.SetTrigger("EarthShatter");
        }

        if (Input.GetButtonDown("Fire1" + playerId.ToString()) && verticalInputRaw == 0 && !isBusy)
        {
            animator.SetTrigger("Fireball");
        }
    }

    public void Fireball()
    {
        var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
        fireball.playerId = playerId;
    }

    public void EarthShatter()
    {
        var earthShatter = Instantiate(earthShatterPrefab, earthShatterSpawnPoint.position, earthShatterSpawnPoint.rotation);
        earthShatter.playerId = playerId;
    }

}
