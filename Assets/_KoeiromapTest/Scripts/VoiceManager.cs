using System;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
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

            // AudioClipをAudioSourceに設定
            _audioSource.clip = await ConvertBase64ToAudioClip(voiceBase64);

            // 再生
            _audioSource.Play();
        }

        private static async UniTask<AudioClip> ConvertBase64ToAudioClip(string base64EncodedMp3String)
        {
            var audioBytes = Convert.FromBase64String(base64EncodedMp3String);
            var tempPath = Application.persistentDataPath + "tmpMP3Base64.wav";
            await File.WriteAllBytesAsync(tempPath, audioBytes);
            var request = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.WAV);
            var asyncOperation = request.SendWebRequest();
            await asyncOperation;
            if (request.result.Equals(UnityWebRequest.Result.ConnectionError))
            {
                Debug.LogError(request.error);
                return null;
            }

            var content = DownloadHandlerAudioClip.GetContent(request);
            request.Dispose();
            return content;

        }
    }
}