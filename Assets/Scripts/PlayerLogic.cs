using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum PlayerId
{
    _P1,
    _P2,
}

public class PlayerLogic : MonoBehaviour
{
    public bool isMovable;
    public bool isBusy;

    protected float horizontalInput;
    protected float verticalInput;
    protected int horizontalFacing;

    protected int jumpCount;

    protected CharacterController characterController;
    protected Animator animator;
    protected Vector3 movement;
    protected Vector3 velocity;

    [SerializeField] protected PlayerId playerId;

    protected const float SPEED = 3f;
    protected const float GRAVITY = 30f;
    protected const float JUMP_SPEED = 12f;
    protected const int MAX_JUMP_COUNT = 2;
    protected const float TURN_TIME = 0.2f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isMovable = true;
        horizontalFacing = 1;
        jumpCount = MAX_JUMP_COUNT;
    }

    protected void HandleMovement()
    {
        // Handle movement.
        horizontalInput = Input.GetAxis("Horizontal" + playerId.ToString());
        verticalInput = Input.GetAxis("Vertical" + playerId.ToString());

        animator.SetFloat("HorizontalInput", horizontalInput);

        if (characterController.isGrounded) jumpCount = MAX_JUMP_COUNT;
        animator.SetBool("IsGrounded", characterController.isGrounded);

        if (Input.GetButtonDown("Jump" + playerId.ToString()) && jumpCount > 0)
        {
            jumpCount--;
            velocity.y = JUMP_SPEED;
            animator.SetTrigger("Jump");
        }
    }

    private void FixedUpdate()
    {
        if (isMovable)
        {
            velocity.x = horizontalInput * SPEED;
            if (horizontalFacing * horizontalInput < 0 && !isBusy)
            {
                horizontalFacing *= -1;
                animator.SetTrigger("Turn");
                StartCoroutine(TurnCoroutine());
            }
        }
        else velocity.x = 0;

        movement = velocity * Time.fixedDeltaTime;

        characterController.Move(movement);

        if (!characterController.isGrounded)
            velocity.y -= GRAVITY * Time.fixedDeltaTime;
        else
            velocity.y = -GRAVITY * Time.fixedDeltaTime;
    }

    public void SetMovable(bool value)
    {
        isMovable = value;
    }

    public void SetBusy(bool value)
    {
        isBusy = value;
    }

    public IEnumerator TurnCoroutine()
    {
        float timer = TURN_TIME;
        Vector3 rotation = new(0, 180, 0);
        var targetRotation = quaternion.Euler(rotation * Mathf.Deg2Rad) * transform.rotation;
        while (timer > 0)
        {
            transform.Rotate(rotation * Time.deltaTime / TURN_TIME);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = targetRotation;
    }
}
