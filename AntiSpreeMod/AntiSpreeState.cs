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
        public int LastAssassinationDate { get; set; } = -1;
        [SerializeField]
        public int LastTakeoverDate { get; set; } = -1;
        [SerializeField]
        public int LastSabotageDate { get; set; } = -1;


        public void SetAssassinationToNow()
        {
            LastAssassinationDate = GameStateManager.Time().daysInCampaign;
        }
        public void SetTakeoverToNow()
        {
            LastTakeoverDate = GameStateManager.Time().daysInCampaign;
        }
        public void SetSabotageToNow()
        {
            LastSabotageDate = GameStateManager.Time().daysInCampaign;
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
                this.LastTakeoverDate = LastTakeoverDate;
                this.LastSabotageDate = LastSabotageDate;
            }
        }
    }
}
