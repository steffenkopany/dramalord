using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class IntercourseAction
    {
        internal static bool HotButterFound = false;
        internal static bool Apply(Hero hero, Hero target, List<Hero> closeHeroes)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);
            HeroDesires heroDesires = hero.GetDesires();
            HeroDesires targetDesires = target.GetDesires();

            heroDesires.Horny -= targetDesires.IntercourseSkill * heroDesires.Libido;
            targetDesires.Horny -= heroDesires.IntercourseSkill * targetDesires.Libido;

            heroRelation.UpdateLove();

            heroRelation.Love += (heroDesires.IntercourseSkill + targetDesires.IntercourseSkill) /20;

            heroDesires.IntercourseSkill += (targetDesires.IntercourseSkill > heroDesires.IntercourseSkill) ? 2 : 1;
            targetDesires.IntercourseSkill += (targetDesires.IntercourseSkill < heroDesires.IntercourseSkill) ? 2 : 1;

            if (target == Hero.MainHero || hero == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;

                TextObject banner = new TextObject("{=Dramalord072}You were intimate with {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            int eventid = DramalordEvents.Instance.AddEvent(hero, target, EventType.Intercourse, 3);

            Hero? witness = null;
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                witness = closeHeroes.GetRandomElementWithPredicate(h => h != hero && h != target);
                if (DramalordMCM.Instance.PlayerAlwaysWitness && hero != Hero.MainHero && target != Hero.MainHero && witness != Hero.MainHero && closeHeroes.Contains(Hero.MainHero))
                {
                    witness = Hero.MainHero;
                }
                if (witness != null)
                {
                    if (witness.IsEmotionalWith(hero) || witness.IsEmotionalWith(target))
                    {
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, hero);
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, target);
                        DramalordIntentions.Instance.AddIntention(witness, hero, IntentionType.Confrontation, eventid);
                        DramalordIntentions.Instance.AddIntention(witness, target, IntentionType.Confrontation, eventid);
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord073}You caught {HERO.LINK} being intimate with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero || target == Hero.MainHero)
                    {
                        Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                        TextObject banner = new TextObject("{=Dramalord074}{HERO.LINK} caught you being intimate with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }

            if (target == Hero.MainHero || hero == Hero.MainHero)
            {
                if (HotButterFound)
                {
                    MBInformationManager.ShowSceneNotification(new HotButterNotification(hero, target, hero.CurrentSettlement, witness));
                }
            }

            return true;
        }
    }
}
