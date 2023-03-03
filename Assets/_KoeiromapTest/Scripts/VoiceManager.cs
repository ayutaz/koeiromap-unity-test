using System;
using System.IO;
using UnityEngine;

namespace _KoeiromapTest
{
    public class VoiceManager : MonoBehaviour
    {
        private string _filePath;
        private AudioSource _audioSource;
        private void Awake()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "voice.wav");
            _audioSource = GetComponent<AudioSource>();
            Debug.Log("filePath: " + _filePath);
        }

        private async void Start()
        {
            var voiceResponse = await GetVoice.Voice("こんにちは");
            var voiceBase64 = GetVoice.VoiceBase64Data(voiceResponse);
            // Base64文字列からbyte配列を作成
            var bytes = Convert.FromBase64String(voiceBase64);

            // byte配列をfloat配列に変換する
            var floatSamples = new float[bytes.Length / 2];
            for (int i = 0; i < floatSamples.Length; i++)
            {
                var sample = BitConverter.ToInt16(bytes, i * 2);
                floatSamples[i] = sample / 32768f;
            }

            // AudioClipを作成する
            var clip = AudioClip.Create("clip", floatSamples.Length, 1, 44100, false);
            clip.SetData(floatSamples, 0);

            // AudioClipをAudioSourceに設定
            _audioSource.clip = clip;

            // 再生
            _audioSource.Play();
        }
    }
}