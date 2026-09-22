using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

public class TimelineDialogue : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    private List<float> dialogueTimepoints = new();
    private List<float> cutEndTimes = new();

    private bool isTimelineDialogueActive;
    private bool waitingForNext;
    private bool passWaiting;
    private int currentDialogueIndex = 0;

    private int cutIndex;
    private bool cutHeld;

    // private void Update()
    // {
    //     if (!isTimelineDialogueActive || cutHeld) return;
    //     if (cutIndex < cutEndTimes.Count && director.time >= cutEndTimes[cutIndex])
    //     {
    //         director.time = cutEndTimes[cutIndex];
    //         director.playableGraph.GetRootPlayable(0).SetSpeed(0);
    //         cutHeld = true;
    //         Debug.Log($"[Update-based Pause] time={director.time}");
    //     }
    // }

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
                        float emitterTime = (float)emitter.time + Time.deltaTime;
                        timepoints.Add(emitterTime);
                    }
                }

                // sort
                timepoints.Sort();

                dialogueTimepoints.AddRange(timepoints);
                
                Debug.Log($"[TimelineDialogue of DialogueSystem] Signal Emitter number: {timepoints.Count}");
                Debug.Log($"[TimelineDialogue of DialogueSystem] Signal Emitter time: {string.Join(", ", cutEndTimes)}");
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

        UIDialoguePanel ui = UIManager.Instance.Get<UIDialoguePanel>();
        if(ui != null)
        {
            Debug.Log($"TimelineDialogue: unbind with DialoguePanel.");
            ui.onTypeWritingEnded -= FinishWaiting;
        }
    }

    private void Update()
    {
        if (isTimelineDialogueActive)
            Debug.Log($"[TimelineDialogue] Update: state={director.state}, time={director.time}, speed={director.playableGraph.GetRootPlayable(0).GetSpeed()}, frame={Time.frameCount}");
    }

    public void BeginDialogue(int startLineId)
    {
        if(DialogueSystem.Instance.IsPlaying)
            return;

        UIDialoguePanel ui = UIManager.Instance.Get<UIDialoguePanel>();
        if(ui != null)
        {
            Debug.Log($"TimelineDialogue: bind with DialoguePanel.");
            ui.onTypeWritingEnded += FinishWaiting;
        }

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

        // director?.Pause();
        waitingForNext = true;
        director?.playableGraph.GetRootPlayable(0).SetSpeed(0);
        Debug.Log($"DialogueSystem : Pause CutScene, time={director.time}");
    }

    private void FinishWaiting()
    {
        Debug.Log($"TimelineDialogue: waiting finish.");
        waitingForNext = false;
    }

    private bool HandleNext()
    {
        // if(!isTimelineDialogueActive)
        //     return false;
        // if(waitingForNext)
        // {
        //     Debug.Log($"TimelineDialogue: HandleNext() waiting finish.");
        //     waitingForNext = false;
        //     return true;
        // }
        
        // waitingForNext = false;

        Debug.Log($"TimelineDialogue: HandleNext() work.");

        director?.Pause();
        
        currentDialogueIndex++;
        if(currentDialogueIndex < dialogueTimepoints.Count)
        {
            float nextTime = dialogueTimepoints[currentDialogueIndex] + 0.02f;
            director.time = nextTime;
            Debug.Log($"[TimelineDialogue] Jump to {nextTime}s");
        }
        
        DialogueSystem.Instance.Advance();
        if(!DialogueSystem.Instance.IsPlaying)
            isTimelineDialogueActive = false;            

        waitingForNext = false;
        director?.playableGraph.GetRootPlayable(0).SetSpeed(1);
        director?.Resume();
        Debug.Log($"[TimelineDialogue] director speed is {director?.playableGraph.GetRootPlayable(0).GetSpeed()}s");
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
