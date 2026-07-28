using NebNes.CPU;
using NebNes.Enums;
using NebNes.Mappers.Misc;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class Mapper {
        protected MOS6502 cpu;
        protected Cart cart;
        protected byte[] prg;
        protected byte[] chr;

        protected PageTable prgPages;
        protected PageTable prgPages8k;
        protected PageTable chrPages;
        protected PageTable chrPages2k;
        protected PageTable chrPages1k;

        public Mapper(MOS6502 cpu, Cart cart) {
            this.cpu = cpu;
            this.cart = cart;
            prg = cart.getPrg();
            chr = cart.getChr();
            int totalPrgPages = prg.Length / (16 * 1024);
            int totalChrPages = chr.Length / (8 * 1024);
            prgPages = new PageTable(prg, 16 * 1024, totalPrgPages);
            prgPages8k = new PageTable(prg, 8 * 1024, totalPrgPages * 2);
            chrPages = new PageTable(chr, 8 * 1024, totalChrPages);
            chrPages2k = new PageTable(chr, 2 * 1024, totalChrPages * 4);
            chrPages1k = new PageTable(chr, 1024, totalChrPages * 8);
            onLoad();
        }

        public void onLoad() { }

        public virtual PPUBgMirroring getMirroring() {
            return cart.getMirroring();
        }


        public Span<byte> getPrgPage(int page) {
            return prgPages.wrapped(page);
        }

        public Span<byte> getPrgPage8K(int page) {
            return prgPages8k.wrapped(page);
        }

        public Span<byte> getChrPage(int page) {
            return chrPages.wrapped(page);
        }

        public Span<byte> getChrPage2K(int page) {
            return chrPages2k.wrapped(page);
        }

        public Span<byte> getChrPage1K(int page) {
            return chrPages1k.wrapped(page);
        }
    }
}
