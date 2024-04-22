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
    private int winnerID = -1;

    [SerializeField] private Transform camBoxBound;

    private void Start()
    {
        GameManager.Instance.OnGameEnd.AddListener(id => winnerID = id);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Normal)
        {
            var targets = GameManager.Instance.Players.NotUnityNull();
            if (targets.Count() == 0) return;
            Vector3 target = Vector3.zero;
            target.x = targets.Average(t => t.transform.position.x);
            target.y = targets.Average(t => t.transform.position.y);
            if (camBoxBound)
            {
                target.x = Mathf.Clamp(target.x, camBoxBound.position.x - camBoxBound.localScale.x / 2, camBoxBound.position.x + camBoxBound.localScale.x / 2);
                target.y = Mathf.Clamp(target.y, camBoxBound.position.y - camBoxBound.localScale.y / 2, camBoxBound.position.y + camBoxBound.localScale.y / 2);
            }
            Vector3 targetCamPos = target + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, SMOOTHING * Time.fixedDeltaTime);

            float targetFieldOfView = 60f;
            List<PlayerLogic> t = targets.ToList();
            if (targets.Count() == 2)
            {
                float deltaX = Mathf.Abs(t[0].transform.position.x - t[1].transform.position.x);
                targetFieldOfView = Mathf.Max(60f, deltaX * 5f);
                targetFieldOfView = Mathf.Min(targetFieldOfView, 100);
            }
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, SMOOTHING * Time.fixedDeltaTime);
        }
        else
        {
            if (winnerID == -1) return;
            else
            {
                Vector3 targetCamPos = GameManager.Instance.playerLastGroundedPositions[winnerID] + offset;
                transform.position = Vector3.Lerp(transform.position, targetCamPos, SMOOTHING * Time.fixedDeltaTime);
                float targetFieldOfView = 40f;
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, SMOOTHING * Time.fixedDeltaTime);
            }
        }
    }
}
