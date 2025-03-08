using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Notifications.Logs;
using Dramalord.Quests;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord
{
    internal sealed class DramalordSave : SaveableTypeDefiner
    {
        public DramalordSave() : base(19831011)
        {

        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(Intention), 1000);
            AddClassDefinition(typeof(LeaveClanToWanderIntention), 1001);
            AddClassDefinition(typeof(GiveBirthIntention), 1002);
            AddClassDefinition(typeof(AbortPregnancyIntention), 1003);
            AddClassDefinition(typeof(OrphanizeChildIntention), 1004); 
            //AddClassDefinition(typeof(EndRelationshipIntention), 1005);
            AddClassDefinition(typeof(IntercourseIntention), 1006);
            AddClassDefinition(typeof(ConfrontIntercourseIntention), 1007);
            AddClassDefinition(typeof(FlirtIntention), 1008);
            AddClassDefinition(typeof(TalkIntention), 1009);
            AddClassDefinition(typeof(DateIntention), 1010);
            AddClassDefinition(typeof(ConfrontDateIntention), 1011);
            AddClassDefinition(typeof(ConfrontMarriageIntention), 1012);
            AddClassDefinition(typeof(ConfrontBetrothedIntention), 1013);
            AddClassDefinition(typeof(BetrothIntention), 1014);
            AddClassDefinition(typeof(ConfrontBirthIntention), 1015);
            AddClassDefinition(typeof(MarriageIntention), 1016);
            AddClassDefinition(typeof(GossipBetrothedIntention), 1017);
            AddClassDefinition(typeof(GossipBirthIntention), 1018);
            AddClassDefinition(typeof(GossipDateIntention), 1019);
            AddClassDefinition(typeof(GossipIntercourseIntention), 1020);
            AddClassDefinition(typeof(GossipMarriageIntention), 1021);
            AddClassDefinition(typeof(PrisonIntercourseIntention), 1022);
            AddClassDefinition(typeof(UseToyIntention), 1023); 
            AddClassDefinition(typeof(FinishJoinPartyQuestIntention), 1024); 
            AddClassDefinition(typeof(ChangeOpinionIntention), 1025); 
            AddClassDefinition(typeof(ConfrontationPlayerIntention), 1026); 
            AddClassDefinition(typeof(DuelIntention), 1027);
            AddClassDefinition(typeof(AdoptIntention), 1028);
            AddClassDefinition(typeof(BlackmailBetrothedIntention), 1029);

            AddClassDefinition(typeof(HeroPregnancy), 2000);
            AddClassDefinition(typeof(HeroDesires), 2001);
            AddClassDefinition(typeof(HeroPersonality), 2002);
            AddClassDefinition(typeof(HeroRelation), 2003);
            AddClassDefinition(typeof(VisitLoverQuest), 2004);
            AddClassDefinition(typeof(DramalordQuest), 2005);
            AddClassDefinition(typeof(JoinPlayerQuest), 2006); 
            AddClassDefinition(typeof(MarriagePermissionQuest), 2007); 
            AddClassDefinition(typeof(ConfrontHeroQuest), 2008);

            AddClassDefinition(typeof(StartRelationshipLog), 3000);
            AddClassDefinition(typeof(EndRelationshipLog), 3001);
            AddClassDefinition(typeof(IntercourseLog), 3002);
            AddClassDefinition(typeof(PrisonIntercourseLog), 3003);
            AddClassDefinition(typeof(LeaveClanLog), 3004);
            AddClassDefinition(typeof(JoinClanLog), 3005);
            AddClassDefinition(typeof(LeaveKingdomLog), 3006);
            AddClassDefinition(typeof(JoinKingdomLog), 3007);
            AddClassDefinition(typeof(ConceiveChildLog), 3008);
            AddClassDefinition(typeof(BirthChildLog), 3009);
            AddClassDefinition(typeof(OrphanizeChildLog), 3010);
            AddClassDefinition(typeof(AdoptChildLog), 3011);
            AddClassDefinition(typeof(AbortChildLog), 3012);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<Hero>));

            ConstructContainerDefinition(typeof(List<Intention>));

            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroPregnancy>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroDesires>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroPersonality>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroRelation>));
            ConstructContainerDefinition(typeof(Dictionary<Hero,Dictionary<Hero, HeroRelation>>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, DramalordQuest>));
        }
    }

    internal sealed class LegacySave : SaveableTypeDefiner
    {
        public class HeroPersonalityLegacy
        {
            [SaveableProperty(1)]
            internal int Openness_ { get; set; }

            [SaveableProperty(2)]
            internal int Conscientiousness_ { get; set; }

            [SaveableProperty(3)]
            internal int Extroversion_ { get; set; }

            [SaveableProperty(4)]
            internal int Agreeableness_ { get; set; }

            [SaveableProperty(5)]
            internal int Neuroticism_ { get; set; }
        }

        public class HeroRelationLegacy
        {
            [SaveableProperty(2)]
            internal int Love_ { get; set; }

            [SaveableProperty(4)]
            internal double LastInteraction_ { get; set; }

            [SaveableProperty(5)]
            internal int RelationshipAsInt_ { get; set; }
        }

        public class HeroPregnancyLegacy
        {
            [SaveableProperty(1)]
            internal string FatherId_ { get; set; }

            [SaveableProperty(2)]
            internal double Conceived_ { get; set; }

            [SaveableProperty(3)]
            internal int EventId_ { get; set; }
        }

        public class HeroDesiresLegacy
        {
            [SaveableProperty(1)]
            internal int AttractionMen_ { get; set; }

            [SaveableProperty(2)]
            internal int AttractionWomen_ { get; set; }

            [SaveableProperty(3)]
            internal int AttractionWeight_ { get; set; }

            [SaveableProperty(4)]
            internal int AttractionBuild_ { get; set; }

            [SaveableProperty(5)]
            internal int AttractionAgeDiff_ { get; set; }

            [SaveableProperty(6)]
            internal int Libido_ { get; set; }

            [SaveableProperty(7)]
            internal int Horny_ { get; set; }

            [SaveableProperty(8)]
            internal int PeriodDayOfSeason_ { get; set; }

            [SaveableProperty(9)]
            internal int IntercourseSkill_ { get; set; }

            [SaveableProperty(10)]
            internal bool HasToy_ { get; set; }

            [SaveableProperty(11)]
            internal bool InfoKnown_ { get; set; } = false;
        }

        public static void LoadLegacyPersonality(Dictionary<Hero, HeroPersonality> personalities, IDataStore dataStore)
        {
            Dictionary<string, HeroPersonalityLegacy> data = new();
            dataStore.SyncData("DramalordPersonalities", ref data);

            data.Do(keypair =>
            {
                Hero? hero = Hero.AllAliveHeroes.Where(item => item.StringId == keypair.Key).FirstOrDefault();
                if (hero != null && !personalities.ContainsKey(hero))
                {
                    personalities.Add(hero, new HeroPersonality(keypair.Value.Openness_, keypair.Value.Conscientiousness_, keypair.Value.Extroversion_, keypair.Value.Agreeableness_, keypair.Value.Neuroticism_));
                }
            });
        }

        public static void LoadLegacyRelations(Dictionary<Hero, Dictionary<Hero, HeroRelation>> relations, IDataStore dataStore)
        {
            Dictionary<string, Dictionary<string, HeroRelationLegacy>> data = new();
            dataStore.SyncData("DramalordRelations", ref data);

            data.Do(firstpair =>
            {
                Hero? hero1 = Hero.AllAliveHeroes.Where(hero => hero.StringId == firstpair.Key).FirstOrDefault();
                if (hero1 != null)
                {
                    if (!relations.ContainsKey(hero1))
                    {
                        relations.Add(hero1, new());
                    }

                    firstpair.Value.Do(secondpair =>
                    {
                        Hero? hero2 = Hero.AllAliveHeroes.Where(hero => hero.StringId == secondpair.Key).FirstOrDefault();
                        if (hero2 != null && hero2 != hero1)
                        {
                            if (!relations[hero1].ContainsKey(hero2))
                            {
                                if (relations.ContainsKey(hero2) && relations[hero2].ContainsKey(hero1))
                                {
                                    relations[hero1].Add(hero2, relations[hero2][hero1]);
                                }
                                else
                                {
                                    HeroRelation newRelation = new(secondpair.Value.Love_, (RelationshipType)secondpair.Value.RelationshipAsInt_);
                                    newRelation.LastInteraction = CampaignTime.Now;
                                    relations[hero1].Add(hero2, newRelation);
                                }
                            }
                        }
                    });
                }
            });
        }

        public static void LoadLegacyPregnancies(Dictionary<Hero, HeroPregnancy> pregnancies, IDataStore dataStore)
        {
            Dictionary<string, HeroPregnancyLegacy> data = new();
            dataStore.SyncData("DramalordPregnancies", ref data);

            data.Do(keypair =>
            {
                Hero? mother = Hero.AllAliveHeroes.Where(hero => hero.StringId == keypair.Key).FirstOrDefault();
                HeroPregnancy pregnancy = new(
                    Hero.AllAliveHeroes.Where(hero => hero.StringId == keypair.Value.FatherId_).FirstOrDefault(),
                    CampaignTime.Milliseconds((long)keypair.Value.Conceived_)
                    );
                if (mother != null && pregnancy.Father != null && !pregnancies.ContainsKey(mother))
                {
                    pregnancies.Add(mother, pregnancy);
                }
            });
        }

        public static void LoadLegacyDesires(Dictionary<Hero, HeroDesires> desires, IDataStore dataStore)
        {
            Dictionary<string, HeroDesiresLegacy> data = new();
            dataStore.SyncData("DramalordDesires", ref data);

            data.Do(keypair =>
            {
                Hero hero = Hero.AllAliveHeroes.Where(item => item.StringId == keypair.Key).FirstOrDefault();
                if (hero != null && !desires.ContainsKey(hero))
                {
                    HeroDesires newHeroDesires = new(
                        keypair.Value.AttractionMen_,
                        keypair.Value.AttractionWomen_,
                        keypair.Value.AttractionWeight_,
                        keypair.Value.AttractionBuild_,
                        keypair.Value.AttractionAgeDiff_,
                        keypair.Value.Libido_,
                        keypair.Value.Horny_,
                        keypair.Value.PeriodDayOfSeason_,
                        keypair.Value.IntercourseSkill_,
                        keypair.Value.HasToy_,
                        keypair.Value.InfoKnown_);
                    desires.Add(hero, newHeroDesires);
                }
            });
        }

        public static void LoadLegacyOrphans(List<Hero> orphans, IDataStore dataStore)
        {
            List<string> data = new();
            dataStore.SyncData("DramalordOrphans", ref data);

            data.Do(heroid =>
            {
                Hero? orphan = Hero.AllAliveHeroes.Where(hero => hero.StringId == heroid).FirstOrDefault();
                if (orphan != null && !orphans.Contains(orphan))
                {
                    orphan.SetNewOccupation(Occupation.NotAssigned);
                    orphans.Add(orphan);
                }
            });
        }

        public LegacySave() : base(20110311)
        {

        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(HeroPersonalityLegacy), 1000);
            AddClassDefinition(typeof(HeroRelationLegacy), 1001);
            AddClassDefinition(typeof(HeroPregnancyLegacy), 1002);
            AddClassDefinition(typeof(HeroDesiresLegacy), 1003);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, HeroPersonalityLegacy>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroRelationLegacy>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroPregnancyLegacy>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroDesiresLegacy>));
            ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<string, HeroRelationLegacy>>));
        }
    }
}
