using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioClip m_music = null;
    [SerializeField] string m_gameScene = "";

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (m_music != null && !SoundSystem.instance.IsPlayingMusic(m_music))
            SoundSystem.instance.PlayMusic(m_music);
    }

    public void OnStartClick()
    {
        SceneSystem.changeScene(m_gameScene);
    }

    public void OnHowToPlayClick()
    {

    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
