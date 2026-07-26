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

        public Operation(byte opCode, int cycles, AddressingMode addressMode, IInstruction instruction) {
            this.opCode = opCode;
            this.cycles = cycles;
            this.addressMode = addressMode;
            this.instruction = instruction;
        }
    }
}
