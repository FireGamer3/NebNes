namespace NebNes.APU.Misc {
    public class FrameSequencer {
        NesAPU apu;
        int counter = 0;

        public FrameSequencer(NesAPU apu) {
            this.apu = apu;
        }

        public void reset() {
            counter = 0;
        }

        public void step() {
            counter++;
            int[] frameTimings = isOnFiveStepSeq() ? fiveStepSeqFrames() : fourStepSeqFrames();
            if(counter == frameTimings[1] ||  counter == frameTimings[3]) {
                apu.onHalfFrameClock();
                apu.onQuarterFrameClock();
            }else if (counter == frameTimings[0] || counter == frameTimings[2]) {
                apu.onQuarterFrameClock();
            }
            if(counter == frameTimings[3]) {
                if (!isOnFiveStepSeq()) apu.cpu.PendingInterrupts.setInterrupt(Enums.InterruptIndex.APU_FRAME);
                reset();
            }
        }

        public bool justSetIRQ() {
            if (counter == 14916 && !isOnFiveStepSeq()) return true;
            return false;
        }

        private bool isOnFiveStepSeq() {
            return apu.registers.apuFrameCounter.use5StepSequencer() == 1;
        }

        private int[] fourStepSeqFrames() {
            return [3729, 7457, 11186, 14916];
        }

        private int[] fiveStepSeqFrames() {
            return [3729, 7457, 11186, 18641];
        }
    }
}
