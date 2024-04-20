using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerId
{
    _P1,
    _P2,
}

public class PlayerLogic : MonoBehaviour
{
    public bool isMovable;
    public bool isBusy;
    public bool isControllable => uncontrollableTime <= 0f;
    public PlayerId playerId;
    public UnityEvent<float> OnHurt;
    public Texture portrait;

    protected float horizontalInput;
    protected float verticalInput;
    protected int horizontalFacing;
    protected float totalDamge;
    protected float uncontrollableTime;
    protected int jumpCount;

    protected CharacterController characterController;
    protected Animator animator;
    protected Vector3 movement;
    protected Vector3 velocity;

    protected const float SPEED = 3f;
    protected const float GRAVITY = 30f;
    protected const float JUMP_SPEED = 12f;
    protected const int MAX_JUMP_COUNT = 2;
    protected const float TURN_TIME = 0.2f;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isMovable = true;
        horizontalFacing = 1;
        uncontrollableTime = 0f;
        jumpCount = MAX_JUMP_COUNT;
    }

    protected virtual void Update()
    {
        ResetAnimatorTriggers();

        if (uncontrollableTime > 0)
            uncontrollableTime -= Time.deltaTime;
        else uncontrollableTime = 0;
        animator.SetFloat("UncontrollableTime", uncontrollableTime);

        animator.SetBool("IsBusy", isBusy);
    }

    protected void HandleMovement()
    {
        // Handle movement.
        horizontalInput = Input.GetAxis("Horizontal" + playerId.ToString());
        verticalInput = Input.GetAxis("Vertical" + playerId.ToString());

        animator.SetFloat("HorizontalInput", horizontalInput);

        if (characterController.isGrounded) jumpCount = MAX_JUMP_COUNT;
        animator.SetBool("IsGrounded", characterController.isGrounded);

        if (Input.GetButtonDown("Jump" + playerId.ToString()) && jumpCount > 0 && !isBusy)
        {
            jumpCount--;
            velocity.y = JUMP_SPEED;
            animator.SetTrigger("Jump");
        }

        if (horizontalFacing * horizontalInput < 0 && !isBusy)
            animator.SetTrigger("Turn");
    }

    protected virtual void FixedUpdate()
    {
        if (isMovable && isControllable)
            velocity.x = horizontalInput * SPEED;
        else velocity.x = 0;

        movement = velocity * Time.fixedDeltaTime;

        characterController.Move(movement);

        if (!characterController.isGrounded)
            velocity.y -= GRAVITY * Time.fixedDeltaTime;
        else
            velocity.y = -GRAVITY * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FireballLogic fireballLogic))
        {
            if (fireballLogic.playerId != playerId)
                GetHurt(10.0f, 0.3f);
        }
    }

    private void ResetAnimatorTriggers()
    {
        animator.ResetTrigger("Turn");
    }

    private void GetHurt(float damage, float stiffTime)
    {
        TakeDamage(damage);
        SetUncontrollableTime(stiffTime);
        animator.SetTrigger("Hurt");
        OnHurt.Invoke(totalDamge);
    }

    private void TakeDamage(float damage)
    {
        totalDamge += damage;
    }

    private void SetUncontrollableTime(float value)
    {
        uncontrollableTime += value;
    }

    public void SetMovable(bool value)
    {
        isMovable = value;
    }

    public void SetBusy(bool value)
    {
        isBusy = value;
    }

    public void Turn() { StartCoroutine(TurnCoroutine()); }

    private IEnumerator TurnCoroutine()
    {
        horizontalFacing *= -1;

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
