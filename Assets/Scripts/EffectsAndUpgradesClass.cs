using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

[Serializable]
public class ContiniousEffectData
{
    public string name;
    public ResourceBloc input;
    public ResourceBloc output;
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