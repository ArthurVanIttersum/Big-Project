using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool startOnPlay;
    private Coroutine controller;
    void Start()
    {
        if (!startOnPlay) return;
        
        StartAnimation();
    }

    [ContextMenu("StartAnimation")]
    private void StartAnimation()
    {
        if (controller != null) return;
        controller = StartCoroutine(ControllAnimation());
    }

    private IEnumerator ControllAnimation()
    {
        
        yield return null;
    }
    
}
