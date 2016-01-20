using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    
    class Timer
    {
        private float timer;
        private float timerInterval;
        public Timer()
        {
            timer = 0;
            timerInterval = 50;
        }
        public Timer(float timerInterval)
        {
            this.timerInterval = timerInterval;
        }
        public void updateTimer()
        {
            timer += Game1.elapsedTime;
        }
    }
}
