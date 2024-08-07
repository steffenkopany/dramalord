﻿using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Behaviours
{
    internal class PlayerCampaignBehavior : CampaignBehaviorBase
    {
        internal PlayerCampaignBehavior(CampaignGameStarter starter)
        {
            PlayerApproachingNPC.AddDialogs(starter);
            PlayerConfrontNPC.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        internal void OnHourlyTick()
        {
            HeroIntention intention = Hero.MainHero.GetIntentions().FirstOrDefault(intention => intention.Type == IntentionType.Confrontation);
            {
                if(intention != null && intention.Target.IsCloseTo(Hero.MainHero))
                {
                    HeroEvent? @event = DramalordEvents.Instance.GetEvent(intention.EventId);
                    if (@event != null && intention.Target.IsEmotionalWith(Hero.MainHero))
                    {
                        PlayerConfrontNPC.Start(intention.Target, @event); 
                    }

                    DramalordIntentions.Instance.RemoveIntention(Hero.MainHero, intention.Target, intention.Type, intention.EventId);
                    return;
                }
            }
        }
    }
}
