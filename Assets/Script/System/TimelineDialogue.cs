using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

public class TimelineDialogue : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    private List<float> dialogueTimepoints = new();

    private bool isTimelineDialogueActive;
    //private bool waitingForNext;
    private int currentDialogueIndex = 0;

    private void OnEnable()
    {
        Debug.Log($"TimelineDialogue: Bind with DialogueSystem in the OnEnable");
        DialogueSystem.Instance.NextRequested += HandleNext;

        director.played += OnDirectorPlayed;
        director.paused += OnDirectorPaused;

        dialogueTimepoints.Clear();

        TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
        if(timelineAsset == null)
        {
            Debug.LogError("[TimelineDialogue of DialogueSystem] PlayableAsset is not Timeline.");
            return;
        }

        foreach(var track in timelineAsset.GetOutputTracks())
        {
            // Find SignalTrack
            if(track is SignalTrack signalTrack)
            {
                var timepoints = new List<float>();
                Debug.Log($"[TimelineDialogue of DialogueSystem] Signal Track: {signalTrack}");

                // retrieve (Signal Emitter)
                foreach (var marker in signalTrack.GetMarkers())
                {
                    if (marker is SignalEmitter emitter)
                    {
                        float emitterTime = (float)emitter.time + 0.02f;
                        timepoints.Add(emitterTime);
                    }
                }

                // sort
                timepoints.Sort();

                dialogueTimepoints.AddRange(timepoints);
                
                Debug.Log($"[TimelineDialogue of DialogueSystem] Signal Emitter number: {timepoints.Count}");
                Debug.Log($"[TimelineDialogue of DialogueSystem] Signal Emitter time: {string.Join(", ", timepoints)}");
                break;
            }
        }

        
        if(dialogueTimepoints.Count > 0 && dialogueTimepoints[0] != 0)
        {
            dialogueTimepoints.Insert(0, 0);
        }
    }

    private void OnDisable()
    {
        if(DialogueSystem.Instance != null)
        {
            Debug.Log($"TimelineDialogue: unbind with DialogueSystem in the OnEnable");
            DialogueSystem.Instance.NextRequested -= HandleNext;
        }
    }

    public void BeginDialogue(int startLineId)
    {
        if(DialogueSystem.Instance.IsPlaying)
            return;

        DialogueSystem.Instance.StartDialogue((uint)startLineId);
        isTimelineDialogueActive = DialogueSystem.Instance.IsPlaying;
        currentDialogueIndex = 1;
        director.time = dialogueTimepoints[0];
        Debug.Log($"DialogueSystem : isTimelineDialogueActive = {isTimelineDialogueActive}");
    }

    public void PauseForNextDialogue()
    {
        if(!isTimelineDialogueActive || !DialogueSystem.Instance.IsPlaying)
        {
            return;
        }

        //waitingForNext = true;
        // director?.Pause();
        director?.playableGraph.GetRootPlayable(0).SetSpeed(0);
        Debug.Log($"DialogueSystem : Pause CutScene.");
    }

    private bool HandleNext()
    {
        // if(!isTimelineDialogueActive)
        //     return false;

        // if(!waitingForNext)
        //     return true;
        
        // waitingForNext = false;

        DialogueSystem.Instance.Advance();
        if(!DialogueSystem.Instance.IsPlaying)
            isTimelineDialogueActive = false;

        currentDialogueIndex++;
        if(currentDialogueIndex < dialogueTimepoints.Count)
        {
            float nextTime = dialogueTimepoints[currentDialogueIndex];
            director.time = nextTime;
            Debug.Log($"[TimelineDialogue] Jump to {nextTime}s");
        }
        
        // director?.Resume();
        director?.playableGraph.GetRootPlayable(0).SetSpeed(1);
        return true;
    }

    private void OnDirectorPlayed(PlayableDirector value)
    {
        Debug.Log($"DialogueSystem[Director] PLAYED / frame={Time.frameCount} / time={value.time}");
    }

    private void OnDirectorPaused(PlayableDirector value)
    {
        Debug.Log($"DialogueSystem[Director] PAUSED / frame={Time.frameCount} / time={value.time}");
    }
}
