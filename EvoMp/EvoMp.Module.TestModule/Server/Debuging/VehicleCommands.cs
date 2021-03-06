using System;
using System.Collections.Generic;
using System.Linq;
using EvoMp.Module.CommandHandler.Server;
using EvoMp.Module.CommandHandler.Server.Attributes;
using EvoMp.Module.VehicleHandler.Server;
using EvoMp.Module.VehicleHandler.Server.Enums;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Gta.Vehicle;

namespace EvoMp.Module.TestModule.Server.Debuging
{
    public class VehicleCommands
    {
        private readonly API _api;
        private readonly IVehicleHandler _vehicleHandler;

        public VehicleCommands(API api, IVehicleHandler vehicleHandler)
        {
            _api = api;
            _vehicleHandler = vehicleHandler;
        }

        [PlayerCommand("/v")]
        public void TestVehicleCommand(Client sender, string vehicleName)
        {
            List<VehicleHash> possibleVehicles = _vehicleHandler.Utils.GetVehiclesByName(vehicleName);

            if (!possibleVehicles.Any()) return;          

            // Create new vehicle
            Vehicle newVehicle = _api.createVehicle(possibleVehicles.First(), sender.position,
                sender.rotation, 1, 1,
                sender.dimension);

            //newVehicle.waitForSynchronization();

            sender.setIntoVehicle(newVehicle, -1);

            _api.sendChatMessageToPlayer(sender,
                $"Vehicle ~o~{possibleVehicles.First()}~w~ ~c~(~w~{(VehicleClass) API.shared.getVehicleClass(possibleVehicles.First())}~c~) ~w~created.");

            // adjust alternatives
            possibleVehicles.RemoveAt(0);
            if (possibleVehicles.Count > 0)
            {
                _api.sendNotificationToPlayer(sender, $"~w~Alternative Vehicles: ~g~{string.Join(",", possibleVehicles)}");
            }
        }

        [PlayerCommand("/togglealldoors", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetAllVehicleDoors(Client sender, bool state)
        {
            // Change door state
            foreach (DoorState doorState in Enum.GetValues(typeof(DoorState)))
                API.shared.setVehicleDoorState(sender.vehicle, (int) doorState, state);
        }

        [PlayerCommand("/setvmod", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetVehicleMod(Client sender, VehicleModType slot, int value)
        {
            API.shared.setVehicleMod(sender.vehicle, (int) slot, value);
        }

        [PlayerCommand("/rvmod", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetRandomVehicleMod(Client sender)
        {
            Random random = new Random();
            foreach (VehicleModType modification in Enum.GetValues(typeof(VehicleModType)))
                API.shared.setVehicleMod(sender.vehicle, (int) modification, random.Next(0, 5));
        }

        [PlayerCommand("/rvcolor", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetRandomVehicleColors(Client sender)
        {
            Random random = new Random();
            API.shared.setVehicleCustomPrimaryColor(sender.vehicle, random.Next(0, 255), random.Next(0, 255),
                random.Next(0, 255));
            API.shared.setVehicleCustomSecondaryColor(sender.vehicle, random.Next(0, 255), random.Next(0, 255),
                random.Next(0, 255));
        }

        [PlayerCommand("/vsethealth", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetVehicleHealth(Client sender, double health)
        {
            sender.vehicle.health = (float) health;

            _api.sendChatMessageToPlayer(sender, $"New vehicle health: ~o~{sender.vehicle.health}");
        }

        [PlayerCommand("/vgethealth", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void GetVehicleHealth(Client sender)
        {
            _api.sendChatMessageToPlayer(sender, $"Vehicle health: ~o~{sender.vehicle.health}");
        }

        [PlayerCommand("/vtyresmokecolor", new[] {"/vtsc"}, PlayerOnlyState.OnlyAsDriver)]
        public void SetVehicleTyreSmokeColor(Client sender, int red, int green, int blue)
        {
            //sender.vehicle.tyreSmokeColor = new Color(red, green, blue);
            API.shared.setVehicleTyreSmokeColor(sender.vehicle, red, green, blue);
            _api.sendChatMessageToPlayer(sender,
                $"Changed vehicle tyre smoke color to ~r~{red} ~g~{green} ~b~{blue}");
        }

        [PlayerCommand("/vpaintjob", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetVehicleLivery(Client sender, int value)
        {
            API.shared.setVehicleLivery(sender.vehicle, value);
            _api.sendChatMessageToPlayer(sender, $"Your ~o~{sender.vehicle.displayName}~w~ got a new paintjob.");
        }

        [PlayerCommand("/rvpaintjob", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void RemoveVehicleLivery(Client sender, int debugvalue)
        {
            API.shared.setVehicleLivery(sender.vehicle, debugvalue);
            _api.sendChatMessageToPlayer(sender, $"Your removed the paintjob from your {sender.vehicle.displayName}");
        }

        [PlayerCommand("/glv", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void GetAllLiveries(Client sender)
        {
            foreach (KeyValuePair<int, string> livery in API.shared.getVehicleLiveries(
                (VehicleHash) API.shared.getEntityModel(sender.vehicle)))
                _api.sendChatMessageToPlayer(sender, $"{livery.Key} | {livery.Value}");
        }

        [PlayerCommand("/vpop", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void PopVehicleTyres(Client sender)
        {
            if(!API.shared.getVehicleBulletproofTyres(sender.vehicle))
            {
                for (int i = 0; i < 5; i++)
                {
                    _api.popVehicleTyre(sender.vehicle, i, true);
                }
            }
            _api.sendChatMessageToPlayer(sender, $"All vehicle tryes are popped.");
        }

        [PlayerCommand("/vdirt", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetDirtLevel(Client sender, float level) 
        {
            if (level < 0)
            {
                _api.setVehicleDirtLevel(sender.vehicle, 0);
                _api.sendChatMessageToPlayer(sender, "now your car is clean.");
            } else if (level > 15)
            {
                _api.setVehicleDirtLevel(sender.vehicle, 15);
                _api.sendChatMessageToPlayer(sender, "now your car is very dirty.");
            } else
            {
                _api.setVehicleDirtLevel(sender.vehicle, level);
                _api.sendChatMessageToPlayer(sender, $"now your car is dirty stage {level}.");
            }
        }

        [PlayerCommand("/voil", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetOilLevel(Client sender, float level)
        {
            if (level < 2.49f)
            {
                _api.setVehicleOilLevel(sender.vehicle, 2.49f);
                _api.sendChatMessageToPlayer(sender, "your car is out of oil.");
            } else if (level > 2.50f)
            {
                _api.setVehicleOilLevel(sender.vehicle, 2.50f);
                _api.sendChatMessageToPlayer(sender, "your oil level is full");
            } else
            {
                _api.setVehicleOilLevel(sender.vehicle, level);
                _api.sendChatMessageToPlayer(sender, $"your oil level is {level}");
            }
        }

        [PlayerCommand("/vfuel", playerOnlyState: PlayerOnlyState.OnlyAsDriver)]
        public void SetFuelLevel(Client sender, float level)
        {
            if (level < 0)
            {
                _api.setVehicleFuelLevel(sender.vehicle, 0);
                _api.sendChatMessageToPlayer(sender, "your car is out of gas.");
            }
            else if (level > 100)
            {
                _api.setVehicleFuelLevel(sender.vehicle, 100);
                _api.sendChatMessageToPlayer(sender, "your oil level is full");
            }
            else
            {
                _api.setVehicleFuelLevel(sender.vehicle, level);
                _api.sendChatMessageToPlayer(sender, $"your oil level is {level}");
            }
        }
    }
}
