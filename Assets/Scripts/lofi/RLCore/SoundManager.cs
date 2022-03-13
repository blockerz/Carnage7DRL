using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public static class SoundManager
    {
        public enum Sound
        {
            SHOOT,
            LOW_GEAR,
            HIGH_GEAR,
            EXPLOSION,
            PICKUP,
            BUMPY,
            STAGE_MUSIC,
            BOSS_MUSIC,
            SHOTGUN_EMPTY,
            SHOTGUN,
        }

        public static float volume = 0.4f;

        private static Dictionary<Sound, float> soundTimers;
        private static GameObject oneShotGameObject;
        private static AudioSource oneShotAudioSource;

        static SoundManager()
        {
            soundTimers = new Dictionary<Sound, float>();
            soundTimers[Sound.SHOOT] = Time.time;
        }

        public static void PlaySound(Sound sound, Vector3 position)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(sound);
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.volume = volume;
                audioSource.Play();

                Object.Destroy(soundGameObject, audioSource.clip.length);
            }
        }

        public static void PlaySound(Sound sound)
        {
            if (CanPlaySound(sound))
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("One Shot Sound");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                }
                oneShotAudioSource.PlayOneShot(GetAudioClip(sound), volume);
                //Object.Destroy(soundGameObject, audioSource.clip.length);
            }
        }

        public static AudioClip GetAudioClip(Sound sound)
        {
            foreach(Assets.SoundAudioClip soundAudioClip in Assets.Instance.soundAudioClipArray)
            {
                if (soundAudioClip.sound == sound)
                {
                    return soundAudioClip.audioClip;
                }
            }

            Debug.LogError("Sound not found: " + sound);
            return null;
        }

        public static bool CanPlaySound(Sound sound)
        {
            switch(sound)
            {
                //case Sound.SHOOT:
                //    if (soundTimers.ContainsKey(sound))
                //    {
                //        float lastTimePlayed = soundTimers[sound];
                //        float shootMax = 0.1f;
                //        if (lastTimePlayed + shootMax < Time.time)
                //        {
                //            soundTimers[sound] = Time.time;
                //            return true;
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        return true;
                //    }
                default:
                    return true;
            }
        }
    }
}