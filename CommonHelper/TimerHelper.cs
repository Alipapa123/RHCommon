using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class TimerHelper
    {
        Stopwatch sw=new Stopwatch();

        public event EventHandler<EventArgs> Elapsed;
        public int Interval { get; set; } = 1000;
        int HaveElapsedCount = 0;

        public void Start()
        {
            if (sw.IsRunning)
            {
                return;
            }
            sw.Reset();
            HaveElapsedCount = 0;
            sw.Start();
            CheckTime();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
        }

        void CheckTime()
        {
            Task.Run(new Action(() => {
                int count = 0;
                while (sw.IsRunning)
                {
                    count++;
                    if (count > 50)
                    {
                        Thread.Sleep(1);
                    }
                    int tem = (int)(sw.ElapsedMilliseconds / Interval);
                    if (tem > HaveElapsedCount)
                    {
                        HaveElapsedCount = tem;
                        if (Elapsed != null)
                        {
                            Elapsed.BeginInvoke(this, null,null,null);
                        }
                    }
                }
            }));
        }
    }
}
