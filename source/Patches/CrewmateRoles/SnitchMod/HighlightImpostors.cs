using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var state in __instance.playerStates)
            {
                var role = Role.GetRole(state);
                if (role.Faction == Faction.Impostors || role.RoleType == RoleEnum.Impostor)
                    state.NameText.color = Palette.ImpostorRed;
                if (role.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                    state.NameText.color = role.Color;
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsImpostor()) player.nameText.color = Palette.ImpostorRed;
                var playerRole = Role.GetRole(player);
                if (playerRole.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                    player.nameText.color = role.Color;
            }
        }
    }
}