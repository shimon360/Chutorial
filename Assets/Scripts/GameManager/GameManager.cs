using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool HasCheese { get; set; }

    public string NextSpawnPointID { get; set; }

    public int Score { get; private set; }

    public int CrumbCount { get; private set; }


    [Header("Crumb減速設定")]
    [Tooltip("crumbを1個取った時の速度倍率")]
    [SerializeField]
    private float crumbSlowRate = 0.9f;

    [Tooltip("最低速度倍率")]
    [SerializeField]
    private float minimumSpeedMultiplier = 0.5f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void ResetGame()
    {
        HasCheese = false;
        NextSpawnPointID = "";
        Score = 0;
        CrumbCount = 0;
    }


    public void CollectCheese()
    {
        HasCheese = true;
    }


    public void CollectCrumb(int score)
    {
        Score += score;
        CrumbCount++;
    }


    /// <summary>
    /// 現在のプレイヤー速度倍率を取得
    /// </summary>
    public float GetCurrentSpeedMultiplier()
    {
        float multiplier = Mathf.Pow(crumbSlowRate, CrumbCount);

        return Mathf.Max(multiplier, minimumSpeedMultiplier);
    }
}