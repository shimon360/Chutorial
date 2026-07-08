using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool HasCheese { get; set; }

    public string NextSpawnPointID { get; set; }

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
    }

    public void CollectCheese()
    {
        HasCheese = true;
    }
}