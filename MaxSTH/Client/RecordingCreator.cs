using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;
using STHMaxzzzie.Client;

namespace STHMaxzzzie.Client
{
    public class RecordingCreator : BaseScript
    {
        bool isKeyPressed = false;
        static bool isRecording = false;
        static bool isInstantReplayRecording = false;

        public RecordingCreator()
        {
            // Register keybinds
            RegisterKeyMapping("+toggleRecPress", "Start/Save recording", "keyboard", "F1");
            RegisterKeyMapping("+toggleReplayPress", "Start/Save replay", "keyboard", "F2");
            RegisterKeyMapping("+stopRecPress", "Stop rec and discard", "keyboard", "F3");
        }

        [Command("rec")]
        static void RockstarRecording(int source, List<object> args, string raw)
        {
            //NotificationScript.ShowNotification($"starting rockstarRecording");
            if (args.Count == 0)
            {
                string rec = "off";
                string clip = "off";
                if (isRecording) rec = "~g~on~w~";
                if (isInstantReplayRecording) clip = "~g~on~w~";
                NotificationScript.ShowNotification($"~r~[Recording] status:~w~\nRecording: {rec}. Instant replay: {clip}.");
                return;
            }
            switch (args[0])
            {
                case "start":
                    if (isRecording) { NotificationScript.ShowErrorNotification($"~r~[Recording]~w~\n~g~Recording was already on."); return; }
                    //NotificationScript.ShowNotification($"~g~[Recording]~w~\nStarting recording.");
                    StartRecording(1);
                    isRecording = true;
                    return;

                case "replay":
                    StartRecording(0);
                    isInstantReplayRecording = true;
                    NotificationScript.ShowNotification($"~g~[Recording]~w~\nStarting instant replay recording.");
                    return;

                case "stop":
                    if (!isRecording && !isInstantReplayRecording) { NotificationScript.ShowErrorNotification($"~r~[Recording]~w~\n~g~Recording was off."); return; }
                    NotificationScript.ShowNotification($"~r~[Recording]~w~\nStopping recording.\n~r~Discarded recording.");
                    StopRecordingAndDiscardClip();
                    isRecording = false;
                    isInstantReplayRecording = false;
                    return;

                case "save":
                    //if (!isRecording && !isInstantReplayRecording) { NotificationScript.ShowErrorNotification($"~r~[Recording]~w~\n~g~Recording was off."); return; }
                    NotificationScript.ShowNotification($"~r~[Recording]~w~\nStopping recording.\n~g~Recording saved.");
                    if (isRecording)
                    {
                        StopRecordingAndSaveClip();
                        isRecording = false;
                    }
                    else //isInstantReplayRecording
                    {
                        SaveRecordingClip();
                        isInstantReplayRecording = false;
                    }
                    return;

            }
            NotificationScript.ShowErrorNotification($"~r~[Recording help]~w~\n~g~See console (f8).");
            NotificationScript.displayClientDebugLine($"\n   ---   ---   ---   ---   [Recording help start]   ---   ---   ---   ---   \n" +
            "This controls rockstar editor recordings.\n" +
            "/rec - See the current recording status.\n" +
            "/rec start - start recording.\n" +
            "/rec replay - start instant replay recording.\n" +
            "/rec stop - stop recording for both normal recording and instant replay recording, discarding clip.\n" +
            "/rec save - saves current recording and stops recording.\n" +
            "   ---   ---   ---   ---   [Recording help end]   ---   ---   ---   ---   \n");
        }

        [Command("+toggleRecPress")]
        private void ToggleRec()
        {
            if (!isRecording)//start rec
            {
                StartRecording(1);
                isRecording = true;
            }
            else //stop rec
            {
                StopRecordingAndSaveClip();
                isRecording = false;
            }
        }

        [Command("+toggleReplayPress")]
        private void ToggleInstantReplayRec()
        {
            if (!isInstantReplayRecording)
            {
                StartRecording(0);
                isInstantReplayRecording = true;
                NotificationScript.ShowNotification($"~r~[Recording]~w~\nStarting instant replay recording.");
            }
            else
            {
                SaveRecordingClip();
                isInstantReplayRecording = false;
                NotificationScript.ShowNotification($"~r~[Recording]~w~\nSaved instant replay clip.");
            }
        }

        [Command("+stopRecPress")]
        private void StopAllRecAndDiscard()
        {
            StopRecordingAndDiscardClip();
            isRecording = false;
            isInstantReplayRecording = false;
            NotificationScript.ShowNotification($"~r~[Recording]~w~\nStopping recording.\n~r~Discarded recording.");
        }


        //when lifting key
        [Command("-toggleRecPress")]
        private void toggleRecRelease()
        {
            if (isKeyPressed) isKeyPressed = false;
            // No action needed for release in this context. But prevents a msg
        }
        [Command("-toggleReplayPress")]
        private void toggleReplayRelease()
        {
            if (isKeyPressed) isKeyPressed = false;
        }
        [Command("-stopRecPress")]
        private void stopRecRelease()
        {
            if (isKeyPressed) isKeyPressed = false;
        }
    }
}