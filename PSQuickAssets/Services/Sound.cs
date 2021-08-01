using System.Media;

namespace PSQuickAssets.Services
{
    public static class Sound
    {
        private static readonly SoundPlayer _soundPlayer = new SoundPlayer();

        public static void Ding()
        {
            //_soundPlayer.SoundLocation = "Resources\\Sounds\\done_2.wav";
            //_soundPlayer.Play();
        }

        public static void Error()
        {
            //_soundPlayer.SoundLocation = "Resources\\Sounds\\error.wav";
            //_soundPlayer.Play();
        }

        public static void Click()
        {
            //_soundPlayer.SoundLocation = "Resources\\Sounds\\click.wav";
            //_soundPlayer.Play();
        }
    }
}
