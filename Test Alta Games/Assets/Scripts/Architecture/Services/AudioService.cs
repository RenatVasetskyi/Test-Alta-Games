using System.Collections.Generic;
using Architecture.Services.Interfaces;
using Audio;
using Data;
using UnityEngine;

namespace Architecture.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IBaseFactory _baseFactory;
        private readonly GameSettings _gameSettings;

        private readonly List<SfxData> _sfxDataList = new();
        private readonly List<MusicData> _musicDataList = new();

        private AudioSource _sfxAudioSource;
        private AudioSource _musicAudioSource;

        public AudioService(IAssetProvider assetProvider, IBaseFactory baseFactory, 
            GameSettings gameSettings) 
        {
            _assetProvider = assetProvider;
            _baseFactory = baseFactory;
            _gameSettings = gameSettings;
        }
        
        public void PlayMusic(MusicType musicType)
        {
            MusicData musicData = GetMusicData(musicType);
            _musicAudioSource.clip = musicData.Clip;
            _musicAudioSource.Play();
        }

        public void PlaySfx(SfxType sfxType)
        {
            SfxData sfxData = GetSfxData(sfxType);
            _sfxAudioSource.PlayOneShot(sfxData.Clip);
        }

        public void Initialize()
        {
            InitializeSfxDataList();
            InitializeMusicDataList();
            InitializeSfxAudioSource();
            InitializeMusicAudioSource();
        }

        public void StopMusic()
        {
            _musicAudioSource.Stop();
        }

        private MusicData GetMusicData(MusicType musicType)
        {
            return _musicDataList.Find(data => data.MusicType == musicType);
        }

        private SfxData GetSfxData(SfxType sfxType)
        {
            return _sfxDataList.Find(data => data.SfxType == sfxType);
        }
        
        private void InitializeSfxDataList()
        {
            SfxHolder sfxHolder = _assetProvider.Initialize<SfxHolder>(AssetPath.SfxHolder);

            _sfxDataList.AddRange(sfxHolder.SoundEffects);
        }

        private void InitializeMusicDataList()
        {
            MusicHolder musicHolder = _assetProvider.Initialize<MusicHolder>(AssetPath.MusicHolder);

            _musicDataList.AddRange(musicHolder.Musics);
        }
        
        private async void InitializeSfxAudioSource()
        {
            _sfxAudioSource = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.SfxAudioSource, Vector3.zero, Quaternion.identity, null)).GetComponent<AudioSource>();
        }

        private async void InitializeMusicAudioSource()
        {
            _musicAudioSource = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.MusicAudioSource, Vector3.zero, Quaternion.identity, null)).GetComponent<AudioSource>();
        }
    }
}