using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using startup.Scripts.Entities.Common;

namespace startup.Scripts.Entities.Base
{
    public class Race
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RacialTypes RacialType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ManaModifier { get; set; }
        public int StrengthModifier { get; set; }
        public int AgilityModifier { get; set; }
        public int StaminaModifier { get; set; }
        public List<string> Strengths { get; set; } = [];
        public List<string> Weaknesses { get; set; } = [];
    }
}