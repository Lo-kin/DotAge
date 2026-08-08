using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.View
{
    public class Animation
    {
        public string Name { get; set; } = "Default";
        public List<TextureRegion> Frames;
        public int FrameIntervalTick = 10;
        
        public int LastChangeTick = 0;

        public int _currentFramePosition = 0;
        public int CurrentFramePosition
        {
            get
            {
                return _currentFramePosition;
            }
            set
            {
                if (Frames.Count < value && value >= 0)
                {
                    _currentFramePosition = value;
                }
            }
        }

        public TextureRegion CurrentFrame 
        {
            get
            {
                if (CurrentFramePosition >= 0 && CurrentFramePosition < Frames.Count)
                {
                    return Frames[CurrentFramePosition];
                }
                else
                {
                    CurrentFramePosition = 0;
                    return null;
                }
            }
            set
            {
                if (CurrentFramePosition >= 0 && CurrentFramePosition < Frames.Count)
                {
                    Frames[CurrentFramePosition] = value;
                }
            }
        }

        public Animation()
        {

        }

        public bool Update(int CurrentTick)
        {
            if (CurrentTick >= LastChangeTick + FrameIntervalTick)
            {
                CurrentFramePosition += (int)((CurrentTick - LastChangeTick) / FrameIntervalTick);
            }

            return true;
        }
    }
}
