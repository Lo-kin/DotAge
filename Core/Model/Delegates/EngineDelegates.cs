using DotAge.Core.Control;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
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
    public delegate ItemInformation? ItemInformationDelegate(TwoStatus ClickStat , Vector2 ClickPosition);
    public delegate TRet? AnyDelegate<TVal , TRet>(TVal InvokeStat);

    class ControlerFunc
    {
        public Keys BindKey = Keys.None;
        public MouseButton BindMouseButton = MouseButton.None;
        public ItemInformationDelegate ControlerFuncDelegate;
        public TwoStatus TriggerStat = TwoStatus.None; // 触发状态
        public string ControlerFuncescription;
        public int invokeCount;

        public ItemInformation? Invoke(TwoStatus InvokeStat , Vector2 CLickPos)
        {
            if (ControlerFuncDelegate != null && InvokeStat == TriggerStat)
            {
                invokeCount++;                
                return ControlerFuncDelegate.Invoke(InvokeStat , CLickPos);
            }
            return default;
        }
    }

}
