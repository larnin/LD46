using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] AudioClip m_clickSound = null;

    Slider m_musicSlider = null;
    Slider m_soundSlider = null;

    private void Start()
    {
        var sliders = GetComponentsInChildren<Slider>();
        foreach(var s in sliders)
        {
            var name = s.gameObject.name.ToLower();
            if (name.Contains("music"))
                m_musicSlider = s;
            else if (name.Contains("sound"))
                m_soundSlider = s;
        }

        m_musicSlider.value = Settings.instance.musicVolume;
        m_soundSlider.value = Settings.instance.soundVolume;
    }
    
    public void OnMusicChange()
    {
        Settings.instance.musicVolume = m_musicSlider.value;

        Event<SettingsChangedEvent>.Broadcast(new SettingsChangedEvent());
    }

    public void OnSoundChange()
    {
        Settings.instance.soundVolume = m_soundSlider.value;

        Event<SettingsChangedEvent>.Broadcast(new SettingsChangedEvent());
    }

    public void OnBackPress()
    {
        if (SoundSystem.instance != null)
            SoundSystem.instance.PlaySound(m_clickSound, 0.4f);

        Destroy(gameObject);
    }
}
