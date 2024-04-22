using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerId
{
    _P1,
    _P2,
    _Neutral,
}

public class PlayerLogic : MonoBehaviour
{
    public bool isMovable;
    public bool isBusy;
    public bool isControllable => uncontrollableTime <= 0f;
    public PlayerId playerId;
    public UnityEvent<float> OnDamaged;
    public UnityEvent<PlayerId> OnDeath;
    public Texture portrait;
    public Vector3 lastGroundedPosition;
    public Vector3 lastGroundedRotation;

    [SerializeField] protected GameObject deathParticlePrefab;

    protected float horizontalInput;
    protected float verticalInput;
    protected float verticalInputRaw;
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
        SetMovable(true);
        horizontalFacing = Mathf.Abs(transform.rotation.eulerAngles.y - 90) < 1f ? 1 : -1;
        uncontrollableTime = 0f;
        jumpCount = MAX_JUMP_COUNT;
    }

    protected virtual void Update()
    {
        CheckDeath();

        ResetAnimatorTriggers();

        if (uncontrollableTime > 0)
            uncontrollableTime -= Time.deltaTime;
        else uncontrollableTime = 0;

        animator.SetBool("IsBusy", isBusy);

        if (characterController.isGrounded && !isMovable && isControllable && !isBusy)
            SetMovable(true);

        animator.SetBool("IsMovable", isMovable);
    }

    protected void HandleMovement()
    {
        // Handle movement.
        horizontalInput = Input.GetAxis("Horizontal" + playerId.ToString());
        verticalInput = Input.GetAxis("Vertical" + playerId.ToString());
        verticalInputRaw = Input.GetAxisRaw("Vertical" + playerId.ToString());

        animator.SetFloat("HorizontalInput", horizontalInput);
        animator.SetFloat("VerticalInput", verticalInput);

        if (characterController.isGrounded) jumpCount = MAX_JUMP_COUNT;
        animator.SetBool("IsGrounded", characterController.isGrounded);

        if (Input.GetButtonDown("Jump" + playerId.ToString()) && jumpCount > 0 && !isBusy)
        {
            jumpCount--;
            velocity.y = JUMP_SPEED;
            if (!isMovable) isMovable = true;
            animator.SetTrigger("Jump");
        }

        if (horizontalFacing * horizontalInput < 0 && !isBusy)
            animator.SetTrigger("Turn");

        animator.SetFloat("ForwardInput", horizontalInput * horizontalFacing);
    }

    protected virtual void FixedUpdate()
    {
        if (isMovable && isControllable)
            velocity.x = horizontalInput * SPEED;

        movement = velocity * Time.fixedDeltaTime;

        characterController.Move(movement);

        if (!characterController.isGrounded)
            velocity.y -= GRAVITY * Time.fixedDeltaTime;
        else
            velocity.y = -GRAVITY * Time.fixedDeltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedPosition = transform.position;
            lastGroundedRotation = transform.rotation.eulerAngles;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            if (damage.playerId != playerId)
                GetHurt(damage.damage, damage.stiffTime, damage.knockback);
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetFloat("CKPT_PlayerPosX" + playerId.ToString(), transform.position.x);
        PlayerPrefs.SetFloat("CKPT_PlayerPosY" + playerId.ToString(), transform.position.y);
        PlayerPrefs.SetFloat("CKPT_PlayerPosZ" + playerId.ToString(), transform.position.z);
        PlayerPrefs.SetFloat("CKPT_PlayerRotX" + playerId.ToString(), transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("CKPT_PlayerRotY" + playerId.ToString(), transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("CKPT_PlayerRotZ" + playerId.ToString(), transform.rotation.eulerAngles.z);
    }

    private void ResetAnimatorTriggers()
    {
        animator.ResetTrigger("Turn");
    }

    private void GetHurt(float damage, float stiffTime, Vector3 knockback)
    {
        TakeDamage(damage);
        SetUncontrollable(stiffTime);
        TakeKnockback(knockback);
        AudioManager.Instance.Play("Hurt");
    }

    private void TakeDamage(float damage)
    {
        totalDamge += damage;
        OnDamaged.Invoke(totalDamge);
    }

    private void SetUncontrollable(float time)
    {
        if (time == 0) return;
        uncontrollableTime += time;
        SetMovable(false);
        animator.SetTrigger("Hurt");
    }

    private void TakeKnockback(Vector3 knockback)
    {
        float knockbackFactor = 1 + totalDamge / 100f;
        velocity = knockbackFactor * knockback;
    }

    private void CheckDeath()
    {
        Transform bound = GameManager.Instance.boxBound;
        if (transform.position.x < bound.position.x - bound.localScale.x / 2
        || transform.position.x > bound.position.x + bound.localScale.x / 2
        || transform.position.y > bound.position.y + bound.localScale.y / 2
        || transform.position.y < bound.position.y - bound.localScale.y / 2)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathParticlePrefab != null)
            Instantiate(deathParticlePrefab, transform.position, transform.rotation);
        OnDeath.Invoke(playerId);
        Destroy(gameObject);
    }

    public void SetMovable(bool value)
    {
        isMovable = value;
        if (!value) velocity.x = 0;
    }

    public void SetBusy(bool value)
    {
        isBusy = value;
        SetMovable(!value);
    }

    public void Turn() { StartCoroutine(TurnCoroutine()); }

    private IEnumerator TurnCoroutine()
    {
        horizontalFacing *= -1;

        float timer = TURN_TIME;
        Vector3 rotation = new(0, 180, 0);
        var targetRotation = Quaternion.Euler(rotation) * transform.rotation;
        while (timer > 0)
        {
            transform.Rotate(rotation * Time.deltaTime / TURN_TIME);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = targetRotation;
    }

}
