using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class TimeCountDownHelper
    {
        public event EventHandler<EventArgs> TimeDownFinished;
        public event EventHandler<EventArgs> TimeChanged;
        public bool CanLoop { get; set; } = false;
        public bool IsRunning { get; set; } = false;

        public double Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        System.Timers.Timer timer=new System.Timers.Timer();
        int Hour;

        int Minute;

        int Second;

        TimeSpan _leftTime = new TimeSpan(0, 0, 10);
        public TimeSpan LeftTime
        {
            get{ return _leftTime; }
        }

        public TimeCountDownHelper()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _leftTime = _leftTime.Subtract(new TimeSpan(0, 0, 1));
            if (TimeChanged!=null)
            {
                TimeChanged.BeginInvoke(this,new EventArgs(),null,null);
            }

            if (_leftTime.TotalSeconds <= 0)
            {
                if (CanLoop)
                {
                    _leftTime = new TimeSpan(Hour, Minute, Second);
                }
                else
                {
                    timer.Stop();
                    IsRunning = false;
                }
                if (TimeDownFinished != null)
                {
                    TimeDownFinished.BeginInvoke(this, new EventArgs(), null, null);
                }
            }
           
        }

        public void SetTime(int Hour, int Minute,int Second)
        {
            this.Hour=Hour;
            this.Minute=Minute;
            this.Second=Second;
            _leftTime = new TimeSpan(Hour, Minute, Second);
        }


        public void Start()
        {
            timer.Start();
            IsRunning = true;
        }
        public void Stop()
        {
            timer.Stop();
            IsRunning = false;
        }
        public void Reset()
        {
            _leftTime = new TimeSpan(Hour, Minute, Second);
        }
        
    }
}
