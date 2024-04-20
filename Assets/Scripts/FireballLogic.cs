using UnityEngine;

public class FireballLogic : Damage
{
    Rigidbody rb;
    const float SPEED = 4f;

    private void Start()
    {
        damage = 10.0f;
        stiffTime = 0.3f;
        knockbackValue = 5f;

        rb = GetComponent<Rigidbody>();
        rb.velocity = SPEED * transform.forward;

        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerLogic playerLogic))
        {
            if (playerLogic.playerId != playerId)
                Destroy(gameObject);
        }
    }
}
