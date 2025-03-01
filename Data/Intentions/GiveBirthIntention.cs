using Dramalord.Actions;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class GiveBirthIntention : Intention
    {
        [SaveableField(4)]
        public readonly HeroPregnancy Pregnancy;

        public GiveBirthIntention(HeroPregnancy pregnancy, Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
        {
            Pregnancy = pregnancy;
        }

        public override bool Action()
        {
            Hero? child = null;
            GiveBirthAction.Apply(IntentionHero, Pregnancy.Father, out child);
            DramalordPregnancies.Instance.RemovePregnancy(IntentionHero);

            if(child == null)
            {
                //misscarry info @todo
                return true;
            }

            if(IntentionHero.Clan == Clan.PlayerClan || Pregnancy.Father.Clan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord077}{HERO.LINK} was born");
                StringHelpers.SetCharacterProperties("HERO", child?.CharacterObject, textObject);

                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(Pregnancy.Father, IntentionHero, CampaignTime.Now));
                MBInformationManager.AddNotice(new ChildBornMapNotification(child, textObject, CampaignTime.Now));
            }

            if((IntentionHero.Clan == Clan.PlayerClan && IntentionHero != Hero.MainHero) || (Pregnancy.Father.Clan != null && Pregnancy.Father.Clan == Clan.PlayerClan))
            {
                int speed = (int)Campaign.Current.TimeControlMode;
                Campaign.Current.SetTimeSpeed(0);
                TextObject title = new TextObject("{=Dramalord524}{CHILD} was born.");
                TextObject text = new TextObject("{=Dramalord525}{MOTHER} has given birth to {CHILD}. {FATHER} is the father. Would urge the mother to keep the child or get rid of them?");
                title.SetTextVariable("CHILD", child?.Name);
                text.SetTextVariable("MOTHER", IntentionHero.Name);
                text.SetTextVariable("CHILD", child?.Name);
                text.SetTextVariable("FATHER", Pregnancy.Father.Name);
                InformationManager.ShowInquiry(
                        new InquiryData(
                            title.ToString(),
                            text.ToString(),
                            true,
                            true,
                            new TextObject("{=Dramalord526}Get rid of the child").ToString(),
                            new TextObject("{=Dramalord527}Keep the child").ToString(),
                            () => { DramalordIntentions.Instance.GetIntentions().Add(new OrphanizeChildIntention(child, IntentionHero, CampaignTime.DaysFromNow(1))); Campaign.Current.SetTimeSpeed(speed); },
                            () => { Campaign.Current.SetTimeSpeed(speed); }), true);
            }
            else if ((IntentionHero.Spouse == null || (IntentionHero.Spouse != Pregnancy.Father && IntentionHero.GetRelationTo(Pregnancy.Father).Relationship != RelationshipType.Spouse)) && ((IntentionHero.Clan != null && IntentionHero.Clan != Clan.PlayerClan && !DramalordMCM.Instance.KeepClanChildren) || (IntentionHero.Clan == null && !DramalordMCM.Instance.KeepNotableChildren)))
            {
                DramalordIntentions.Instance.GetIntentions().Add(new OrphanizeChildIntention(child, IntentionHero, CampaignTime.DaysFromNow(1)));
            }
            else if (child.Clan == Clan.PlayerClan && child.Occupation == Occupation.Wanderer)
            {
                if (Pregnancy.Father == Hero.MainHero || IntentionHero == Hero.MainHero)
                {
                    child.SetNewOccupation(Occupation.Lord);
                    Clan.PlayerClan.Companions.Remove(child);
                }
                else if (Clan.PlayerClan.Companions.Count >= Campaign.Current.Models.ClanTierModel.GetCompanionLimit(Clan.PlayerClan))
                {
                    child.SetNewOccupation(Occupation.Lord);
                    Clan.PlayerClan.Companions.Remove(child);
                    DramalordIntentions.Instance.GetIntentions().Add(new OrphanizeChildIntention(child, IntentionHero, CampaignTime.DaysFromNow(1)));
                }
            }

            if(child.Clan == null && child.Occupation == Occupation.Lord)
            {
                child.SetNewOccupation(Occupation.NotAssigned); //TODO thats some workaround
            }

            if (IntentionHero.Clan == Clan.PlayerClan || Pregnancy.Father.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new BirthChildLog(IntentionHero, Pregnancy.Father, child));
            }

            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
                Hero? witness = DramalordMCM.Instance.PlayerAlwaysWitness && child.Father != Hero.MainHero && closeHeroes.Contains(Hero.MainHero) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h != child && h != child.Father);
                if (witness != null)
                {
                    if (witness != Hero.MainHero && witness.IsEmotionalWith(IntentionHero))
                    {
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontBirthIntention(IntentionHero, Pregnancy.Father, child, witness, CampaignTime.DaysFromNow(7), true));
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontBirthIntention(Pregnancy.Father, IntentionHero, child, witness, CampaignTime.DaysFromNow(7), true));
                    }
                    else if(witness != Hero.MainHero && IntentionHero.Spouse != Pregnancy.Father)
                    {
                        List<Hero> targets = new() { IntentionHero, Pregnancy.Father };
                        DramalordIntentions.Instance.GetIntentions().Add(new GossipBirthIntention(this, child, true, targets, witness, CampaignTime.DaysFromNow(7)));
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord528}You have witnessed {HERO.LINK} giving birth to a child of {FATHER.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("FATHER", Pregnancy.Father.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (IntentionHero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord529}{HERO.LINK} saw you giving birth to the child of {FATHER.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("FATHER", Pregnancy.Father.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }

            return true;
        }

        public override void OnConversationEnded()
        {

        }

        public override void OnConversationStart()
        {
           
        }
    }
}
