using NebNes.CPU;
using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class Mapper {
        protected MOS6502 cpu;
        protected Cart cart;
        protected byte[] prg;
        protected byte[] chr;
        private int totalPrgPages;
        private int totalChrPages;

        protected byte[][] prgPages;
        protected byte[][] chrPages;

        public Mapper(MOS6502 cpu, Cart cart) {
            this.cpu = cpu;
            this.cart = cart;
            prg = cart.getPrg();
            chr = cart.getChr();
            totalPrgPages = (int)Math.Floor((double)(prg.Length / (16 * 1024)));
            totalChrPages = (int)Math.Floor((double)(chr.Length / (8 * 1024)));
            prgPages = new byte[totalPrgPages][];
            chrPages = new byte[totalChrPages][];
            for (int i = 0; i < totalPrgPages; i++) {
                int offset = i * (16 * 1024);
                prgPages[i] = prg[offset..(offset + (16 * 1024))];
            }
            for (int i = 0; i < totalChrPages; i++) {
                int offset = i * (8 * 1024);
                chrPages[i] = chr[offset..(offset + (8 * 1024))];
            }
            onLoad();
        }

        public void onLoad() { }

        public virtual PPUBgMirroring getMirroring() {
            return cart.getMirroring();
        }


        public byte[] getPrgPage(int page) {
            return prgPages[Math.Max(0, page %  prgPages.Length)];
        }

        public byte[] getChrPage(int page) {
            return chrPages[Math.Max(0, page % chrPages.Length)];
        }
    }
}
