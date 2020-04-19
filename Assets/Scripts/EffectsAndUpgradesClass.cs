using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}