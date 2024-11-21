using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;

namespace STHMaxzzzie.Client
{
public class NotificationScript : BaseScript
{
    
[EventHandler("ShowNotification")]
    public static void ShowNotification(string text)
    {
        SetNotificationTextEntry("STRING");
        AddTextComponentSubstringPlayerName(text);
        DrawNotification(false, false);
        PlaySoundFrontend(-1, "ATM_WINDOW", "HUD_FRONTEND_DEFAULT_SOUNDSET", false); //make little sound when it pops up.
        // Call the method to strip formatting tags
        string DebugString = StripFormattingTags(text);

        // Print the changed string
        Debug.WriteLine(DebugString);
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"{DebugString}"}});
    }

    public static string StripFormattingTags(string input)
    {
        // Remove sequences enclosed in ~...~
        while (input.Contains("~"))
        {
            int start = input.IndexOf("~");
            int end = input.IndexOf("~", start + 1);
            if (end > start)
                input = input.Remove(start, end - start + 1);
            else
                break;
        }

        // Remove sequences enclosed in <...>
        while (input.Contains("<"))
        {
            int start = input.IndexOf("<");
            int end = input.IndexOf(">", start);
            if (end > start)
                input = input.Remove(start, end - start + 1);
            else
                break;
        }

        return input;
    }
}
}