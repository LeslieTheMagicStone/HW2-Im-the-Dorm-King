using UnityEngine;

public class SpikeLogic : Damage
{
    public override Vector3 knockback => knockbackValue * transform.up;
    private BoxCollider boxCollider;
    private float startTime;

    private void Start()
    {
        damage = 30f;
        stiffTime = 0.3f;
        knockbackValue = 20f;
        playerId = PlayerId._Neutral;
        boxCollider = GetComponent<BoxCollider>();
        startTime = Time.time;
    }
}
