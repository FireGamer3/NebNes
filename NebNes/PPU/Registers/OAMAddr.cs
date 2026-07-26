using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.PPU.Registers {
    public class OAMAddr : BaseRegister {
        public OAMAddr(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte value) {
            set(value);
        }
    }
}
