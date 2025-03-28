using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Dramalord.Conversations
{
    internal static class ConversationTools
    {
        internal static Intention? ConversationIntention { get; set; } = null;

        private static Mission? ConversationMission = null;

        private class ConversationListener : IMissionListener
        {
            Vec2 _position;

            public ConversationListener(Vec2 mapPos)
            {
                _position = mapPos;
            }

            public void OnConversationCharacterChanged() { }

            public void OnEndMission()
            {
                ConversationIntention = null;
                ConversationMission?.RemoveListener(this);
                ConversationMission = null;
                PlayerEncounter.Finish();

                if (_position.IsValid)
                {
                    MobileParty.MainParty.Position2D = _position;
                }
            }

            public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType) { }

            public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType) { }

            public void OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan) { }

            public void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
            {
                if(ConversationMission?.Mode != MissionMode.Conversation && ConversationMission?.Mode != MissionMode.Barter)
                {
                    ConversationMission?.EndMission();
                }
            }

            public void OnResetMission() { }
        }


        internal static bool StartConversation(Intention intention, bool civilian)
        {
            if(ConversationIntention == null)
            {
                ConversationIntention = intention;
                ConversationIntention.OnConversationStart();
                Hero speaker = (ConversationIntention.IntentionHero == Hero.MainHero) ? ConversationIntention.Target : ConversationIntention.IntentionHero;
                CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(speaker.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
                return true;
            }
            return false;
        }

        internal static bool StartVisit(Intention intention, bool civilian)
        {
            if (ConversationIntention == null)
            {
                ConversationIntention = intention;
                ConversationIntention.OnConversationStart();
                Hero speaker = (ConversationIntention.IntentionHero == Hero.MainHero) ? ConversationIntention.Target : ConversationIntention.IntentionHero;

                PlayerEncounter.Start();
                PlayerEncounter.Current.SetupFields(PartyBase.MainParty, PartyBase.MainParty);
                Campaign.Current.CurrentConversationContext = ConversationContext.Default;

                Vec2 position = new(Hero.MainHero.GetMapPoint().Position2D);

                if (speaker.CurrentSettlement != null)
                {
                    PlayerEncounter.EnterSettlement();

                    var locationOfTarget = LocationComplex.Current.GetLocationOfCharacter(speaker);
                    var locationOfCharacter = LocationComplex.Current.GetLocationOfCharacter(Hero.MainHero);

                    CampaignEventDispatcher.Instance.OnPlayerStartTalkFromMenu(speaker);
                    ConversationMission = (Mission)PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(locationOfTarget, locationOfCharacter, speaker.CharacterObject);
                }
                else
                {
                    position = Vec2.Invalid;

                    var specialScene = "";
                    var sceneLevels = "";

                    ConversationMission = (Mission)Campaign.Current.CampaignMissionManager.OpenConversationMission(
                        new ConversationCharacterData(Hero.MainHero.CharacterObject, null, true),
                        new ConversationCharacterData(speaker.CharacterObject, null, true),
                        specialScene, sceneLevels);
                }

                ConversationMission.AddListener(new ConversationListener(position));

                return true;
            }

            return false;
        }

        internal static void EndConversation(bool leaveEncounter = true)
        {
            if(leaveEncounter)
            {
                if (PlayerEncounter.Current != null)
                {
                    PlayerEncounter.LeaveEncounter = true;
                }
            }
        }

        internal static void OnConversationStart(IAgent agent)
        {
            if(agent.Character != CharacterObject.PlayerCharacter)
            {
                Intention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i => i.IntentionHero.CharacterObject == agent.Character && i.Target == Hero.MainHero);

                if (intention != null && ConversationIntention == null)
                {
                    ConversationIntention = intention;
                    ConversationIntention.OnConversationStart();
                    DramalordIntentions.Instance.GetIntentions().Remove(intention);
                }
            }
        }

        internal static void OnConversationEnded(IEnumerable<CharacterObject> characters)
        {
            if (ConversationIntention != null)
            {
                 ConversationIntention.OnConversationEnded();
            }
            ConversationIntention = null;

            //characters.Where(ch => ch.IsHero && ch.HeroObject != Hero.MainHero && ch.HeroObject.IsDramalordLegit()).Do(ch => ch.HeroObject.GetRelationTo(Hero.MainHero).LastInteraction = CampaignTime.Now);
        }

        internal static TextObject GetHeroGreeting(Hero hero, Hero target, bool capital)
        {
            bool name = MBRandom.RandomInt(1, 100) < 50;
            RelationshipType relationship = hero.GetRelationTo(target).Relationship;
            string text;
            if (relationship == RelationshipType.Spouse || hero.Spouse == target)
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject("{=8eHRth3U}my wife").ToString()) : ((name) ? target.FirstName.ToString() : new TextObject("{=QuVgluRH}my husband").ToString());
            }
            else if (relationship == RelationshipType.Betrothed)
            {
                text = (name) ? target.FirstName.ToString() : new TextObject("{=Dramalord025}my betrothed").ToString();
            }
            else if (relationship == RelationshipType.Lover)
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject("{=Dramalord024}my love").ToString()) : ((name) ? target.FirstName.ToString() : new TextObject("{=Dramalord023}my lover").ToString());
            }
            else if (hero.Father == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else if (hero.Mother == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else if (hero.Siblings.Contains(target))
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject("{=Dramalord487}sister").ToString()) : ((name) ? target.FirstName.ToString() : new TextObject("{=Dramalord486}brother").ToString());
            }
            else if (target.Father == hero || target.Mother == hero)
            {
                text = target.IsFemale ? (name) ? target.FirstName.ToString() : new TextObject("{=Dramalord489}daughter").ToString() : (name) ? target.FirstName.ToString() : new TextObject("{=Dramalord488}son").ToString();
            }
            else if (relationship == RelationshipType.FriendWithBenefits)
            {
                text = (name) ? target.FirstName.ToString() : new TextObject("{=Dramalord026}my special friend").ToString();
            }
            else if (relationship == RelationshipType.Friend)
            {
                text = (name) ? target.FirstName.ToString() : new TextObject("{=edRggEQ4}my friend").ToString();
            }
            else
            {
                if (hero.IsLord && target.IsLord)
                {
                    text = target.FirstName.ToString();
                }
                else if ((!hero.IsLord && target.IsLord) || target.MapFaction.Leader == target)
                {
                    text = GameTexts.FindText(target.IsFemale ? "str_player_salutation_my_lady" : "str_player_salutation_my_lord").ToString();
                }
                else if (hero.IsPlayerCompanion)
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_captain", target.CharacterObject).ToString();
                }
                else if (target.IsFemale)
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_madame", target.CharacterObject).ToString();
                }
                else
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_sir", target.CharacterObject).ToString();
                }
            }

            if (capital)
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

        internal static TextObject GetHeroRelation(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject("{=Dramalord174}my friend");
            if (relation == RelationshipType.FriendWithBenefits) return new TextObject("{=Dramalord026}my special friend");
            if (relation == RelationshipType.Lover) return new TextObject("{=Dramalord023}my lover");
            if (relation == RelationshipType.Betrothed) return new TextObject("{=Dramalord025}my betrothed");
            if (relation == RelationshipType.Spouse || hero.Spouse == partner) return new TextObject("{=Dramalord173}my spouse");
            return new TextObject("{=Dramalord175}my acquaintance");
        }

        internal static string FormatNumber(int number)
        {
            return (number > 0) ? "+" + number.ToString() : number.ToString();
        }
    }
}
