namespace NebNes.APU.Misc {
    public class VolumeEnvelope {
        private bool startFlag = false;
        private int dividerCount = 0;
        public int volume = 0;

        public void start() {
            startFlag = true;
        }

        public void clock(int period, bool loop) {
            if (startFlag) {
                startFlag = false;
                volume = 15;
                dividerCount = period;
                return;
            } else if (dividerCount > 0) {
                dividerCount--;
                return;
            }
            dividerCount = period;
            if (volume == 0) {
                if (loop) volume = 15;
            } else {
                volume--;
            }
        }
    }
}
