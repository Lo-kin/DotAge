using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
    class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; } // Duration in seconds
        public int Strength { get; set; } // Strength of the effect
        public string Type { get; set; } // Type of effect (e.g., buff, debuff)
        public bool IsActive { get; set; } // Indicates if the effect is currently active
        public Effect(int id, string name, string description, int duration, int strength, string type)
        {
            Id = id;
            Name = name;
            Description = description;
            Duration = duration;
            Strength = strength;
            Type = type;
            IsActive = false;
        }
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
