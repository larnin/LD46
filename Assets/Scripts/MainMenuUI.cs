using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioClip m_music = null;
    [SerializeField] string m_gameScene = "";
    [SerializeField] AudioClip m_startSound = null;
    [SerializeField] AudioClip m_clickSound = null;
    [SerializeField] GameObject m_settingsPrefab = null;

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_WEBGL
        var quitButton = transform.Find("Quit");
        if (quitButton != null)
            Destroy(quitButton.gameObject);
#endif
    }

    private void Start()
    {
        if (m_music != null && SoundSystem.instance != null && !SoundSystem.instance.IsPlayingMusic(m_music))
            SoundSystem.instance.PlayMusic(m_music);
    }

    public void OnStartClick()
    {
        SceneSystem.changeScene(m_gameScene);
        if(SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_startSound, 0.5f);
    }

    public void OnHowToPlayClick()
    {
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);


    }

    public void OnSettingsClick()
    {
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);
        Instantiate(m_settingsPrefab);
    }

    public void OnExitClick()
    {
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);
        Application.Quit();
    }
}
