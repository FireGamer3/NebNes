using System.Text;
using System.Text.RegularExpressions;
using NebNes.APU;
using NebNes.CPU;
using NebNes.Mappers;
using NebNes.Misc;
using NebNes.PPU;

namespace NebNes.Frontend {
    /// <summary>
    /// Headless nestest runner. Executes the CPU-only "automated" mode (entry at $C000),
    /// writes a nestest.log-format trace, diffs it against the canonical reference log if
    /// present, and prints the $02/$03 result codes the ROM leaves behind.
    /// </summary>
    internal static class Nestest {
        private const ushort EntryPoint = 0xC000;
        private const int MaxInstructions = 9000;

        public static int Run(string romPath, string? referencePath) {
            byte[] rom = File.ReadAllBytes(romPath);

            // Mirror NES wiring, but skip the RESET interrupt so we can enter at $C000.
            Bus bus = new Bus();
            MOS6502 cpu = new MOS6502(bus);
            Cart cart = new Cart(rom);
            if (cart.isRomInvalid()) {
                Console.WriteLine("nestest: ROM is not a valid iNES 1.0 image.");
                return 1;
            }
            Controller[] controllers = { new Controller(), new Controller() };
            controllers[0].SetOther(controllers[1]);
            controllers[1].SetOther(controllers[0]);
            IMapper mapper = MapperFactory.CreateMapper(cpu, cart);
            NesPPU ppu = new NesPPU(cpu, bus, mapper);
            NesAPU apu = new NesAPU(cpu);
            ppu.bus.Initialize(cart, mapper);
            bus.Initialize(mapper, ppu, apu, controllers);

            // nestest.log's first line: PC=C000 A:00 X:00 Y:00 P:24 SP:FD CYC:7
            cpu.PC.set(EntryPoint);
            cpu.A.set(0);
            cpu.X.set(0);
            cpu.Y.set(0);
            cpu.SP.set(0xFD);
            cpu.Flags.set(0x24);

            StringBuilder trace = new StringBuilder();
            long cyc = 7;
            int executed = 0;
            for (; executed < MaxInstructions; executed++) {
                ushort pc = cpu.PC.get();
                byte op = bus.read(pc);
                byte b1 = bus.read((ushort)(pc + 1));
                byte b2 = bus.read((ushort)(pc + 2));

                trace.Append(pc.ToString("X4")).Append("  ")
                     .Append(op.ToString("X2")).Append(' ')
                     .Append(b1.ToString("X2")).Append(' ')
                     .Append(b2.ToString("X2")).Append("  ")
                     .Append("A:").Append(cpu.A.get().ToString("X2"))
                     .Append(" X:").Append(cpu.X.get().ToString("X2"))
                     .Append(" Y:").Append(cpu.Y.get().ToString("X2"))
                     .Append(" P:").Append(cpu.Flags.get().ToString("X2"))
                     .Append(" SP:").Append(cpu.SP.get().ToString("X2"))
                     .Append(" CYC:").Append(cyc)
                     .Append('\n');

                int n = cpu.step();          // executes exactly one instruction
                if (n <= 0) {                // unknown opcode (MOS6502 prints its own message)
                    Console.WriteLine($"nestest: stopped at ${pc:X4} opcode ${op:X2} (unimplemented or stuck) after {executed} instructions.");
                    executed++;
                    break;
                }
                for (int j = 0; j < n; j++) cpu.step();   // drain the cycle counter to the next boundary
                cyc += n;
            }

            string outPath = Path.Combine(Path.GetDirectoryName(romPath) ?? ".", "nestest.mine.log");
            File.WriteAllText(outPath, trace.ToString());
            Console.WriteLine($"nestest: executed {executed} instructions. Trace -> {outPath}");

            byte r02 = bus.read(0x0002);
            byte r03 = bus.read(0x0003);
            Console.WriteLine($"nestest: result codes  $02={r02:X2}  $03={r03:X2}  ({(r02 == 0 && r03 == 0 ? "PASS" : "FAIL")})");

            if (referencePath is not null && File.Exists(referencePath)) {
                DiffAgainstReference(trace.ToString(), referencePath);
            } else {
                Console.WriteLine("nestest: no reference log found; skipping diff. (Expected at testRoms/nestest.log)");
            }
            return 0;
        }

        private static readonly Regex FieldsRx =
            new(@"^(?<pc>[0-9A-F]{4}).*?(?<regs>A:[0-9A-F]{2} X:[0-9A-F]{2} Y:[0-9A-F]{2} P:[0-9A-F]{2} SP:[0-9A-F]{2})",
                RegexOptions.Compiled);

        private static void DiffAgainstReference(string mine, string referencePath) {
            string[] mineLines = mine.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string[] refLines = File.ReadAllLines(referencePath);

            int limit = Math.Min(mineLines.Length, refLines.Length);
            for (int i = 0; i < limit; i++) {
                Match m = FieldsRx.Match(mineLines[i]);
                Match r = FieldsRx.Match(refLines[i]);
                if (!m.Success || !r.Success) continue;

                bool pcOk = m.Groups["pc"].Value == r.Groups["pc"].Value;
                bool regsOk = m.Groups["regs"].Value == r.Groups["regs"].Value;
                if (pcOk && regsOk) continue;

                Console.WriteLine($"nestest: FIRST DIVERGENCE at trace line {i + 1}:");
                Console.WriteLine($"  expected: {refLines[i].TrimEnd()}");
                Console.WriteLine($"  actual:   {mineLines[i].TrimEnd()}");
                if (i > 0) Console.WriteLine($"  (prev ok: {refLines[i - 1].TrimEnd()})");
                return;
            }
            Console.WriteLine($"nestest: {limit} lines match the reference (PC + registers).");
        }
    }
}
