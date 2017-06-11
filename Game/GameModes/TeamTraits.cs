using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.GameModes;

namespace Thera.GameModes
{
    public class TeamTraits
    {
        private int _teamIndex;
        private InheritablePlayerTraits _teamPlayerTraits;
        private List<int> _friendlyTeams = new List<int>();
        private List<int> _enemyTeams;
    }
}
