using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PortalController : MonoBehaviour
{
    [Header("遷移先シーン名")]
    [SerializeField] private string nextSceneName;

    [Header("チーズ所持時の遷移先（空なら通常遷移）")]
    [SerializeField] private string clearSceneName;

    [Header("操作キー")]
    [SerializeField] private Key interactKey = Key.E;

    [SerializeField] private string destinationSpawnPointID;

    [SerializeField] private GameObject interactUI;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Keyboard.current[interactKey].wasPressedThisFrame)
        {
            // GameManagerに移動先情報を保存
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NextSpawnPointID = destinationSpawnPointID;
            }

            if (GameManager.Instance.HasCheese &&
                !string.IsNullOrEmpty(clearSceneName))
            {
                SceneManager.LoadScene(clearSceneName);
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null)
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
            interactUI.SetActive(false);
        }
    }
}