using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Normal,
    GameEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    public PlayerLogic[] Players => players;
    public Transform boxBound;
    public GameState gameState = GameState.Normal;
    public UnityEvent<int> OnGameEnd;


    private static GameManager instance;
    private PlayerLogic[] players;
    private DamageTextLogic[] damageTexts;
    private CoinLogic[] coinLogics;
    public Vector3[] playerLastGroundedPositions;
    private Vector3[] playerLastGroundedRotations;
    private int[] coinCounts;
    private float startTime;

    [SerializeField] private PlayerLogic playerPrefab;
    [SerializeField] private DamageTextLogic damageTextPrefab;
    [SerializeField] private Transform damageTextParent;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform respawnPlatformPrefab;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private GameObject respawnParticles;
    [SerializeField] private TMP_Text[] coinCountTexts;
    [SerializeField] private TMP_Text timerText;

    private const int PLAYER_COUNT = 2;
    private const float RESPAWN_INTERVAL = 1f;
    private const float MAX_TIME_SECONDS = 120f;

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
        playerLastGroundedPositions = new Vector3[PLAYER_COUNT];
        playerLastGroundedRotations = new Vector3[PLAYER_COUNT];
        coinCounts = new int[PLAYER_COUNT];
        coinLogics = FindObjectsOfType<CoinLogic>();
        startTime = Time.time;

        LoadGame();
    }

    private void Update()
    {
        if (gameState == GameState.GameEnd)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        float currentTime = Time.time - startTime;
        float timerTime = MAX_TIME_SECONDS - currentTime;
        int timerSeconds = (int)timerTime;
        if (timerTime < 0f)
        {
            timerTime = 0f;
            // TODO: More player logics.
            int winnerID = coinCounts[0] > coinCounts[1] ? 0 : (coinCounts[0] == coinCounts[1] ? -1 : 1);
            PlayerPrefs.SetInt("Winner", winnerID);
            gameState = GameState.GameEnd;
            OnGameEnd.Invoke(winnerID);


            ResetGame(true);
        }
        timerText.text = timerSeconds.ToString() + ":" + ((int)((timerTime - timerSeconds) * 60f)).ToString("D2");

    }

    private void FixedUpdate()
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i] != null)
            {
                playerLastGroundedPositions[i] = players[i].lastGroundedPosition;
                playerLastGroundedRotations[i] = players[i].lastGroundedRotation;
            }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Respawn(PlayerId id, Vector3 position, Quaternion rotation)
    {
        StartCoroutine(RespawnCoroutine(id, position, rotation));
    }

    private IEnumerator RespawnCoroutine(PlayerId id, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(RESPAWN_INTERVAL);

        int i = (int)id;
        PlayerLogic player = SpawnPlayer(i, position, rotation);
        Instantiate(respawnParticles, position, rotation);
        Vector3 platformPos = position - player.GetComponent<CharacterController>().center.y * Vector3.up;
        Instantiate(respawnPlatformPrefab, platformPos, rotation);
    }

    private PlayerLogic SpawnPlayer(int i, Transform spawnPoint)
    {
        return SpawnPlayer(i, spawnPoint.position, spawnPoint.rotation);
    }

    private PlayerLogic SpawnPlayer(int i, Vector3 position, Quaternion rotation)
    {
        var player = Instantiate(playerPrefab, position, rotation);
        player.playerId = (PlayerId)i;
        player.name = playerPrefab.name + player.playerId.ToString();
        player.OnDeath.AddListener(LoadPlayerCKPT);
        players[i] = player;

        if (damageTextPrefab)
        {
            var damageText = Instantiate(damageTextPrefab, damageTextParent);
            damageText.name = "Damage Text " + player.playerId.ToString();
            damageText.SetMaster(player);
            damageTexts[i] = damageText;
        }

        return player;
    }

    private void SavePlayer(PlayerId id)
    {
        int i = (int)id;
        PlayerPrefs.SetFloat("PlayerPosX" + id.ToString(), playerLastGroundedPositions[i].x);
        PlayerPrefs.SetFloat("PlayerPosY" + id.ToString(), playerLastGroundedPositions[i].y);
        PlayerPrefs.SetFloat("PlayerPosZ" + id.ToString(), playerLastGroundedPositions[i].z);
        PlayerPrefs.SetFloat("PlayerRotX" + id.ToString(), playerLastGroundedRotations[i].x);
        PlayerPrefs.SetFloat("PlayerRotY" + id.ToString(), playerLastGroundedRotations[i].y);
        PlayerPrefs.SetFloat("PlayerRotZ" + id.ToString(), playerLastGroundedRotations[i].z);
        PlayerPrefs.Save();
    }

    private void ResetPlayer(PlayerId id)
    {
        int i = (int)id;
        PlayerPrefs.SetFloat("PlayerPosX" + id.ToString(), spawnPoints[i].transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY" + id.ToString(), spawnPoints[i].transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ" + id.ToString(), spawnPoints[i].transform.position.z);
        PlayerPrefs.SetFloat("PlayerRotX" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("PlayerRotY" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("PlayerRotZ" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.z);
        PlayerPrefs.SetFloat("CKPT_PlayerPosX" + id.ToString(), spawnPoints[i].transform.position.x);
        PlayerPrefs.SetFloat("CKPT_PlayerPosY" + id.ToString(), spawnPoints[i].transform.position.y);
        PlayerPrefs.SetFloat("CKPT_PlayerPosZ" + id.ToString(), spawnPoints[i].transform.position.z);
        PlayerPrefs.SetFloat("CKPT_PlayerRotX" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("CKPT_PlayerRotY" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("CKPT_PlayerRotZ" + id.ToString(), spawnPoints[i].transform.rotation.eulerAngles.z);
        PlayerPrefs.Save();
    }

    private void LoadPlayer(PlayerId id)
    {
        float playerPosX = PlayerPrefs.GetFloat("PlayerPosX" + id.ToString());
        float playerPosY = PlayerPrefs.GetFloat("PlayerPosY" + id.ToString());
        float playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ" + id.ToString());

        float playerRotX = PlayerPrefs.GetFloat("PlayerRotX" + id.ToString());
        float playerRotY = PlayerPrefs.GetFloat("PlayerRotY" + id.ToString());
        float playerRotZ = PlayerPrefs.GetFloat("PlayerRotZ" + id.ToString());

        var position = new Vector3(playerPosX, playerPosY, playerPosZ);
        var rotation = Quaternion.Euler(playerRotX, playerRotY, playerRotZ);

        SpawnPlayer((int)id, position, rotation);
    }

    private void LoadPlayerCKPT(PlayerId id)
    {
        float playerPosX = PlayerPrefs.GetFloat("CKPT_PlayerPosX" + id.ToString());
        float playerPosY = PlayerPrefs.GetFloat("CKPT_PlayerPosY" + id.ToString());
        float playerPosZ = PlayerPrefs.GetFloat("CKPT_PlayerPosZ" + id.ToString());

        float playerRotX = PlayerPrefs.GetFloat("CKPT_PlayerRotX" + id.ToString());
        float playerRotY = PlayerPrefs.GetFloat("CKPT_PlayerRotY" + id.ToString());
        float playerRotZ = PlayerPrefs.GetFloat("CKPT_PlayerRotZ" + id.ToString());

        var position = new Vector3(playerPosX, playerPosY, playerPosZ);
        var rotation = Quaternion.Euler(playerRotX, playerRotY, playerRotZ);

        Respawn(id, position + new Vector3(0, 5f, 0), rotation);
    }

    public void AddCoin(PlayerId id)
    {
        int i = (int)id;
        coinCounts[i]++;
        coinCountTexts[i].text = "X " + coinCounts[i].ToString();
    }

    public void ResetGame(bool outro = false)
    {
        StartCoroutine(ResetGameCoroutine(outro));
    }

    private IEnumerator ResetGameCoroutine(bool outro)
    {
        for (int i = 0; i < coinLogics.Length; i++)
            coinLogics[i].ResetSave(i);
        for (int i = 0; i < players.Length; i++)
            ResetPlayer((PlayerId)i);
        for (int i = 0; i < players.Length; i++)
            PlayerPrefs.SetInt("CoinCount" + ((PlayerId)i).ToString(), 0);
        PlayerPrefs.Save();

        if (outro)
        {
            yield return VignetteLogic.Instance.Outro();
            yield return new WaitForSeconds(1);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetGameWithOutro()
    {
        for (int i = 0; i < coinLogics.Length; i++)
            coinLogics[i].ResetSave(i);
        for (int i = 0; i < players.Length; i++)
            ResetPlayer((PlayerId)i);
        for (int i = 0; i < players.Length; i++)
            PlayerPrefs.SetInt("CoinCount" + ((PlayerId)i).ToString(), 0);
        PlayerPrefs.Save();

        StartCoroutine(VignetteLogic.Instance.Outro());
    }

    public void SaveGame()
    {
        for (int i = 0; i < coinLogics.Length; i++)
            coinLogics[i].Save(i);
        for (int i = 0; i < players.Length; i++)
            SavePlayer((PlayerId)i);
        for (int i = 0; i < players.Length; i++)
            PlayerPrefs.SetInt("CoinCount" + ((PlayerId)i).ToString(), coinCounts[i]);
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        for (int i = 0; i < coinLogics.Length; i++)
            coinLogics[i].Load(i);
        for (int i = 0; i < players.Length; i++)
            LoadPlayer((PlayerId)i);
        for (int i = 0; i < players.Length; i++)
            coinCounts[i] = PlayerPrefs.GetInt("CoinCount" + ((PlayerId)i).ToString(), 0);
    }
}
