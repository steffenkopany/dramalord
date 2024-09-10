using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.Conversations
{
    internal static class GoodsConversation
    {
        private static ItemObject? _object = null;
        private static int _amount = 0;

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("goods_greeting", "hero_main_options", "goods_npc_offer", "{=Dramalord463}I wish to acquire certain goods...", ConditionGoodsConversation, null);

            starter.AddDialogLine("goods_npc_offer", "goods_npc_offer", "goods_player_select", "{=Dramalord464}Alright. What are you interested in?", null, null);

            starter.AddPlayerLine("goods_player_select_sausage", "goods_player_select", "goods_player_select_number", "{=Dramalord465}Something long, rounded that can withstand confrontations with moist environments.", null, ConsequencePlayerSelectSausage);
            starter.AddPlayerLine("goods_player_select_pie", "goods_player_select", "goods_player_select_number", "{=Dramalord466}Something soft and moist that you can stick your finger into multiple times.", null, ConsequencePlayerSelectPie);
            starter.AddPlayerLine("goods_player_select_none", "goods_player_select", "goods_player_select_abort", "{=Dramalord255}Nevermind.", null, null);

            starter.AddDialogLine("goods_player_select_abort", "goods_player_select_abort", "hero_main_options", "{=Dramalord186}As you wish, {TITLE}.", ConditionPlayerSelectAbort, null);

            starter.AddDialogLine("goods_player_select_number", "goods_player_select_number", "goods_player_select_number_choice", "{=Dramalord467}I can sell you a {GOOD} or more if you want.", ConditionPlayerSelectNumber, null);

            starter.AddPlayerLine("goods_player_select_number_choice_1", "goods_player_select_number_choice", "goods_player_select_bill", "{=Dramalord468}One will do.", null, ConsequencePlayerSelectOne);
            starter.AddPlayerLine("goods_player_select_number_choice_5", "goods_player_select_number_choice", "goods_player_select_bill", "{=Dramalord469}Five should suffice.", null, ConsequencePlayerSelectFive);
            starter.AddPlayerLine("goods_player_select_number_choice_10", "goods_player_select_number_choice", "goods_player_select_bill", "{=Dramalord470}I would need at least ten.", null, ConsequencePlayerSelectTen);

            starter.AddDialogLine("goods_player_select_bill", "goods_player_select_bill", "goods_player_select_bill_confirm", "{=Dramalord471}No problem! That would be {AMOUNT}{GOLD_ICON} for you. Special price of course.", ConditionGoodsPrice, null);

            starter.AddPlayerLine("goods_player_select_bill_confirm_yes", "goods_player_select_bill_confirm", "goods_player_select_pay", "{=str_yes}Yes.", ConditionPlayerPays, ConsequencePlayerPays);
            starter.AddPlayerLine("goods_player_select_bill_confirm_no", "goods_player_select_bill_confirm", "goods_player_select_abort", "{=str_no}No.", null, null);

            starter.AddDialogLine("goods_player_select_pay", "goods_player_select_pay", "hero_main_options", "{=Dramalord472}Nice doing business with you.", null, null);
        }

        private static bool ConditionGoodsConversation()
        {
            return Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader;
        }

        private static bool ConditionPlayerSelectAbort()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return true;
        }

        private static bool ConditionPlayerSelectNumber()
        {
            MBTextManager.SetTextVariable("GOOD", _object?.Name);
            return true;
        }

        private static bool ConditionGoodsPrice()
        {
            float relation = Hero.OneToOneConversationHero.GetRelationWithPlayer() / 100f;
            int singleprice = 100 - (int)(100 * relation);
            int totalprice = _amount * singleprice;
            MBTextManager.SetTextVariable("AMOUNT", totalprice.ToString());
            return true;
        }

        private static bool ConditionPlayerPays()
        {
            float relation = Hero.OneToOneConversationHero.GetRelationWithPlayer() / 100f;
            int singleprice = 100 - (int)(100 * relation);
            int totalprice = _amount * singleprice;
            return Hero.MainHero.Gold >= totalprice;
        }

        private static void ConsequencePlayerSelectSausage()
        {
            _object = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
        }

        private static void ConsequencePlayerSelectPie()
        {
            _object = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
        }

        private static void ConsequencePlayerSelectOne()
        {
            _amount = 1;
        }

        internal static void ConsequencePlayerSelectFive()
        {
            _amount = 5;
        }

        private static void ConsequencePlayerSelectTen()
        {
            _amount = 10;
        }

        private static void ConsequencePlayerPays()
        {
            float relation = Hero.OneToOneConversationHero.GetRelationWithPlayer() / 100f;
            int singleprice = 100 - (int)(100 * relation);
            int totalprice = _amount * singleprice;

            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(_object, _amount);
            Hero.MainHero.Gold -= totalprice;

            TextObject banner = new TextObject("{=Dramalord294}You bought {AMOUNT} pieces of {TOY}.");
            banner.SetTextVariable("AMOUNT", _amount);
            banner.SetTextVariable("TOY", _object?.Name);
            MBInformationManager.AddQuickInformation(banner, 0, Hero.MainHero.CharacterObject, "event:/ui/notification/relation");
        }
    }
}
