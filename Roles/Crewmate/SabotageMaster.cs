using System.Collections.Generic;

namespace TOHEXI.Roles.Crewmate;

public static class SabotageMaster
{
    private static readonly int Id = 20300;
    public static List<byte> playerIdList = new();

    public static OptionItem SkillLimit;
    public static OptionItem FixesDoors;
    public static OptionItem FixesReactors;
    public static OptionItem FixesOxygens;
    public static OptionItem FixesComms;
    public static OptionItem FixesElectrical;
    public static int UsedSkillCount;

    private static bool DoorsProgressing = false;

    public static void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.SabotageMaster);
        SkillLimit = IntegerOptionItem.Create(Id + 10, "SabotageMasterSkillLimit", new(0, 99, 1), 10, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster])
            .SetValueFormat(OptionFormat.Times);
        FixesDoors = BooleanOptionItem.Create(Id + 11, "SabotageMasterFixesDoors", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesReactors = BooleanOptionItem.Create(Id + 12, "SabotageMasterFixesReactors", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesOxygens = BooleanOptionItem.Create(Id + 13, "SabotageMasterFixesOxygens", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesComms = BooleanOptionItem.Create(Id + 14, "SabotageMasterFixesCommunications", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesElectrical = BooleanOptionItem.Create(Id + 15, "SabotageMasterFixesElectrical", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
    }
    public static void Init()
    {
        playerIdList = new();
        UsedSkillCount = 0;
    }
    public static void Add(byte playerId)
    {
        playerIdList.Add(playerId);
    }
    public static bool IsEnable() => playerIdList.Count > 0;
    public static void UpdateSystem(ShipStatus __instance, SystemTypes systemType, byte amount)
    {
        switch (systemType)
        {
            case SystemTypes.Reactor:
                if (!FixesReactors.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 17);
                    UsedSkillCount++;
                }
                break;
            case SystemTypes.Laboratory:
                if (!FixesReactors.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 67);
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 66);
                    UsedSkillCount++;
                }
                break;
            case SystemTypes.LifeSupp:
                if (!FixesOxygens.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 67);
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 66);
                    UsedSkillCount++;
                }
                break;
            case SystemTypes.Comms:
                if (!FixesComms.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16);
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 17);
                    UsedSkillCount++;
                }
                break;
            case SystemTypes.Doors:
                if (!FixesDoors.GetBool()) break;
                if (DoorsProgressing == true) break;

                int mapId = Main.NormalOptions.MapId;
                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) mapId = AmongUsClient.Instance.TutorialMapId;

                DoorsProgressing = true;
                if (mapId == 2)
                {
                    //Polus
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 71, 72);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 67, 68);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 64, 66);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 73, 74);
                }
                else if (mapId == 4)
                {
                    //Airship
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 64, 67);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 71, 73);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 74, 75);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 76, 78);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 68, 70);
                    UpdateSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 83, 84);
                }
                DoorsProgressing = false;
                break;
        }
    }
    public static void SwitchSystemRepair(SwitchSystem __instance, byte amount)
    {
        if (!FixesElectrical.GetBool()) return;
        if (SkillLimit.GetFloat() > 0 &&
            UsedSkillCount >= SkillLimit.GetFloat())
            return;

        if (amount is >= 0 and <= 4)
        {
            __instance.ActualSwitches = 0;
            __instance.ExpectedSwitches = 0;
            UsedSkillCount++;
        }
    }
}