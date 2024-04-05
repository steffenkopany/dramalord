using Dramalord.Data;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord
{
    internal class DramalordSafeableTypesDefiner : SaveableTypeDefiner
    {
        public DramalordSafeableTypesDefiner() : base(26051980)
        {
        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(HeroTuple), 1);
            AddClassDefinition(typeof(HeroInfoData), 2);
            AddClassDefinition(typeof(HeroMemoryData), 3);
            AddClassDefinition(typeof(HeroOffspringData), 4);

            AddClassDefinition(typeof(EncyclopediaLogStartAffair), 5);
            AddClassDefinition(typeof(EncyclopediaLogBreakup), 6);
            AddClassDefinition(typeof(EncyclopediaLogMarriage), 7);
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
            AddClassDefinition(typeof(EncyclopediaLogKilledSuizide), 27);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroInfoData>));
            ConstructContainerDefinition(typeof(Dictionary<HeroTuple, HeroMemoryData>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroOffspringData>));
            ConstructContainerDefinition(typeof(List<Hero>));
        }
    }
}
