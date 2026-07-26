namespace NebNes.APU.Misc {
    public class LengthCounter {
        protected int counter = 0;

        public void reset() {
            counter = 0;
        }

        public int get() { 
            return counter; 
        }

        public void set(int counter) {
            this.counter = counter;
        }

        public bool isActive() {
            return counter > 0;
        }

        public virtual void clock(bool enabled, bool halted) {
            if (!enabled) {
                reset();
            } else {
                if (isActive() && !halted)
                    counter--;
            }
        }
    }
}
