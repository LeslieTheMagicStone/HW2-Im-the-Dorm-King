using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    public PlayerLogic[] Players => players;
    public Transform boxBound;


    private static GameManager instance;
    private PlayerLogic[] players;
    private DamageTextLogic[] damageTexts;

    [SerializeField] private PlayerLogic playerPrefab;
    [SerializeField] private DamageTextLogic damageTextPrefab;
    [SerializeField] private Transform damageTextParent;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform respawnPlatformPrefab;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private GameObject respawnParticles;

    private const int PLAYER_COUNT = 2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        players = new PlayerLogic[PLAYER_COUNT];
        damageTexts = new DamageTextLogic[PLAYER_COUNT];
        for (int i = 0; i < PLAYER_COUNT; i++)
        {
            SpawnPlayer(i, spawnPoints[i]);
        }
    }

    private void Respawn(PlayerId id)
    {
        int i = (int)id;
        PlayerLogic player = SpawnPlayer(i, respawnPoints[i]);
        Instantiate(respawnParticles, respawnPoints[i].position, respawnPoints[i].rotation);
        Vector3 platformPos = respawnPoints[i].position - player.GetComponent<CharacterController>().center.y * Vector3.up;
        Instantiate(respawnPlatformPrefab, platformPos, respawnPoints[i].rotation);
    }

    PlayerLogic SpawnPlayer(int i, Transform spawnPoint)
    {
        var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.playerId = (PlayerId)i;
        player.name = playerPrefab.name + player.playerId.ToString();
        player.OnDeath.AddListener(Respawn);
        players[i] = player;

        var damageText = Instantiate(damageTextPrefab, damageTextParent);
        damageText.name = "Damage Text " + player.playerId.ToString();
        damageText.SetMaster(player);
        damageTexts[i] = damageText;
        return player;
    }
}
