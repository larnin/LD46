using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Station : MonoBehaviour
{
    class ContiniousEffect
    {
        public ContiniousEffectData effect;
        public float timer;

        public ContiniousEffect(ContiniousEffectData _effect, float _timer = 0)
        {
            effect = _effect;
            timer = _timer;
        }
    }

    static Station m_instance;
    public static Station instance { get { return m_instance; } }

    [SerializeField] int m_lifeSupply = 0;
    [SerializeField] int m_lifeSupplyMax = 0;
    [SerializeField] int m_power = 0;
    [SerializeField] int m_powerMax = 0;
    [SerializeField] int m_resource = 0;
    [SerializeField] List<ContiniousEffectData> m_baseContiniousEffects;

    List<ContiniousEffect> m_continousEffects;

    void Awake()
    {
        m_instance = this;

        foreach(var e in m_baseContiniousEffects)
            m_continousEffects.Add(new ContiniousEffect(e));
    }
    
    void Update()
    {
        foreach(var e in m_continousEffects)
        {
            e.timer += Time.deltaTime;
            if(e.timer >= e.effect.maxTimer)
            {
                ApplyEffec(e.effect);
                e.timer = 0;
            }
        }
    }

    void ApplyEffec(ContiniousEffectData data)
    {
        if (m_resource < data.input.resource)
            return;

        m_lifeSupply -= data.input.lifeSupply;
        m_power -= data.input.power;
        m_resource -= data.input.resource;

        m_lifeSupply += data.output.lifeSupply;
        m_power += data.output.power;
        m_resource += data.output.resource;

        if (m_lifeSupply < 0)
            m_lifeSupply = 0;
        if (m_power < 0)
            m_power = 0;
        if (m_resource < 0)
            m_resource = 0;
        if (m_lifeSupply > m_lifeSupplyMax)
            m_lifeSupply = m_lifeSupplyMax;
        if (m_power > m_powerMax)
            m_power = m_powerMax;

        if(m_power == 0 || m_lifeSupply == 0)
        {
            //todo death
        }
    }

    public int GetEffectNb()
    {
        return m_continousEffects.Count;
    }

    public ContiniousEffectData GetEffect(int index)
    {
        if (index >= GetEffectNb())
            return null;
        return m_continousEffects[index].effect;
    }

    public ContiniousEffectData GetEffect(string name)
    {
        foreach (var e in m_continousEffects)
            if (e.effect.name == name)
                return e.effect;
        return null;
    }

    public float GetEffectTimerPercent(int index)
    {
        if (index >= GetEffectNb())
            return 0;
        return m_continousEffects[index].timer;
    }

    public float GetEffectTimerPercent(string name)
    {
        foreach (var e in m_continousEffects)
            if (e.effect.name == name)
                return e.timer;
        return 0;
    }

    public void AddEffect(ContiniousEffectData e)
    {
        foreach(var effect in m_continousEffects)
        {
            if (effect.effect.name == e.name)
            {
                effect.effect = e;
                return;
            }
        }

        m_continousEffects.Add(new ContiniousEffect(e));
    }
    
    public void RemoveEffect(int index)
    {
        if (index >= GetEffectNb())
            return;

        m_continousEffects.RemoveAt(index);
    }

    public void RemoveEffect(string name)
    {
        for(int i = 0; i < m_continousEffects.Count; i++)
        {
            if(m_continousEffects[i].effect.name == name)
            {
                m_continousEffects.RemoveAt(i);
                return;
            }
        }
    }
}
