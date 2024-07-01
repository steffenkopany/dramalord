using Dramalord.Data.Deprecated;
using Dramalord.Quests;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal sealed class HeroDataSaver : SaveableTypeDefiner
    {
        public HeroDataSaver() : base(26051980)
        {
        }

        protected override void DefineClassTypes()
        {
            //REDUNDANT
            AddClassDefinition(typeof(HeroTuple), 1);
            AddClassDefinition(typeof(HeroInfoData), 2);
            AddClassDefinition(typeof(HeroMemoryData), 3);
            AddClassDefinition(typeof(HeroOffspringData), 4);

            //STAYS
            AddClassDefinition(typeof(EncyclopediaLogStartAffair), 5);
            AddClassDefinition(typeof(EncyclopediaLogBreakup), 6);
            AddClassDefinition(typeof(EncyclopediaLogDivorce), 8);
            AddClassDefinition(typeof(EncyclopediaLogPutChildToOrphanage), 9);
            AddClassDefinition(typeof(EncyclopediaLogKilledWhenCaught), 10);
            AddClassDefinition(typeof(EncyclopediaLogJoinClan), 11);
            AddClassDefinition(typeof(EncyclopediaLogKilledWhenPregnant), 12);
            AddClassDefinition(typeof(LogIntercourse), 13);
            AddClassDefinition(typeof(EncyclopediaLogConceived), 14);
            AddClassDefinition(typeof(LogFlirt), 15);
            AddClassDefinition(typeof(LogAffairMeeting), 16);
            AddClassDefinition(typeof(EncyclopediaLogAdopted), 17);
            AddClassDefinition(typeof(EncyclopediaLogBirth), 18);
            AddClassDefinition(typeof(EncyclopediaLogLeaveClan), 19);
            AddClassDefinition(typeof(LogWitnessFlirt), 20);
            AddClassDefinition(typeof(LogWitnessDate), 21);
            AddClassDefinition(typeof(LogWitnessIntercourse), 22);
            AddClassDefinition(typeof(LogWitnessPregnancy), 23);
            AddClassDefinition(typeof(LogWitnessBastard), 24);
            AddClassDefinition(typeof(EncyclopediaLogKilledWhenBornBastard), 25);
            AddClassDefinition(typeof(LogUsedToy), 26);
            AddClassDefinition(typeof(EncyclopediaLogKilledSuicide), 27);
            AddClassDefinition(typeof(EncyclopediaLogClanLeftKingdom), 28);
            AddClassDefinition(typeof(EncyclopediaLogClanJoinedKingdom), 29);
            AddClassDefinition(typeof(VisitLoverQuest), 30); 

            //NEW
            AddClassDefinition(typeof(HeroEventSave), 31);
            AddClassDefinition(typeof(HeroPregnancySave), 32); 
            AddClassDefinition(typeof(HeroFeelingsSave), 33);
            AddClassDefinition(typeof(HeroMemorySave), 34);
            AddClassDefinition(typeof(DramalordEndCaptivityLogEntry), 35); 
            AddClassDefinition(typeof(HeroOrphanSave), 36); 
            AddClassDefinition(typeof(LogConfrontation), 37); 
            AddClassDefinition(typeof(DramalordTakePrisonerLogEntry), 38); 
            AddClassDefinition(typeof(LogWitnessMarriage), 39); 
            AddClassDefinition(typeof(LogGossipConversation), 40); 
            AddClassDefinition(typeof(EncyclopediaLogStartFWB), 41);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroInfoData>));
            ConstructContainerDefinition(typeof(Dictionary<HeroTuple, HeroMemoryData>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroOffspringData>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, VisitLoverQuest>)); //TODO!

            //NEW  
            ConstructContainerDefinition(typeof(Dictionary<string, int>));
            ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<string, int>>));
            ConstructContainerDefinition(typeof(List<string>));
            ConstructContainerDefinition(typeof(Dictionary<int, HeroEventSave>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroPregnancySave>));
            ConstructContainerDefinition(typeof(Dictionary<string, List<string>>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroFeelingsSave>));
            ConstructContainerDefinition(typeof(Dictionary<string,Dictionary<string, HeroFeelingsSave>>));
            ConstructContainerDefinition(typeof(List<HeroMemorySave>));
            ConstructContainerDefinition(typeof(Dictionary<string, List<HeroMemorySave>>)); 
            ConstructContainerDefinition(typeof(List<HeroOrphanSave>));
        }

        internal static void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsLoading)
            {
                // OLD STUFF
                Dictionary<Hero, HeroInfoData> info = new();
                Dictionary<HeroTuple, HeroMemoryData> memories = new();
                Dictionary<Hero, HeroOffspringData> offsprings = new();
                List<Hero> orphanage = new List<Hero>();

                dataStore.SyncData("DramalordHeroInfo", ref info);
                dataStore.SyncData("DramalordHeroMemory", ref memories);
                dataStore.SyncData("DramalordHeroOffspring", ref offsprings);
                dataStore.SyncData("DramalordHeroOrphanage", ref orphanage);

                //OLD CHECK
                if (info != null && info.Count > 0)
                {
                    ConvertOldInfo(info);
                }

                //TRAITS
                Dictionary<string, Dictionary<string, int>> traits = new();
                dataStore.SyncData("DramalordHeroTraits", ref traits);

                //check if it needs upgrade to new trait values
                for(int i = 0; i< traits.Count; ++i)
                {
                    bool traitsNeedUpgrade = false;
                    Dictionary<string, int> v = traits.ElementAt(i).Value;
                    if(v.ContainsKey(HeroTraits.Openness.StringId))
                    {
                        if (v[HeroTraits.Openness.StringId] <= 0)
                        {
                            traitsNeedUpgrade = true;
                        }
                    }
                    if (v.ContainsKey(HeroTraits.Conscientiousness.StringId))
                    {
                        if (v[HeroTraits.Conscientiousness.StringId] <= 0)
                        {
                            traitsNeedUpgrade = true;
                        }
                    }
                    if (v.ContainsKey(HeroTraits.Extroversion.StringId))
                    {
                        if (v[HeroTraits.Extroversion.StringId] <= 0)
                        {
                            traitsNeedUpgrade = true;
                        }
                    }
                    if (v.ContainsKey(HeroTraits.Neuroticism.StringId))
                    {
                        if (v[HeroTraits.Neuroticism.StringId] <= 0)
                        {
                            traitsNeedUpgrade = true;
                        }
                    }
                    if (v.ContainsKey(HeroTraits.Agreeableness.StringId))
                    {
                        if (v[HeroTraits.Agreeableness.StringId] <= 0)
                        {
                            traitsNeedUpgrade = true;
                        }
                    }

                    if (traitsNeedUpgrade)
                    {
                        v.Keys.ToList().ForEach(key =>
                        {
                            TraitObject? trait = HeroTraits.GetTraitObject(key);
                            if (trait != null)
                            {
                                v[key] = FixTraitUpdate(trait, v[key]);
                            }
                        });
                    }
                }

                foreach (string charID in traits.Keys)
                {
                    CharacterObject? charObj = CharacterObject.Find(charID);
                    if (charObj != null && charObj.IsHero)
                    {
                        Hero h = charObj.HeroObject;
                        HeroTraits.ApplyToHero(h, false);
                        foreach (string key in traits[charID].Keys)
                        {
                            TraitObject? trait = HeroTraits.GetTraitObject(key);
                            if (trait != null)
                            {
                                int value = traits[charID][key];
                                h.GetHeroTraits().SetPropertyValue(trait, value);
                            }
                        }
                    }
                }

                traits.Clear();
                HeroTraits.AddToAllTraits();

                // EVENTS
                Dictionary<int, HeroEventSave> events = new();
                dataStore.SyncData("DramalordHeroEvents", ref events);

                int lastId = 0;
                DramalordEvents.Events.Clear();
                foreach (KeyValuePair<int, HeroEventSave> it in events)
                {
                    lastId = (lastId < it.Key) ? it.Key : lastId;
                    DramalordEvents.Events.Add(it.Key, it.Value.Create());
                }
                DramalordEvents.LastId = lastId;
                events.Clear();


                // PREGNANCY
                Dictionary<string, HeroPregnancySave> pregnancies = new();
                dataStore.SyncData("DramalordHeroPregnancies", ref pregnancies);


                //OLD STUFF
                if(offsprings != null && offsprings.Count > 0)
                {
                    foreach (Hero h in offsprings.Keys)
                    {
                        HeroOffspringData hod = offsprings[h];
                        HeroPregnancy preg = new(hod.Father.CharacterObject, (uint)hod.Conceived, 0);
                        HeroPregnancySave pregSave = new(preg);
                        pregnancies.Add(h.CharacterObject.StringId, pregSave);
                    }
                }

                DramalordPregnancies.Pregnancies.Clear();
                foreach(KeyValuePair<string, HeroPregnancySave> it in pregnancies)
                {
                    CharacterObject? obj = CharacterObject.Find(it.Key);
                    HeroPregnancy? peg = it.Value.Create();
                    if (obj != null && peg != null)
                    {
                        DramalordPregnancies.Pregnancies.Add(obj, peg);
                    }
                }
                pregnancies.Clear();


                // ORPHANS
                List<HeroOrphanSave> orphans = new();
                Dictionary<string, int> lastAdotions = new();
                string orphanManager = string.Empty;
                dataStore.SyncData("DramalordHeroOrphans", ref orphans);
                dataStore.SyncData("DramalordHeroAdotionDays", ref lastAdotions);
                dataStore.SyncData("DramalordHeroAdotionManager", ref orphanManager);

                //OLD STUFF
                if (orphanage != null && orphanage.Count > 0)
                {
                    foreach (Hero h in orphanage)
                    {
                        HeroOrphan newOrphan = new(h.CharacterObject);
                        orphans.Add(new HeroOrphanSave(newOrphan));
                    }
                }

                if(memories != null && memories.Count() > 0)
                {
                    foreach(HeroTuple ht in memories.Keys)
                    {
                        if (memories[ht].LastAdoption > 0)
                        {
                            lastAdotions.Add(ht.Actor.CharacterObject.StringId, (int)memories[ht].LastAdoption);
                            lastAdotions.Add(ht.Target.CharacterObject.StringId, (int)memories[ht].LastAdoption);
                        }
                    }
                }

                DramalordOrphanage.Orphans.Clear();
                foreach(HeroOrphanSave hos in orphans)
                {
                    HeroOrphan? ho = hos.Create();
                    if(ho != null)
                    {
                        DramalordOrphanage.Orphans.Add(ho);
                    }  
                }
                orphans.Clear();

                DramalordOrphanage.LastAdoptionDays.Clear();
                foreach(string h in lastAdotions.Keys)
                {
                    CharacterObject? co = CharacterObject.Find(h);
                    if(co != null)
                    {
                        DramalordOrphanage.LastAdoptionDays.Add(co, (uint)lastAdotions[h]);
                    }
                }
                lastAdotions.Clear();

                if(orphanManager.Length > 0)
                {
                    DramalordOrphanage.OrphanManager = CharacterObject.Find(orphanManager);
                }
                

                // RELATIONS
                Dictionary<string, List<string>> spouses = new();
                Dictionary<string, List<string>> lovers = new();
                Dictionary<string, List<string>> friendswithbenefits = new();
                Dictionary<string, Dictionary<string, HeroFeelingsSave>> feelings = new();
                dataStore.SyncData("DramalordHeroSpouses", ref spouses);
                dataStore.SyncData("DramalordHeroLovers", ref lovers);
                dataStore.SyncData("DramalordHeroFriendsWithBenefits", ref friendswithbenefits);
                dataStore.SyncData("DramalordHeroFeelings", ref feelings);

                //OLD STUFF
                if (memories != null && memories.Count > 0)
                {
                    ConvertOldMemories(memories, feelings, spouses, lovers, friendswithbenefits);
                }

                DramalordRelations.Partners.Clear();
                foreach(KeyValuePair<string, List<string>> it in spouses)
                {
                    CharacterObject charO = CharacterObject.Find(it.Key);
                    if(charO == null)
                    {
                        continue;
                    }
                    HeroPartners partners = new();
                    foreach(string name in it.Value)
                    {
                        CharacterObject co = CharacterObject.Find(name);
                        if(co != null)
                        {
                            partners.Spouses.Add(co);
                        }
                    }
                    foreach(string name in lovers[it.Key])
                    {
                        CharacterObject co = CharacterObject.Find(name);
                        if(co != null)
                        {
                            partners.Lovers.Add(co);
                        }
                    }
                    foreach(string name in friendswithbenefits[it.Key])
                    {
                        CharacterObject co = CharacterObject.Find(name);
                        if (co != null)
                        {
                            partners.FriendsWithBenefits.Add(co);
                        }
                    }
                    foreach(KeyValuePair<string, HeroFeelingsSave> feel in feelings[it.Key])
                    {
                        CharacterObject co = CharacterObject.Find(feel.Key);
                        if(co != null)
                        {
                            partners.Feelings.Add(co, feel.Value.Create());
                        }
                    }
                    DramalordRelations.Partners.Add(charO, partners);
                }
                spouses.Clear();
                lovers.Clear();
                friendswithbenefits.Clear();
                feelings.Clear();


                /// MEMORY
                Dictionary<string, List<HeroMemorySave>> memory = new();
                dataStore.SyncData("DramalordHeroMemories", ref memory);

                DramalordMemories.Memories.Clear();
                foreach(KeyValuePair<string, List<HeroMemorySave>> mem in memory)
                {
                    List<HeroMemory> heroMemories = new();
                    foreach(HeroMemorySave hms in mem.Value)
                    {
                        heroMemories.Add(hms.Create());
                    }
                    CharacterObject co = CharacterObject.Find(mem.Key);
                    if(co != null)
                    {
                        DramalordMemories.Memories.Add(co, heroMemories);
                    }
                }
                memory.Clear();

                //GENERAL DATA STUFF
                Hero.AllAliveHeroes.Where(hero => hero.IsDramalordLegit()).Do(hero =>
                {
                    if(hero.Spouse != null && !hero.GetHeroSpouses().Contains(hero.Spouse.CharacterObject))
                    {
                        hero.SetSpouse(hero.Spouse);
                    }
                });
            }
            else if(dataStore.IsSaving)
            {
                // TRAITS
                Dictionary<string, Dictionary<string, int>> traits = new();

                Hero.AllAliveHeroes.Where(item => item.IsDramalordLegit()).Do(item =>
                {
                    Dictionary<string, int> heroTraits = new();
                    HeroTraits.AllTraits.ForEach(trait =>
                    {
                        if (trait != null)
                        {
                            heroTraits.Add(trait.StringId, item.GetHeroTraits().GetPropertyValue(trait));
                        }
                    });
                    traits.Add(item.CharacterObject.StringId, heroTraits);
                });

                dataStore.SyncData("DramalordHeroTraits", ref traits);

                //EVENTS
                Dictionary<int, HeroEventSave> events = new();
                foreach(KeyValuePair<int, HeroEvent> it in DramalordEvents.Events)
                {
                    if(it.Value.Hero1 != null && it.Value.Hero2 != null)
                    {
                        events.Add(it.Key, new HeroEventSave(it.Value));
                    }
                }
                dataStore.SyncData("DramalordHeroEvents", ref events);


                //PREGNANCY
                Dictionary<string, HeroPregnancySave> pregnancies = new();
                foreach(KeyValuePair <CharacterObject, HeroPregnancy> it in DramalordPregnancies.Pregnancies)
                {
                    pregnancies.Add(it.Key.StringId, new HeroPregnancySave(it.Value));
                }
                dataStore.SyncData("DramalordHeroPregnancies", ref pregnancies);



                //RELATIONS
                Dictionary<string, List<string>> spouses = new();
                Dictionary<string, List<string>> lovers = new();
                Dictionary<string, List<string>> friendswithbenefits = new();
                Dictionary<string, Dictionary<string, HeroFeelingsSave>> feelings = new();

                foreach(KeyValuePair<CharacterObject, HeroPartners> it in DramalordRelations.Partners)
                {
                    List<string> spouse = new();
                    foreach(CharacterObject obj in it.Value.Spouses)
                    {
                        spouse.Add(obj.StringId);
                    }
                    if(!spouses.ContainsKey(it.Key.StringId))
                        spouses.Add(it.Key.StringId, spouse);

                    List<string> lover = new();
                    foreach (CharacterObject obj in it.Value.Lovers)
                    {
                        lover.Add(obj.StringId);
                    }
                    if (!lovers.ContainsKey(it.Key.StringId))
                        lovers.Add(it.Key.StringId, lover);

                    List<string> friend = new();
                    foreach (CharacterObject obj in it.Value.FriendsWithBenefits)
                    {
                        friend.Add(obj.StringId);
                    }
                    if (!friendswithbenefits.ContainsKey(it.Key.StringId))
                        friendswithbenefits.Add(it.Key.StringId, friend);

                    Dictionary<string, HeroFeelingsSave> feeling = new();
                    foreach(KeyValuePair<CharacterObject, HeroFeelings> fe in it.Value.Feelings)
                    {
                        if(!feeling.ContainsKey(fe.Key.StringId))
                            feeling.Add(fe.Key.StringId, new HeroFeelingsSave(fe.Value));
                    }
                    if (!feelings.ContainsKey(it.Key.StringId))
                        feelings.Add(it.Key.StringId, feeling);
                }

                dataStore.SyncData("DramalordHeroSpouses", ref spouses);
                dataStore.SyncData("DramalordHeroLovers", ref lovers);
                dataStore.SyncData("DramalordHeroFriendsWithBenefits", ref friendswithbenefits);
                dataStore.SyncData("DramalordHeroFeelings", ref feelings);


                ///MEMORY
                Dictionary<string, List<HeroMemorySave>> memory = new();
                foreach(CharacterObject co in DramalordMemories.Memories.Keys)
                {
                    List<HeroMemorySave> list = new();
                    List<HeroMemory> mems = DramalordMemories.GetHeroMemory(co.HeroObject);
                    mems.Where(item => item.Source != null).Do(item =>
                    {
                        list.Add(new HeroMemorySave(item));
                    });

                    if (list.Count() > 0 && !memory.ContainsKey(co.StringId))
                    {
                        memory.Add(co.StringId, list);
                    }
                }

                dataStore.SyncData("DramalordHeroMemories", ref memory);


                ///ORPHANS
                string orphanManager = DramalordOrphanage.OrphanManager?.StringId ?? string.Empty;
                if(orphanManager != string.Empty)
                {
                    dataStore.SyncData("DramalordHeroAdotionManager", ref orphanManager);
                }

                List<HeroOrphanSave> orphans = new();
                DramalordOrphanage.Orphans.ForEach(item =>
                {
                    orphans.Add(new HeroOrphanSave(item));
                });
                dataStore.SyncData("DramalordHeroOrphans", ref orphans);

                Dictionary<string, int> lastAdotions = new();
                foreach(CharacterObject o in DramalordOrphanage.LastAdoptionDays.Keys)
                {
                    lastAdotions.Add(o.StringId, (int)DramalordOrphanage.LastAdoptionDays[o]);
                }
                dataStore.SyncData("DramalordHeroAdotionDays", ref lastAdotions);
            }
        }

        private static void ConvertOldInfo(Dictionary<Hero, HeroInfoData> oldInfo)
        {
            //convert data
            foreach (Hero h in oldInfo.Keys)
            {
                HeroTraits.ApplyToHero(h, true);
                HeroInfoData data = oldInfo[h];

                h.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionMen, data.AttractionMen);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionWomen, data.AttractionWomen);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionWeight, (int)(data.AttractionWeight * 100f));
                h.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionBuild, (int)(data.AttractionBuild * 100f));
                h.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionAgeDiff, data.AttractionAgeDiff);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.Libido, (int)data.Libido);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, (int)data.Horny);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.IntercourseSkill, (int)data.IntercourseSkill);
                h.GetHeroTraits().SetPropertyValue(HeroTraits.HasToy, data.HasToy ? 1 : 0);
            }
        }

        private static int FixTraitUpdate(TraitObject trait, int traitValue)
        {
            if(trait == HeroTraits.Openness || trait == HeroTraits.Conscientiousness || trait == HeroTraits.Extroversion || trait == HeroTraits.Agreeableness || trait == HeroTraits.Neuroticism)
            {
                return (traitValue * 50) + 101;
            }
            if (trait == HeroTraits.AttractionMen || trait == HeroTraits.AttractionWomen || trait == HeroTraits.AttractionWeight || trait == HeroTraits.AttractionBuild || trait == HeroTraits.Horny || trait == HeroTraits.Libido || trait == HeroTraits.IntercourseSkill || trait == HeroTraits.HasToy)
            {
                return traitValue + 1;
            }
            if(trait == HeroTraits.AttractionAgeDiff)
            {
                return traitValue + 21;
            }
            return traitValue;
        }

        private static void ConvertOldMemories(Dictionary<HeroTuple, HeroMemoryData> memories, Dictionary<string, Dictionary<string, HeroFeelingsSave>> feelings, Dictionary<string, List<string>> spouses, Dictionary<string, List<string>> lovers, Dictionary<string, List<string>> fwb)
        {
            //convert data
            foreach (HeroTuple tuple in memories.Keys)
            {
                List<string> spouse1 = (spouses.ContainsKey(tuple.Actor.CharacterObject.StringId)) ? spouses[tuple.Actor.CharacterObject.StringId] : new();
                List<string> spouse2 = (spouses.ContainsKey(tuple.Target.CharacterObject.StringId)) ? spouses[tuple.Target.CharacterObject.StringId] : new();

                List<string> lovers1 = (lovers.ContainsKey(tuple.Actor.CharacterObject.StringId)) ? lovers[tuple.Actor.CharacterObject.StringId] : new();
                List<string> lovers2 = (lovers.ContainsKey(tuple.Target.CharacterObject.StringId)) ? lovers[tuple.Target.CharacterObject.StringId] : new();

                List<string> fwb1 = (fwb.ContainsKey(tuple.Actor.CharacterObject.StringId)) ? fwb[tuple.Actor.CharacterObject.StringId] : new();
                List<string> fwb2 = (fwb.ContainsKey(tuple.Target.CharacterObject.StringId)) ? fwb[tuple.Target.CharacterObject.StringId] : new();

                Dictionary<string, HeroFeelingsSave> feelingList1 = (feelings.ContainsKey(tuple.Actor.CharacterObject.StringId)) ? feelings[tuple.Actor.CharacterObject.StringId] : new();
                Dictionary<string, HeroFeelingsSave> feelingList2 = (feelings.ContainsKey(tuple.Target.CharacterObject.StringId)) ? feelings[tuple.Target.CharacterObject.StringId] : new();

                HeroFeelingsSave feelings1 = (feelingList1.ContainsKey(tuple.Target.CharacterObject.StringId)) ? feelingList1[tuple.Target.CharacterObject.StringId] : new(new HeroFeelings((int)memories[tuple].Emotion,  memories[tuple].IsCouple ? 20 : 0, (uint)memories[tuple].LastMet));
                HeroFeelingsSave feelings2 = (feelingList2.ContainsKey(tuple.Actor.CharacterObject.StringId)) ? feelingList2[tuple.Actor.CharacterObject.StringId] : new(new HeroFeelings((int)memories[tuple].Emotion,  memories[tuple].IsCouple ? 20 : 0, (uint)memories[tuple].LastMet));

                if(tuple.Actor.Spouse == tuple.Target)
                {
                    spouse1.Add(tuple.Target.CharacterObject.StringId);
                    spouse2.Add(tuple.Actor.CharacterObject.StringId);
                }
                if(!spouses.ContainsKey(tuple.Actor.CharacterObject.StringId))
                {
                    spouses.Add(tuple.Actor.CharacterObject.StringId, spouse1);
                }
                if (!spouses.ContainsKey(tuple.Target.CharacterObject.StringId))
                {
                    spouses.Add(tuple.Target.CharacterObject.StringId, spouse2);
                }

                if(tuple.Actor.Spouse != tuple.Target && memories[tuple].IsCouple)
                {
                    lovers1.Add(tuple.Target.CharacterObject.StringId);
                    lovers2.Add(tuple.Actor.CharacterObject.StringId);
                }
                if (!lovers.ContainsKey(tuple.Actor.CharacterObject.StringId))
                {
                    lovers.Add(tuple.Actor.CharacterObject.StringId, lovers1);
                }
                if (!lovers.ContainsKey(tuple.Target.CharacterObject.StringId))
                {
                    lovers.Add(tuple.Target.CharacterObject.StringId, lovers2);
                }

                if (!fwb.ContainsKey(tuple.Actor.CharacterObject.StringId))
                {
                    fwb.Add(tuple.Actor.CharacterObject.StringId, fwb1);
                }
                if (!fwb.ContainsKey(tuple.Target.CharacterObject.StringId))
                {
                    fwb.Add(tuple.Target.CharacterObject.StringId, fwb2);
                }

                if (!feelingList1.ContainsKey(tuple.Target.CharacterObject.StringId))
                {
                    feelingList1.Add(tuple.Target.CharacterObject.StringId, feelings1);
                }

                if (!feelingList2.ContainsKey(tuple.Actor.CharacterObject.StringId))
                {
                    feelingList2.Add(tuple.Actor.CharacterObject.StringId, feelings2);
                }

                if(!feelings.ContainsKey(tuple.Actor.CharacterObject.StringId))
                {
                    feelings.Add(tuple.Actor.CharacterObject.StringId, feelingList1);
                }

                if (!feelings.ContainsKey(tuple.Target.CharacterObject.StringId))
                {
                    feelings.Add(tuple.Target.CharacterObject.StringId, feelingList2);
                }
            }
        }
    }
}
