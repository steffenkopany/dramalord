using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroJoinClanAction
    {
        internal static void Apply(Hero hero, Clan clan, bool withChildren)
        {
            if (Info.ValidateHeroInfo(hero) && hero.Clan == null)
            {
                hero.Clan = clan;
                hero.UpdateHomeSettlement();
                hero.ChangeState(Hero.CharacterStates.Active);

                if (withChildren)
                {
                    foreach (Hero child in hero.Children)
                    {
                        if (child.IsChild)
                        {
                            child.Clan = clan;
                            child.UpdateHomeSettlement();
                            child.ChangeState(Hero.CharacterStates.Active);
                        }
                    }
                }

                LogEntry.AddLogEntry(new EncyclopediaLogJoinClan(hero, clan));
                DramalordEvents.OnHeroesJoinClan(hero, clan);
            }
        }
    }
}
