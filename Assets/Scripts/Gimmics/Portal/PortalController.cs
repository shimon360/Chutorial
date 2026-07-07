using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PortalController : MonoBehaviour
{
    [Header("遷移先シーン名")]
    [SerializeField] private string nextSceneName;

    [Header("操作キー")]
    [SerializeField] private Key interactKey = Key.E;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Keyboard.current[interactKey].wasPressedThisFrame)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}