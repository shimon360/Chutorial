using UnityEngine;

/// <summary>
/// ゲーム全体で共有する情報を管理するクラス
/// シーンをまたいで保持したい情報をここで管理する。
/// </summary>
public class GameManager : MonoBehaviour
{
    // シングルトン
    public static GameManager Instance { get; private set; }

    [Header("プレイヤー情報")]
    [Tooltip("チーズを取得しているか")]
    public bool HasCheese = false;

    [Header("シーン遷移")]
    [Tooltip("次のシーンで出現するSpawnPointのID")]
    public string NextSpawnPointID = "";

    private void Awake()
    {
        // 既にGameManagerが存在するなら自分を削除
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンが切り替わっても消えない
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ゲーム開始時の状態へ戻す
    /// </summary>
    public void ResetGame()
    {
        HasCheese = false;
        NextSpawnPointID = "";
    }
}