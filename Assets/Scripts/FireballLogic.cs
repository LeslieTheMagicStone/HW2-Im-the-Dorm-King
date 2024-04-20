using UnityEngine;

public class FireballLogic : MonoBehaviour
{
    public PlayerId playerId;
    Rigidbody rb;
    const float SPEED = 4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = SPEED * transform.forward;
    }
}
