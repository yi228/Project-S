using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
public class DarkRoom : MapController
{
    PostProcessProfile _ppProfile;
    Vignette _vignette;
    float buffIntensity = 0.5f;
    float generalIntensity = 0.7f;
    float lerpTime = 1.0f;
    float currentIntensity = 0f;
    float currentTime = 0f;
    bool isUpdate = false;
    bool isStop = false;
    void Update()
    {
        if (isUpdate)
        {
            ShowDarkRoom();
        }
        else if (isStop)
        {
            OffDarkRoom();
        }
    }
    void ShowDarkRoom()
    {
        if (currentTime >= lerpTime)
        {
            return;
        }
        currentTime += Time.deltaTime;
        if (Managers.Object.MyPlayer.CanLightBuff)
        {
            if (_ppProfile == null)
            {
                _ppProfile = GameObject.Find("PP Volume_Ingame").GetComponent<PostProcessVolume>().profile;
                _ppProfile.TryGetSettings<Vignette>(out _vignette);
            }
            currentIntensity = Mathf.Lerp(0, buffIntensity, currentTime / lerpTime);
            _vignette.intensity.value = currentIntensity;
        }
        else
        {
            if (_ppProfile == null)
            {
                _ppProfile = GameObject.Find("PP Volume_Ingame").GetComponent<PostProcessVolume>().profile;
                _ppProfile.TryGetSettings<Vignette>(out _vignette);
            }
            currentIntensity = Mathf.Lerp(0, generalIntensity, currentTime / lerpTime);
            _vignette.intensity.value = currentIntensity;
        }
    }
    void OffDarkRoom()
    {
        if (_ppProfile == null) return;

        currentTime += Time.deltaTime;
        if (Managers.Object.MyPlayer != null && Managers.Object.MyPlayer.CanLightBuff)
        {
            currentIntensity = Mathf.Lerp(buffIntensity, 0f, currentTime / lerpTime);
            _vignette.intensity.value = currentIntensity;
        }
        else
        {
            currentIntensity = Mathf.Lerp(generalIntensity, 0f, currentTime / lerpTime);
            _vignette.intensity.value = currentIntensity;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isUpdate = true;
            isStop = false;
            currentTime = 0f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isUpdate = false;
            isStop = true;
            currentTime = 0f;
        }
    }



}
