using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;

namespace EvoMp.Module.MapHandler.Server
{
    public class MapHandler
    {
        private readonly API _api;
        public MapHandler(API api)
        {
            _api = api;
        }
    }
}