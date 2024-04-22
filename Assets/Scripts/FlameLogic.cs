using UnityEngine;

public class FlameLogic : Damage
{
    public override Vector3 knockback => knockbackValue * transform.up;

    private void Start()
    {
        damage = 10f;
        stiffTime = 0.1f;
        knockbackValue = 10f;
        playerId = PlayerId._Neutral;
    }
}