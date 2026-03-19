using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Notifications;
using Dramalord.Quests;
using System.Collections.Generic;
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
            AddInterfaceDefinition(typeof(IDramalordEvent), 999);
            AddClassDefinition(typeof(TalkEvent), 1000);
            AddClassDefinition(typeof(FlirtEvent), 1001);
            AddClassDefinition(typeof(DateEvent), 1002);
            AddClassDefinition(typeof(SexEvent), 1003);
            AddClassDefinition(typeof(ConceiveEvent), 1004);
            AddClassDefinition(typeof(BirthEvent), 1005);
            AddClassDefinition(typeof(RelationshipEvent), 1006);
            AddClassDefinition(typeof(JoinClanEvent), 1007);
            AddClassDefinition(typeof(LeaveClanEvent), 1008);
            AddClassDefinition(typeof(MarriageEvent), 1009);
            AddClassDefinition(typeof(PrisonSexEvent), 1010);
            AddClassDefinition(typeof(OrphanizeEvent), 1011);
            AddClassDefinition(typeof(ConfrontEvent), 1012);
            AddClassDefinition(typeof(GossiptEvent), 1013);
            AddClassDefinition(typeof(AdoptEvent), 1014);
            AddClassDefinition(typeof(BreakUpEvent), 1015);
            AddClassDefinition(typeof(BlackmailEvent), 1016);
            AddClassDefinition(typeof(ThreesomeEvent), 1017);

            AddClassDefinition(typeof(HeroPregnancy), 2000);
            AddClassDefinition(typeof(HeroDesires), 2001);
            AddClassDefinition(typeof(HeroPersonality), 2002);
            AddClassDefinition(typeof(HeroRelation), 2003);

            AddClassDefinition(typeof(VisitLoverQuest), 2004);
            AddClassDefinition(typeof(DramalordQuest), 2005);
            AddClassDefinition(typeof(MarriagePermissionQuest), 2007); 
            AddClassDefinition(typeof(ConfrontHeroQuest), 2008); 
            AddClassDefinition(typeof(DivorceLoverSpouseQuest), 2009);
            AddClassDefinition(typeof(BlackmailQuest), 2010);

            AddClassDefinition(typeof(DramalordEventNotification), 3000);
            AddClassDefinition(typeof(DramalordQuestNotification), 3001);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<Hero>));
            ConstructContainerDefinition(typeof(List<IDramalordEvent>));

            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroPregnancy>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroDesires>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroPersonality>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, HeroRelation>));
            ConstructContainerDefinition(typeof(Dictionary<Hero,Dictionary<Hero, HeroRelation>>));
            ConstructContainerDefinition(typeof(Dictionary<Hero, DramalordQuest>));
        }
    }
}
