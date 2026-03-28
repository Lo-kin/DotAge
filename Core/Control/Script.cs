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
    public class Script
    {
        public EngineAccessor Accessor = null;
        public List<TickDelegate> TickScripts = new List<TickDelegate>();
        public int CurrentTick { get{ return Accessor.GetGameTime(); } }

        public Script(EngineAccessor _accessor)
        {
            Accessor = _accessor;
            foreach (var item in ScriptTest.GetTest(Accessor))
            {
                AddTickDelegate(item);
            }
        }

        public virtual void Update()
        {
            if (Accessor == null)
            {
                return;
            }
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

    public class TickDelegate
    {
        public EngineAccessor Accessor = null;
        public int TriggerTick = 0;
        public int NextTriggerTick = 0;
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
                    NextTriggerTick += RepeatInterval;
                }
                else
                {
                    return stat;
                }

            }
            return stat;
        }
    }

    public static class ScriptTest
    {
        public static List<TickDelegate> GetTest(EngineAccessor Accessor)
        {
            int CurrentTick = Accessor.GetGameTime();

            var TC = new TargetCore(Accessor)
            {
                PhysicProperty =
                {
                    SpeedLength = 0f,
                    Position = new Vector2(45 * 32, 45 * 32),
                    Size = new Vector2(32, 32),
                    IsSoild = false
                },
            };
            Accessor.AddEntity(TC);
            var item1 = new TickDelegate(Accessor)
            {
                TriggerTick = 0,
                TotalRepeatCount = -1,
                RepeatInterval = 1,
                TickDelegateFunc = (engineAccessor) =>
                {
                    engineAccessor.AddEntity(new Zombie(Accessor)
                    {
                        PhysicProperty =
                        {
                            SpeedLength= 0.03f,
                            Position = new Vector2(200, 200),
                            Size = new Vector2(32, 32),
                            IsSoild = true
                        },
                        GameProperty =
                        {
                            Health = (CurrentTick / 400) + 500,
                        }

                    });
                    return true;
                }
            };
            item1.TriggerCondition = () =>
            {
                if (Accessor.GetGameTime() % 2000 >= 1000)
                {
                    item1.RepeatInterval = 100;
                }
                else
                {
                    item1.RepeatInterval = 200;
                }
                if (Accessor.GetGameTime() == item1.NextTriggerTick)
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
                RepeatInterval = 1000,
                TickDelegateFunc = (engineAccessor) =>
                {
                    engineAccessor.AddEntity(new GoldMine(Accessor)
                    {
                        PhysicProperty =
                        {
                            Position = new Vector2(Random.Shared.NextInt64(0 , 0), Accessor.GetGameTime() * 32 / 250),
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
            var item3 = new TickDelegate(Accessor)
            {
                TriggerTick = 0,
                TotalRepeatCount = -1,
                RepeatInterval = 1,
                TickDelegateFunc = (engineAccessor) =>
                {
                    if (TC.GameProperty.IsAlive == false)
                    {
                        item1.RepeatedCount = 0;
                        item2.RepeatedCount = 0;
                    }
                    return true;
                }
            };
            item3.TriggerCondition = () =>
            {
                if (TC.GameProperty.IsAlive == false && Accessor.GetGameTime() == item3.NextTriggerTick)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            return [item1, item2, item3];
        }
    }
}
