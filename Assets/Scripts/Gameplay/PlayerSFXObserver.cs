using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXObserver : PlayerObserver
{
    //Add the proper sound effects to showcase when a player loses or wins
    [SerializeField] private List<AudioSource> audio = new List<AudioSource>();

    protected override void OnVSFX(int value)
    {
        if (value == 0)
        {
            audio[0].Play();
            Debug.Log("SFX lose");
        }

        else
        {
            audio[1].Play();
            Debug.Log("SFX win");
        }
    }

    protected override void OnTimeEnd()
    {}

    protected override void OnScoreUpdate()
    {}
}
