using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class MarriageAction
    {
        internal static bool Apply(Hero firstHero, Hero secondHero, List<Hero> closeHeroes)
        {
            Settlement? currentSettlement = (firstHero.CurrentSettlement == secondHero.CurrentSettlement) ? firstHero.CurrentSettlement : null;

            if(firstHero != Hero.MainHero && firstHero.Spouse != null)
            {
                BreakupAction.Apply(firstHero, firstHero.Spouse);
            }

            if (secondHero != Hero.MainHero && secondHero.Spouse != null)
            {
                BreakupAction.Apply(secondHero, secondHero.Spouse);
            }

            if (firstHero.Clan == null && secondHero.Clan == null)
            {
                Clan? targetClan = firstHero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Friend).Select(keyvalue => keyvalue.Key).FirstOrDefault(relation => relation.Clan != null && relation.Clan != Clan.PlayerClan)?.Clan
                    ?? secondHero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Friend).Select(keyvalue => keyvalue.Key).FirstOrDefault(selected => selected.Clan != null && selected.Clan != Clan.PlayerClan)?.Clan;

                if (targetClan != null)
                {
                    JoinClanAction.Apply(firstHero, targetClan, true);
                    JoinClanAction.Apply(secondHero, targetClan, true);
                }
                else
                {
                    return false;
                }
            }
            else if (firstHero.Clan != secondHero.Clan)
            {
                if (firstHero.Clan == null)
                {
                    JoinClanAction.Apply(firstHero, secondHero.Clan, true);
                }
                else if (secondHero.Clan == null)
                {
                    JoinClanAction.Apply(secondHero, firstHero.Clan, true);
                }
                else if (firstHero.Clan == Clan.PlayerClan)
                {
                    LeaveClanAction.Apply(secondHero, secondHero, true);
                    JoinClanAction.Apply(secondHero, firstHero.Clan, true);
                }
                else if (secondHero.Clan == Clan.PlayerClan)
                {
                    LeaveClanAction.Apply(firstHero, firstHero, true);
                    JoinClanAction.Apply(firstHero, secondHero.Clan, true);
                }
                else if(firstHero.Clan.Leader == firstHero && secondHero.Clan.Leader != secondHero)
                {
                    LeaveClanAction.Apply(secondHero, secondHero, true);
                    JoinClanAction.Apply(secondHero, firstHero.Clan, true);
                }
                else if (firstHero.Clan.Leader != firstHero && secondHero.Clan.Leader == secondHero)
                {
                    LeaveClanAction.Apply(firstHero, firstHero, true);
                    JoinClanAction.Apply(firstHero, secondHero.Clan, true);
                }
                else if (firstHero.IsFemale != secondHero.IsFemale && secondHero.IsFemale)
                {
                    LeaveClanAction.Apply(secondHero, secondHero, true);
                    JoinClanAction.Apply(secondHero, firstHero.Clan, true);
                }
                else
                {
                    LeaveClanAction.Apply(firstHero, firstHero, true);
                    JoinClanAction.Apply(firstHero, secondHero.Clan, true);
                }
            }

            ChangeRomanticStateAction.Apply(firstHero, secondHero, Romance.RomanceLevelEnum.Marriage);
            if (firstHero.Spouse != null && !firstHero.ExSpouses.Contains(firstHero.Spouse)) firstHero.ExSpouses.Add(firstHero.Spouse);
            if (secondHero.Spouse != null && !secondHero.ExSpouses.Contains(secondHero.Spouse)) secondHero.ExSpouses.Add(firstHero.Spouse);
            firstHero.ExSpouses.Remove(secondHero);
            secondHero.ExSpouses.Remove(firstHero);
            firstHero.Spouse = secondHero;
            secondHero.Spouse = firstHero;
            var ex1 = firstHero.ExSpouses.Distinct().Where(h => h != null).ToList();
            firstHero.ExSpouses.Clear();
            firstHero.ExSpouses.AddRange(ex1);
            var ex2 = secondHero.ExSpouses.Distinct().Where(h => h != null).ToList();
            secondHero.ExSpouses.Clear();
            secondHero.ExSpouses.AddRange(ex2);

            firstHero.GetRelationTo(secondHero).UpdateLove();
            firstHero.GetRelationTo(secondHero).Relationship = RelationshipType.Spouse;


            //CampaignEventDispatcher.Instance.OnHeroesMarried(firstHero, secondHero, false);
            if(currentSettlement != null)
            {
                if(firstHero.CurrentSettlement == null)
                {
                    TeleportHeroAction.ApplyImmediateTeleportToSettlement(firstHero, currentSettlement);
                }
                if (secondHero.CurrentSettlement == null)
                {
                    TeleportHeroAction.ApplyImmediateTeleportToSettlement(secondHero, currentSettlement);
                }
            }

            if (firstHero.Clan == Clan.PlayerClan || secondHero.Clan == Clan.PlayerClan)
            {
                Hero groom = firstHero.IsFemale ? secondHero : firstHero;
                Hero bride = groom == firstHero ? secondHero : firstHero;

                if(firstHero == Hero.MainHero || secondHero == Hero.MainHero)
                {
                    Hero otherHero = (firstHero == Hero.MainHero) ? secondHero : firstHero;
                    otherHero.SetNewOccupation(Occupation.Lord);
                    if (Clan.PlayerClan.Companions.Contains(otherHero)) Clan.PlayerClan.Companions.Remove(otherHero);
                    if(!Clan.PlayerClan.Lords.Contains(otherHero)) Clan.PlayerClan.Lords.Add(otherHero);
                }

                TextObject textObject = new TextObject("{=Dramalord080}{HERO.LINK} married {TARGET.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", groom.CharacterObject, textObject);
                StringHelpers.SetCharacterProperties("TARGET", bride.CharacterObject, textObject);

                MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(groom, bride, CampaignTime.Now));
                MBInformationManager.AddNotice(new MarriageMapNotification(firstHero, secondHero, textObject, CampaignTime.Now));
            }

            int oldEvent = DramalordEvents.Instance.FindEvent(firstHero, secondHero, EventType.Betrothed);
            DramalordEvents.Instance.RemoveEvent(oldEvent);

            int eventID = DramalordEvents.Instance.AddEvent(firstHero, secondHero, EventType.Marriage, 10);
            closeHeroes.Where(closeHero => closeHero != firstHero && closeHero != secondHero && (closeHero.IsEmotionalWith(firstHero) || closeHero.IsEmotionalWith(secondHero))).ToList().ForEach(closeHero => 
            {
                closeHero.RemoveIntentionsTo(firstHero);
                closeHero.RemoveIntentionsTo(secondHero);
                closeHero.AddIntention(firstHero, IntentionType.Confrontation, eventID);
                closeHero.AddIntention(secondHero, IntentionType.Confrontation, eventID); 
            });

            if ((firstHero.Clan == Clan.PlayerClan || secondHero.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new StartRelationshipLog(firstHero, secondHero, RelationshipType.Spouse));
            }

            return true;
        }
    }
}
