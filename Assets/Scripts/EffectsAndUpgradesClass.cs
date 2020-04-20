using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Defs
{
    public static string lifeSuplyText = "<sprite=0>";
    public static string powerText = "<sprite=1>";
    public static string resourceText = "<sprite=2>";
    public static string arrowText = "<sprite=3>";
}

[Serializable]
public class ResourceBloc
{
    public int lifeSupply = 0;
    public int power = 0;
    public int resource = 0;

    public string GenerateText()
    {
        string text = "";

        if (lifeSupply > 0)
            text += lifeSupply + Defs.lifeSuplyText + "  ";
        if (power > 0)
            text += power + Defs.powerText + "  ";
        if (resource > 0)
            text += resource + Defs.resourceText + "  ";

        return text;
    }
}

[Serializable]
public class ContiniousEffectData
{
    public string name = "";
    public ResourceBloc input = new ResourceBloc();
    public ResourceBloc output = new ResourceBloc();
    public float maxTimer;

    public string GenerateText()
    {
        string text = "";
        bool isOnlyInput = output.lifeSupply == 0 && output.power == 0 && output.resource == 0;
        bool isOnlyOutput = input.lifeSupply == 0 && input.power == 0 && input.resource == 0;

        if (isOnlyInput && isOnlyOutput)
            return "NOTHING";
        
        if(input.lifeSupply != 0)
        {
            if (isOnlyInput)
                text += "-";
            text += input.lifeSupply + Defs.lifeSuplyText + "  ";
        }
        if(input.power != 0)
        {
            if (isOnlyInput)
                text += "-";
            text += input.power + Defs.powerText + "  ";
        }
        if(input.resource != 0)
        {
            if (isOnlyInput)
                text += "-";
            text += input.resource + Defs.resourceText + "  ";
        }

        if (!isOnlyInput && !isOnlyOutput)
            text += Defs.arrowText + "  ";

        if (output.lifeSupply != 0)
        {
            if (isOnlyOutput)
                text += "+";
            text += output.lifeSupply + Defs.lifeSuplyText + "  ";
        }
        if (output.power != 0)
        {
            if (isOnlyOutput)
                text += "+";
            text += output.power + Defs.powerText + "  ";
        }
        if (output.resource != 0)
        {
            if (isOnlyOutput)
                text += "+";
            text += output.resource + Defs.resourceText + "  ";
        }

        return text;
    }
}

public abstract class UpgradeBase
{
    [SerializeField] ResourceBloc m_cost = new ResourceBloc();
    [SerializeField] float m_weight = 1;
    float m_totalIncreased = 1;
    float m_totalMore = 1;

    void CheckPrice()
    {
        if (m_totalIncreased < 1)
            m_totalIncreased = 1;
        if (m_totalMore < 1)
            m_totalMore = 1;
    }

    public ResourceBloc GetFullPrice()
    {
        CheckPrice();

        var cost = new ResourceBloc();
        cost.lifeSupply = (int)(m_cost.lifeSupply * m_totalIncreased * m_totalMore);
        cost.power = (int)(m_cost.power * m_totalIncreased * m_totalMore);
        cost.resource = (int)(m_cost.resource * m_totalIncreased * m_totalMore);

        return cost;
    }

    public float GetWeight()
    {
        return m_weight;
    }

    public bool CanBuy()
    {
        var cost = GetFullPrice();

        var station = Station.instance;

        if (station.lifeSupply < cost.lifeSupply)
            return false;
        if (station.power < cost.power)
            return false;
        if (station.resource < cost.resource)
            return false;
        return true;
    }

    public void ApplyCost()
    {
        var cost = GetFullPrice();

        var station = Station.instance;

        station.lifeSupply -= cost.lifeSupply;
        station.power -= cost.power;
        station.resource -= cost.resource;
    }

    public void IncreaseCost(float increased, float more, float weightMultiplier)
    {
        CheckPrice();

        m_totalIncreased += increased;
        m_totalMore *= more;
        m_weight *= weightMultiplier;
    }

    public abstract string GetDescription();

    public abstract void ApplyEffect();
}

