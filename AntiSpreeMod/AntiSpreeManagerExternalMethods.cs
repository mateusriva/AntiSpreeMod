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
        /// Returns the level of a faction by its instance
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        /// <returns></returns>
        public static int? GetFactionLastAssassinationDate(TIFactionState faction)
        {
            return Manager[faction].LastAssassinationDate;
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
        public static void AddFactionRegister(TIFactionState faction, AntiSpreeState lastAssassinationDate = null)
        {
            Manager.RegisterList(faction, lastAssassinationDate);
        }

        /// <summary>
        /// Updates last murder
        /// </summary>
        /// <param name="faction">The TIFactionState instance</param>
        public static void UpdateFactionLastAssassinationDate(TIFactionState faction)
        {
            Manager.UpdateFactionLastAssassinationDate(faction);
        }
    }
}
