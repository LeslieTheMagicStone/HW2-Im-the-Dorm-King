using UnityEngine;

public enum PlayerId
{
    _P1,
    _P2,
}

public class PlayerLogic : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    int jumpCount;

    CharacterController characterController;
    Vector3 movement;
    Vector3 velocity;

    [SerializeField] PlayerId playerId;

    const float SPEED = 3f;
    const float GRAVITY = 30f;
    const float JUMP_SPEED = 12f;
    const int MAX_JUMP_COUNT = 2;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        jumpCount = MAX_JUMP_COUNT;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal" + playerId.ToString());
        verticalInput = Input.GetAxis("Vertical" + playerId.ToString());

        if (characterController.isGrounded) jumpCount = MAX_JUMP_COUNT;

        if (Input.GetButtonDown("Jump" + playerId.ToString()) && jumpCount > 0)
        {
            jumpCount--;
            velocity.y = JUMP_SPEED;
        }
    }

    private void FixedUpdate()
    {
        velocity.x = horizontalInput * SPEED;

        movement = velocity * Time.fixedDeltaTime;

        characterController.Move(movement);

        if (!characterController.isGrounded)
            velocity.y -= GRAVITY * Time.fixedDeltaTime;
        else
            velocity.y = -GRAVITY * Time.fixedDeltaTime;
    }
}
