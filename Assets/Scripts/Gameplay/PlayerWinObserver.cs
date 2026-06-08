using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerWinObserver : PlayerObserver
{
    protected override void OnAddHealth(int value)
    {}

    protected override void OnDoDamage(int value)
    {}

    protected override void OnScoreUpdate()
    {}

    protected override void OnTimeEnd()
    {
        Debug.Log("Time finished");
    }
}
