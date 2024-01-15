using Audio;

namespace Architecture.Services.Interfaces
{
    public interface IAudioService
    {
        void Initialize();
        void PlayMusic(MusicType musicType);
        void PlaySfx(SfxType sfxType);
        void StopMusic();
    }
}