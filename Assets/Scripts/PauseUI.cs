using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    [SerializeField] AudioClip m_clickSound = null;
    [SerializeField] AudioClip m_openSound = null;

    private void Awake()
    {
        if(SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_openSound, 0.4f);
        Time.timeScale = 0;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void OnBack()
    {
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);
        Destroy(gameObject);
    }

    public void OnMainMenu()
    {
        SceneSystem.changeScene("MainMenu");
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);
    }
}
