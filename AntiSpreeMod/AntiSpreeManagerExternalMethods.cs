using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiSpreeMod
{
    public static class AntiSpreeManagerExternalMethods
    {
        /// <summary>
        /// ExternalMethods manages the content with an instance of AntiSpreeManager
        /// </summary>
        public static AntiSpreeManager Manager = new AntiSpreeManager();

        /// <summary>
        /// Returns the last murder in a faction by its instance
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        /// <returns></returns>
        public static int? GetFactionLastAssassinationDate(TIFactionState faction)
        {
            return Manager[faction].LastAssassinationDate;
        }

        /// <summary>
        /// Returns the last takeover of a faction by its instance
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        /// <returns></returns>
        public static int? GetFactionLastTakeoverDate(TIFactionState faction)
        {
            return Manager[faction].LastTakeoverDate;
        }

        /// <summary>
        /// Returns the last Sabotage of a faction by its instance
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        /// <returns></returns>
        public static int? GetFactionLastSabotageDate(TIFactionState faction)
        {
            return Manager[faction].LastSabotageDate;
        }

        /// <summary>
        /// Removes a faction from the register
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void RemoveFactionRegister(TIFactionState faction)
        {
            Manager.DeRegisterList(faction);
        }

        /// <summary>
        /// Increments or adds a faction to the register
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void AddFactionRegister(TIFactionState faction, AntiSpreeState currentSpreeState = null)
        {
            Manager.RegisterList(faction, currentSpreeState);
        }

        /// <summary>
        /// Updates last murder
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void UpdateFactionLastAssassinationDate(TIFactionState faction)
        {
            Manager.UpdateFactionLastAssassinationDate(faction);
        }

        /// <summary>
        /// Updates last takeover
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void UpdateFactionLastTakeoverDate(TIFactionState faction)
        {
            Manager.UpdateFactionLastTakeoverDate(faction);
        }


        /// <summary>
        /// Updates last Sabotage
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void UpdateFactionLastSabotageDate(TIFactionState faction)
        {
            Manager.UpdateFactionLastSabotageDate(faction);
        }
    }
}
