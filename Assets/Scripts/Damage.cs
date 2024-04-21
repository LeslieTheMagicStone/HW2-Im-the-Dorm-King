using UnityEngine;

public class Damage : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float stiffTime;
    [HideInInspector] public float knockbackValue;
    [HideInInspector] public PlayerId playerId;
    public virtual Vector3 knockback => knockbackValue * (transform.up + transform.forward).normalized;
    protected const float KNOCKBACK_UPWARD_ANGLE_OFFSET = 45f;

}
