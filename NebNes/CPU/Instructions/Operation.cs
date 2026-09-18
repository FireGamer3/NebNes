using NebNes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.CPU.Instructions {
    public readonly struct Operation {
        public byte opCode { get; }
        public int cycles { get; }
        public AddressingMode addressMode { get; }
        public IInstruction instruction { get; }

        // Only read-type indexed instructions pay the page-cross cycle. Writes and RMWs always
        // take the extra cycle, so their table entry already includes it (5/7 vs 4, 6 vs 5).
        public bool hasPageCrossPenalty => addressMode switch {
            AddressingMode.ABSOLUTE_INDEXED_X or AddressingMode.ABSOLUTE_INDEXED_Y => cycles == 4,
            AddressingMode.INDIRECT_INDEXED => cycles == 5,
            _ => false,
        };

        public Operation(byte opCode, int cycles, AddressingMode addressMode, IInstruction instruction) {
            this.opCode = opCode;
            this.cycles = cycles;
            this.addressMode = addressMode;
            this.instruction = instruction;
        }
    }
}
