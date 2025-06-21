using DotAge.Core.Control;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model.Delegates
{
    public delegate bool NoArgDelegate();
    public delegate bool ArgDelegate<T>(T arg);
    public delegate bool StatArgDelegate<TwoStat, T2>(TwoStat InvokeStat, T2 arg2);
    public delegate bool StatDelegate<TwoStat>(TwoStat InvokeStat);

    class ControlerFunc
    {
        public Keys BindKey = Keys.None;
        public MouseButton BindMouseButton = MouseButton.None;
        public StatDelegate<TwoStat> ControlerFuncDelegate;
        public TwoStat TriggerStat = TwoStat.None; // 触发状态
        public string ControlerFuncescription;
        public int invokeCount;

        public bool Invoke(TwoStat _triggerStat)
        {
            if (ControlerFuncDelegate != null)
            {
                bool ExcuteStaat = ControlerFuncDelegate.Invoke(_triggerStat);
                invokeCount++;
                return ExcuteStaat;
            }
            return false;
        }
    }
}
