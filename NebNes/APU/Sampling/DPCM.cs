using NebNes.APU.Generators;
using NebNes.APU.Registers;
using NebNes.Misc;

namespace NebNes.APU.Sampling {
    /// <summary>
    /// DPCM (Δ-Modulation) channel handler.
    /// Manages sample playback: fetching bits, looping, and outputting variations.
    /// </summary>
    public class DPCM {
        /// <summary>
        /// A list of all DPCM periods. The <c>DMCControl</c> register points to this table.
        /// </summary>
        int[] dpcmPeriods = [
          214,
          190,
          170,
          160,
          143,
          127,
          113,
          107,
          95,
          80,
          71,
          64,
          53,
          42,
          36,
          27
        ];
        DMCChannel channel;

        bool startFlag = false;
        bool isActive = false;
        byte? buffer = null;
        int cursorByte = 0;
        int cursorBit = 0;
        int dividerPeriod = 0;
        int dividerCount = 0;
        int sampleAddress = 0;
        int sampleLength = 0;

        public DPCM(DMCChannel channel) {
            this.channel = channel;
        }

        /// <summary>
        /// Called each APU cycle to handle DPCM timing, sample fetching,
        /// bit processing, output variation, and looping.
        /// </summary>
        public void update() {
            if (startFlag) {
                dividerPeriod = dpcmPeriods[registers().control.dpcmPeriodId()];

                startFlag = false;
                isActive = true;
                cursorByte = -1;
                cursorBit = 0;
                dividerCount = dividerPeriod - 1;

                // sample address = %11AAAAAA.AA000000 = $C000 + (A * 64)
                sampleAddress = 0xc000 + registers().sampleAddress.getValue() * 64;
                // sample length = %LLLL.LLLL0001 = (L * 16) + 1 bytes
                sampleLength = registers().sampleLength.getValue() * 16 + 1;
            }

            if (!isActive) return;

            dividerCount++;
            if (dividerCount >= dividerPeriod) dividerCount = 0;
            else return;

            bool needFetch = buffer == null || cursorBit == 8;
            if (needFetch) {
                int nextByte = cursorByte + 1;

                if (nextByte >= sampleLength) {
                    isActive = false;
                    buffer = null;
                    if (registers().control.loop() == 1) start();
                    return;
                }

                cursorByte = nextByte;
                cursorBit = 0;

                int address = sampleAddress + cursorByte;
                if (address > 0xffff) {
                    // (if it exceeds $FFFF, it is wrapped around to $8000)
                    address = 0x8000 + (address % 0x10000);
                }
                buffer = channel.cpu.read((ushort)address);
            }

            int variation = ByteLib.getFlag(buffer!.Value, cursorBit) ? 2 : -2;
            double newSample = channel.sample() + variation;
            if (newSample >= 0 && newSample <= 127)
                channel.set(newSample);

            cursorBit++;
        }

        /// <summary>
        /// Sets the startFlag so playback begins on next update.
        /// </summary>
        public void start() {
            startFlag = true;
        }

        /// <summary>
        /// Stops playback immediately by moving cursor to sample end.
        /// </summary>
        public void stop() {
            cursorByte = sampleLength;
            isActive = false;
        }

        /// <summary>
        /// Returns remaining bytes in sample, or 0 if inactive.
        /// </summary>
        public int remainingBytes() {
            if (!isActive) return 0;
            return Math.Max(0, sampleLength - (cursorByte + 1));
        }

        public DMCRegisters registers() {
            return channel.registers();
        }
    }
}
