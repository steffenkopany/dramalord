﻿using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal enum ConversationType
    {
        PlayerInteraction,
        NPCInteraction,
        PlayerConfrontation,
        NPCConfrontation,
        OrphanConversation
    }

    internal static class ConversationHelper
    {
        internal static CharacterObject? ConversationCharacter = null;
        internal static ConversationType ConversationIntention = ConversationType.PlayerInteraction;

        internal static bool SetConversationCharacter()
        {
            if(Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsDramalordLegit())
            {
                ConversationCharacter = Hero.OneToOneConversationHero.CharacterObject;
            }
            
            return false;
        }

        internal static TextObject GetHeroGreeting(Hero hero, Hero target, bool capital)
        {
            string text;
            if (hero.IsSpouse(target))
            {
                text = target.IsFemale ? new TextObject("{=8eHRth3U}my wife").ToString() : new TextObject("{=QuVgluRH}my husband").ToString();
            }
            else if (hero.IsLover(target))
            {
                text = target.IsFemale ? new TextObject("{=Dramalord097}my love").ToString() : new TextObject("{=Dramalord096}my lover").ToString();
            }
            else
            {
                text = target.IsFemale ? GameTexts.FindText("str_my_lady").ToString() : GameTexts.FindText("str_my_lord").ToString();
            }

            if(capital)
            {
                char[] array = text.ToCharArray();
                text = array[0].ToString().ToUpper();
                for (int i = 1; i < array.Length; i++)
                {
                    text += array[i];
                }
            }

            return new TextObject(text);
        }

        internal static void PlayerConfrontationPopup(Hero cheater, HeroMemory memory, Hero otherHero)
        {
            TextObject title = new TextObject();
            TextObject text = new TextObject();
            if (memory.Event.Type == EventType.Pregnancy)
            {
                title = new TextObject("{=Dramalord270}You noticed {HERO.LINK} is pregnant from someone else");
                text = new TextObject("{=Dramalord300}While being close to {HERO.LINK} you notice her belly being unusally big and instantly know that not food was causing this.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, text);
            }
            else if(memory.Event.Type == EventType.Date)
            {
                title = new TextObject("{=Dramalord266}You saw {HERO.LINK} meeting in secret with {TARGET.LINK}");
                text = new TextObject("{=Dramalord303}When looking for {HERO.LINK} you found them together with {TARGET.LINK}. They were too close for conversing harmlessly.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, title);
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, text);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, text);
            }
            else if (memory.Event.Type == EventType.Intercourse)
            {
                title = new TextObject("{=Dramalord268}You caught {HERO.LINK} being intimate with {TARGET.LINK}");
                text = new TextObject("{=Dramalord301}When you were going for a stroll you noticed a suspicious sound. When following it you caught {HERO.LINK} and {TARGET.LINK} in an unambiguous situation. {TARGET.LINK} fled the scene.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, title);
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, text);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, text);
            }
            else if (memory.Event.Type == EventType.Birth)
            {
                title = new TextObject("{=Dramalord272}You noticed that {TARGET.LINK} born by {HERO.LINK} is not your child");
                text = new TextObject("{=Dramalord302}The date of birth does not fit and you find no similarities to yourself in the childs face.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, title);
            }
            else if (memory.Event.Type == EventType.Pregnancy)
            {
                title = new TextObject("{=Dramalord270}You noticed {HERO.LINK} is pregnant from someone else");
                text = new TextObject("{=Dramalord392}There is clearly a bulge on {HERO.LINK}'s belly, and after doing some calculations it becomes clear to you that you are not the cause.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, text);
            }
            else if (memory.Event.Type == EventType.Marriage)
            {
                title = new TextObject("{=Dramalord393}You saw {HERO.LINK} marry {TARGET.LINK}");
                text = new TextObject("{=Dramalord395}You heard cheering next to the church and when you got close you saw to your surprise that {HERO.LINK} and {TARGET.LINK} just got married.");
                StringHelpers.SetCharacterProperties("HERO", cheater.CharacterObject, title);
                StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, text);
            }

            Campaign.Current.SetTimeSpeed(0);
            Notification.DrawMessageBox(
                    title,
                    text,
                    false,
                    () => {
                        
                        PlayerConfrontation.start(cheater, memory, otherHero);
                    },
                    () => {

                    }
                );

        }

        internal static Action<Hero>? PostConversationAction;

        internal static void OnConversationEnded(IEnumerable<CharacterObject> characters)
        {
            if(ConversationCharacter != null)
            {
                PostConversationAction?.Invoke(ConversationCharacter.HeroObject);
            }
            
            PostConversationAction = null;
            ConversationCharacter = null;
            /*
            Hero? npc = null;
            foreach (CharacterObject character in characters)
            {
                if (character.HeroObject != Hero.MainHero)
                {
                    npc = character.HeroObject;
                    PostConversationAction?.Invoke(npc);
                    PostConversationAction = null;
                    return;
                }
            }
            */
        }

        internal static void PlayerFlirtAction(Hero npc)
        {
            HeroFlirtAction.Apply(Hero.MainHero, npc, Hero.MainHero.GetCloseHeroes());
        }

        internal static void PlayerDateAction(Hero npc)
        {
            if (!Hero.MainHero.IsLover(npc) && !Hero.MainHero.IsSpouse(npc))
            {
                //Hero.MainHero.GetDramalordRelation(npc).Status = RelationshipStatus.Lover;
                TextObject banner = new TextObject("{=Dramalord258}You and {HERO.LINK} are now a couple.");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
                Hero.MainHero.SetLover(npc);
            }

            HeroDateAction.Apply(Hero.MainHero, npc, Hero.MainHero.GetCloseHeroes());

            if (npc.GetDramalordTraits().Horny >= DramalordMCM.Get.MinHornyForIntercourse)
            {
                TextObject title = new TextObject("{=Dramalord245}Intimate Opportunity");
                TextObject text = new TextObject("{=Dramalord246}{HERO.LINK} has a special spark in their eyes today and you have a feeling they want to go further. Will you let it happen?");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, text);

                Campaign.Current.SetTimeSpeed(0);
                Notification.DrawMessageBox(
                        title,
                        text,
                        true,
                        () => {

                            HeroIntercourseAction.Apply(Hero.MainHero, npc, Hero.MainHero.GetCloseHeroes());
                            if (Hero.MainHero.IsFemale != npc.IsFemale && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                            {
                                Hero father = npc.IsFemale ? Hero.MainHero : npc;
                                HeroConceiveAction.Apply(npc.IsFemale ? npc : Hero.MainHero, father);
                            }
                        },
                        () => {

                        }
                    );
            }
        }

        internal static void PlayerMarriageAction(Hero npc)
        {
            HeroMarriageAction.Apply(Hero.MainHero, npc, Hero.MainHero.GetCloseHeroes());
        }

        internal static void PlayerBrokeUpNpcLeaveClan(Hero npc)
        {
            if (npc.Clan != null && npc.Clan == Hero.MainHero.Clan && DramalordMCM.Get.AllowClanChanges)
            {
                HeroLeaveClanAction.Apply(npc, npc);
            }
        }

        internal static void PlayerPerformsPrisonerDeal(Hero npc)
        {
            if (npc.IsDramalordLegit())
            {
                HeroIntercourseAction.Apply(Hero.MainHero, npc, Hero.MainHero.GetCloseHeroes());

                Hero mother = Hero.MainHero.IsFemale ? Hero.MainHero : npc;
                Hero father = npc.IsFemale ? Hero.MainHero : npc;

                if (mother != father && !mother.IsPregnant && mother.GetDramalordIsFertile() && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                {
                    HeroConceiveAction.Apply(mother, father);
                }
                if(npc.IsPrisoner)
                {
                    EndCaptivityAction.ApplyByRansom(npc, Hero.MainHero);
                }
                else if(Hero.MainHero.IsPrisoner)
                {
                    EndCaptivityAction.ApplyByRansom(Hero.MainHero, npc);
                }
            }
        }

        internal static void PlayerKillsNpc(Hero npc)
        {
            HeroFightAction.Apply(npc, Hero.MainHero);
        }
    }
}
