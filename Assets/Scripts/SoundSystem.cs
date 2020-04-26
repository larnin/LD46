using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class SoundSystem : MonoBehaviour
{
    class LoopData
    {
        public int id = -1;
        public int sourceIndex = -1;
        public float volume = 0;
        public float maxTimer = 0;
        public float timer = 0;
        public bool fadeout = false;
    }

    static SoundSystem m_instance;

    public static SoundSystem instance { get { return m_instance; } }

    AudioSource[] m_sources;
    AudioSource m_musicSource1;
    AudioSource m_musicSource2;
    bool m_currentSource1 = true;

    List<LoopData> m_loops = new List<LoopData>();
    int m_nextLoopID = 0;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_sources = transform.Find("Sounds").GetComponentsInChildren<AudioSource>();
        m_musicSource1 = transform.Find("Music1").GetComponent<AudioSource>();
        m_musicSource2 = transform.Find("Music2").GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f, float transitionTime = 1)
    {
        if (clip == null)
            return;

        if (m_currentSource1)
        {
            m_musicSource1.DOFade(0, transitionTime).OnComplete(() =>
            {
                if (this != null)
                    m_musicSource1.Stop();
            });
            m_musicSource2.clip = clip;
            m_musicSource2.volume = 0;
            m_musicSource2.Play();
            m_musicSource2.DOFade(volume, transitionTime);
        }
        else
        {
            m_musicSource2.DOFade(0, transitionTime).OnComplete(() =>
            {
                if (this != null)
                    m_musicSource2.Stop();
            }); ;
            m_musicSource1.clip = clip;
            m_musicSource1.volume = 0;
            m_musicSource1.Play();
            m_musicSource1.DOFade(volume, transitionTime);
        }

        m_currentSource1 = !m_currentSource1;
    }

    public bool IsPlayingMusic(AudioClip clip)
    {
        if (m_currentSource1 && m_musicSource1.clip == clip)
            return true;
        if (!m_currentSource1 && m_musicSource2.clip == clip)
            return true;
        return false;
    }

    public void PlaySound(AudioClip clip, float volume = 0.5f, bool force = true)
    {
        if (clip == null)
            return;

        bool canBeAdded = true;
        if (!force)
        {
            foreach (var s in m_sources)
            {
                if (s.isPlaying && clip == s.clip)
                {
                    canBeAdded = false;
                    break;
                }
            }
        }

        if (!canBeAdded)
            return;

        foreach (var s in m_sources)
        {
            if (!s.isPlaying)
            {
                s.clip = clip;
                s.volume = volume;
                s.loop = false;
                s.Play();
                return;
            }
        }
    }

    public bool IsPlayingSound(AudioClip clip)
    {
        foreach (var s in m_sources)
        {
            if (s.isPlaying && s.clip == clip)
                return true;
        }

        return false;
    }

    public int PlayLoop(AudioClip clip, float volume = 0.5f, float fade = 0.5f)
    {
        if (clip == null)
            return -1;

        if (fade < 0.001f)
            fade = 0.001f;

        for(int i = 0; i < m_sources.Length; i++)
        {
            var s = m_sources[i];
            if (!s.isPlaying)
            {
                s.clip = clip;
                s.volume = 0;
                s.loop = true;
                s.Play();

                var loop = new LoopData();
                loop.id = m_nextLoopID ++;
                loop.volume = volume;
                loop.timer = 0;
                loop.maxTimer = fade;
                if (m_nextLoopID >= int.MaxValue)
                    m_nextLoopID = 0;
                loop.sourceIndex = i;
                m_loops.Add(loop);

                return loop.id;
            }
        }

        return -1;
    }

    public void StopLoop(int id, float fade)
    {
        if (id < 0)
            return;

        if (fade < 0.001f)
            fade = 0.001f;

        foreach(var l in m_loops)
        {
            if(l.id == id && !l.fadeout)
            {
                l.fadeout = true;
                l.timer = 0;
                l.maxTimer = fade;
            }
        }
    }

    private void Update()
    {
        for(int i = 0; i < m_loops.Count; i++)
        {
            var l = m_loops[i];

            var s = m_sources[l.sourceIndex];
            l.timer += Time.deltaTime;

            if(!l.fadeout)
            {
                float volume = 1;
                if (l.timer < l.maxTimer)
                    volume = l.timer / l.maxTimer;
                volume *= l.volume;
                s.volume = volume;
            }
            else
            {
                float volume = 1 - (l.timer / l.maxTimer);
                if(volume <= 0)
                {
                    s.volume = 0;
                    s.Stop();
                    m_loops.RemoveAt(i);
                    i++;
                    continue;
                }

                s.volume = volume * l.volume;
            }
        }
    }
}