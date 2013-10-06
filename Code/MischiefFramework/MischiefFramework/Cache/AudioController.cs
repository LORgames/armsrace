using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MischiefFramework.Cache {
    internal class AudioController {
        private static bool disableAudio = false;
        private static List<SoundEffectInstance> currentlyActiveLoops = new List<SoundEffectInstance>();

        internal static void PlayOnce(string filename, float volumeMultiplier = 1.0f, bool mp3 = false) {
            if (disableAudio) return;

            try {
                if (mp3) {
                    //ResourceManager.LoadAsset<Song>("Sounds/" + filename).Play(volumeMultiplier * SettingManager._soundEffectsVolume, 0.0f, 0.0f);
                } else {
                    ResourceManager.LoadAsset<SoundEffect>("Sounds/" + filename).Play(volumeMultiplier * SettingManager._soundEffectsVolume, 0.0f, 0.0f);
                }
            } catch {
                disableAudio = true;
            }
        }

        internal static SoundEffectInstance GetSoundEffect(string filename, float volumeMultiplier = 1.0f) {
            if (disableAudio) return null;

            try {
                SoundEffectInstance sei = ResourceManager.LoadAsset<SoundEffect>("Sounds/" + filename).CreateInstance();
                sei.Volume = volumeMultiplier * SettingManager._soundEffectsVolume;
                return sei;
            } catch {
                disableAudio = true;
            }
            return null;
        }

        internal static void PlayLooped(string filename, float volumeMultiplier = 1.0f, bool mp3 = false) {
            if (disableAudio) return;

            try {
                SoundEffectInstance bg = ResourceManager.LoadAsset<SoundEffect>("Sounds/" + filename).CreateInstance();
                if (mp3) {
                    //bg = ResourceManager.LoadAsset<Song>("Sounds/" + filename).CreateInstance();
                } else {
                    bg = ResourceManager.LoadAsset<SoundEffect>("Sounds/" + filename).CreateInstance();
                }
                bg.IsLooped = true;
                bg.Volume = volumeMultiplier * SettingManager._musicVolume;
                bg.Play();

                currentlyActiveLoops.Add(bg);
            } catch {
                disableAudio = true;
            }
        }

        internal static Song GetSong(string filename, float volumeMultiplier = 1.0f) {
            if (disableAudio) return null;

            try {
                Song song = ResourceManager.LoadAsset<Song>("Sounds/" + filename);
                return song;
            } catch {
                disableAudio = true;
            }

            return null;
        }

        internal static void StopAllLoops() {
            foreach (SoundEffectInstance effect in currentlyActiveLoops) {
                effect.Stop();
            }
        }

        internal static void RemoveAllLoops() {
            for (int i = currentlyActiveLoops.Count -1; i >= 0; i--) {
                currentlyActiveLoops[i].Stop();
                currentlyActiveLoops.RemoveAt(i);
            }
        }
    }
}
