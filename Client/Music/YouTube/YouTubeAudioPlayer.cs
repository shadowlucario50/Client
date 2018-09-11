﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Logic.Music.YouTube
{
    public class YouTubeAudioPlayer
    {
        public static YouTubeAudioPlayer Instance { get; set; }

        Form audioPlayerForm;
        WebBrowser webBrowser;

        const string page = @"<html>
    <head>
        <!-- Use latest version of IE -->
        <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
        <title></title>
    </head>
    <body>{0}</body>
</html>";

        public YouTubeAudioPlayer() {
            this.audioPlayerForm = new Form();

            webBrowser = new WebBrowser();
            audioPlayerForm.Controls.Add(webBrowser);
            var ptr = audioPlayerForm.Handle;
        }

        private string GenerateEmbedUrl(string id, bool loop) {
            var embedUrlBuilder = new StringBuilder();

            embedUrlBuilder.Append($"https://www.youtube.com/embed/{id}?version=3&autoplay=1");

            if (loop) {
                embedUrlBuilder.Append("&loop=1");
                // Workaround for YouTube player bug: https://developers.google.com/youtube/player_parameters
                embedUrlBuilder.Append($"&playlist={id}");
            }

            return embedUrlBuilder.ToString();
        }

        private delegate void PlayDelegate(string id);
        public void Play(string id) {
            if (audioPlayerForm.InvokeRequired) {
                audioPlayerForm.Invoke(new PlayDelegate(Play), id);
                return;
            }

            if (IO.Options.Music == false) {
                Stop();
                return;
            }

            var embedUrl = GenerateEmbedUrl(id, true);

            webBrowser.DocumentText = string.Format(page, $"<iframe width=\"100\" height=\"100\" src=\"{embedUrl}\" frameborder=\"0\" allowfullscreen></iframe>");
        }

        private delegate void StopDelegate();
        public void Stop() {
            if (audioPlayerForm.InvokeRequired) {
                audioPlayerForm.Invoke(new StopDelegate(Stop));
                return;
            }

            webBrowser.DocumentText = page;
        }
    }
}
