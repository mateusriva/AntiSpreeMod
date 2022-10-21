using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using System.Reflection;
using PavonisInteractive.TerraInvicta;
using AntiSpreeMod;

namespace AntiSpreeMod
{
    public class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;
        public static Settings settings;

        //Boilerplate code, the entry point to the mod
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            //
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            mod = modEntry;
            modEntry.OnToggle = OnToggle;
            return true;
        }

        //Boilerplate code, called when the user toggles the mod on/off in UMM in-game
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        //Boilerplate code, draws the configurable settings in the UMM
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        //Boilerplate code, saves settings changes to the xml file
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }

    //Settings class to interface with Unity Mod Manager
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        //TODO: switch to localization once it is available
        [Draw("Recent Assassination Malus", Collapsible = true)] public int recentAssassinationMalus = 10;
        [Draw("Assassination Malus Decay each 15 days", Collapsible = true)] public int assassinationMalusDecay = 2;
        [Draw("Assassination Malus Caps to Security Maximum", Collapsible = true)] public bool assassinationMalusCap = true;

        [Draw("Recent Takeover Malus", Collapsible = true)] public int recentTakeoverMalus = 8;
        [Draw("Takeover Malus Decay each 15 days", Collapsible = true)] public int takeoverMalusDecay = 2;
        [Draw("Takeover Malus Caps to Security Maximum", Collapsible = true)] public bool takeoverMalusCap = true;
        

        //Boilerplate code to save your settings to a Settings.xml file when changed
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        //Hook to allow to do things when a value is changed, if you want
        public void OnChange()
        {
        }
    }

    [HarmonyPatch(typeof(TIFactionState), nameof(TIFactionState.InitWithTemplate))]
    static class InitWithTemplatePatch
    {

        static void Postfix(TIDataTemplate template, TIFactionState __instance)
        {
            if (Main.enabled)
            {
                TIFactionTemplate tifactiontemplate = template as TIFactionTemplate;
                bool flag = tifactiontemplate == null;
                if (!flag)
                {
                    AntiSpreeManagerExternalMethods.AddFactionRegister(__instance);
                }
            }
        }
    }

    // Harmony patch
    // Stores last assassination for a faction
    [HarmonyPatch(typeof(TIMissionEffect_Assassinate), nameof(TIMissionEffect_Assassinate.ApplyEffect))]
    static class StoreLastAssassinationPhasePatch
    {
        static void Postfix(TIMissionState mission, TIGameState target, TIMissionOutcome outcome, TIMissionEffect_Assassinate __instance)
        {
            if (Main.enabled)
            {
                // If assassination was a success...
                if (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess)
                {
                    // Get current mission phase and store it in the faction state
                    AntiSpreeManagerExternalMethods.UpdateFactionLastAssassinationDate(target.ref_councilor.faction);
                }
            }
        }
    }

    // Harmony patch
    // Stores last takeover for a faction
    [HarmonyPatch(typeof(TIMissionEffect_HostileTakeover), nameof(TIMissionEffect_HostileTakeover.ApplyEffect))]
    static class StoreLastTakeoverPhasePatch
    {
        static void Postfix(TIMissionState mission, TIGameState target, TIMissionOutcome outcome, TIMissionEffect_Assassinate __instance)
        {
            if (Main.enabled)
            {
                // If takeover was a success...
                if (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess)
                {
                    // Get current mission phase and store it in the faction state
                    AntiSpreeManagerExternalMethods.UpdateFactionLastTakeoverDate(target.ref_faction);
                }
            }
        }
    }
}


// TIMissionModifier for the recent assassination malus
public class TIMissionModifier_RecentAssassination : TIMissionModifier
{
    public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0, FactionResource resource = FactionResource.None)
    {
        int current_date = GameStateManager.Time().daysInCampaign;
        int lastAssassinationDate = (int)AntiSpreeManagerExternalMethods.GetFactionLastAssassinationDate(target.ref_councilor.faction);
        float target_security = (float)target.ref_councilor.GetAttribute(CouncilorAttribute.Security);
        int maxCouncilorAttribute = TemplateManager.global.maxCouncilorAttribute;

        // If no assassinations yet, no malus
        if (lastAssassinationDate < 0) { return 0f; }

        // Compute malus
        float malus = Main.settings.recentAssassinationMalus - ((Main.settings.assassinationMalusDecay / 15f) * (current_date - lastAssassinationDate));
        if (malus < 0f) { malus = 0f; }  // Can't be less than zero

        // If capped, cap to security
        if (Main.settings.assassinationMalusCap)
        {
            if (malus + target_security > maxCouncilorAttribute)
            {
                malus = maxCouncilorAttribute - (float)target_security;
            }
        }
        return malus;

    }

    public override string displayName
    {
        get
        {
            return "Recent Assassination";
        }
    }
}


// TIMissionModifier for the recent takeover malus
public class TIMissionModifier_RecentTakeover : TIMissionModifier
{
    public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0, FactionResource resource = FactionResource.None)
    {
        int current_date = GameStateManager.Time().daysInCampaign;
        int lastTakeoverDate = (int)AntiSpreeManagerExternalMethods.GetFactionLastTakeoverDate(target.ref_faction);
        float target_administration = (float)target.ref_councilor.GetAttribute(CouncilorAttribute.Administration);
        int maxCouncilorAttribute = TemplateManager.global.maxCouncilorAttribute;

        // If no takeovers yet, no malus
        if (lastTakeoverDate < 0) { return 0f; }

        // Compute malus
        float malus = Main.settings.recentTakeoverMalus - ((Main.settings.takeoverMalusDecay/ 15f) * (current_date - lastTakeoverDate));
        if (malus < 0f) { malus = 0f; }  // Can't be less than zero

        // If capped, cap to security
        if (Main.settings.takeoverMalusCap)
        {
            if (malus + target_administration > maxCouncilorAttribute)
            {
                malus = maxCouncilorAttribute - (float)target_administration;
            }
        }
        return malus;

    }

    public override string displayName
    {
        get
        {
            return "Recent Takeover";
        }
    }
}