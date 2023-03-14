using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _KoeiromapTest
{
    public class GetVoice
    {
        // private const string URL = "http://koeiromap-api.rinna.jp/api/inference/";
        private const string URL = "https://api.rinna.co.jp/models/cttse/koeiro";
        public static async UniTask<VoiceResponse> Voice(string text)
        {
            var voiceParam = new VoiceParam()
            {
                text = text,
                speaker_x = 2.44f,
                speaker_y = 2.88f,
                style = "talk"
            };
            var json = JsonUtility.ToJson(voiceParam);
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            var request = new UnityWebRequest(URL, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            return JsonUtility.FromJson<VoiceResponse>(request.downloadHandler.text);
        }

        public static string VoiceBase64Data(VoiceResponse voiceResponse)
        {
            var audio = voiceResponse.audio;
            return audio[(audio.IndexOf(",", StringComparison.Ordinal) + 1)..];
        }
    }

    public class VoiceParam
    {
        public string text;
        public float speaker_x;
        public float speaker_y;
        public string style;
    }

    public class VoiceResponse
    {
        public string audio;
        public string phonemes;
        public int seed;
    }
}