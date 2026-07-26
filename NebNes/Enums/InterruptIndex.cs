using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.Enums {
    public enum InterruptIndex {
        RESET,
        NMI,
        BRK,
        APU_FRAME,
        APU_DMC,
        MAPPER
    }
}
