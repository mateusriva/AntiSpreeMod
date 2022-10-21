using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiSpreeMod
{
    public class AntiSpreeManager
    {
        /// <summary>
        /// A Dictionary containing all levels keyed to TIFactionState
        /// </summary>
        public Dictionary<GameStateID, AntiSpreeState> AntiSpreeStateList = new Dictionary<GameStateID, AntiSpreeState>();

        /// <summary>
        /// Returns the lastAssassination associated with the TIFactionState. Will add the key if it is missing w/ 0 days.
        /// </summary>
        public AntiSpreeState this[TIFactionState faction]
        {
            get
            {
                if (faction == null)
                {
                    return null;
                }
                if (!AntiSpreeStateList.ContainsKey(faction.ID))
                {
                    this.RegisterList(faction);
                }
                if (AntiSpreeStateList.TryGetValue(faction.ID, out var value)) return value;
                return null;
            }
        }

        /// <summary>
        /// Registers a new member in the AntiSpreeStateList keyed by their ID with default date 0
        /// </summary>
        public void RegisterList(TIFactionState faction, AntiSpreeState lastAssassinationDate = null)
        {
            if (!AntiSpreeStateList.ContainsKey(faction.ID))
            {
                bool flag = (lastAssassinationDate == null);
                if (!flag)
                {
                    AntiSpreeStateList.Add(faction.ID, lastAssassinationDate);
                }
                else
                {
                    AntiSpreeState _antiSpreeState = GameStateManager.CreateNewGameState<AntiSpreeState>();
                    _antiSpreeState.InitWithFactionState(faction);
                    AntiSpreeStateList.Add(faction.ID, _antiSpreeState);
                }
            }

        }

        /// <summary>
        /// Deregisters an existing member from the AntiSpreeStateList
        /// </summary>
        public void DeRegisterList(TIFactionState faction)
        {
            if (!AntiSpreeStateList.ContainsKey(faction.ID))
            {
                AntiSpreeState factionLevel = this[faction];
                bool result = GameStateManager.RemoveGameState<AntiSpreeState>(factionLevel.ID, false);
                AntiSpreeStateList.Remove(faction.ID);
            }
        }

        /// <summary>
        /// Sets the value of TIFactionState faction to int value
        /// </summary>
        /// <param name="faction">The instance to update</param>
        /// <param name="value">The value to insert</param>
        //public void UpdateRegisterList(TIFactionState faction, int value)
        //{
        //    if (AntiSpreeStateList.ContainsKey(faction.ID))
        //    {
        //        this[faction].LastAssassinationDate = value;
        //    }
        //}

        /// <summary>
        /// Clears the AntiSpreeStateList Dictionary
        /// </summary>
        public void ClearAntiSpreeStateList()
        {
            AntiSpreeStateList.Clear();
        }

        /// <summary>
        /// Set last murder to today
        /// </summary>
        /// <param name="faction"></param>
        public void UpdateFactionLastAssassinationDate(TIFactionState faction)
        {
            if (AntiSpreeStateList.ContainsKey(faction.ID))
            {
                this[faction].SetAssassinationToNow();
            }
            else
            {
                RegisterList(faction);
                this[faction].SetAssassinationToNow();
            }
        }

        /// <summary>
        /// Set last takeover to today
        /// </summary>
        /// <param name="faction"></param>
        public void UpdateFactionLastTakeoverDate(TIFactionState faction)
        {
            if (AntiSpreeStateList.ContainsKey(faction.ID))
            {
                this[faction].SetTakeoverToNow();
            }
            else
            {
                RegisterList(faction);
                this[faction].SetTakeoverToNow();
            }
        }


        /// <summary>
        /// Set last Sabotage to today
        /// </summary>
        /// <param name="faction"></param>
        public void UpdateFactionLastSabotageDate(TIFactionState faction)
        {
            if (AntiSpreeStateList.ContainsKey(faction.ID))
            {
                this[faction].SetSabotageToNow();
            }
            else
            {
                RegisterList(faction);
                this[faction].SetSabotageToNow();
            }
        }
    }
}
