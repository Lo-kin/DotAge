using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Model;
using DotAge.Core.Model.Dialogue;
using Microsoft.Xna.Framework;

namespace DotAge.Core.View
{
    class UIElement
    {
        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public Vector2 Size { get; set; } = new Vector2(96, 0);
        public RenderPropertyGroup RenderGroup { get; set; } = new RenderPropertyGroup(_renderCanvasCount: 5);
        public PhysicEntity PhysicEntity { get; set; } = new PhysicEntity();
        public List<ZoneEntity> _zoneEntity = new List<ZoneEntity>();
        public List<ZoneEntity> ZoneEntity
        {
            get
            {
                return _zoneEntity;
            }
            set
            {
                foreach (var _zoneEntity in _zoneEntity)
                {
                    GameData.ZoneEntities.Remove(_zoneEntity);
                }
                _zoneEntity = value;
                GameData.ZoneEntities.AddRange(_zoneEntity);
            }
        }

        public UIElement(int ChainRenderCount = 5)
        {
            InitialChainRenderEntity(ChainRenderCount);
        }

        public bool InitialChainRenderEntity(int Count)
        {
            RenderGroup = new RenderPropertyGroup(Count);
            RenderGroup.SetAllTextureState(true);
            return true;
        }

        public virtual bool Update()
        {
            return true;
        }

        public virtual bool ChangeVisiblility(bool Stat)
        {
            RenderGroup.SetAllVisibility(Stat);
            return true;
        }
    }

    class HealthBar : UIElement
    {//在组合体这类的渲染问题中，继续使用RenderCanvas组很蠢，用单个RenderCanvas来实现比较好
        public HealthBar()
        {
            RenderGroup.RenderProperties[0] = new(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_background"),
                Size = new Vector2(96, 32),
                ActualPosition = new Vector2(0, GameSetting.ScreenHeight - 32)
            };
            RenderGroup.RenderProperties[1] = new(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_per"),
                TintColor = Color.Red,
                Size = new Vector2(96, 32),
                ActualPosition = new Vector2(0, GameSetting.ScreenHeight - 32)
            };

            RenderGroup.RenderProperties[2].RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_front");
            RenderGroup.RenderProperties[2].Size = new Vector2(32, 32);
            RenderGroup.RenderProperties[2].ActualPosition = new Vector2(0, GameSetting.ScreenHeight - 32);
            RenderGroup.RenderProperties[2].TintColor = Color.Brown;
            RenderGroup.RenderProperties[3].RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_mid");
            RenderGroup.RenderProperties[3].Size = new Vector2(32, 32);
            RenderGroup.RenderProperties[3].ActualPosition = new Vector2(32, GameSetting.ScreenHeight - 32);
            RenderGroup.RenderProperties[3].TintColor = Color.Brown;
            RenderGroup.RenderProperties[4].RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_end");
            RenderGroup.RenderProperties[4].Size = new Vector2(32, 32);
            RenderGroup.RenderProperties[4].ActualPosition = new Vector2(64, GameSetting.ScreenHeight - 32);
            RenderGroup.RenderProperties[4].TintColor = Color.Brown;

            RenderGroup.SetAllFixed(true);
        }

        public bool Trigger(float _health, float _maxHealth)
        {
            var _per = _health / _maxHealth;
            RenderGroup.RenderProperties[1].Size = new Vector2(96 * _per, 32);
            return true;
        }
    }

    class Chatbox : UIElement
    {
        public (int , Rectangle?) Avatar { get; set; } = (-1, null);
        public string Content { get; set; }
        public Paragraph ChatContent = new Paragraph();
        public int stdWidth = 16;
        public int stdHeight = 4;
        public Chatbox() 
        {
            InitialChainRenderEntity(3);
            RenderGroup.RenderProperties[0] = new RenderProperty(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_per"),
                Size = new Vector2(512, 128),
                TintColor = Color.FromNonPremultiplied(255, 165, 79,255),
                ActualPosition = new Vector2((GameSetting.ScreenWidth / 2) - 256, GameSetting.ScreenHeight - 128)
            };
            RenderGroup.RenderProperties[1] = new RenderProperty(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Status", "Coin"),
                Size = new Vector2(32, 32),
                TintColor = Color.White,
                ActualPosition = new Vector2((GameSetting.ScreenWidth / 2) - 256, GameSetting.ScreenHeight - 128 )
            };
            RenderGroup.RenderProperties[2] = new RenderProperty(_init: true)
            {
                IsShowTexture = false,
                TintColor = Color.Black,
                Size = new Vector2(64, 32),
                ActualPosition = new Vector2((GameSetting.ScreenWidth / 2) - 256 + 32, GameSetting.ScreenHeight - 128),
                Text = "Chatbox Content"
            };
            RenderGroup.SetAllFixed(true);

            string[] bula = {"makes","my","heart","warm","again","this","is","a","complex","content"};
            for (int i = 0; i < 100; i++)
            {
                Sentence sentence = new Sentence()
                {
                    Content = i + " : ",
                    Title = "Test",
                    LoopShow = false,
                    TimerDueTime = 0,
                    TimerPeriod = 75
                };
                for (int j = 0; j < 10; j++)
                {
                    sentence.Content += " " + bula[(int)Random.Shared.Next(0, bula.Length)];
                }
                ChatContent.Sentences.Add(sentence);
            }
            ChatContent.Start();

        }

        public override bool Update()
        {
            //ChatContent.Update();
            RenderGroup.RenderProperties[2].Text = ChatContent.GetCurrentContent;
            return base.Update();
        }

        public bool ModifyAvatar((int , Rectangle?) _avatar)
        {
            Avatar = _avatar;
            RenderGroup.RenderProperties[1].RenderTexture = Avatar;
            return true;
        }

