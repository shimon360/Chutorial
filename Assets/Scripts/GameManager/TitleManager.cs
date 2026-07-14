using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    private void Update()
    {
        bool start =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        if (Keyboard.current != null)
        {
            start |= Keyboard.current.enterKey.wasPressedThisFrame;
            start |= Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        if (!start)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        SceneFader.Instance.FadeToScene("Shelter");
    }
}