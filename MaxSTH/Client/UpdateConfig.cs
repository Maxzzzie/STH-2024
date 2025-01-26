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
    public class UpdateConfig : BaseScript
    {
        public UpdateConfig()
        {
            TriggerServerEvent("SendConfigToClient");
        }
    }
}