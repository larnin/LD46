using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ResumeUI : MonoBehaviour
{
    Image m_lifeGauge;
    Image m_powerGauge;
    TextMeshProUGUI m_cargo;

    void Awake()
    {
        var parent = transform.Find("Background");
        m_lifeGauge = parent.Find("LifeGauge").GetComponent<Image>();
        m_powerGauge = parent.Find("PowerGauge").GetComponent<Image>();
        m_cargo = parent.Find("CargoValue").GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        var station = Station.instance;
        if(station != null)
        {
            float life = (float)station.lifeSupply / station.lifeSupplyMax;
            m_lifeGauge.fillAmount = life;
            m_lifeGauge.color = life < 0.21f ? Color.red : Color.white;

            float power = (float)station.power / station.powerMax;
            m_powerGauge.fillAmount = power;
            m_powerGauge.color = power < 0.21f ? Color.red : Color.white;
        }

        var mining = MiningBehaviour.instance;
        if(mining != null)
            m_cargo.text = mining.GetCargo() + " / " + mining.GetMaxCargo();
    }
}
