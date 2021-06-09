using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MiniJam61Egypt.Sound
{
    public class XnaSFXManager
    {
        public float Volume { get; set; }

        private Dictionary<string, SoundEffect> _dict;
        private SoundEffectInstance[] _instances;

        public XnaSFXManager(int maxSounds = 100)
        {
            Volume = 1f;

            _instances = new SoundEffectInstance[maxSounds];
            _dict = new Dictionary<string, SoundEffect>();
        }

        public void PlaySound(string name, float volume = 1f, float pan = 0f, float pitch = 0f)
        {
            if (_dict.TryGetValue(name, out SoundEffect sfx))
            {
                int index = GetFreeIndex();

                _instances[index] = sfx.CreateInstance();
                _instances[index].Volume = volume * Volume;
                _instances[index].Pan = pan;
                _instances[index].Pitch = pitch;
                _instances[index].Play();
            }
        }

        public void LoadAllFromFolder(ContentManager cm, string directoryInContent)
        {
            string directory = cm.RootDirectory + Path.DirectorySeparatorChar + directoryInContent;
            string[] files = Directory.GetFiles(directory);
            foreach(string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string ext = Path.GetExtension(file);

                if (ext != ".xnb")
                    continue;

                _dict[name] = cm.Load<SoundEffect>($"{directoryInContent}\\{name}");
            }
        }

        private int GetFreeIndex()
        {
            for (int i = 0; i < _instances.Length; i++)
            {
                if (_instances[i] == null || _instances[i].IsDisposed || _instances[i].State == SoundState.Stopped)
                {
                    return i;
                }
            }
            return 0;
        }

        public void Dispose()
        {
            //Dispose SoundEffectInstance objects
            for (int i = 0; i < _instances.Length; i++)
            {
                _instances[i]?.Dispose();
            }
            //Dispose SoundEffect objects
            foreach(KeyValuePair<string, SoundEffect> pair in _dict)
            {
                pair.Value.Dispose();
            }
        }

        public SoundEffect this[string key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
            }
        }
    }
}
