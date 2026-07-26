using NebNes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.Misc {
    public class Controller {
        private int cursor = 0;
        private bool strobe = false;
        private bool[] buttons = { false, false, false, false, false, false, false, false };

        Controller? other;

        public void SetOther(Controller? other) {
            this.other = other;
        }

        public void Update(ButtonKey key, bool value) {
            buttons[(int)key] = value;
        }

        public byte OnRead() {
            if (cursor >= buttons.Length) return 1;
            bool state = buttons[cursor];
            if (!strobe) cursor += 1;
            return (byte)(state ? 0x01 : 0x00);
        }

        public void OnWrite(byte value) {
            if((value & 0x01) == 1) {
                resetCursor();
                other!.resetCursor();
                setStrobe(true);
                other!.setStrobe(true);
            }else {
                setStrobe(false);
                other!.setStrobe(false);
            }
        }

        public void resetCursor() {
            cursor = 0;
        }

        public void setStrobe(bool value) { strobe = value; }
    }
}
