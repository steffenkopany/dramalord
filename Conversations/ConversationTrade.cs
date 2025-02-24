using Dramalord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.Conversations
{
    internal static class ConversationTrade
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow goodsFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .PlayerLine("{player_goods_greeting}")
                    .Condition(() => { if (Hero.OneToOneConversationHero.IsDramalordLegit()) SetupLines(); return Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader; })
                    .NpcLine("{npc_goods_offer}")
                        .BeginPlayerOptions()
                            .PlayerOption("{player_goods_select_sausage}")
                                .NpcLine("{npc_goods_select_number}")
                                .Condition(() => { ConversationLines.npc_goods_select_number.SetTextVariable("GOOD", new TextObject("{=Dramalord240}Sausage")); return true; })
                                    .BeginPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_1}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 1);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord240}Sausage"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_5}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 5);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord240}Sausage"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_10}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 10);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord240}Sausage"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                    .EndPlayerOptions()
                            .PlayerOption("{player_goods_select_pie}")
                                .NpcLine("{npc_goods_select_number}")
                                .Condition(() => { ConversationLines.npc_goods_select_number.SetTextVariable("GOOD", new TextObject("{=Dramalord241}Pie")); return true; })
                                    .BeginPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_1}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 1 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 1);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord241}Pie"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_5}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 5 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 5);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord241}Pie"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                        .PlayerOption("{player_goods_select_number_10}")
                                            .NpcLine("{npc_goods_select_number_reply}")
                                                .Condition(() =>
                                                {
                                                    int totalprice = 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                    MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
                                                    return true;
                                                })
                                                .BeginPlayerOptions()
                                                    .PlayerOption("{=str_yes}Yes.")
                                                        .Condition(() => Hero.MainHero.Gold >= 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer()))
                                                        .Consequence(() =>
                                                        {
                                                            Hero.MainHero.Gold -= 10 * (100 - (int)Hero.OneToOneConversationHero.GetRelationWithPlayer());
                                                            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie"), 1);
                                                            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
                                                            banner.SetTextVariable("AMOUNT", 10);
                                                            banner.SetTextVariable("TOY", new TextObject("{=Dramalord241}Pie"));
                                                            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
                                                        })
                                                        .NpcLine("{player_goods_select_pay}")
                                                            .GotoDialogState("hero_main_options")
                                                    .PlayerOption("{=str_no}No.")
                                                        .NpcLine("{npc_as_you_wish_reply}")
                                                            .GotoDialogState("hero_main_options")
                                                .EndPlayerOptions()
                                    .EndPlayerOptions()
                            .PlayerOption("{nevermind}")
                                .NpcLine("{npc_as_you_wish_reply}")
                                .GotoDialogState("hero_main_options")
                        .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(goodsFlow);
        }

        internal static void SetupLines()
        {
            ConversationLines.player_goods_greeting.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord463}I wish to acquire certain goods...");
            ConversationLines.npc_goods_offer.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord464}Alright. What are you interested in?");
            ConversationLines.player_goods_select_sausage.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord465}Something long, rounded that can withstand confrontations with moist environments.");
            ConversationLines.player_goods_select_pie.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord466}Something soft and moist that you can stick your finger into multiple times.");
            ConversationLines.npc_goods_select_number.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord467}I can sell you a {GOOD} or more if you want.");
            ConversationLines.player_goods_select_number_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord468}One will do.");
            ConversationLines.player_goods_select_number_5.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord469}Five should suffice.");
            ConversationLines.player_goods_select_number_10.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord470}I would need at least ten.");
            ConversationLines.npc_goods_select_number_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord471}No problem! That would be {AMOUNT}{GOLD_ICON} for you. Special price of course.");
            ConversationLines.player_goods_select_pay.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord472}Nice doing business with you.");
            ConversationLines.npc_as_you_wish_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord186}As you wish, {TITLE}.");
        }
    }
}
