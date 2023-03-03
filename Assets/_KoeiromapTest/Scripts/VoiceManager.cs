using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _KoeiromapTest
{
    public class VoiceManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button playVoiceButton;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            playVoiceButton.OnClickAsObservable()
                .Subscribe(_ => PlayVoice(inputField.text).Forget()).AddTo(this);
        }

        private async UniTask PlayVoice(string text)
        {
            var voiceResponse = await GetVoice.Voice(text);
            var voiceBase64 = GetVoice.VoiceBase64Data(voiceResponse);
            // Base64文字列からbyte配列を作成
            var bytes = Convert.FromBase64String(voiceBase64);

            // byte配列をfloat配列に変換する
            var floatSamples = new float[bytes.Length / 2];
            for (var i = 0; i < floatSamples.Length; i++)
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