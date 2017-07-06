using System.Collections.Generic;

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
