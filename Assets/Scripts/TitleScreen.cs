using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Button playButton;
    public Button loadButton;
    public Button settingsButton;
    public Button exitButton;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Prison");
    }

    private void OnLoadButtonClick()
    {
        // Load game somehow
    }

    private void OnSettingsButtonClick()
    {
        //
    }

    private void OnExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
