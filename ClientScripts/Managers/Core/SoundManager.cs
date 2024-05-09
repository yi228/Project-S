using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public bool SoundOn = true;
    public bool ShakeOn = true;

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // MP3 Player   -> AudioSource
    // MP3 음원     -> AudioClip
    // 관객(귀)     -> AudioListener

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.Effect].spatialBlend = 1f;
            _audioSources[(int)Define.Sound.Effect].rolloffMode = AudioRolloffMode.Linear;
            _audioSources[(int)Define.Sound.Effect].minDistance = 1f;
            _audioSources[(int)Define.Sound.Effect].maxDistance = 20f;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void AudioShift()
    {
        if (SoundOn)
        {
            SoundOn = false;
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.mute = true;
            }
        }
        else
        {
            SoundOn = true;
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.mute = false;
            }
        }
    }

    public void AudioSync()
    {
        if (SoundOn)
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.mute = false;
            }
        }
        else
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.mute = true;
            }
        }
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
	{
        if (audioClip == null)
            return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
            audioSource.volume = 0.6f;
			audioSource.Play();
		}
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	public AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }

    public void ChangeBgmWhenSceneLoaded()
    {
        AudioClip willPlayingClip = _audioSources[(int)Define.Sound.Bgm].clip;
        AudioClip currentPlayingClip;
        switch (Managers.Scene.CurrentScene)
        {
            case (Define.Scene.StartMenu):
                currentPlayingClip = GetOrAddAudioClip("Bgm/Lobby", Define.Sound.Bgm);
                if (willPlayingClip != currentPlayingClip)
                    Play(currentPlayingClip, Define.Sound.Bgm);
                break;
            case (Define.Scene.Lobby):
                currentPlayingClip = GetOrAddAudioClip("Bgm/Lobby", Define.Sound.Bgm);
                if (willPlayingClip != currentPlayingClip)
                    Play(currentPlayingClip, Define.Sound.Bgm);
                break;
            case (Define.Scene.Game):
                if (Managers.Game.IsStartGame == false)
                    return;
                currentPlayingClip = GetOrAddAudioClip("Bgm/Game", Define.Sound.Bgm);
                if (willPlayingClip != currentPlayingClip)
                    Play(currentPlayingClip, Define.Sound.Bgm);
                break;
            case (Define.Scene.SingleGame):
                currentPlayingClip = GetOrAddAudioClip("Bgm/Game", Define.Sound.Bgm);
                if (willPlayingClip != currentPlayingClip)
                    Play(currentPlayingClip, Define.Sound.Bgm);
                break;
        }
    }
    public WeaponType GetWeaponTypeByPath(string path)
    {
        if (path == "Effect/Weapon/Pistol/Pistol_Fire")
            return WeaponType.Pistol;
        else if (path == "Effect/Weapon/Rifle/Rifle_Fire")
            return WeaponType.Rifle;
        else if (path == "Effect/Weapon/Sniper/Sniper_Fire")
            return WeaponType.Sniper;
        else if (path == "Effect/Weapon/Shotgun/Shotgun_Fire")
            return WeaponType.Shotgun;
        else
            return WeaponType.Default;
    }
    public void PlayFireSound()
    {
        C_PlaySound soundPacket = new C_PlaySound();

        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                soundPacket.Path = "Effect/Weapon/Pistol/Pistol_Fire";
                break;
            case WeaponType.Rifle:
                soundPacket.Path = "Effect/Weapon/Rifle/Rifle_Fire";
                break;
            case WeaponType.Sniper:
                soundPacket.Path = "Effect/Weapon/Sniper/Sniper_Fire";
                break;
            case WeaponType.Shotgun:
                soundPacket.Path = "Effect/Weapon/Shotgun/Shotgun_Fire";
                break;
        }
        Managers.Network.Send(soundPacket);
    }
    public void PlayReloadSound()
    {
        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                Play("Effect/Weapon/Pistol/Pistol_Reload");
                break;
            case WeaponType.Rifle:
                Play("Effect/Weapon/Rifle/Rifle_Reload");
                break;
            case WeaponType.Sniper:
                Play("Effect/Weapon/Sniper/Sniper_Reload");
                break;
            case WeaponType.Shotgun:
                Play("Effect/Weapon/Shotgun/Shotgun_Reload");
                break;
        }
    }
    public void PlayHitSound(int id)
    {
        C_PlaySound soundPacket = new C_PlaySound();
        soundPacket.Path = "Effect/Player/HitSound";
        soundPacket.IsNotPlayer = true;
        soundPacket.NotPlayerId = id;
        Managers.Network.Send(soundPacket);
    }
}
