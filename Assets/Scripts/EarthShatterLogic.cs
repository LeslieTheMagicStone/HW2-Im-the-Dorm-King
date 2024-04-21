using UnityEngine;

public class EarthShatterLogic : Damage
{
    public override Vector3 knockback => knockbackValue * transform.up;
    private BoxCollider boxCollider;
    private float startTime;
    const float ANIM_TIME = 0.5f;
    const float Z_ORIGIN = 0f;
    const float Z_TARGET = 8f;

    private void Start()
    {
        damage = 12f;
        stiffTime = 0.3f;
        knockbackValue = 15f;
        boxCollider = GetComponent<BoxCollider>();
        startTime = Time.time;
    }

    private void Update()
    {
        var center = boxCollider.center;
        center.z = Z_ORIGIN + (Z_TARGET - Z_ORIGIN) * (Time.time - startTime) / ANIM_TIME;
        boxCollider.center = center;
    }


}
