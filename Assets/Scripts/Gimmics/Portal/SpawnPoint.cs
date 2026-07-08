using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private string spawnPointID;

    private void Start()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.NextSpawnPointID != spawnPointID)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = transform.position;
        }
    }
}