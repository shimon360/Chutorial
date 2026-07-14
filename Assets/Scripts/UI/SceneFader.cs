using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField]
    private CanvasGroup fadeGroup;

    [SerializeField]
    private float fadeDuration = 0.5f;

    private bool isFading = false;

    private bool isFirstScene = true;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isFirstScene)
        {
            isFirstScene = false;

            fadeGroup.alpha = 1f;
            StartCoroutine(FadeIn());
        }
        else
        {
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (isFading)
            return;

        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        isFading = true;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 0f;

        isFading = false;
    }
}