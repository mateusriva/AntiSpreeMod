using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Entities;
using UnityEngine;

namespace AntiSpreeMod
{
    public class AntiSpreeState : TIGameState
    {
        new public TIFactionState ref_faction;

        [SerializeField]
        public int LastAssassinationDate { get; set; } = 0;

        public void SetToNow()
        {
            FileLog.Log("Entered SetToNow");
            LastAssassinationDate = GameStateManager.Time().daysInCampaign;
            FileLog.Log("Set LastAssassinationDate to" + LastAssassinationDate.ToString());
        }

        public AntiSpreeState ref_antiSpreeState
        {
            get
            {
                return this;
            }
        }

        public void InitWithFactionState(TIFactionState faction)
        {
            bool flag = faction.template == null;
            if (!flag)
            {
                this.ref_faction = faction;
                this.templateName = faction.template.dataName;
                this.LastAssassinationDate = LastAssassinationDate;
            }
        }
    }
}
