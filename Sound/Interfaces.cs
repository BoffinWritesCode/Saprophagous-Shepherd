using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniJam61Egypt.Sound
{
    public interface ISFXManager : IDisposable
    {
        void PlaySound(string name, float volume = 1f, float pan = 0f, float pitch = 0f);
    }
}
