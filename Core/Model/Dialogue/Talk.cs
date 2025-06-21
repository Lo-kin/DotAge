using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Model.Dialogue
{
    class Sentence
    {
        public string Title { get; set; } = "null";
        public string Content { get; set; } = string.Empty;
        public string BufferContent { get; set; } = string.Empty;
        public int WordLength { get; set; } = 0;
        public int Step = 1;
        public bool LoopShow { get; set; } = false;
        public bool IsDispose { get; set; } = false;

        public Timer TextTimer = null;
        public int TimerDueTime { get; set; } = 0;
        public int TimerPeriod { get; set; } = 1000;

        public string GetCurrentContent
        {
            get
            {
                return BufferContent;
            }
        }

        public bool IsEnd
        {
            get
            {
                return WordLength >= Content.Length;
            }
        }

        public void Update(object _timer)
        {
            WordLength += Step;

            if (Content.Length == 0)
            {
                WordLength = 0;
            }
            if (WordLength > Content.Length)
            {
                if (LoopShow == true)
                {
                    WordLength %= Content.Length;
                }
                else
                {
                    WordLength = Content.Length;
                }
            }
            BufferContent = Content[0..WordLength];
        }

        public bool Start(int _ms)
        {
            if (TextTimer != null)
            {
                return false;
            }
            TextTimer = new Timer(new TimerCallback(Update), null, dueTime: 0, period: _ms);
            return true;
        }

        public bool SetTimer(int _dueTime = 0, int _period = 75)
        {
            if (TextTimer == null)
            {
                return false;
            }
            
            TimerDueTime = _dueTime;
            TimerPeriod = _period;
            TextTimer.Change(dueTime: TimerDueTime, period: TimerPeriod);
            return true;
        }

        public bool Pause()
        {
            if (TextTimer == null)
            {
                return false;
            }
            SetTimer(-1, 0);
            return true;
        }

        public bool Resume()
        {
            if (TextTimer == null)
            {
                return false;
            }
            SetTimer(TimerDueTime, TimerPeriod);
            return true;
        }

        public bool Reset()
        {
            WordLength = 0;
            return true;
        }

        public bool Dispose()
        {
            if (TextTimer == null)
            {
                return false;
            }
            TextTimer.Dispose();
            TextTimer = null;
            //BufferContent = string.Empty;
            WordLength = 0;

            return true;
        }
    }

    class Paragraph
    {
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        public int SentencePosition = 0;

        public string GetCurrentContent
        {
            get
            {
                if (!CheckVaild(SentencePosition))
                {
                    return "Null Position";
                }
                return Sentences[SentencePosition].GetCurrentContent;
            }
        }

        public Sentence GetCurrentSentence
        {
            get
            {
                if (!CheckVaild(SentencePosition))
                {
                    return null;
                }
                return Sentences[SentencePosition];
            }
        }

        public Paragraph() { }

        public bool CheckVaild(int VerificationPos)
        {
            if (VerificationPos < 0 || VerificationPos >= Sentences.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Update()
        {
            if (GetCurrentSentence == null)
            {
                return;
            }
            if (GetCurrentSentence.IsEnd == true)
            {

                GetCurrentSentence.Dispose();
                if (CheckVaild(SentencePosition + 1) == true)
                {
                    SentencePosition++;
                    GetCurrentSentence.Start(75);
                }
                else
                {
                    SentencePosition = 0;
                    GetCurrentSentence.Start(75);
                }
            }
        }

        public void Start()
        {
            GetCurrentSentence.Start(75);
        }

    }

    class Talk
    {
    }
}
