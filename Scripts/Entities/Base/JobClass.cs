using System;
using startup.Scripts.Entities.Common;

namespace startup.Scripts.Entities.Base
{
    public class JobClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobClassTypes Type { get; set; }
        public string Description { get; set; }
        public int Mana { get; set; } = 2;
        public int Strength { get; set; } = 2;
        public int Agility { get; set; } = 2;
        public int Stamina { get; set; } = 2;
    }
}
