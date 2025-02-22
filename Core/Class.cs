using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core
{
    static class GameProp
    {
        public static int BaseQuadTreeSize = 128;
        public static int QuadTreeMaxDeep = 3;
    }

    static class Dump
    {

        public static Dictionary<int, Entity> CacheEntity = new Dictionary<int, Entity>();
        public static Dictionary<Vector2, QuadTree> MainTree = new Dictionary<Vector2, QuadTree>();

        public static bool Register(Entity entity)
        {
            if (entity != null)
            {
                entity.Id = CacheEntity.Count;
                CacheEntity.Add(CacheEntity.Count, entity);
                int depth = 1;
                bool LTDeeperFlag = true;
                bool RTDeeperFlag = true;
                bool LBDeeperFlag = true;
                bool RBDeeperFlag = true;
                var LPos = entity.Positon.X / (GameProp.BaseQuadTreeSize * depth);
                var RPos = (entity.Positon.X + entity.Size.X) / (GameProp.BaseQuadTreeSize * depth);
                var TPos = entity.Positon.Y / (GameProp.BaseQuadTreeSize * depth);
                var BPos = (entity.Positon.Y + entity.Size.Y) / (GameProp.BaseQuadTreeSize * depth);
                var LRoot = MathF.Floor(LPos);
                var RRoot = MathF.Floor(RPos);
                var TRoot = MathF.Floor(TPos);
                var BRoot = MathF.Floor(BPos);
                while (true)
                {
                    if (LTDeeperFlag)
                    {
                        if (MainTree.ContainsKey(new Vector2(LRoot, TRoot)) == false)
                        {
                            MainTree[new Vector2(LRoot, TRoot)] = new QuadTree();
                        }
                        else
                        {
                            MainTree[new Vector2(LRoot, TRoot)].Children.Add(entity.Id);
                            if (MainTree[new Vector2(LRoot, TRoot)].Children.Count > 5)
                            {
                                LTDeeperFlag = true;
                            }
                        }
                        QuadTree tmp = MainTree[new Vector2(LRoot, TRoot)];
                        for (int i = 0; i < depth; i++)
                        {
                            int[,] LTPos = new int[2, 2];
                            if (RPos % 1 >= 0.5)
                            {

                            }
                            else
                            {

                            }
                            if (LPos % 1 >= 0.5)
                            {

                            }
                            else
                            {

                            }
                            tmp = tmp.ChildTree[0, 0];
                        }
                    }

                    if (LPos != RPos)
                    {
                        MainTree[new Vector2(RPos, TPos)] = new QuadTree();
                    }
                    else
                    {

                        if (MainTree[new Vector2(RPos, TPos)].Children.Count > 5)
                        {
                            RTDeeperFlag = true;
                        }
                    }
                    if (TPos != BPos)
                    {
                        MainTree[new Vector2(LPos, BPos)] = new QuadTree();
                    }
                    else
                    {
                        if (MainTree[new Vector2(LPos, BPos)].Children.Count > 5)
                        {
                            LBDeeperFlag = true;
                        }
                    }
                    if (LPos != RPos || TPos != BPos)
                    {
                        MainTree[new Vector2(RPos, BPos)] = new QuadTree();
                    }
                    else
                    {
                        if (MainTree[new Vector2(RPos, TPos)].Children.Count > 5)
                        {
                            RTDeeperFlag = true;
                        }
                    }
                    if (LTDeeperFlag == true)
                    {
                        continue;
                    }
                    if (LBDeeperFlag == true)
                    {
                        depth++;
                    }
                    break;
                }
                return true;
            }
            return false;
        }
    }

    struct RectF
    {
        public float Right { get { return Position.X + Size.X; } }
        public float Left { get { return Position.X; }}
        public float Top { get { return Position.Y; }}
        public float Bottom { get { return Position.Y + Size.Y; } }
        public Vector2 Position { get; set; } = new Vector2();
        public Vector2 Size { get; set; } = new Vector2();

        public RectF(Vector2 _position, Vector2 _size)
        {
            Position = _position;
            Size = _size;
        }

        public Vector2 ContainCount(RectF rectF)
        {
            Vector2 xyContain = new Vector2();
            if (rectF.Right > Left && rectF.Left < Right)//如果相交，值总是正数，比较值的右边或者上边相对被比较值的左边或下边大
            {
                xyContain.X = Left - rectF.Left;
            }
            if (rectF.Bottom > Top && rectF.Top < Bottom)
            {
                xyContain.Y = Top - rectF.Top;
            }
            return xyContain;
        }

        public bool IsContain(RectF rectF)
        {
            bool stat  = false;
            if (rectF.Right > Left && rectF.Left < Right)
            {
                stat = true;
            }
            if (rectF.Bottom > Top && rectF.Top < Bottom)
            {
                stat = true;
            }
            return stat;
        }

        public bool Move(Vector2 Vec)
        {
            Position += Vec;
            return true;
        }

        public bool ChangeSize(Vector2 Vec)
        {
            Size += Vec;
            return true;
        }
    }

    class CrashBox
    {
        public RectF Bounds;

    }

    class QuadTree
    {
        public int Depth = 0;
        public int MaxChild = 5;
        public Point BaseSize = new Point(GameProp.BaseQuadTreeSize);
        public QuadTree[,] ChildTree = new QuadTree[2, 2];
        public List<int> Children = new List<int>();

    }

    class Entity
    {
        public int Id;
        public Point Size;
        public Vector2 Positon { get; set; }//Left-Top Position
        public Vector2 Forward { get; set; }
    }


    class Movement
    {
        public Vector2 Straight(Vector2 pos)
        {

            return new Vector2();
        }
    }

}
