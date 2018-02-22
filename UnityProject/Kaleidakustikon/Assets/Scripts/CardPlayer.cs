using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is responsible for handling the audio sources and audio clips. 
//The class relies on three audio sources that are programmed to load the correct audiosource to play next. 
//Three sources are used to ensure no clips are loaded too late
public class CardPlayer : MonoBehaviour {
    private double _timeline;
    private AudioSource[] _sources;
    private AudioClip[] _clips;
    public double Tempo;
    private double _clipLength;

    private int currentSource = 0;
    
    //Initialise references
    void Start () {
        if (Tempo <= 0){
            throw new Exception("Tempo must be over 0");
        }
        _sources = GetComponents<AudioSource> ();
        _clipLength = 60/Tempo * 3;
    }

    public void Stop(){
        foreach (var audioSource in _sources){
            audioSource.Stop();
        }
    }

    //Takes a string and loads the audioclips described by it
    public void LoadAudio(List<string> toLoad){
        var groups = new [] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v'};
        _clips = new AudioClip[toLoad.Count];

        int currentClip = 0;
        bool isMinor = false;
        while (currentClip < toLoad.Count){
            var group = groups[currentClip];
            if (group == 'h'){
                isMinor = hIsMinor(Int32.Parse("" + toLoad[currentClip]));
                Debug.Log("Minor: " + isMinor);
            }

            if (group == 'i' || group == 'k' || group == 'l' || group == 'm' || group == 'n' || group == 'o'){
                //Debug.Log("Loading a minor card");
                _clips[currentClip] = ResourceManager.GetAudioClip(groups[currentClip], toLoad[currentClip], isMinor);
            }
            else{
                _clips[currentClip] = ResourceManager.GetAudioClip(groups[currentClip], toLoad[currentClip], false);
            }
            currentClip++;
        }
    }

    //Determines if the h-card is minor
    public bool hIsMinor(int card){
        switch (card){
            case 3: return true;
            case 4: return true;
            case 10: return true;
            case 11: return true;
            case 12: return true;
            default: return false;
        }
    }

    public void PlayClip(int clip, double time){
        _sources[currentSource].clip = _clips[clip];
        _sources[currentSource].PlayScheduled(time);
        currentSource++;
        if (currentSource > _sources.Length - 1)
            currentSource = 0;
    }

    //This returns the order and seconds inbetween playback of clips. It is how unity handles sequential timed behavior
    public IEnumerator PlayCoroutine(string toPlay){
        int currentClip = 0;
        int currentSource = 0;
        _sources[currentSource].clip = _clips[currentClip];
        _sources [currentSource].PlayScheduled (_timeline);

        _timeline = AudioSettings.dspTime;
        //compensate for forplay silence
        _timeline += _clipLength;
        currentClip++;
        currentSource++;
        yield return new WaitForSecondsRealtime ((float)_clipLength);
        while(currentClip < toPlay.Length){
            if (currentSource > _sources.Length - 1)
                currentSource = 0;
            _sources[currentSource].clip = _clips[currentClip];
            _timeline += _clipLength;
            _sources [currentSource].PlayScheduled (_timeline);
            currentClip++;
            currentSource++;
            yield return new WaitForSecondsRealtime ((float)_clipLength);
        }
        //Wait till last clip is over
        yield return new WaitForSeconds((float)_clipLength);
    }
}
