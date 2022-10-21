using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using System.Reflection;
using PavonisInteractive.TerraInvicta;

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
        [Draw("Recent Assassination Malus", Collapsible = true)] public int recentAssassinationMalus = 8;
        [Draw("Assassination Malus Decay/Phase", Collapsible = true)] public int assassinationMalusDecay = 2;
        [Draw("Assassination Malus + Security Caps to 25", Collapsible = true)] public bool assassinationMalusCap = true;

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
                FileLog.Log("Got to the postfix");
                // If assassination was a success...
                if (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess)
                {
                    FileLog.Log("Entered Success Condition");
                    FileLog.Log("Target is " + target.ToString());
                    FileLog.Log("Target ref_councilor is " + target.ref_councilor.ToString());
                    FileLog.Log("Target ref_councilor faction is " + target.ref_councilor.faction.ToString());
                    // Get current mission phase and store it in the faction state
                    AntiSpreeManagerExternalMethods.UpdateFactionLastAssassinationDate(target.ref_councilor.faction);
                    FileLog.Log("Stored Assassination of faction " + target.ref_councilor.faction.ToString());
                    FileLog.Log("Stored at date " + AntiSpreeManagerExternalMethods.GetFactionLastAssassinationDate(target.ref_councilor.faction));
                }
            }
        }
    }
}