namespace NebNes.APU.Misc {
    public class LinearLengthCounter : LengthCounter {
        public int reload = 0;
        public bool reloadFlag;

        public void fullReset() {
            reset();
            reload = 0;
            reloadFlag = false;
        }

        public override void clock(bool enabled, bool halted) {
            if(!enabled) {
                reset();
                return;
            }

            if(reloadFlag) {
                counter = reload;
            } else base.clock(enabled, false);
            if (!halted) reloadFlag = false;
        }
    }
}
