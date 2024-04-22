using UnityEngine;

public class FlameLogic : Damage
{
    public override Vector3 knockback => knockbackValue * transform.up;

    private void Start()
    {
        damage = 5f;
        stiffTime = 0.1f;
        knockbackValue = 8f;
        playerId = PlayerId._Neutral;
    }
}