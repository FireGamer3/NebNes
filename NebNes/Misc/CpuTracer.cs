using System.Text;
using NebNes.CPU;
using NebNes.CPU.Instructions;
using NebNes.Enums;

namespace NebNes.Misc {
    /// <summary>
    /// Writes a nestest.log-format instruction trace by hooking <see cref="MOS6502.OnInstruction"/>.
    /// Lines look like:
    /// <code>C000  4C F5 C5  JMP $C5F5                       A:00 X:00 Y:00 P:24 SP:FD CYC:7</code>
    /// Column layout matches the canonical log, so lines can be diffed against it directly. The
    /// operand annotations the reference log adds (<c>@ 0300 = 89</c>) are not reproduced — only
    /// PC, the instruction bytes and the registers are meant to be compared.
    /// </summary>
    public sealed class CpuTracer : IDisposable {
        /// <summary>Pass as maxLines to trace until the tracer is stopped.</summary>
        public const long Unlimited = 0;

        private const int FlushEvery = 4096;

        private readonly MOS6502 cpu;
        private readonly Bus bus;
        private readonly TextWriter writer;
        private readonly long maxLines;
        private readonly long cycleOffset;
        private readonly StringBuilder line = new StringBuilder(96);
        private long written;
        private int sinceFlush;
        private bool attached;

        /// <param name="cycleOffset">Added to the CPU cycle counter, e.g. 7 to line up with nestest's first line.</param>
        public CpuTracer(MOS6502 cpu, Bus bus, TextWriter writer, long maxLines = Unlimited, long cycleOffset = 0) {
            this.cpu = cpu;
            this.bus = bus;
            this.writer = writer;
            this.maxLines = maxLines;
            this.cycleOffset = cycleOffset;
        }

        public static CpuTracer ToFile(MOS6502 cpu, Bus bus, string path, long maxLines = Unlimited, long cycleOffset = 0) {
            StreamWriter file = new StreamWriter(path, append: false) { AutoFlush = false };
            return new CpuTracer(cpu, bus, file, maxLines, cycleOffset);
        }

        public long LinesWritten => written;

        /// <summary>True once the line cap has been reached and further instructions are being dropped.</summary>
        public bool IsFull => maxLines != Unlimited && written >= maxLines;

        /// <summary>Installs the tracer as the CPU's instruction hook, replacing any previous one.</summary>
        public void Attach() {
            cpu.OnInstruction = _ => Log();
            attached = true;
        }

        public void Detach() {
            if (attached) cpu.OnInstruction = null;
            attached = false;
        }

        /// <summary>Emits one trace line for the instruction the CPU is about to execute.</summary>
        public void Log() {
            if (IsFull) return;

            ushort pc = cpu.PC.get();
            byte opcode = bus.peek(pc);
            Operation op = cpu.getOperation(opcode);
            byte b1 = bus.peek((ushort)(pc + 1));
            byte b2 = bus.peek((ushort)(pc + 2));
            int length = op.instruction is null ? 1 : Length(op.addressMode);

            line.Clear();
            line.Append(pc.ToString("X4")).Append("  ");

            // Instruction bytes, padded to the reference log's 10-column field.
            line.Append(opcode.ToString("X2"));
            if (length > 1) line.Append(' ').Append(b1.ToString("X2"));
            if (length > 2) line.Append(' ').Append(b2.ToString("X2"));
            Pad(line, 6 + 10);

            line.Append(op.instruction?.GetType().Name ?? "???");
            if (op.instruction is not null) {
                string operand = Operand(op.addressMode, pc, b1, b2);
                if (operand.Length > 0) line.Append(' ').Append(operand);
            }
            Pad(line, 6 + 10 + 32);

            line.Append("A:").Append(cpu.A.get().ToString("X2"))
                .Append(" X:").Append(cpu.X.get().ToString("X2"))
                .Append(" Y:").Append(cpu.Y.get().ToString("X2"))
                .Append(" P:").Append(cpu.Flags.get().ToString("X2"))
                .Append(" SP:").Append(cpu.SP.get().ToString("X2"))
                .Append(" CYC:").Append(cpu.currentCycle + cycleOffset);

            writer.Write(line.ToString());
            writer.Write('\n');
            written++;

            // Flush regularly so a hang or hard crash still leaves the tail of the trace on disk.
            if (++sinceFlush >= FlushEvery) {
                writer.Flush();
                sinceFlush = 0;
            }
        }

        public void Dispose() {
            Detach();
            writer.Flush();
            writer.Dispose();
        }

        private static void Pad(StringBuilder sb, int column) {
            while (sb.Length < column) sb.Append(' ');
        }

        private static int Length(AddressingMode mode) {
            switch (mode) {
                case AddressingMode.IMPLICIT:
                case AddressingMode.ACCUMULATOR:
                    return 1;
                case AddressingMode.ABSOLUTE:
                case AddressingMode.ABSOLUTE_INDEXED_X:
                case AddressingMode.ABSOLUTE_INDEXED_Y:
                case AddressingMode.INDIRECT:
                    return 3;
                default:
                    return 2;
            }
        }

        private static string Operand(AddressingMode mode, ushort pc, byte b1, byte b2) {
            ushort word = (ushort)(b1 | (b2 << 8));
            switch (mode) {
                case AddressingMode.IMPLICIT:            return "";
                case AddressingMode.ACCUMULATOR:         return "A";
                case AddressingMode.IMMEDIATE:           return $"#${b1:X2}";
                case AddressingMode.ZERO_PAGE:           return $"${b1:X2}";
                case AddressingMode.ZERO_PAGE_INDEXED_X: return $"${b1:X2},X";
                case AddressingMode.ZERO_PAGE_INDEXED_Y: return $"${b1:X2},Y";
                case AddressingMode.ABSOLUTE:            return $"${word:X4}";
                case AddressingMode.ABSOLUTE_INDEXED_X:  return $"${word:X4},X";
                case AddressingMode.ABSOLUTE_INDEXED_Y:  return $"${word:X4},Y";
                case AddressingMode.INDIRECT:            return $"(${word:X4})";
                case AddressingMode.INDEXED_INDIRECT:    return $"(${b1:X2},X)";
                case AddressingMode.INDIRECT_INDEXED:    return $"(${b1:X2}),Y";
                case AddressingMode.RELATIVE:            return $"${(ushort)(pc + 2 + (sbyte)b1):X4}";
                default:                                 return "";
            }
        }
    }
}
