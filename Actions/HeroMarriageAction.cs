using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroMarriageAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if(Info.ValidateHeroMemory(hero, target))
            {
                if (target == Hero.MainHero)
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
            if(hero.Occupation == Occupation.Wanderer)
            {
                hero.SetName(hero.FirstName, hero.FirstName);
            }

            if (target.Occupation == Occupation.Wanderer)
            {
                target.SetName(TextObject.Empty, target.FirstName);
            }

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero Npc = (hero == Hero.MainHero) ? target : hero;
                HeroLeaveClanAction.Apply(Npc, Npc);
                HeroJoinClanAction.Apply(Npc, Clan.PlayerClan);
            }
            else if(hero.Clan != null && target.Clan == null)
            {
                HeroJoinClanAction.Apply(target, hero.Clan);
            }
            else if (hero.Clan == null && target.Clan != null)
            {
                HeroJoinClanAction.Apply(hero, target.Clan);
            }
            else if (hero.Clan != null && target.Clan != null && hero.Clan != target.Clan)
            {
                HeroLeaveClanAction.Apply(hero, hero);
                HeroJoinClanAction.Apply(hero, target.Clan);
            }
                
            MarriageAction.Apply(hero, target, false);

            foreach (Hero child in hero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    if (child.Occupation == Occupation.Wanderer)
                    {
                        child.SetName(child.FirstName, child.FirstName);
                    }
                    child.Clan = target.Clan;
                    child.UpdateHomeSettlement();
                    child.SetNewOccupation(Occupation.Lord);
                    child.ChangeState(Hero.CharacterStates.Active);
                }
            }

            Info.SetIsCoupleWithHero(hero, target, true);

            DramalordEvents.OnHeroesMarried(hero, target);
        }
    }
}
