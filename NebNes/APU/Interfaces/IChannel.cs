using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.APU.Interfaces {
    public interface IChannel {
        public bool sweepEnabled();
        public bool sweepNegate();
        public byte sweepShiftCount();
        public byte sweepDividerPeriodMinusOne();
        public int getTimer();
        public void setTimer(int timer);
    }
}
