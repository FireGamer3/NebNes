using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.CPU.Interfaces {
    public interface IEmulatorHost {
        void PresentFrame(uint[] framebuffer);
        void PushAudio(float[] samples);
        void PushAudio(float[] samples, int count);

        //TODO: Controller State
    }
}
