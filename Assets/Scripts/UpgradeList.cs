using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using NRand;

public class UpgradeList : SerializedMonoBehaviour
{
    [SerializeField] List<UpgradeBase> m_upgrades = new List<UpgradeBase>();
    [SerializeField] int m_upgradeNb = 4;
    [SerializeField] int m_regenCost = 5;
    [SerializeField] float m_increaseCost = 0.2f;
    [SerializeField] float m_moreCost = 0.2f;
    [SerializeField] float m_weightMultiplier = 0.8f;

    static UpgradeList m_instance = null;
    public static UpgradeList instance { get { return m_instance; } }

    List<UpgradeBase> m_availableUpgrades = new List<UpgradeBase>();
    int m_regenUseNb = 0;
    int m_upgradeBought = 0;

    private void Awake()
    {
        m_instance = this;
        GenerateUpgrades();
    }

    public void GenerateUpgrades()
    {
        m_availableUpgrades.Clear();

        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();

        for(int i = 0; i < m_upgradeNb; i++)
        {
            List<float> weighs = new List<float>();
            foreach(var u in m_upgrades)
            {
                if (m_availableUpgrades.Contains(u))
                    weighs.Add(0);
                else weighs.Add(u.GetWeight());
            }
            var gen = new DiscreteDistribution(weighs);

            m_availableUpgrades.Add(m_upgrades[gen.Next(rand)]);
        }
    }

    public int GetRegenCost()
    {
        return (int)(m_regenCost * (1 + m_increaseCost * m_regenUseNb) * Mathf.Pow(1 + m_moreCost, m_regenUseNb));
    }

    public void UseRegen()
    {
        var station = Station.instance;
        if (station == null)
            return;

        station.resource -= GetRegenCost();
        m_regenUseNb++;

        GenerateUpgrades();
    }

    public void UseUpgrade(UpgradeBase upgrade)
    {
        upgrade.ApplyCost();
        upgrade.IncreaseCost(m_increaseCost, m_moreCost, m_weightMultiplier);
        upgrade.ApplyEffect();

        m_upgradeBought++;

        Event<UpdateStationModulesEvent>.Broadcast(new UpdateStationModulesEvent(m_upgradeBought));
    }

    public int GetUpgradeNb()
    {
        return m_availableUpgrades.Count;
    }

    public UpgradeBase GetUpgrade(int index)
    {
        if (index < 0 || index >= m_availableUpgrades.Count)
            return null;
        return m_availableUpgrades[index];
    }
}
