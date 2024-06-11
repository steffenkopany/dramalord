using Dramalord.Conversations;
using Dramalord.Data;
using System;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Behaviors
{
    internal class PlayerCampaignBehavior : CampaignBehaviorBase
    {
        public PlayerCampaignBehavior(CampaignGameStarter starter)
        {

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
            if(DramalordMCM.Get.InteractOnBeingWitness)
            {
                Hero.MainHero.GetDramalordMemory().ForEach(item =>
                {
                    if (item.Type == MemoryType.Witness && (item.Event.Type == EventType.Date || item.Event.Type == EventType.Intercourse || item.Event.Type == EventType.Marriage || item.Event.Type == EventType.Birth))
                    {
                        if (item.Event.Hero1.HeroObject.IsNearby(Hero.MainHero) && (item.Event.Hero1.HeroObject.IsLover(Hero.MainHero) || item.Event.Hero1.HeroObject.IsSpouse(Hero.MainHero)))
                        {
                            ConversationHelper.PlayerConfrontationPopup(item.Event.Hero1.HeroObject, item, item.Event.Hero2.HeroObject);
                        }

                        if (item.Event.Hero2.HeroObject.IsNearby(Hero.MainHero) && (item.Event.Hero2.HeroObject.IsLover(Hero.MainHero) || item.Event.Hero2.HeroObject.IsSpouse(Hero.MainHero)))
                        {
                            ConversationHelper.PlayerConfrontationPopup(item.Event.Hero2.HeroObject, item, item.Event.Hero1.HeroObject);
                        }
                        item.Active = false;
                    }
                });
            }
        }
    }
}
