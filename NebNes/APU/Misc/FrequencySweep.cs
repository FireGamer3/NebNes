using NebNes.APU.Interfaces;

namespace NebNes.APU.Misc {
    public class FrequencySweep {
        IChannel channel;

        private bool startFlag = false;
        private int dividerCount = 0;
        public bool mute = false;

        public FrequencySweep(IChannel channel) {
            this.channel = channel;
        }

        public void clock() {
            if(canClock()) {
                int sweepDelta = channel.getTimer() >> shiftCount();
                channel.setTimer(channel.getTimer() + (sweepDelta * (channel.sweepNegate() ? -1 : 1)));
            }
            if (dividerCount == 0 || startFlag) {
                dividerCount = channel.sweepDividerPeriodMinusOne();
                startFlag = false;
            } else dividerCount--;
        }

        public void start() {
            startFlag = true;
        }

        public void muteIfNeeded() {
            mute = channel.getTimer() < 8 || channel.getTimer() > 0x7FF;
        }

        private bool canClock() {
            return enabled() && shiftCount() > 0 && dividerCount == 0 && !mute;
        }

        private bool enabled() {
            return channel.sweepEnabled();
        }

        private byte shiftCount() {
            return channel.sweepShiftCount();
        }
    }
}
