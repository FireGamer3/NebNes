using System.Text;
using System.Text.RegularExpressions;
using NebNes.APU;
using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Mappers.Misc;
using NebNes.Misc;
using NebNes.PPU;

namespace NebNes.Frontend {
    /// <summary>
    /// Headless nestest runner. Executes the CPU-only "automated" mode (entry at $C000),
    /// writes a nestest.log-format trace, diffs it against the canonical reference log if
    /// present, and prints the $02/$03 result codes the ROM leaves behind.
    /// <para>
    /// Exit codes: 0 = pass, 1 = the ROM reported a failure or the trace diverged from the
    /// reference, 2 = the run could not be set up (bad ROM, missing file). Non-zero means
    /// "do not trust the CPU", so this is usable as a build gate.
    /// </para>
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
                return 2;
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

            // The CPU's own trace hook emits one line per instruction, in the same format the
            // live --trace flag produces. CYC starts at 7 to match nestest.log's first line.
            StringWriter sink = new StringWriter();
            using CpuTracer tracer = new CpuTracer(cpu, bus, sink, MaxInstructions, cycleOffset: 7);
            tracer.Attach();

            bool halted = false;
            int executed = 0;
            for (; executed < MaxInstructions; executed++) {
                ushort pc = cpu.PC.get();
                byte op = bus.peek(pc);

                int n = cpu.step();          // executes exactly one instruction
                if (n <= 0) {                // unknown opcode (MOS6502 prints its own message)
                    Console.WriteLine($"nestest: stopped at ${pc:X4} opcode ${op:X2} (unimplemented or stuck) after {executed} instructions.");
                    executed++;
                    halted = true;
                    break;
                }
                for (int j = 0; j < n; j++) cpu.step();   // drain the cycle counter to the next boundary
            }
            tracer.Detach();

            string trace = sink.ToString();
            string outPath = Path.Combine(Path.GetDirectoryName(romPath) ?? ".", "nestest.mine.log");
            File.WriteAllText(outPath, trace);
            Console.WriteLine($"nestest: executed {executed} instructions. Trace -> {outPath}");

            byte r02 = bus.read(0x0002);
            byte r03 = bus.read(0x0003);
            bool codesOk = r02 == 0 && r03 == 0;
            Console.WriteLine($"nestest: result codes  $02={r02:X2}  $03={r03:X2}  ({(codesOk ? "PASS" : "FAIL")})");

            bool traceOk = true;
            if (referencePath is not null && File.Exists(referencePath)) {
                traceOk = DiffAgainstReference(trace, referencePath);
            } else {
                Console.WriteLine("nestest: no reference log found; skipping diff. (Expected at testRoms/nestest.log)");
            }

            bool pass = codesOk && traceOk && !halted;
            Console.WriteLine($"nestest: {(pass ? "PASS" : "FAIL")}");
            return pass ? 0 : 1;
        }

        private static readonly Regex FieldsRx =
            new(@"^(?<pc>[0-9A-F]{4}).*?(?<regs>A:[0-9A-F]{2} X:[0-9A-F]{2} Y:[0-9A-F]{2} P:[0-9A-F]{2} SP:[0-9A-F]{2})",
                RegexOptions.Compiled);

        /// <summary>Returns true when the trace matches the reference over their common prefix.</summary>
        private static bool DiffAgainstReference(string mine, string referencePath) {
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
                return false;
            }
            Console.WriteLine($"nestest: {limit} lines match the reference (PC + registers).");
            return true;
        }
    }
}
