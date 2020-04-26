using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioClip m_music = null;
    [SerializeField] string m_gameScene = "";
    [SerializeField] AudioClip m_startSound = null;

    private void Awake()
    {
        
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

    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
