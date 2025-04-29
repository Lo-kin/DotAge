using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    class RenderEffect
    {
        public ChainRenderEntity[] ChainRenderEntity { get; set; } = new ChainRenderEntity[4];
        public PhysicEntity PhysicEntity { get; set; } = new PhysicEntity();

        public RenderEffect()
        {
            InitialChainRenderEntity(4);
        }

        public RenderEffect(int ChainRenderCount)
        {
            InitialChainRenderEntity(ChainRenderCount);
        }

        public bool InitialChainRenderEntity(int Count)
        {
            if (Count < 0)
            {
                return false;
            }
            for (int i = 0; i < Count; i++)
            {
                ChainRenderEntity[i] = new ChainRenderEntity();
            }
            return true;
        }

        public virtual bool Update()
        {

            return true;
        }
    }

    class HealthBar : RenderEffect
    {
        public HealthBar()
        {
            ChainRenderEntity[0].UniformPosition = new Vector2(0, GameSetting.ScreenHeight - 32);
            ChainRenderEntity[0].UniformSize = new Vector2(32, 32);
            ChainRenderEntity[0].EndChainpoint = new Vector2(ChainRenderEntity[0].ChainBox.Right , ChainRenderEntity[0].ChainBox.Top);
            for (int i = 1; i < ChainRenderEntity.Count(); i++)
            {
                ChainRenderEntity[i].ChainPoint = ChainRenderEntity[i - 1].EndChainpoint;
            }
            for (int i = 0; i < ChainRenderEntity.Count(); i++)
            {
                ChainRenderEntity[i].RenderCanvas[0] = 
            }

        }
    }
}
