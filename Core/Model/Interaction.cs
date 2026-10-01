using DotAge.Core.Control;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DotAge.Core.Model
{
    public class ValueClamp<T> where T : INumber<T>
    {
        public T MaxValue { get; set; }
        public T MinValue { get; set; }
        public T LastValue { get;private set; } = default;
        public T CurrentValue { get; private set; } = default;
        public Func<T , bool> OnValueChanged { get; set; }
        public ValueClamp(T max , T min , T current) 
        {
            MaxValue = max;MinValue = min;LastValue = default;CurrentValue = current;
        }
        public bool SetValue(T value)
        {
            if (value != CurrentValue) return false;
            LastValue = CurrentValue;
            if (value >= MinValue && value <= MaxValue)
            {
                CurrentValue = value;
            }
            else
            {
                CurrentValue = value > MaxValue ? MaxValue : MinValue;
            }
            OnValueChanged(CurrentValue);
            return true;
        }
        public bool ModifyValue(T delta)
        {
            return SetValue(delta + CurrentValue);
        }
    }

    public interface IGame
    {
        public bool IsAlive { get { return Health.CurrentValue > 0; } }
        public ValueClamp<float> Health { get; set; }
        public ValueClamp<float> Sight { get; set; }
        public ValueClamp<float> Money { get; set; }
        public string Name { get; set; }
    }

    public interface ITrigger : ILocation
    {
        public bool IsFixedToWindow { get; set; }
        public Func<Vector2, bool> OnTrigger { get; set; }
        public string Description { get; set; }
        public Stator TriggerStator { get; set; }
        public MessageEntity InformationSource { get; set; }

        public bool Invoke(ITrigger invoker)
        {
            if (CheckTrigger(invoker) == false)
            {
                return false;
            }
            else
            {
                OnTrigger.Invoke(CrashBox.Center);
                return true;
            }
        }

        public bool CheckTrigger(ITrigger it)
        {
            if (OnTrigger == null)
            {
                return false;
            }
            if (TriggerStator.Update(RectF.IsContain(CrashBox, it.CrashBox.Center)) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
