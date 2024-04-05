using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroMarriageAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) && hero.Spouse != target)
            {
                if(target == Hero.MainHero)
                {
                    TextObject title = new TextObject("{=Dramalord134}Marriage request");
                    TextObject text = new TextObject("{=Dramalord135}{HERO.LINK} says they love you and asks you if you consider marrying them.");
                    TextObject banner = new TextObject("{=Dramalord136}You broke {HERO.LINK}s heart!");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);

                    Notification.DrawMessageBox(
                            title,
                            text,
                            true,
                            () => {

                                DoMarry(target, hero);
                            },
                            () => { 

                                Info.ChangeEmotionToHeroBy(hero, target, -DramalordMCM.Get.EmotionalLossBreakup);
                                MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation"); 
                            }
                        );
                }
                else
                {
                    DoMarry(hero, target);
                }
            }
        }

        private static void DoMarry(Hero hero, Hero target)
        {
            if (hero.Spouse != null)
            {
                HeroDivorceAction.Apply(hero, hero.Spouse);
            }

            if (target.Spouse != null)
            {
                HeroDivorceAction.Apply(target, target.Spouse);
            }

            if(hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero Npc = (hero == Hero.MainHero) ? target : hero;
                HeroLeaveClanAction.Apply(Npc, false, Npc);
                HeroJoinClanAction.Apply(Npc, Clan.PlayerClan, false);
            }
            else if(hero.Clan != null && target.Clan == null)
            {
                HeroJoinClanAction.Apply(target, hero.Clan, false);
            }
            else if (hero.Clan == null && target.Clan != null)
            {
                HeroJoinClanAction.Apply(hero, target.Clan, false);
            }
            else if (hero.Clan != null && target.Clan != null && hero.Clan != target.Clan)
            {
                HeroLeaveClanAction.Apply(hero, false, hero);
                HeroJoinClanAction.Apply(hero, target.Clan, false);
            }
                
            MarriageAction.Apply(hero, target, false);

            Info.SetIsCoupleWithHero(hero, target, true);
            Info.ChangeEmotionToHeroBy(hero, target, DramalordMCM.Get.EmotionalWinMarriage);
            Info.ChangeEmotionToHeroBy(target, hero, DramalordMCM.Get.EmotionalWinMarriage);

            //alread logged by system
            //LogEntry.AddLogEntry(new EncyclopediaLogMarriage(hero, target));
            DramalordEvents.OnHeroesMarried(hero, target);
        }
    }
}
