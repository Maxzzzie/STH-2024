using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;

namespace STHMaxzzzie.Client
{
public class NotificationScript : BaseScript
{
    static string MOTD = "No MotD has been set yet. I hope you have a good day though.";
    
    
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

    [EventHandler("ShowErrorNotification")]
    public static void ShowErrorNotification(string text)
    {
        SetNotificationTextEntry("STRING");
        AddTextComponentSubstringPlayerName("~r~Error~s~ " + text);
        DrawNotification(false, false);
        PlaySoundFrontend(-1, "Highlight_Error", "HUD_AMMO_SHOP_SOUNDSET", false);
        
        // Call the method to strip formatting tags
        string DebugString = StripFormattingTags(text);

        // Print the changed string
        Debug.WriteLine(DebugString);
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"{DebugString}"}});
    }

    [EventHandler("ShowSpecialNotification")]
        //usage server side = TriggerClientEvent(Player, "ShowSpecialNotification", "Text" , "audioName" , "audioRef" );
        //usage client side = NotificationScript.ShowSpecialNotification("text", "audioName", "audioRef");
    public static void ShowSpecialNotification(string text, string audioName, string audioRef)
    {
        SetNotificationTextEntry("STRING");
        AddTextComponentSubstringPlayerName(text);
        DrawNotification(false, false);
        PlaySoundFrontend(-1, audioName, audioRef, false);
        
        // Call the method to strip formatting tags
        string DebugString = StripFormattingTags(text);

        // Print the changed string
        Debug.WriteLine(DebugString);
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"{DebugString}"}});
    }

[EventHandler("ShowMotD")]
    public static void ShowMOTD()
    {
        ShowNotification($"~o~{MOTD}");
    }

[EventHandler("updateMessageOfTheDay")]
    public static void updateMessageOfTheDay(string newMoTd)
    {
        MOTD = newMoTd;
    }

    [EventHandler("displayClientDebugLine")]
    public static void displayClientDebugLine(string debugString)
    {
        Debug.WriteLine(debugString);
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