namespace NebNes.APU.Registers {
    public class AudioRegisters {
        private NesAPU apu;
        public PulseRegisters[] pulses = new PulseRegisters[2];
        public TriagnleRegisters triangle;
        public NoiseRegisters noise;
        public DMCRegisters dmc;

        public APUFrameCounter apuFrameCounter;
        public APUControl apuControl;
        public APUStatus apuStatus;

        public AudioRegisters(NesAPU apu) {
            this.apu = apu;
            pulses[0] = new PulseRegisters(apu, 0);
            pulses[1] = new PulseRegisters(apu, 1);
            triangle = new TriagnleRegisters(apu);
            noise = new NoiseRegisters(apu);
            dmc = new DMCRegisters(apu);

            apuFrameCounter = new APUFrameCounter(apu, 0);
            apuControl = new APUControl(apu, 0);
            apuStatus = new APUStatus(apu, 0);

        }

        public byte read(ushort address) {
            if(address == 0x4015) return apuStatus.onRead();
            return GetRegister(address).onRead();
        }

        public void write(ushort address, byte value) {
            if(address == 0x4015) {
                apuControl.onWrite(value);
                return;
            }
            GetRegister(address).onWrite(value);
        }

        private BaseRegister GetRegister(ushort address) {
            switch (address) {
                case 0x4000:
                    return pulses[0].control;
                case 0x4001:
                    return pulses[0].sweep;
                case 0x4002:
                    return pulses[0].timerLow;
                case 0x4003:
                    return pulses[0].timerHighLCL;
                case 0x4004:
                    return pulses[1].control;
                case 0x4005:
                    return pulses[1].sweep;
                case 0x4006:
                    return pulses[1].timerLow;
                case 0x4007:
                    return pulses[1].timerHighLCL;
                case 0x4008:
                    return triangle.lengthControl;
                case 0x400a:
                    return triangle.timerLow;
                case 0x400b:
                    return triangle.timerHighLCL;
                case 0x400c:
                    return noise.control;
                case 0x400e:
                    return noise.form;
                case 0x400f:
                    return noise.lcl;
                case 0x4010:
                    return dmc.control;
                case 0x4011:
                    return dmc.load;
                case 0x4012:
                    return dmc.sampleAddress;
                case 0x4013:
                    return dmc.sampleLength;
                case 0x4017:
                    return apuFrameCounter;
                default:
                    return apuFrameCounter;
            }
        }
    }

    public struct PulseRegisters {
        public PulseControl control;
        public PulseSweep sweep;
        public PulseTimerLow timerLow;
        public PulseTimerHighLCL timerHighLCL;

        public PulseRegisters(NesAPU apu, int id) {
            control = new PulseControl(apu, id);
            sweep = new PulseSweep(apu, id);
            timerLow = new PulseTimerLow(apu, id);
            timerHighLCL = new PulseTimerHighLCL(apu, id);
        }
    }

    public struct TriagnleRegisters {
        public TriangleLengthControl lengthControl;
        public TriangleTimerLow timerLow;
        public TriangleTimerHighLCL timerHighLCL;

        public TriagnleRegisters(NesAPU apu) {
            lengthControl = new TriangleLengthControl(apu, 0);
            timerHighLCL = new TriangleTimerHighLCL(apu, 0);
            timerLow = new TriangleTimerLow(apu, 0);
        }
    }

    public struct NoiseRegisters {
        public NoiseControl control;
        public NoiseForm form;
        public NoiseLCL lcl;

        public NoiseRegisters(NesAPU apu) {
            control = new NoiseControl(apu, 0);
            form = new NoiseForm(apu, 0);
            lcl = new NoiseLCL(apu, 0);
        }
    }

    public struct DMCRegisters {
        public DMCControl control;
        public DMCLoad load;
        public DMCSampleAddress sampleAddress;
        public DMCSampleLength sampleLength;

        public DMCRegisters(NesAPU apu) {
            control = new DMCControl(apu, 0);
            load = new DMCLoad(apu, 0);
            sampleAddress = new DMCSampleAddress(apu, 0);
            sampleLength = new DMCSampleLength(apu, 0);
        }
    }
}
