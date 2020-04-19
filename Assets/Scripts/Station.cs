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
    [SerializeField] float m_baseEffectDuration = 5;
    [SerializeField] List<ContiniousEffectData> m_baseContiniousEffects = new List<ContiniousEffectData>();

    List<ContiniousEffect> m_continousEffects = new List<ContiniousEffect>();

    public int lifeSupply
    {
        get { return m_lifeSupply; }
        set
        {
            m_lifeSupply = value;
            if(m_lifeSupply <= 0)
            {
                m_lifeSupply = 0;
                OnDeath();
            }
            if (m_lifeSupply > m_lifeSupplyMax)
                m_lifeSupply = m_lifeSupplyMax;
        }
    }
    public int lifeSupplyMax
    {
        get { return m_lifeSupplyMax; }
        set
        {
            m_lifeSupplyMax = value;
            if (m_lifeSupplyMax < 0)
                m_lifeSupplyMax = 0;
            if (m_lifeSupply > m_lifeSupplyMax)
                m_lifeSupply = m_lifeSupplyMax;
            if (m_lifeSupply <= 0)
                OnDeath();
        }
    }
    public int power
    {
        get { return m_power; }
        set
        {
            m_power = value;
            if(m_power <= 0)
            {
                m_power = 0;
                OnDeath();
            }
            if (m_power > m_powerMax)
                m_power = m_powerMax;
        }
    }
    public int powerMax
    {
        get { return m_powerMax; }
        set
        {
            m_powerMax = value;
            if (m_powerMax < 0)
                m_powerMax = 0;
            if (m_power > m_powerMax)
                m_power = m_powerMax;
            if (m_power <= 0)
                OnDeath();
        }
    }
    public int resource
    {
        get { return m_resource; }
        set
        {
            m_resource = value;
            if (m_resource < 0)
                m_resource = 0;
        }
    }

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
        bool canSuppy = true;

        if (m_resource < data.input.resource)
            canSuppy = false;
        if (m_power < data.input.power)
            canSuppy = false;
        if (m_lifeSupply < data.input.lifeSupply)
            canSuppy = false;

        m_lifeSupply -= data.input.lifeSupply;
        m_power -= data.input.power;
        m_resource -= data.input.resource;

        if (m_lifeSupply < 0)
            m_lifeSupply = 0;
        if (m_power < 0)
            m_power = 0;
        if (m_resource < 0)
            m_resource = 0;

        if (!canSuppy)
            return;

        m_lifeSupply += data.output.lifeSupply;
        m_power += data.output.power;
        m_resource += data.output.resource;

        if (m_lifeSupply > m_lifeSupplyMax)
            m_lifeSupply = m_lifeSupplyMax;
        if (m_power > m_powerMax)
            m_power = m_powerMax;

        if (m_power == 0 || m_lifeSupply == 0)
            OnDeath();
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
        return m_continousEffects[index].timer / m_continousEffects[index].effect.maxTimer;
    }

    public float GetEffectTimerPercent(string name)
    {
        foreach (var e in m_continousEffects)
            if (e.effect.name == name)
                return e.timer / e.effect.maxTimer;
        return 0;
    }

    public void AddEffect(ContiniousEffectData e)
    {
        foreach(var effect in m_continousEffects)
        {
            if (effect.effect.name == e.name)
            {
                effect.effect.input.lifeSupply += e.input.lifeSupply;
                effect.effect.input.power += e.input.power;
                effect.effect.input.resource += e.input.resource;

                effect.effect.output.lifeSupply += e.output.lifeSupply;
                effect.effect.output.power += e.output.power;
                effect.effect.output.resource += e.output.resource;

                return;
            }
        }

        var newEffect = new ContiniousEffect(e);
        newEffect.timer = m_baseEffectDuration;

        m_continousEffects.Add(newEffect);
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

    void OnDeath()
    {

    }
}
