using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.View
{
    internal class Animation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int FrameCount { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameRate { get; set; }
        public bool Loop { get; set; }
        public bool IsPlaying { get; set; }
        public int CurrentFrame { get; set; }
        public Animation(string name, string path, int frameCount, int frameWidth, int frameHeight, int frameRate, bool loop)
        {
            Name = name;
            Path = path;
            FrameCount = frameCount;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameRate = frameRate;
            Loop = loop;
            IsPlaying = false;
            CurrentFrame = 0;
        }
        public void Play()
        {
            IsPlaying = true;
        }
        public void Stop()
        {
            IsPlaying = false;
        }
        public void Update()
        {
            if (IsPlaying)
            {
                CurrentFrame++;
                if (CurrentFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        CurrentFrame = 0;
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
        }
    }
}
