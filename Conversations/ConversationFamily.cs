using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationFamily
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow adoptFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                    .PlayerOption("{orphanage_player_start_adopt}")
                        .Condition(() => { if(Hero.OneToOneConversationHero.IsDramalordLegit()) SetupLines(); return Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader; })
                        .BeginNpcOptions()
                            .NpcOption("{adopt_reply_yes}", () => 
                            {
                                int boys = DramalordOrphans.Instance.CountOrphans(false);
                                int girls = DramalordOrphans.Instance.CountOrphans(true);
                                if (Hero.MainHero.Spouse != null && boys + girls > 0)
                                {
                                    MBTextManager.SetTextVariable("BOYS", boys);
                                    MBTextManager.SetTextVariable("GIRLS", girls);
                                    return true;
                                }
                                return false;
                            })
                                .BeginPlayerOptions()
                                    .PlayerOption("{orphanage_player_select_boy}")
                                        .Condition( () => DramalordOrphans.Instance.CountOrphans(false) > 0)
                                        .Consequence(() => ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(false)))
                                        .GotoDialogState("orphanage_child_selection")
                                    .PlayerOption("{orphanage_player_select_girl}")
                                        .Condition(() => DramalordOrphans.Instance.CountOrphans(true) > 0)
                                        .Consequence(() => ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(true)))
                                        .GotoDialogState("orphanage_child_selection")
                                    .PlayerOption("{nevermind}")
                                        .GotoDialogState("hero_main_options")
                                .EndPlayerOptions()
                            .NpcOption("{adopt_reply_married}", () => Hero.MainHero.Spouse == null)
                            .NpcOption("{adopt_reply_empty}", () => DramalordOrphans.Instance.CountOrphans(false) + DramalordOrphans.Instance.CountOrphans(true) == 0)
                        .EndNpcOptions()
                    .PlayerOption("{orphanage_player_start_orphanize}")
                        .Condition(() => { if (Hero.OneToOneConversationHero.IsDramalordLegit()) SetupLines(); return Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader; })
                        .BeginNpcOptions()
                            .NpcOption("{orphanize_reply_yes}", () => Hero.MainHero.Children.Where(c => c.Age < 18).ToList().Count > 0)
                                .Consequence(() => ConversationSentence.SetObjectsToRepeatOver(Hero.MainHero.Children.Where(c => c.Age < 18).ToList()))
                                .GotoDialogState("orphanage_player_ownchild_selection")    
                            .NpcOption("{orphanize_reply_empty}", () => Hero.MainHero.Children.Where(c => c.Age < 18).ToList().Count == 0)
                                .GotoDialogState("hero_main_options")
                        .EndNpcOptions()
                .EndPlayerOptions();


            starter.AddDialogLine("orphanage_select_orphan", "orphanage_child_selection", "orphanage_player_select_child", "{orphanage_select_orphan}", null, null);
            starter.AddRepeatablePlayerLine("orphanage_player_select_child", "orphanage_player_select_child", "orphanage_confirm_adopt", "{=Dramalord262}{ORPHAN.NAME}, {ORPHANAGE} years old.", "{orphanage_player_select_other}", "orphanage_child_selection", 
                () => 
                {
                    Hero? child = ConversationSentence.CurrentProcessedRepeatObject as Hero;
                    if (child != null)
                    {
                        StringHelpers.SetRepeatableCharacterProperties("ORPHAN", child.CharacterObject);
                        MBTextManager.SetTextVariable("ORPHANAGE", (int)child.Age);
                        return true;
                    }

                    return false;
                }, 
                null);

            starter.AddDialogLine("orphanage_player_ownchild", "orphanage_player_ownchild_selection", "orphanage_player_select_ownchild", "{orphanage_player_ownchild}", null, null);
            starter.AddRepeatablePlayerLine("orphanage_player_select_ownchild", "orphanage_player_select_ownchild", "orphanage_confirm_orphanize", "{=Dramalord262}{ORPHAN.NAME}, {ORPHANAGE} years old.", "{orphanage_player_select_other}", "orphanage_player_ownchild_selection", 
                () =>
                {
                    Hero? child = ConversationSentence.CurrentProcessedRepeatObject as Hero;
                    if (child != null)
                    {
                        StringHelpers.SetRepeatableCharacterProperties("ORPHAN", child.CharacterObject);
                        MBTextManager.SetTextVariable("ORPHANAGE", (int)child.Age);
                        return true;
                    }

                    return false;
                }, 
                null);

            DialogFlow doAdoptFlow = DialogFlow.CreateDialogFlow("orphanage_confirm_adopt")
                .NpcLine("{orphanage_confirm_adopt}")
                    .Condition(() =>
                    {
                        StringHelpers.SetCharacterProperties("ORPHAN", (ConversationSentence.SelectedRepeatObject as Hero)?.CharacterObject);
                        return true;
                    })
                    .BeginPlayerOptions()
                        .PlayerOption("{=str_yes}Yes.")
                            .NpcLine("{orphanage_do_adopt}")
                            .Consequence(() =>
                            {
                                Hero? orphan = (ConversationSentence.SelectedRepeatObject as Hero);
                                if (orphan != null)
                                {
                                    AdoptAction.Apply(Hero.MainHero.IsFemale ? Hero.MainHero : Hero.MainHero.Spouse, !Hero.MainHero.IsFemale ? Hero.MainHero : Hero.MainHero.Spouse, orphan);
                                    JoinClanAction.Apply(orphan, Clan.PlayerClan);
                                }
                            })
                        .PlayerOption("{=str_no}No.")
                            .NpcLine("{orphanage_dont_adopt}")
                            .GotoDialogState("hero_main_options")
                    .EndPlayerOptions();

            DialogFlow doOrphanizeFlow = DialogFlow.CreateDialogFlow("orphanage_confirm_orphanize")
                .NpcLine("{orphanage_confirm_adopt_child}")
                    .Condition(() =>
                    {
                        StringHelpers.SetCharacterProperties("ORPHAN", (ConversationSentence.SelectedRepeatObject as Hero)?.CharacterObject);
                        return true;
                    })
                    .BeginPlayerOptions()
                        .PlayerOption("{=str_yes}Yes.")
                            .NpcLine("{orphanage_do_orphanize}")
                            .Consequence(() =>
                            {
                                Hero? orphan = (ConversationSentence.SelectedRepeatObject as Hero);
                                if (orphan != null)
                                {
                                    OrphanizeAction.Apply(orphan);
                                }
                            })
                        .PlayerOption("{=str_no}No.")
                            .NpcLine("{orphanage_dont_orphanize}")
                            .GotoDialogState("hero_main_options")
                    .EndPlayerOptions();

            DialogFlow companionAddFamilyFlow = DialogFlow.CreateDialogFlow("npc_invite_companion_family")
                .BeginNpcOptions()
                    .NpcOption("{npc_invite_companion_family_yes}", () => SetupLines() && (Hero.OneToOneConversationHero.Father == null || !Hero.OneToOneConversationHero.Father.IsPlayerCompanion) && (Hero.OneToOneConversationHero.Mother == null || !Hero.OneToOneConversationHero.Mother.IsPlayerCompanion) && FamilyLikePlayer(Hero.OneToOneConversationHero))
                        .Consequence(() =>
                            {
                                AdoptCompanionFamily(Hero.OneToOneConversationHero);
                                TextObject banner = new TextObject("{=Dramalord087}{HERO.LINK} joined {CLAN}.");
                                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                                banner.SetTextVariable("CLAN", Hero.OneToOneConversationHero.Clan?.Name);
                                MBInformationManager.AddQuickInformation(banner, 1000, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                            })
                        .CloseDialog()
                    .NpcOption("{npc_invite_companion_family_no}", () => (Hero.OneToOneConversationHero.Father == null || !Hero.OneToOneConversationHero.Father.IsPlayerCompanion) && (Hero.OneToOneConversationHero.Mother == null || !Hero.OneToOneConversationHero.Mother.IsPlayerCompanion) && !FamilyLikePlayer(Hero.OneToOneConversationHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_invite_companion_family_elder}", () => (Hero.OneToOneConversationHero.Father != null && Hero.OneToOneConversationHero.Father.IsPlayerCompanion) || (Hero.OneToOneConversationHero.Mother != null && Hero.OneToOneConversationHero.Mother.IsPlayerCompanion))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(adoptFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(doAdoptFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(doOrphanizeFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(companionAddFamilyFlow);
        }

        internal static void AdoptCompanionFamily(Hero hero)
        {
            if(hero.Spouse != null && hero.Spouse.IsPlayerCompanion)
            {
                hero.Spouse.CompanionOf = null;
                JoinClanAction.Apply(hero.Spouse, Clan.PlayerClan);
            }

            hero.Siblings.ToList().ForEach(s =>
            {
                if (s.IsPlayerCompanion || (s.IsChild && s.Occupation == Occupation.Wanderer))
                {
                    s.CompanionOf = null;
                    JoinClanAction.Apply(s, Clan.PlayerClan);
                }
            });

            hero.Children.ToList().ForEach(c =>
            {
                if (c.IsPlayerCompanion || (c.IsChild && c.Occupation == Occupation.Wanderer))
                {
                    AdoptCompanionFamily(c);
                }
            });

            hero.CompanionOf = null;
            JoinClanAction.Apply(hero, Clan.PlayerClan);
        }

        internal static bool FamilyLikePlayer(Hero hero)
        {
            if(!hero.IsChild && hero.GetRelationWithPlayer() < DramalordMCM.Instance.MinTrustFriends)
            {
                return false;
            }

            if(!hero.IsChild && hero.Spouse != null && hero.Spouse.IsPlayerCompanion && hero.Spouse.GetRelationWithPlayer() < DramalordMCM.Instance.MinTrustFriends)
            {
                return false;
            }
            bool like = true;
            hero.Siblings.ToList().ForEach(s =>
            {
                if (!s.IsChild && s.IsPlayerCompanion && s.GetRelationWithPlayer() < DramalordMCM.Instance.MinTrustFriends)
                {
                    like = false;
                }
            });
            if(!like)
            {
                return false;
            }
            hero.Children.ToList().ForEach(c =>
            {
                if (!c.IsChild && c.IsPlayerCompanion && !FamilyLikePlayer(c))
                {
                    like = false;
                }
            });
            return like;
        }

        internal static bool SetupLines()
        {
            ConversationLines.orphanage_player_start_adopt.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord253}I would like to extend my family and adopt an orphan.");
            ConversationLines.orphanage_player_start_orphanize.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord254}I want to get rid off a child in my clan.");
            ConversationLines.adopt_reply_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord256}Oh that is great {TITLE}. We have currently {BOYS} boys and {GIRLS} girls in our orphanage.");
            ConversationLines.adopt_reply_married.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord257}I am truly sorry {TITLE}, but you have to be married in order to adopt a child.");
            ConversationLines.adopt_reply_empty.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord258}My apologies {TITLE}, but there are currently no children in our orphanage.");
            ConversationLines.orphanage_player_select_boy.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord259}I would like to adopt a boy.");
            ConversationLines.orphanage_player_select_girl.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord260}I would like to adopt a girl.");
            ConversationLines.orphanage_select_orphan.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord261}Who would you like to adopt?");
            ConversationLines.orphanage_player_select_child.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord262}{ORPHAN.NAME}, {ORPHANAGE} years old.");
            ConversationLines.orphanage_player_select_other.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord263}I am thinking of a different child.");
            ConversationLines.orphanage_confirm_adopt.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord264}Very well. Are you absolutely sure you want to adopt {ORPHAN.NAME}?");
            ConversationLines.orphanage_do_adopt.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord265}Congratulations! I'm sure {ORPHAN.NAME} will be in good hands.");
            ConversationLines.orphanage_dont_adopt.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord266}Oh well, that's sad. {ORPHAN.NAME} will be very disappointed.");
            ConversationLines.orphanize_reply_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord267}Of course {TITLE}, we can arrange that.");
            ConversationLines.orphanize_reply_empty.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord268}I appears {TITLE}, that there are no children in your clan.");
            ConversationLines.orphanage_player_ownchild.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord269}Who would be the unfortunate child we should take into custody?");
            ConversationLines.orphanage_confirm_adopt_child.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord270}Very well. Are you absolutely sure you want to give away {ORPHAN.NAME}?");
            ConversationLines.orphanage_do_orphanize.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord271}Thus, it is decided! We will take care of {ORPHAN.NAME} and find a new home for the child.");
            ConversationLines.orphanage_dont_orphanize.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord272}Very well, {TITLE}. {ORPHAN.NAME} will stay in your clan.");

            ConversationLines.player_invite_companion_family.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, true));
            ConversationLines.npc_invite_companion_family_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_invite_companion_family_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_invite_companion_family_elder.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));

            return true;
        }
    }
}