public class UpgradeStats : UpgradeBase
{
    [TextArea]
    [SerializeField] string m_description = "";
    [SerializeField] int m_maxLifeSupply = 0;
    [SerializeField] int m_maxPower = 0;
    [SerializeField] List<ContiniousEffectData> m_effects = new List<ContiniousEffectData>();
    [SerializeField] float m_shipSpeed = 0;
    [SerializeField] float m_shipMiningSpeed = 0;
    [SerializeField] int m_shipMiningResource = 0;
    [SerializeField] int m_shipCargo = 0;

    public override string GetDescription()
    {
        //copy
        var description = m_description.ToString();

        int index = 0;
        do
        {
            index = description.IndexOf('[', index);
            if (index < 0)
                break;
            int index2 = description.IndexOf(']', index);
            if (index2 < 0)
                break;

            if (index2 == index + 1)
            {
                index = index2 + 1;
                continue;
            }

            var substr = description.Substring(index + 1, index2 - index - 1).ToLower();

            if (substr.StartsWith("life"))
                substr = "+" + m_maxLifeSupply + Defs.lifeSuplyText;
            else if (substr.StartsWith("power"))
                substr = "+" + m_maxPower + Defs.powerText;
            else if (substr.StartsWith("effect"))
            {
                substr = substr.Remove(0, 6);
                int effectIndex = -1;
                int.TryParse(substr, out effectIndex);
                if (effectIndex >= 0 && effectIndex < m_effects.Count)
                {
                    substr = substr.Remove(0, 1);
                    if (substr.StartsWith("in"))
                    {
                        substr = substr.Remove(0, 2);
                        if (substr.StartsWith("life"))
                            substr = m_effects[effectIndex].input.lifeSupply + Defs.lifeSuplyText;
                        else if (substr.StartsWith("power"))
                            substr = m_effects[effectIndex].input.power + Defs.powerText;
                        else if (substr.StartsWith("resource"))
                            substr = m_effects[effectIndex].input.resource + Defs.powerText;
                    }
                    else if (substr.StartsWith("out"))
                    {
                        substr = substr.Remove(0, 3);
                        if (substr.StartsWith("life"))
                            substr = m_effects[effectIndex].output.lifeSupply + Defs.lifeSuplyText;
                        else if (substr.StartsWith("power"))
                            substr = m_effects[effectIndex].output.power + Defs.powerText;
                        else if (substr.StartsWith("resource"))
                            substr = m_effects[effectIndex].output.resource + Defs.powerText;
                    }
                    else if (substr.StartsWith("full"))
                        substr = m_effects[effectIndex].GenerateText();
                    else if (substr.StartsWith("name"))
                        substr = m_effects[effectIndex].name.ToUpper();
                }
            }
            else if (substr.StartsWith("speed"))
                substr = "+" + (int)(m_shipSpeed * 100) + "%";
            else if (substr.StartsWith("mining"))
                substr = "+" + (int)(m_shipMiningSpeed * 100) + "%";
            else if (substr.StartsWith("resource"))
                substr = "+" + m_shipMiningResource + Defs.resourceText;
            else if (substr.StartsWith("cargo"))
                substr = "+" + m_shipCargo + Defs.resourceText;

            description = description.Remove(index, index2 - index + 1);
            description = description.Insert(index, substr);

            index = index + substr.Length;

        } while (index >= 0);

        return description;
    }

    public override void ApplyEffect()
    {
        var station = Station.instance;

        if (m_maxLifeSupply != 0)
            station.lifeSupplyMax += m_maxLifeSupply;
        if (m_maxPower != 0)
            station.powerMax += m_maxPower;

        foreach (var e in m_effects)
            station.AddEffect(e);

        if(m_shipSpeed != 0)
            ShipControler.instance.IncreaseSpeed(m_shipSpeed);
        if (m_shipMiningSpeed != 0)
            MiningBehaviour.instance.IncreaseMiningSpeed(m_shipMiningSpeed);
        if (m_shipMiningResource != 0)
            MiningBehaviour.instance.IncreaseMiningResource(m_shipMiningResource);
        if (m_shipCargo != 0)
            MiningBehaviour.instance.IncreaseCargo(m_shipCargo);
    }
}