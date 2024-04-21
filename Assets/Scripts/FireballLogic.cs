using UnityEngine;

public class FireballLogic : Damage
{
    Rigidbody rb;
    const float SPEED = 4f;
    const float FADE_OUT_TIME = 0.5f;
    [SerializeField] private GameObject boomPrefab;

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
                Boom();
        }
    }

    private void Boom()
    {
        Instantiate(boomPrefab, transform.position, transform.rotation);
        StartCoroutine(AudioManager.StartFade(GetComponent<AudioSource>(), FADE_OUT_TIME, 0f));
        rb.velocity = Vector3.zero;
        Destroy(gameObject, FADE_OUT_TIME + 0.1f);
    }
}