        public bool ModifyContent(string _content) 
        { 
            Content = _content;
            RenderGroup.RenderProperties[2].Text = _content;
            return true;
        }
    }

    class Shelf : UIElement
    {
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public Dictionary<Point, (string, int)> Storages = new Dictionary<Point, (string, int)>();
        public Dictionary<ZoneEntity , Point> GetPoint = new Dictionary<ZoneEntity, Point>();

        public Shelf(int _width, int _height)
        {
            InitialChainRenderEntity(_width * _height * 3);
            Width = _width;
            Height = _height;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    RenderGroup.RenderProperties[i * Width + j] = new RenderProperty(_init: true)
                    {
                        RenderTexture = TextureManager.GetTextureRegionByName("Status", "shelf"),
                        Size = new Vector2(32, 32),
                        ActualPosition = new Vector2(j * 32, i * 32),
                        IsFixed = true
                    };
                }
            }
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    RenderGroup.RenderProperties[i * Width + j + Width * Height] = new RenderProperty(_init: true)
                    {
                        RenderTexture = TextureManager.GetTextureRegionByName("Character", "Empty"),
                        Size = new Vector2(24, 24),
                        ActualPosition = new Vector2(j * 32 + 4, i * 32 + 4),
                        IsFixed = true
                    };
                }
            }
            List<ZoneEntity> list = new List<ZoneEntity>();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    RenderGroup.RenderProperties[i * Width + j + Width * Height * 2] = new RenderProperty(_init: true)
                    {
                        RenderTexture = TextureManager.GetTextureRegionByName("Character", "Empty"),
                        Size = new Vector2(24, 24),
                        ActualPosition = new Vector2(j * 32 + 4, i * 32 + 4),
                        IsFixed = true
                    };
                    ZoneEntity zoneEntity = new ZoneEntity()
                    {
                        TriggerZone = new RectF(new Vector2(j * 32 + 4, i * 32 + 4), new Vector2(24, 24))
                    };
                    zoneEntity.Condition = Control.TwoStat.FreezeToActive;
                    zoneEntity.TriggerDelegate += (p) =>
                    {
                        Point _p = this.GetPoint[zoneEntity];

                        AddItem(_p.X, _p.Y, "Bullet_Yellow", -1);
                        return true;
                    };
                    GetPoint.Add(zoneEntity, new Point(j, i));
                    list.Add(zoneEntity);
                }
            }

            ZoneEntity = list;
        }

        public void AddItem(int _x, int _y, string _textureName, int Count)
        {
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height)
            {
                throw new ArgumentOutOfRangeException("Position out of bounds");
            }
            if (Storages.ContainsKey(new Point(_x, _y)))
            {
                Storages[new Point(_x, _y)] = (_textureName, Storages[new Point(_x, _y)].Item2 + Count);
            }
            else
            {
                Storages.Add(new Point(_x, _y), (_textureName, 1));
            }
            RenderGroup.RenderProperties[_y * Width + _x + Width * Height].RenderTexture = TextureManager.GetTextureRegionByName("Character", _textureName);
            RenderGroup.RenderProperties[_y * Width + _x + Width * Height * 2].Text = Storages[new Point(_x, _y)].Item2.ToString();
        }
    }

    class InformationBox : UIElement
    {
        public Vector2 Position { get { return Control.Controlers.CurrentMousePosition.ToVector2(); } }
        public bool Visible { get; set; } = true;
        public MessageEntity MessageSource { get; set; } = new MessageEntity() { MessageItem = ItemInformation.NullItem };

        public InformationBox()
        {
            RenderGroup = new RenderPropertyGroup(8);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    RenderGroup.RenderProperties[j * 3 + i] = new RenderProperty(_init: true)
                    {
                        RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_per"),
                        TintColor = Color.Gray,
                        Size = new Vector2(32, 32),
                        ActualPosition = new Vector2(i * 32, j * 32)
                    };
                }
            }
            RenderGroup.RenderProperties[6] = new RenderProperty(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Character", "Castle_Dark"),
                Size = new Vector2(32, 32),
                TintColor = Color.Gray,
                ActualPosition = new Vector2(16, 16)
            };
            RenderGroup.RenderProperties[7] = new RenderProperty(_init: true)
            {
                RenderTexture = TextureManager.GetTextureRegionByName("Status", "bar_per"),
                Size = new Vector2(32, 32),
                TintColor = Color.Red,
                IsShowTexture = false,
                ActualPosition = new Vector2(16 + 32, 16),
                Text = "Information Box , Content is a dark castle"
            };

            RenderGroup.SetAllFixed(true);
        }

        public bool SetInformationSource(MessageEntity messageEntity)
        {
            MessageSource = messageEntity;
            return true;
        }

        public override bool Update()
        {
            RenderGroup.RenderProperties[7].Text = MessageSource.MessageItem.Content;
            RenderGroup.RenderProperties[6].RenderTexture = MessageSource.MessageItem.Icon;
            for (int i = 0; i < RenderGroup.RenderProperties.Length; i++)
            {
                RenderGroup.RenderProperties[i].Offset = Position;
            }
            return true;
        }
    }

    class ProcessBar : UIElement
    {
        public int Length { get; set; } = 80;
        public int MinLength { get; set; } = 4;
        public ProcessBar()
        {

        }

        public bool SetLength(int _length)
        {
            Length = _length;
            //for 哎呀，之后再搞这个吧，先做游戏吧
            RenderGroup.RenderProperties[0].Size = new Vector2(_length, 32);
            return true;
        }

        public bool SetBackGround(string _textureName)
        {
            RenderGroup.RenderProperties[0].RenderTexture = TextureManager.GetTextureRegionByName("Status", _textureName);
            return true;
        }
    }
}
