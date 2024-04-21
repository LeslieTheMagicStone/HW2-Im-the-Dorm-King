using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    const float Y_OFFSET = 1.5f;
    const float Z_OFFSET = -10f;
    const float SMOOTHING = 5f;
    private Vector3 offset = new(0, Y_OFFSET, Z_OFFSET);
    [SerializeField] private Transform camBoxBound;

    void FixedUpdate()
    {
        var targets = GameManager.Instance.Players.NotNull();
        Vector3 target = Vector3.zero;
        target.x = targets.Average(t => t.transform.position.x);
        target.y = targets.Average(t => t.transform.position.y);
        target.x = Mathf.Clamp(target.x, camBoxBound.position.x - camBoxBound.localScale.x / 2, camBoxBound.position.x + camBoxBound.localScale.x / 2);
        target.y = Mathf.Clamp(target.y, camBoxBound.position.y - camBoxBound.localScale.y / 2, camBoxBound.position.y + camBoxBound.localScale.y / 2);
        Vector3 targetCamPos = target + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, SMOOTHING * Time.fixedDeltaTime);
    }
}
