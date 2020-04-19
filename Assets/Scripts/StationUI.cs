using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StationUI : MonoBehaviour
{
    class StatusData
    {
        public Image gauge;
        public TextMeshProUGUI value;
    }

    class ContiniousEffectData
    {
        public GameObject obj;
        public TextMeshProUGUI title;
        public TextMeshProUGUI effect;
        public Image gauge;
    }

    class UpgradeData
    {
        public GameObject obj;
        public Button button;
        public TextMeshProUGUI description;
        public TextMeshProUGUI cost;
    }

    class RerollData
    {
        public Button button;
        public TextMeshProUGUI label;
    }

    [SerializeField] Color m_activeColor = Color.white;
    [SerializeField] Color m_disabledColor = Color.grey;

    bool m_statusPageEnabled = true;

    Button m_statusButton = null;
    Button m_upgradesButton = null;

    GameObject m_statusPage = null;
    GameObject m_upgradesPage = null;

    StatusData m_lifeSupply = new StatusData();
    StatusData m_power = new StatusData();
    TextMeshProUGUI m_resources = null;
    List<ContiniousEffectData> m_continiousEffects = new List<ContiniousEffectData>();
    List<UpgradeData> m_upgrades = new List<UpgradeData>();
    RerollData m_reroll = new RerollData();

    void Awake()
    {
        InitData();
        UpdateInterface();
    }

    void Update()
    {
        UpdateGauges();
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    public void ClickStatus()
    {
        m_statusPageEnabled = true;
        UpdateInterface();
    }

    public void ClickUpgrade()
    {
        m_statusPageEnabled = false;
        UpdateInterface();
    }

    public void BuyUpgrade(int index)
    {

    }

    public void BuyReroll()
    {

    }

    void InitData()
    {
        m_statusButton = transform.Find("StatusButton").GetComponent<Button>();
        m_upgradesButton = transform.Find("UpgradesButton").GetComponent<Button>();

        var statusPage = transform.Find("Status");
        m_statusPage = statusPage.gameObject;
        InitStatusData(statusPage.Find("Life"), m_lifeSupply);
        InitStatusData(statusPage.Find("Power"), m_power);

        m_resources = statusPage.Find("Resource").Find("Value").GetComponent<TextMeshProUGUI>();

        for(int i = 0; i < statusPage.childCount; i++)
        {
            var obj = statusPage.GetChild(i).gameObject;
            if (obj.name.Contains("Continious"))
                AddContiniousEffect(obj.transform);
        }

        var upgradesPage = transform.Find("Upgrades");
        m_upgradesPage = upgradesPage.gameObject;

        for(int i = 0; i < upgradesPage.childCount; i++)
        {
            var obj = upgradesPage.GetChild(i).gameObject;
            if (obj.name.Contains("Upgrade"))
                AddUpgrade(obj.transform);
        }

        var reroll = upgradesPage.Find("Reroll");
        m_reroll.button = reroll.GetComponent<Button>();
        m_reroll.label = reroll.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    void InitStatusData(Transform parent, StatusData status)
    {
        status.gauge = parent.Find("Gauge").GetComponent<Image>();
        status.value = parent.Find("Value").GetComponent<TextMeshProUGUI>();
    }

    void AddContiniousEffect(Transform parent)
    {
        var effect = new ContiniousEffectData();
        effect.obj = parent.gameObject;
        effect.title = parent.Find("Label").GetComponent<TextMeshProUGUI>();
        effect.effect = parent.Find("Value").GetComponent<TextMeshProUGUI>();
        effect.gauge = parent.Find("Gauge").GetComponent<Image>();
        m_continiousEffects.Add(effect);
    }

    void AddUpgrade(Transform parent)
    {
        var upgrade = new UpgradeData();
        upgrade.obj = parent.gameObject;
        upgrade.button = parent.GetComponent<Button>();
        upgrade.description = parent.Find("Description").GetComponent<TextMeshProUGUI>();
        upgrade.cost = parent.Find("Cost").GetComponent<TextMeshProUGUI>();
        m_upgrades.Add(upgrade);
    }

    void UpdateInterface()
    {
        m_statusButton.interactable = !m_statusPageEnabled;
        m_upgradesButton.interactable = m_statusPageEnabled;

        m_statusPage.SetActive(m_statusPageEnabled);
        m_upgradesPage.SetActive(!m_statusPageEnabled);

        var station = Station.instance;
        if (station == null)
            return;

        //continuous effects
        for (int i = 0; i < m_continiousEffects.Count; i++)
        {
            if(station.GetEffectNb() <= i)
            {
                m_continiousEffects[i].obj.SetActive(false);
                continue;
            }

            m_continiousEffects[i].obj.SetActive(true);

            var effect = station.GetEffect(i);
            if (effect == null)
                continue;

            m_continiousEffects[i].title.text = effect.name;
            m_continiousEffects[i].effect.text = effect.GenerateText();
            
        }
    }

    void UpdateGauges()
    {
        var station = Station.instance;

        float life = (float)station.lifeSupply / station.lifeSupplyMax;
        bool criticLife = life < 0.21f;
        m_lifeSupply.gauge.fillAmount = life;
        m_lifeSupply.gauge.color = criticLife ? Color.red : Color.white;
        m_lifeSupply.value.text = station.lifeSupply.ToString() + " / " + station.lifeSupplyMax.ToString();

        float power = (float)station.power / station.powerMax;
        bool criticPower = power < 0.21f;
        m_power.gauge.fillAmount = power;
        m_power.gauge.color = criticPower ? Color.red : Color.white;
        m_power.value.text = station.power.ToString() + " / " + station.powerMax.ToString();

        m_resources.text = station.resource.ToString();

        for (int i = 0; i < m_continiousEffects.Count; i++)
        {
            var effect = station.GetEffect(i);
            if (effect == null)
                continue;

            m_continiousEffects[i].gauge.fillAmount = station.GetEffectTimerPercent(i);
        }
    }
}
