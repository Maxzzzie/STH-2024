using CitizenFX.Core;
using System.Collections.Generic;
using System;

public class ShotBlipServer : BaseScript
{
    bool areShotsFiredVisible = true;

[EventHandler("OnShotsFired")]
    private void OnShotsFired(int X, int Y)
    {
        //Debug.WriteLine($"OnShotsFired 1");
        if (areShotsFiredVisible)
        {
            //Debug.WriteLine($"onShotsFired {X},{Y}");
            TriggerClientEvent("CreateShotBlip", X,Y); 
        }
    }

    [Command("togglesf", Restricted = true)]
    void togglesf(int source, List<object> args, string raw)
    {
        if (args.Count == 0)
        {
            areShotsFiredVisible = !areShotsFiredVisible;
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shots fired are now: {areShotsFiredVisible}" } });
        }
        else if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
        {
            areShotsFiredVisible = bool.Parse(args[0].ToString());
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shots fired are now: {areShotsFiredVisible}" } });
        }
        else
        {
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"toggleShotsFired using /togglesf (true/false)" } });
        }
    }
}
