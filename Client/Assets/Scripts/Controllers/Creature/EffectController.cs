using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private AudioSource _audio;

    private CreatureController _creature;

    public CreatureController Creature { get { return _creature; } set { _creature = value; } }

    void Start()
    {
        //_audio = GetComponent<AudioSource>();
        //if(!Managers.Sound.SoundOn)
        //{
        //    _audio.mute = true;
        //}
        //_audio.Play();
    }

    void Update()
    {
        if (_creature != null)
            transform.position = _creature.transform.position;
    }
}
