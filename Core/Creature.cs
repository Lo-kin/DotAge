using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using DotAge.Render;
using Microsoft.Xna.Framework;

namespace DotAge.Core
{
    class RenderEntity
    {
        public RenderProperty[] RenderCanvas = new RenderProperty[16];
    }//制作一个绘图实体的基类

    class Creature
    {
        public int ID = -1;
        private Vector2 _position = new Vector2();
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public Path PathNode = new Path();
        public float Health { get; set; }
        public float Speed { get; set; } = 0.1f;
        public string Name { get; set; }
        public bool Liveable { get; set; }
        public int _maxLiveUint = 20;
        public int MaxLiveUnit
        {
            get
            {
                if (Liveable == true)
                {
                    return _maxLiveUint;
                }
                else { return 0; }
            }
            set
            {
                if (value >= 0 || value <= 200)//限定最大存储单位数
                {
                    _maxLiveUint = value;
                }
            }
        }
        public List<Creature> ContainsUnit = new List<Creature>();
        public RenderProperty[] RenderCanvas = new RenderProperty[8];//一个对象最多保存8个图层
        public bool Visibility { get; set; } = true;
        public int GroupID { get; set; } = 0;

        public int BindCreatureID { get; set; } = -1;

        public bool BindingTargetCreature(int CreatureID)
        {
            if (!GameData.GameCreatures.ContainsKey(CreatureID))
            {
                return false;
            }
            else
            {
                BindCreatureID = CreatureID;
                return true;
            }
           
        }

        public Creature()
        {
            RenderProperty _rp = new RenderProperty(new Vector2(), new Vector2(16, 16), 0);
            _rp.TintColor = Color.White;
            _rp.Visibility = true;
            RenderCanvas[0] = _rp;
            RenderCanvas[^1] = _rp;
        }

        public virtual bool UpdatePosition()
        {
            if (BindCreatureID != -1 && GameData.GameCreatures.ContainsKey(BindCreatureID))
            {
                Position = GameData.GameCreatures[BindCreatureID].Position;
            }
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                RenderCanvas[i].RenderPosition = RenderCanvas[i].RalativePosition + Position;
            }
            return false;
        }

        public bool SetDefaultTexture(int _groupID, int DefaultTexutreIndex)
        {
            if (TextureIndex.CheckIndexVaild(DefaultTexutreIndex))
            {
                TextureIndex.SetGroupUniformTexture(GroupID, GetType(), DefaultTexutreIndex, false);
                RenderCanvas[^1].RenderTexture = DefaultTexutreIndex;
            }
            else
            {
                RenderCanvas[^1].RenderTexture = 0;
            }
            return false;
        }


        public bool SetCanvas(int _groupID, int TextureIndex, int CanvasIndex)
        {
            if (CanvasIndex < 0)
            {
                CanvasIndex = 15 + CanvasIndex;
            }

            return true;
        }

        public bool SetCanvas(int TextureIndex)
        {
            int _rpindex = -1;
            foreach (var item in RenderCanvas)
            {
                _rpindex++;
                if (item.Init == false)
                {
                    RenderCanvas[_rpindex].RenderTexture = TextureIndex;
                }
            }
            return true;
        }

        public bool SetGroup(int _groupID)
        {
            if (GameData.GameGroups.Keys.Contains(_groupID))
            {
                GameData.JoinGroup(this, _groupID);
                GroupID = _groupID;

                return true;
            }
            return false;
        }
    }

    class Group
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Color TintColor { get; set; } = Color.White;
        public int FlagTexutre;
        public List<int> IncludeCreatures { get; set; } = new List<int>();

        public bool JoinCreature(ref Creature creature)
        {
            if (creature == null)
            {
                return false;
            }
            else
            {
                IncludeCreatures.Add(IncludeCreatures.Count);
                return true;
            }

        }
    }

    class Soildre : Creature
    {
        public Soildre()
        {
            RenderCanvas[7].RenderTexture = (int)TextureName.Human_Engineer;
            RenderCanvas[0].RenderTexture = (int)TextureName.Shadow_White;
            RenderCanvas[0].RalativePosition = new Vector2(0, 5);

            PathNode.Cycle = true;
            PathNode.AddNode(new Vector2(10, 232));

        }

        public override bool UpdatePosition()
        {


            return base.UpdatePosition();
        }
    }

    class Turret : Creature
    {
        public Turret()
        {
            RenderCanvas[7].RenderTexture = (int)TextureName.Turret_Gun;
            RenderCanvas[0].RenderTexture = (int)TextureName.Shadow_White;
        }



    }

    class Bullet
    {
        public Bullet()
        {

        }
    }

    class Mine : Creature
    {
        public bool Minable = true;
        public float Storage = 100;

        public Mine()
        {

        }

        public virtual void Dig()
        {

        }

    }

    class GoldMine : Mine
    {
        public GoldMine()
        {
            RenderCanvas[^1].RenderTexture = (int)TextureName.Mine_Gold;
            RenderCanvas[0].RenderTexture = (int)TextureName.Shadow_White;
            Name = "Gold_Mine";
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
            
            
        }


    }
}
