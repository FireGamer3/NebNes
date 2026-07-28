using NebNes.CPU.Instructions.ALU;
using NebNes.CPU.Instructions.Branching;
using NebNes.CPU.Instructions.Checks;
using NebNes.CPU.Instructions.Data;
using NebNes.CPU.Instructions.Illegal;
using NebNes.CPU.Instructions.System;
using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.CPU.Instructions {
    public static class DefineOperations {

        public static Operation[] define(MOS6502 cpu, Bus bus) {
            Operation[] table = new Operation[256];

            // One shared instance per mnemonic — instructions only hold cpu/bus,
            // so every opcode/addressing-mode variant reuses the same object.

            // ALU
            ADC adc = new ADC(cpu, bus);
            ASL asl = new ASL(cpu, bus);
            DEC dec = new DEC(cpu, bus);
            DEX dex = new DEX(cpu, bus);
            DEY dey = new DEY(cpu, bus);
            INC inc = new INC(cpu, bus);
            INX inx = new INX(cpu, bus);
            INY iny = new INY(cpu, bus);
            LSR lsr = new LSR(cpu, bus);
            ROL rol = new ROL(cpu, bus);
            ROR ror = new ROR(cpu, bus);
            SBC sbc = new SBC(cpu, bus);

            // Branching / control flow
            BCC bcc = new BCC(cpu, bus);
            BCS bcs = new BCS(cpu, bus);
            BEQ beq = new BEQ(cpu, bus);
            BMI bmi = new BMI(cpu, bus);
            BNE bne = new BNE(cpu, bus);
            BPL bpl = new BPL(cpu, bus);
            BVC bvc = new BVC(cpu, bus);
            BVS bvs = new BVS(cpu, bus);
            JMP jmp = new JMP(cpu, bus);
            JSR jsr = new JSR(cpu, bus);
            RTI rti = new RTI(cpu, bus);
            RTS rts = new RTS(cpu, bus);

            // Checks / bitwise / compares
            AND and = new AND(cpu, bus);
            BIT bit = new BIT(cpu, bus);
            CMP cmp = new CMP(cpu, bus);
            CPX cpx = new CPX(cpu, bus);
            CPY cpy = new CPY(cpu, bus);
            EOR eor = new EOR(cpu, bus);
            ORA ora = new ORA(cpu, bus);

            // Data / loads / stores / transfers / flags
            CLC clc = new CLC(cpu, bus);
            CLD cld = new CLD(cpu, bus);
            CLI cli = new CLI(cpu, bus);
            CLV clv = new CLV(cpu, bus);
            LDA lda = new LDA(cpu, bus);
            LDX ldx = new LDX(cpu, bus);
            LDY ldy = new LDY(cpu, bus);
            PHA pha = new PHA(cpu, bus);
            PHP php = new PHP(cpu, bus);
            PLA pla = new PLA(cpu, bus);
            PLP plp = new PLP(cpu, bus);
            SEC sec = new SEC(cpu, bus);
            SED sed = new SED(cpu, bus);
            SEI sei = new SEI(cpu, bus);
            STA sta = new STA(cpu, bus);
            STX stx = new STX(cpu, bus);
            STY sty = new STY(cpu, bus);
            TAX tax = new TAX(cpu, bus);
            TAY tay = new TAY(cpu, bus);
            TSX tsx = new TSX(cpu, bus);
            TXA txa = new TXA(cpu, bus);
            TXS txs = new TXS(cpu, bus);
            TYA tya = new TYA(cpu, bus);

            // System
            NOP nop = new NOP(cpu, bus);
            BRK brk = new BRK(cpu, bus);

            //Illegal
            SLO slo = new SLO(cpu, bus);
            RLA rla = new RLA(cpu, bus);
            SRE sre = new SRE(cpu, bus);
            RRA rra = new RRA(cpu, bus);
            SAX sax = new SAX(cpu, bus);
            LAX lax = new LAX(cpu, bus);
            DCP dcp = new DCP(cpu, bus);
            ISC isc = new ISC(cpu, bus);

            // ── ADC ────────────────────────────────────────────────────────────
            table[0x69] = new Operation(0x69, 2, AddressingMode.IMMEDIATE, adc);
            table[0x65] = new Operation(0x65, 3, AddressingMode.ZERO_PAGE, adc);
            table[0x75] = new Operation(0x75, 4, AddressingMode.ZERO_PAGE_INDEXED_X, adc);
            table[0x6D] = new Operation(0x6D, 4, AddressingMode.ABSOLUTE, adc);
            table[0x7D] = new Operation(0x7D, 4, AddressingMode.ABSOLUTE_INDEXED_X, adc);
            table[0x79] = new Operation(0x79, 4, AddressingMode.ABSOLUTE_INDEXED_Y, adc);
            table[0x61] = new Operation(0x61, 6, AddressingMode.INDEXED_INDIRECT, adc);
            table[0x71] = new Operation(0x71, 5, AddressingMode.INDIRECT_INDEXED, adc);

            // ── AND ────────────────────────────────────────────────────────────
            table[0x29] = new Operation(0x29, 2, AddressingMode.IMMEDIATE, and);
            table[0x25] = new Operation(0x25, 3, AddressingMode.ZERO_PAGE, and);
            table[0x35] = new Operation(0x35, 4, AddressingMode.ZERO_PAGE_INDEXED_X, and);
            table[0x2D] = new Operation(0x2D, 4, AddressingMode.ABSOLUTE, and);
            table[0x3D] = new Operation(0x3D, 4, AddressingMode.ABSOLUTE_INDEXED_X, and);
            table[0x39] = new Operation(0x39, 4, AddressingMode.ABSOLUTE_INDEXED_Y, and);
            table[0x21] = new Operation(0x21, 6, AddressingMode.INDEXED_INDIRECT, and);
            table[0x31] = new Operation(0x31, 5, AddressingMode.INDIRECT_INDEXED, and);

            // ── ASL ────────────────────────────────────────────────────────────
            table[0x0A] = new Operation(0x0A, 2, AddressingMode.ACCUMULATOR, asl);
            table[0x06] = new Operation(0x06, 5, AddressingMode.ZERO_PAGE, asl);
            table[0x16] = new Operation(0x16, 6, AddressingMode.ZERO_PAGE_INDEXED_X, asl);
            table[0x0E] = new Operation(0x0E, 6, AddressingMode.ABSOLUTE, asl);
            table[0x1E] = new Operation(0x1E, 7, AddressingMode.ABSOLUTE_INDEXED_X, asl);

            // ── Branches (base 2; +1 if taken, +1 more on page cross) ───────────
            table[0x90] = new Operation(0x90, 2, AddressingMode.RELATIVE, bcc);
            table[0xB0] = new Operation(0xB0, 2, AddressingMode.RELATIVE, bcs);
            table[0xF0] = new Operation(0xF0, 2, AddressingMode.RELATIVE, beq);
            table[0x30] = new Operation(0x30, 2, AddressingMode.RELATIVE, bmi);
            table[0xD0] = new Operation(0xD0, 2, AddressingMode.RELATIVE, bne);
            table[0x10] = new Operation(0x10, 2, AddressingMode.RELATIVE, bpl);
            table[0x50] = new Operation(0x50, 2, AddressingMode.RELATIVE, bvc);
            table[0x70] = new Operation(0x70, 2, AddressingMode.RELATIVE, bvs);

            // ── BIT ────────────────────────────────────────────────────────────
            table[0x24] = new Operation(0x24, 3, AddressingMode.ZERO_PAGE, bit);
            table[0x2C] = new Operation(0x2C, 4, AddressingMode.ABSOLUTE, bit);

            // ── BRK ──-------------------------------------------───────────────
            table[0x00] = new Operation(0x00, 7, AddressingMode.IMPLICIT, brk);

            // ── Flag clears ────────────────────────────────────────────────────
            table[0x18] = new Operation(0x18, 2, AddressingMode.IMPLICIT, clc);
            table[0xD8] = new Operation(0xD8, 2, AddressingMode.IMPLICIT, cld);
            table[0x58] = new Operation(0x58, 2, AddressingMode.IMPLICIT, cli);
            table[0xB8] = new Operation(0xB8, 2, AddressingMode.IMPLICIT, clv);

            // ── CMP ────────────────────────────────────────────────────────────
            table[0xC9] = new Operation(0xC9, 2, AddressingMode.IMMEDIATE, cmp);
            table[0xC5] = new Operation(0xC5, 3, AddressingMode.ZERO_PAGE, cmp);
            table[0xD5] = new Operation(0xD5, 4, AddressingMode.ZERO_PAGE_INDEXED_X, cmp);
            table[0xCD] = new Operation(0xCD, 4, AddressingMode.ABSOLUTE, cmp);
            table[0xDD] = new Operation(0xDD, 4, AddressingMode.ABSOLUTE_INDEXED_X, cmp);
            table[0xD9] = new Operation(0xD9, 4, AddressingMode.ABSOLUTE_INDEXED_Y, cmp);
            table[0xC1] = new Operation(0xC1, 6, AddressingMode.INDEXED_INDIRECT, cmp);
            table[0xD1] = new Operation(0xD1, 5, AddressingMode.INDIRECT_INDEXED, cmp);

            // ── CPX ────────────────────────────────────────────────────────────
            table[0xE0] = new Operation(0xE0, 2, AddressingMode.IMMEDIATE, cpx);
            table[0xE4] = new Operation(0xE4, 3, AddressingMode.ZERO_PAGE, cpx);
            table[0xEC] = new Operation(0xEC, 4, AddressingMode.ABSOLUTE, cpx);

            // ── CPY ────────────────────────────────────────────────────────────
            table[0xC0] = new Operation(0xC0, 2, AddressingMode.IMMEDIATE, cpy);
            table[0xC4] = new Operation(0xC4, 3, AddressingMode.ZERO_PAGE, cpy);
            table[0xCC] = new Operation(0xCC, 4, AddressingMode.ABSOLUTE, cpy);

            // ── DEC ────────────────────────────────────────────────────────────
            table[0xC6] = new Operation(0xC6, 5, AddressingMode.ZERO_PAGE, dec);
            table[0xD6] = new Operation(0xD6, 6, AddressingMode.ZERO_PAGE_INDEXED_X, dec);
            table[0xCE] = new Operation(0xCE, 6, AddressingMode.ABSOLUTE, dec);
            table[0xDE] = new Operation(0xDE, 7, AddressingMode.ABSOLUTE_INDEXED_X, dec);

            // ── DEX / DEY ──────────────────────────────────────────────────────
            table[0xCA] = new Operation(0xCA, 2, AddressingMode.IMPLICIT, dex);
            table[0x88] = new Operation(0x88, 2, AddressingMode.IMPLICIT, dey);

            // ── EOR ────────────────────────────────────────────────────────────
            table[0x49] = new Operation(0x49, 2, AddressingMode.IMMEDIATE, eor);
            table[0x45] = new Operation(0x45, 3, AddressingMode.ZERO_PAGE, eor);
            table[0x55] = new Operation(0x55, 4, AddressingMode.ZERO_PAGE_INDEXED_X, eor);
            table[0x4D] = new Operation(0x4D, 4, AddressingMode.ABSOLUTE, eor);
            table[0x5D] = new Operation(0x5D, 4, AddressingMode.ABSOLUTE_INDEXED_X, eor);
            table[0x59] = new Operation(0x59, 4, AddressingMode.ABSOLUTE_INDEXED_Y, eor);
            table[0x41] = new Operation(0x41, 6, AddressingMode.INDEXED_INDIRECT, eor);
            table[0x51] = new Operation(0x51, 5, AddressingMode.INDIRECT_INDEXED, eor);

            // ── INC ────────────────────────────────────────────────────────────
            table[0xE6] = new Operation(0xE6, 5, AddressingMode.ZERO_PAGE, inc);
            table[0xF6] = new Operation(0xF6, 6, AddressingMode.ZERO_PAGE_INDEXED_X, inc);
            table[0xEE] = new Operation(0xEE, 6, AddressingMode.ABSOLUTE, inc);
            table[0xFE] = new Operation(0xFE, 7, AddressingMode.ABSOLUTE_INDEXED_X, inc);

            // ── INX / INY ──────────────────────────────────────────────────────
            table[0xE8] = new Operation(0xE8, 2, AddressingMode.IMPLICIT, inx);
            table[0xC8] = new Operation(0xC8, 2, AddressingMode.IMPLICIT, iny);

            // ── JMP ────────────────────────────────────────────────────────────
            table[0x4C] = new Operation(0x4C, 3, AddressingMode.ABSOLUTE, jmp);
            table[0x6C] = new Operation(0x6C, 5, AddressingMode.INDIRECT, jmp);

            // ── JSR ────────────────────────────────────────────────────────────
            table[0x20] = new Operation(0x20, 6, AddressingMode.ABSOLUTE, jsr);

            // ── LDA ────────────────────────────────────────────────────────────
            table[0xA9] = new Operation(0xA9, 2, AddressingMode.IMMEDIATE, lda);
            table[0xA5] = new Operation(0xA5, 3, AddressingMode.ZERO_PAGE, lda);
            table[0xB5] = new Operation(0xB5, 4, AddressingMode.ZERO_PAGE_INDEXED_X, lda);
            table[0xAD] = new Operation(0xAD, 4, AddressingMode.ABSOLUTE, lda);
            table[0xBD] = new Operation(0xBD, 4, AddressingMode.ABSOLUTE_INDEXED_X, lda);
            table[0xB9] = new Operation(0xB9, 4, AddressingMode.ABSOLUTE_INDEXED_Y, lda);
            table[0xA1] = new Operation(0xA1, 6, AddressingMode.INDEXED_INDIRECT, lda);
            table[0xB1] = new Operation(0xB1, 5, AddressingMode.INDIRECT_INDEXED, lda);

            // ── LDX ────────────────────────────────────────────────────────────
            table[0xA2] = new Operation(0xA2, 2, AddressingMode.IMMEDIATE, ldx);
            table[0xA6] = new Operation(0xA6, 3, AddressingMode.ZERO_PAGE, ldx);
            table[0xB6] = new Operation(0xB6, 4, AddressingMode.ZERO_PAGE_INDEXED_Y, ldx);
            table[0xAE] = new Operation(0xAE, 4, AddressingMode.ABSOLUTE, ldx);
            table[0xBE] = new Operation(0xBE, 4, AddressingMode.ABSOLUTE_INDEXED_Y, ldx);

            // ── LDY ────────────────────────────────────────────────────────────
            table[0xA0] = new Operation(0xA0, 2, AddressingMode.IMMEDIATE, ldy);
            table[0xA4] = new Operation(0xA4, 3, AddressingMode.ZERO_PAGE, ldy);
            table[0xB4] = new Operation(0xB4, 4, AddressingMode.ZERO_PAGE_INDEXED_X, ldy);
            table[0xAC] = new Operation(0xAC, 4, AddressingMode.ABSOLUTE, ldy);
            table[0xBC] = new Operation(0xBC, 4, AddressingMode.ABSOLUTE_INDEXED_X, ldy);

            // ── LSR ────────────────────────────────────────────────────────────
            table[0x4A] = new Operation(0x4A, 2, AddressingMode.ACCUMULATOR, lsr);
            table[0x46] = new Operation(0x46, 5, AddressingMode.ZERO_PAGE, lsr);
            table[0x56] = new Operation(0x56, 6, AddressingMode.ZERO_PAGE_INDEXED_X, lsr);
            table[0x4E] = new Operation(0x4E, 6, AddressingMode.ABSOLUTE, lsr);
            table[0x5E] = new Operation(0x5E, 7, AddressingMode.ABSOLUTE_INDEXED_X, lsr);

            // ── NOP ────────────────────────────────────────────────────────────
            table[0xEA] = new Operation(0xEA, 2, AddressingMode.IMPLICIT, nop);

            // ── ORA ────────────────────────────────────────────────────────────
            table[0x09] = new Operation(0x09, 2, AddressingMode.IMMEDIATE, ora);
            table[0x05] = new Operation(0x05, 3, AddressingMode.ZERO_PAGE, ora);
            table[0x15] = new Operation(0x15, 4, AddressingMode.ZERO_PAGE_INDEXED_X, ora);
            table[0x0D] = new Operation(0x0D, 4, AddressingMode.ABSOLUTE, ora);
            table[0x1D] = new Operation(0x1D, 4, AddressingMode.ABSOLUTE_INDEXED_X, ora);
            table[0x19] = new Operation(0x19, 4, AddressingMode.ABSOLUTE_INDEXED_Y, ora);
            table[0x01] = new Operation(0x01, 6, AddressingMode.INDEXED_INDIRECT, ora);
            table[0x11] = new Operation(0x11, 5, AddressingMode.INDIRECT_INDEXED, ora);

            // ── Stack push / pull ──────────────────────────────────────────────
            table[0x48] = new Operation(0x48, 3, AddressingMode.IMPLICIT, pha);
            table[0x08] = new Operation(0x08, 3, AddressingMode.IMPLICIT, php);
            table[0x68] = new Operation(0x68, 4, AddressingMode.IMPLICIT, pla);
            table[0x28] = new Operation(0x28, 4, AddressingMode.IMPLICIT, plp);

            // ── ROL ────────────────────────────────────────────────────────────
            table[0x2A] = new Operation(0x2A, 2, AddressingMode.ACCUMULATOR, rol);
            table[0x26] = new Operation(0x26, 5, AddressingMode.ZERO_PAGE, rol);
            table[0x36] = new Operation(0x36, 6, AddressingMode.ZERO_PAGE_INDEXED_X, rol);
            table[0x2E] = new Operation(0x2E, 6, AddressingMode.ABSOLUTE, rol);
            table[0x3E] = new Operation(0x3E, 7, AddressingMode.ABSOLUTE_INDEXED_X, rol);

            // ── ROR ────────────────────────────────────────────────────────────
            table[0x6A] = new Operation(0x6A, 2, AddressingMode.ACCUMULATOR, ror);
            table[0x66] = new Operation(0x66, 5, AddressingMode.ZERO_PAGE, ror);
            table[0x76] = new Operation(0x76, 6, AddressingMode.ZERO_PAGE_INDEXED_X, ror);
            table[0x6E] = new Operation(0x6E, 6, AddressingMode.ABSOLUTE, ror);
            table[0x7E] = new Operation(0x7E, 7, AddressingMode.ABSOLUTE_INDEXED_X, ror);

            // ── RTI / RTS ──────────────────────────────────────────────────────
            table[0x40] = new Operation(0x40, 6, AddressingMode.IMPLICIT, rti);
            table[0x60] = new Operation(0x60, 6, AddressingMode.IMPLICIT, rts);

            // ── SBC ────────────────────────────────────────────────────────────
            table[0xE9] = new Operation(0xE9, 2, AddressingMode.IMMEDIATE, sbc);
            table[0xE5] = new Operation(0xE5, 3, AddressingMode.ZERO_PAGE, sbc);
            table[0xF5] = new Operation(0xF5, 4, AddressingMode.ZERO_PAGE_INDEXED_X, sbc);
            table[0xED] = new Operation(0xED, 4, AddressingMode.ABSOLUTE, sbc);
            table[0xFD] = new Operation(0xFD, 4, AddressingMode.ABSOLUTE_INDEXED_X, sbc);
            table[0xF9] = new Operation(0xF9, 4, AddressingMode.ABSOLUTE_INDEXED_Y, sbc);
            table[0xE1] = new Operation(0xE1, 6, AddressingMode.INDEXED_INDIRECT, sbc);
            table[0xF1] = new Operation(0xF1, 5, AddressingMode.INDIRECT_INDEXED, sbc);

            // ── Flag sets ──────────────────────────────────────────────────────
            table[0x38] = new Operation(0x38, 2, AddressingMode.IMPLICIT, sec);
            table[0xF8] = new Operation(0xF8, 2, AddressingMode.IMPLICIT, sed);
            table[0x78] = new Operation(0x78, 2, AddressingMode.IMPLICIT, sei);

            // ── STA (indexed stores take the fixed cycle, no page-cross bonus) ──
            table[0x85] = new Operation(0x85, 3, AddressingMode.ZERO_PAGE, sta);
            table[0x95] = new Operation(0x95, 4, AddressingMode.ZERO_PAGE_INDEXED_X, sta);
            table[0x8D] = new Operation(0x8D, 4, AddressingMode.ABSOLUTE, sta);
            table[0x9D] = new Operation(0x9D, 5, AddressingMode.ABSOLUTE_INDEXED_X, sta);
            table[0x99] = new Operation(0x99, 5, AddressingMode.ABSOLUTE_INDEXED_Y, sta);
            table[0x81] = new Operation(0x81, 6, AddressingMode.INDEXED_INDIRECT, sta);
            table[0x91] = new Operation(0x91, 6, AddressingMode.INDIRECT_INDEXED, sta);

            // ── STX ────────────────────────────────────────────────────────────
            table[0x86] = new Operation(0x86, 3, AddressingMode.ZERO_PAGE, stx);
            table[0x96] = new Operation(0x96, 4, AddressingMode.ZERO_PAGE_INDEXED_Y, stx);
            table[0x8E] = new Operation(0x8E, 4, AddressingMode.ABSOLUTE, stx);

            // ── STY ────────────────────────────────────────────────────────────
            table[0x84] = new Operation(0x84, 3, AddressingMode.ZERO_PAGE, sty);
            table[0x94] = new Operation(0x94, 4, AddressingMode.ZERO_PAGE_INDEXED_X, sty);
            table[0x8C] = new Operation(0x8C, 4, AddressingMode.ABSOLUTE, sty);

            // ── Register transfers ─────────────────────────────────────────────
            table[0xAA] = new Operation(0xAA, 2, AddressingMode.IMPLICIT, tax);
            table[0xA8] = new Operation(0xA8, 2, AddressingMode.IMPLICIT, tay);
            table[0xBA] = new Operation(0xBA, 2, AddressingMode.IMPLICIT, tsx);
            table[0x8A] = new Operation(0x8A, 2, AddressingMode.IMPLICIT, txa);
            table[0x9A] = new Operation(0x9A, 2, AddressingMode.IMPLICIT, txs);
            table[0x98] = new Operation(0x98, 2, AddressingMode.IMPLICIT, tya);

            // ── Unofficial NOPs ────────────────────────────────────────────────
            table[0x04] = new Operation(0x04, 3, AddressingMode.ZERO_PAGE, nop);
            table[0x0C] = new Operation(0x0C, 4, AddressingMode.ABSOLUTE, nop);
            table[0x14] = new Operation(0x14, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0x1A] = new Operation(0x1A, 2, AddressingMode.IMPLICIT, nop);
            table[0x1C] = new Operation(0x1C, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);
            table[0x34] = new Operation(0x34, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0x3A] = new Operation(0x3A, 2, AddressingMode.IMPLICIT, nop);
            table[0x3C] = new Operation(0x3C, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);
            table[0x44] = new Operation(0x44, 3, AddressingMode.ZERO_PAGE, nop);
            table[0x54] = new Operation(0x54, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0x5A] = new Operation(0x5A, 2, AddressingMode.IMPLICIT, nop);
            table[0x5C] = new Operation(0x5C, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);
            table[0x64] = new Operation(0x64, 3, AddressingMode.ZERO_PAGE, nop);
            table[0x74] = new Operation(0x74, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0x7A] = new Operation(0x7A, 2, AddressingMode.IMPLICIT, nop);
            table[0x7C] = new Operation(0x7C, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);
            table[0x80] = new Operation(0x80, 2, AddressingMode.IMMEDIATE, nop);
            table[0x82] = new Operation(0x82, 2, AddressingMode.IMMEDIATE, nop);
            table[0x89] = new Operation(0x89, 2, AddressingMode.IMMEDIATE, nop);
            table[0xC2] = new Operation(0xC2, 2, AddressingMode.IMMEDIATE, nop);
            table[0xD4] = new Operation(0xD4, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0xDA] = new Operation(0xDA, 2, AddressingMode.IMPLICIT, nop);
            table[0xDC] = new Operation(0xDC, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);
            table[0xE2] = new Operation(0xE2, 2, AddressingMode.IMMEDIATE, nop);
            table[0xF4] = new Operation(0xF4, 4, AddressingMode.ZERO_PAGE_INDEXED_X, nop);
            table[0xFA] = new Operation(0xFA, 2, AddressingMode.IMPLICIT, nop);
            table[0xFC] = new Operation(0xFC, 4, AddressingMode.ABSOLUTE_INDEXED_X, nop);

            // ── Unofficial SLOs ────────────────────────────────────────────────
            table[0x03] = new Operation(0x03, 8, AddressingMode.INDEXED_INDIRECT, slo);
            table[0x07] = new Operation(0x07, 5, AddressingMode.ZERO_PAGE, slo);
            table[0x0F] = new Operation(0x0F, 6, AddressingMode.ABSOLUTE, slo);
            table[0x13] = new Operation(0x13, 8, AddressingMode.INDIRECT_INDEXED, slo);
            table[0x17] = new Operation(0x17, 6, AddressingMode.ZERO_PAGE_INDEXED_X, slo);
            table[0x1B] = new Operation(0x1B, 7, AddressingMode.ABSOLUTE_INDEXED_Y, slo);
            table[0x1F] = new Operation(0x1F, 7, AddressingMode.ABSOLUTE_INDEXED_X, slo);

            // ── Unofficial RLAs ────────────────────────────────────────────────
            table[0x23] = new Operation(0x23, 8, AddressingMode.INDEXED_INDIRECT, rla);
            table[0x27] = new Operation(0x27, 5, AddressingMode.ZERO_PAGE, rla);
            table[0x2F] = new Operation(0x2F, 6, AddressingMode.ABSOLUTE, rla);
            table[0x33] = new Operation(0x33, 8, AddressingMode.INDIRECT_INDEXED, rla);
            table[0x37] = new Operation(0x37, 6, AddressingMode.ZERO_PAGE_INDEXED_X, rla);
            table[0x3B] = new Operation(0x3B, 7, AddressingMode.ABSOLUTE_INDEXED_Y, rla);
            table[0x3F] = new Operation(0x3F, 7, AddressingMode.ABSOLUTE_INDEXED_X, rla);

            // ── Unofficial SREs ────────────────────────────────────────────────
            table[0x43] = new Operation(0x43, 8, AddressingMode.INDEXED_INDIRECT, sre);
            table[0x47] = new Operation(0x47, 5, AddressingMode.ZERO_PAGE, sre);
            table[0x4F] = new Operation(0x4F, 6, AddressingMode.ABSOLUTE, sre);
            table[0x53] = new Operation(0x53, 8, AddressingMode.INDIRECT_INDEXED, sre);
            table[0x57] = new Operation(0x57, 6, AddressingMode.ZERO_PAGE_INDEXED_X, sre);
            table[0x5B] = new Operation(0x5B, 7, AddressingMode.ABSOLUTE_INDEXED_Y, sre);
            table[0x5F] = new Operation(0x5F, 7, AddressingMode.ABSOLUTE_INDEXED_X, sre);

            // ── Unofficial RRAs ────────────────────────────────────────────────
            table[0x63] = new Operation(0x63, 8, AddressingMode.INDEXED_INDIRECT, rra);
            table[0x67] = new Operation(0x67, 5, AddressingMode.ZERO_PAGE, rra);
            table[0x6F] = new Operation(0x6F, 6, AddressingMode.ABSOLUTE, rra);
            table[0x73] = new Operation(0x73, 8, AddressingMode.INDIRECT_INDEXED, rra);
            table[0x77] = new Operation(0x77, 6, AddressingMode.ZERO_PAGE_INDEXED_X, rra);
            table[0x7B] = new Operation(0x7B, 7, AddressingMode.ABSOLUTE_INDEXED_Y, rra);
            table[0x7F] = new Operation(0x7F, 7, AddressingMode.ABSOLUTE_INDEXED_X, rra);

            // ── Unofficial SAXs ────────────────────────────────────────────────
            table[0x83] = new Operation(0x83, 6, AddressingMode.INDEXED_INDIRECT, sax);
            table[0x87] = new Operation(0x87, 3, AddressingMode.ZERO_PAGE, sax);
            table[0x8F] = new Operation(0x8F, 4, AddressingMode.ABSOLUTE, sax);
            table[0x97] = new Operation(0x97, 4, AddressingMode.ZERO_PAGE_INDEXED_Y, sax);

            // ── Unofficial LAXs ────────────────────────────────────────────────
            table[0xA3] = new Operation(0xA3, 6, AddressingMode.INDEXED_INDIRECT, lax);
            table[0xA7] = new Operation(0xA7, 3, AddressingMode.ZERO_PAGE, lax);
            table[0xAB] = new Operation(0xAB, 2, AddressingMode.IMMEDIATE, lax);
            table[0xAF] = new Operation(0xAF, 4, AddressingMode.ABSOLUTE, lax);
            table[0xB3] = new Operation(0xB3, 5, AddressingMode.INDIRECT_INDEXED, lax);
            table[0xB7] = new Operation(0xB7, 4, AddressingMode.ZERO_PAGE_INDEXED_Y, lax);
            table[0xBF] = new Operation(0xBF, 4, AddressingMode.ABSOLUTE_INDEXED_Y, lax);

            // ── Unofficial DCPs ────────────────────────────────────────────────
            table[0xC3] = new Operation(0x63, 8, AddressingMode.INDEXED_INDIRECT, dcp);
            table[0xC7] = new Operation(0x67, 5, AddressingMode.ZERO_PAGE, dcp);
            table[0xCF] = new Operation(0x6F, 6, AddressingMode.ABSOLUTE, dcp);
            table[0xD3] = new Operation(0x73, 8, AddressingMode.INDIRECT_INDEXED, dcp);
            table[0xD7] = new Operation(0x77, 6, AddressingMode.ZERO_PAGE_INDEXED_X, dcp);
            table[0xDB] = new Operation(0x7B, 7, AddressingMode.ABSOLUTE_INDEXED_Y, dcp);
            table[0xDF] = new Operation(0x7F, 7, AddressingMode.ABSOLUTE_INDEXED_X, dcp);

            // ── Unofficial ISCs ────────────────────────────────────────────────
            table[0xE3] = new Operation(0xE3, 8, AddressingMode.INDEXED_INDIRECT, isc);
            table[0xE7] = new Operation(0xE7, 5, AddressingMode.ZERO_PAGE, isc);
            table[0xEF] = new Operation(0xEF, 6, AddressingMode.ABSOLUTE, isc);
            table[0xF3] = new Operation(0xF3, 8, AddressingMode.INDIRECT_INDEXED, isc);
            table[0xF7] = new Operation(0xF7, 6, AddressingMode.ZERO_PAGE_INDEXED_X, isc);
            table[0xFB] = new Operation(0xFB, 7, AddressingMode.ABSOLUTE_INDEXED_Y, isc);
            table[0xFF] = new Operation(0xFF, 7, AddressingMode.ABSOLUTE_INDEXED_X, isc);

            return table;
        }
    }
}
