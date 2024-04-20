using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    [SerializeField] private PlayerLogic playerPrefab;
    [SerializeField] private DamageTextLogic damageTextPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] damageTextPlaces;

    private const int PLAYER_COUNT = 2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < PLAYER_COUNT; i++)
        {
            // Init players.
            var player = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            player.playerId = (PlayerId)i;
            player.name = playerPrefab.name + player.playerId.ToString();

            // Init UIs.
            var damageText = Instantiate(damageTextPrefab, damageTextPlaces[i]);
            damageText.transform.localPosition = Vector3.zero;
            damageText.name = "Damage Text " + player.playerId.ToString();
            damageText.SetMaster(player);
        }
    }
}
