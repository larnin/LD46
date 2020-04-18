using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateStationModulesEvent
{
    public int moduleNb;
    
    public UpdateStationModulesEvent(int _moduleNb)
    {
        moduleNb = _moduleNb;
    }
}
