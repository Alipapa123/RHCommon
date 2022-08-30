using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class VoiceHelper
    {
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="Content"></param>
        public static void PlayStringAsync(string Content)
        {
            Task.Run(new Action(() =>
            {
                SpeechSynthesizer ssh = new SpeechSynthesizer();
                var t = ssh.GetInstalledVoices();
                ssh.SelectVoice("Microsoft Huihui Desktop");
                ssh.Rate = 1;
                ssh.Speak(Content);
            }));
        }
    }
}
