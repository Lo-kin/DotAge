using DotAge.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Security.AccessControl;

namespace DotAge.Core.Control
{
    internal class Script
    {
        public EngineAccessor Accessor = null;
        public List<TickDelegate> TickScripts = new List<TickDelegate>();
        public int CurrentTick { get{ return Accessor.GetGameTime(); } }

        public Script(EngineAccessor _accessor)
        {
            Accessor = _accessor;
            var item1 = new TickDelegate(Accessor)
            {
                TriggerTick = CurrentTick + 1000,
                TotalRepeatCount = -1,
                RepeatInterval = 100,
                TickDelegateFunc = (engineAccessor) =>
                {
                    engineAccessor.AddEntity(new Zombie(Accessor)
                    {
                        PhysicProperty =
                        {
                            SpeedLength= 0.2f,
                            Position = new Vector2(200, 200),
                            Size = new Vector2(32, 32),
                        }
                    });
                    return true;
                }
            };
            item1.TriggerCondition = () =>
            {
                if (Accessor.GetGameTime() % 2000 >= 1000 && Accessor.GetGameTime() == item1.NextTriggerTick)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
            var item2 = new TickDelegate(Accessor)
            {
                TriggerTick = CurrentTick + 0,
                TotalRepeatCount = -1,
                RepeatInterval = 250,
                TickDelegateFunc = (engineAccessor) =>
                {
                    engineAccessor.AddEntity(new GoldMine(Accessor)
                    {
                        PhysicProperty =
                        {
                            Position = new Vector2(Random.Shared.NextInt64(0 , 500), Random.Shared.NextInt64(0,500)),
                            Size = new Vector2(32, 32),
                        }
                    });
                    return true;
                }
            };
            item2.TriggerCondition = () =>
            {
                if (Accessor.GetGameTime() % 2000 <= 1000 && Accessor.GetGameTime() == item2.NextTriggerTick)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            AddTickDelegate(item1);
            AddTickDelegate(item2);
        }

        public virtual void Update()
        {
            foreach (var script in TickScripts)
            {
                script.Invoke();
            }
        }

        public void AddTickDelegate(TickDelegate _tickDelegate)
        {
            TickScripts.Add(_tickDelegate);
        }
    }

    class TickDelegate
    {
        public EngineAccessor Accessor = null;
        public int TriggerTick = 0;
        public int NextTriggerTick
        {
            get 
            {
                return TriggerTick + (RepeatInterval * RepeatedCount);
            }
        }
        public int TotalRepeatCount = 1;
        public int RepeatedCount = 0;
        public int RepeatInterval = 0;
        public bool IsRepeating { get { return RepeatedCount - TotalRepeatCount > 0; } }

        public int InvokeCount = 0;

        public Func<EngineAccessor , bool> TickDelegateFunc = null;
        public Func<bool> TriggerCondition = () => { 
            return true;
        };
        public TickDelegate(EngineAccessor _accessor)
        {
            Accessor = _accessor;
            TriggerCondition = () =>
            {
                if (this.Accessor.GetGameTime() == TriggerTick)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

        }

        public bool Invoke()
        {
            bool stat = false;
            if (TickDelegateFunc != null)
            {
                bool triggered = TriggerCondition.Invoke();
                if (triggered == true)
                {
                    InvokeCount++;
                    stat = TickDelegateFunc.Invoke(Accessor);
                }
                if (IsRepeating == true && NextTriggerTick == Accessor.GetGameTime())
                {
                    RepeatedCount++;
                }
                else
                {
                    return stat;
                }

            }
            return stat;
        }
    }
}
