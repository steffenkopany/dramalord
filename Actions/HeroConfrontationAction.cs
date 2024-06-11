using Dramalord.Conversations;
using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroConfrontationAction
    {
        internal static void Apply(Hero hero, Hero target, DramalordTraits heroTraits, HeroMemory memory)
        {
            Hero otherHero = (memory.Event.Hero1.HeroObject != target) ? memory.Event.Hero1.HeroObject : memory.Event.Hero2.HeroObject;

            if (target == Hero.MainHero && otherHero != hero)
            {
                // initiate confrontation
                NPCConfrontation.start(hero, memory);

                if (DramalordMCM.Get.ConfrontationOutput)
                {
                    LogEntry.AddLogEntry(new LogConfrontation(hero, target, otherHero, memory.Event));
                }
            }
            else if (otherHero != hero)
            {
                if (DramalordMCM.Get.ConfrontationOutput)
                {
                    LogEntry.AddLogEntry(new LogConfrontation(hero, target, otherHero, memory.Event));
                }

                if(memory.Event.Type == EventType.Birth && memory.Event.Hero2.HeroObject.Father != hero)
                {
                    HeroPutInOrphanageAction.Apply(hero, memory.Event.Hero2.HeroObject);
                }

                if (target.IsSpouse(hero) && hero.GetDramalordFeelings(target).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                {
                    HeroDivorceAction.Apply(hero, target);
                    if(heroTraits.IsInstable && DramalordMCM.Get.AllowRageKills)
                    {
                        HeroKillAction.Apply(hero, target, otherHero, memory.Event.Type);
                    }
                    else if(heroTraits.IsHotTempered && hero.Clan != null && hero.Clan == target.Clan && DramalordMCM.Get.AllowClanChanges)
                    {
                        if(hero.Clan.Leader == hero)
                        {
                            HeroLeaveClanAction.Apply(target, hero);
                        }
                        else
                        {
                            HeroLeaveClanAction.Apply(hero, hero);
                        }
                    }
                }
                else if (target.IsLover(hero) && hero.GetDramalordFeelings(target).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                {
                    HeroBreakupAction.Apply(hero, target);
                    if (heroTraits.IsInstable && DramalordMCM.Get.AllowRageKills)
                    {
                        HeroKillAction.Apply(hero, target, otherHero, memory.Event.Type);
                    }
                    else if (heroTraits.IsHotTempered && hero.Clan != null && hero.Clan == target.Clan && DramalordMCM.Get.AllowClanChanges)
                    {
                        if (hero.Clan.Leader == hero)
                        {
                            HeroLeaveClanAction.Apply(target, hero);
                        }
                        else
                        {
                            HeroLeaveClanAction.Apply(hero, hero);
                        }
                    }
                }
            }

            DramalordEventCallbacks.OnHeroesConfrontation(hero, target, otherHero, memory.Event);
        }
    }
}
