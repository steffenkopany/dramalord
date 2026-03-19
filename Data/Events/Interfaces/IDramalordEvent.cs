using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Data.Events.Interfaces
{
    public interface IDramalordEvent
    {
        Hero Actor { get; }

        Hero Target { get; }

        List<Hero> IsKnownTo { get; }

        void Action(int modifier = 0);

        void AfterDialog();

        DialogFlow? GetInitiationDialog();

        IDramalordEvent? CreateReaction(Hero hero);

        DialogFlow? GetConfrontationDialog(Hero speaker);

        DialogFlow? GetGossipDialog(Hero speaker);

        DialogFlow? GetBlackmailDialog(Hero speaker);

        void ReactionResult(Hero speaker, Hero listener, out int trust, out int love);
    }
}
