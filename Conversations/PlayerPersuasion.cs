using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    /*
    
            private void ExecuteOpenConversation()
        {
            if (CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Right && CurrentCharacter.Character != CharacterObject.PlayerCharacter)
            {
                if (Settlement.CurrentSettlement == null)
                {
                    CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty), new ConversationCharacterData(CurrentCharacter.Character, PartyBase.MainParty, noHorse: false, noWeapon: false, spawnAfterFight: false, CurrentCharacter.IsPrisoner));
                }
                else
                {
                    PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationOfCharacter(LocationComplex.Current.GetFirstLocationCharacterOfCharacter(CurrentCharacter.Character)), null, CurrentCharacter.Character);
                }

                IsInConversation = true;
            }
        }
    
    internal class PlayerPersuasion
    {
        private List<PersuasionTask> GetPersuasionTasksForCourtshipStage1(Hero wooed, Hero wooer)
        {
            StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            List<PersuasionTask> list = new List<PersuasionTask>();
            PersuasionTask persuasionTask = new PersuasionTask(0);
            list.Add(persuasionTask);
            persuasionTask.FinalFailLine = new TextObject("{=dY2PzpIV}I'm not sure how much we have in common..");
            persuasionTask.TryLaterLine = new TextObject("{=PoDVgQaz}Well, it would take a bit long to discuss this.");
            persuasionTask.SpokenLine = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_courtship_travel_task", CharacterObject.OneToOneConversationCharacter);
            Tuple<TraitObject, int>[] traitCorrelations = GetTraitCorrelations(1, -1, 0, 0, 1);
            PersuasionOptionArgs option = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations), skill: DefaultSkills.Leadership, trait: DefaultTraits.Valor, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=YNBm3LkC}I feel lucky to live in a time where a valiant warrior can make a name for {?PLAYER.GENDER}herself{?}himself{\\?}."), traitCorrelation: traitCorrelations, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask.AddOptionToTask(option);
            Tuple<TraitObject, int>[] traitCorrelations2 = GetTraitCorrelations(1, -1, 0, 0, 1);
            PersuasionOptionArgs option2 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations2), skill: DefaultSkills.Roguery, trait: DefaultTraits.Valor, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=rtqD9cnu}Yeah, it's a rough world, but there are lots of opportunities to be seized right now if you're not afraid to get your hands a bit dirty."), traitCorrelation: traitCorrelations2, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask.AddOptionToTask(option2);
            Tuple<TraitObject, int>[] traitCorrelations3 = GetTraitCorrelations(0, 1, 1, 0, -1);
            PersuasionOptionArgs option3 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations3), skill: DefaultSkills.Charm, trait: DefaultTraits.Mercy, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=rfyalLyY}What can I say? It's a beautiful world, but filled with so much suffering."), traitCorrelation: traitCorrelations3, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask.AddOptionToTask(option3);
            Tuple<TraitObject, int>[] traitCorrelations4 = GetTraitCorrelations(-1, 0, -1, -1);
            PersuasionOptionArgs option4 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations4), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Negative, givesCriticalSuccess: false, line: new TextObject("{=ja5bAOMr}The world's a dungheap, basically. The sooner I earn enough to retire, the better."), traitCorrelation: traitCorrelations4, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask.AddOptionToTask(option4);
            PersuasionTask persuasionTask2 = new PersuasionTask(1);
            list.Add(persuasionTask2);
            persuasionTask2.SpokenLine = new TextObject("{=5Vk6I1sf}Between your followers, your rivals and your enemies, you must have met a lot of interesting people...");
            persuasionTask2.FinalFailLine = new TextObject("{=lDJUL4lZ}I think we maybe see the world a bit differently.");
            persuasionTask2.TryLaterLine = new TextObject("{=ZmxbIXsp}I am sorry you feel that way. We can speak later.");
            Tuple<TraitObject, int>[] traitCorrelations5 = GetTraitCorrelations(1, 0, 1, 2);
            PersuasionOptionArgs option5 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations5), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=8BnWa83o}I'm just honored to have fought alongside comrades who thought nothing of shedding their blood to keep me alive."), traitCorrelation: traitCorrelations5, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask2.AddOptionToTask(option5);
            Tuple<TraitObject, int>[] traitCorrelations6 = GetTraitCorrelations(0, 0, -1, 0, 1);
            PersuasionOptionArgs option6 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations6), skill: DefaultSkills.Roguery, trait: DefaultTraits.Calculating, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=QHG6LU1g}Ah yes, I've seen cruelty, degradation and degeneracy like you wouldn't believe. Fascinating stuff, all of it."), traitCorrelation: traitCorrelations6, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask2.AddOptionToTask(option6);
            Tuple<TraitObject, int>[] traitCorrelations7 = GetTraitCorrelations(0, 2);
            PersuasionOptionArgs option7 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations7), skill: DefaultSkills.Leadership, trait: DefaultTraits.Mercy, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=bwWdGLDv}I have seen great good and great evil, but I can only hope the good outweights the evil in most people's hearts."), traitCorrelation: traitCorrelations7, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask2.AddOptionToTask(option7);
            Tuple<TraitObject, int>[] traitCorrelations8 = GetTraitCorrelations(-1, 0, -1, -1);
            PersuasionOptionArgs option8 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations8), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Negative, givesCriticalSuccess: false, line: new TextObject("{=3skTM1DC}Most people would put a knife in your back for a few coppers. Have a few friends and keep them close, I guess."), traitCorrelation: traitCorrelations8, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask2.AddOptionToTask(option8);
            PersuasionTask persuasionTask3 = new PersuasionTask(2);
            list.Add(persuasionTask3);
            persuasionTask3.SpokenLine = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_courtship_aspirations_task", CharacterObject.OneToOneConversationCharacter);
            persuasionTask3.ImmediateFailLine = new TextObject("{=8hEVO9hw}Hmm. Perhaps you and I have different priorities in life.");
            persuasionTask3.FinalFailLine = new TextObject("{=HAtHptbV}In the end, I don't think we have that much in common.");
            persuasionTask3.TryLaterLine = new TextObject("{=ZmxbIXsp}I am sorry you feel that way. We can speak later.");
            Tuple<TraitObject, int>[] traitCorrelations9 = GetTraitCorrelations(0, 2, 1);
            PersuasionOptionArgs option9 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations9), skill: DefaultSkills.Leadership, trait: DefaultTraits.Mercy, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=6kjacaiB}I hope I can bring peace to the land, and justice, and alleviate people's suffering."), traitCorrelation: traitCorrelations9, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask3.AddOptionToTask(option9);
            Tuple<TraitObject, int>[] traitCorrelations10 = GetTraitCorrelations(1, 1, 0, 2);
            PersuasionOptionArgs option10 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations10), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=rrqCZa0H}I'll make sure those who stuck their necks out for me, who sweated and bled for me, get their due."), traitCorrelation: traitCorrelations10, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask3.AddOptionToTask(option10);
            Tuple<TraitObject, int>[] traitCorrelations11 = GetTraitCorrelations(0, 0, 0, 0, 2);
            PersuasionOptionArgs option11 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations11), skill: DefaultSkills.Roguery, trait: DefaultTraits.Calculating, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=ggKa4Bd8}Hmm... First thing to do after taking power is to work on your plan to remain in power."), traitCorrelation: traitCorrelations11, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask3.AddOptionToTask(option11);
            Tuple<TraitObject, int>[] traitCorrelations12 = GetTraitCorrelations(0, -2, 0, -1, 1);
            PersuasionOptionArgs option12 = new PersuasionOptionArgs(argumentStrength: Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations12), skill: DefaultSkills.Charm, trait: DefaultTraits.Calculating, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=6L1b1nJa}Oh I have a long list of scores to settle. You can be sure of that."), traitCorrelation: traitCorrelations12, canBlockOtherOption: false, canMoveToTheNextReservation: true);
            persuasionTask3.AddOptionToTask(option12);
            persuasionTask2.FinalFailLine = new TextObject("{=Ns315pxY}Perhaps we are not meant for each other.");
            persuasionTask2.TryLaterLine = new TextObject("{=PoDVgQaz}Well, it would take a bit long to discuss this.");
            return list;
        }

        private Tuple<TraitObject, int>[] GetTraitCorrelations(int valor = 0, int mercy = 0, int honor = 0, int generosity = 0, int calculating = 0)
        {
            return new Tuple<TraitObject, int>[5]
            {
                new Tuple<TraitObject, int>(DefaultTraits.Valor, valor),
                new Tuple<TraitObject, int>(DefaultTraits.Mercy, mercy),
                new Tuple<TraitObject, int>(DefaultTraits.Honor, honor),
                new Tuple<TraitObject, int>(DefaultTraits.Generosity, generosity),
                new Tuple<TraitObject, int>(DefaultTraits.Calculating, calculating)
            };
        }

        private List<PersuasionTask> GetPersuasionTasksForCourtshipStage2(Hero wooed, Hero wooer)
        {
            StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            List<PersuasionTask> list = new List<PersuasionTask>();
            IEnumerable<RomanceReservationDescription> romanceReservations = GetRomanceReservations(wooed, wooer);
            bool flag = romanceReservations.Any((RomanceReservationDescription x) => x == RomanceReservationDescription.AttractionIAmDrawnToYou);
            List<RomanceReservationDescription> list2 = romanceReservations.Where((RomanceReservationDescription x) => x == RomanceReservationDescription.CompatibiliyINeedSomeoneDangerous || x == RomanceReservationDescription.CompatibilityNeedSomethingInCommon || x == RomanceReservationDescription.CompatibilityINeedSomeoneUpright || x == RomanceReservationDescription.CompatibilityStrongPoliticalBeliefs).ToList();
            if (list2.Count > 0)
            {
                RomanceReservationDescription num = list2[0];
                PersuasionTask persuasionTask = new PersuasionTask(3);
                list.Add(persuasionTask);
                persuasionTask.SpokenLine = new TextObject("{=rtP6vnmj}I'm not sure we're compatible.");
                persuasionTask.FinalFailLine = new TextObject("{=bBTHy6f9}I just don't think that we would be happy together.");
                persuasionTask.TryLaterLine = new TextObject("{=o9ouu97M}I will endeavor to be worthy of your affections.");
                PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;
                if (num == RomanceReservationDescription.CompatibiliyINeedSomeoneDangerous)
                {
                    if (Hero.OneToOneConversationHero.IsFemale)
                    {
                        persuasionTask.SpokenLine = new TextObject("{=EkkNQb5N}I like a warrior who strikes fear in the hearts of his enemies. Are you that kind of man?");
                    }
                    else
                    {
                        persuasionTask.SpokenLine = new TextObject("{=3cw5pRFM}I had not thought that I might marry a shieldmaiden. But it is intriguing. Tell me, have you killed men in battle?");
                    }

                    PersuasionOptionArgs option = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=FEmiPPbO}Perhaps you've heard the stories about me, then. They're all true."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option);
                    PersuasionOptionArgs option2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength + 1, givesCriticalSuccess: false, new TextObject("{=Oe5Tf7OZ}My foes may not fear my sword, but they should fear my cunning."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option2);
                    if (flag)
                    {
                        PersuasionOptionArgs option3 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, givesCriticalSuccess: true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                        persuasionTask.AddOptionToTask(option3);
                    }

                    PersuasionOptionArgs option4 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength + 1, givesCriticalSuccess: false, new TextObject("{=8a13MGzr}All I can say is that I try to repay good with good, and evil with evil."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option4);
                }

                if (num == RomanceReservationDescription.CompatibilityINeedSomeoneUpright)
                {
                    persuasionTask.SpokenLine = new TextObject("{=lay7hKUK}I insist that my {?PLAYER.GENDER}wife{?}husband{\\?} conduct {?PLAYER.GENDER}herself{?}himself{\\?} according to the highest standards.");
                    PersuasionOptionArgs option5 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, givesCriticalSuccess: false, new TextObject("{=bOQEc7jA}I am a {?PLAYER.GENDER}woman{?}man{\\?} of my word. I hope that it is sufficient."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option5);
                    PersuasionOptionArgs option6 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, PersuasionArgumentStrength.Normal, givesCriticalSuccess: false, new TextObject("{=faa9sFfE}I do what I can to alleviate suffering in this world. I hope that is enough."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option6);
                    if (flag)
                    {
                        PersuasionOptionArgs option7 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, givesCriticalSuccess: true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                        persuasionTask.AddOptionToTask(option7);
                    }

                    PersuasionOptionArgs option8 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, PersuasionArgumentStrength.Hard, givesCriticalSuccess: false, new TextObject("{=b2ePtImV}Those who are loyal to me, I am loyal to them."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option8);
                }

                if (num == RomanceReservationDescription.CompatibilityNeedSomethingInCommon)
                {
                    persuasionTask.SpokenLine = new TextObject("{=ZsGqHBlR}I need a partner whom I can trust...");
                    PersuasionOptionArgs option9 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength - 1, givesCriticalSuccess: false, new TextObject("{=LTUEFTaF}I hope that I am known as someone who understands the value of loyalty."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option9);
                    PersuasionOptionArgs option10 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=9qoLQva5}Whatever oath I give to you, you may be sure that I will keep it."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option10);
                    if (flag)
                    {
                        PersuasionOptionArgs option11 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, givesCriticalSuccess: true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                        persuasionTask.AddOptionToTask(option11);
                    }

                    PersuasionOptionArgs option12 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=b2ePtImV}Those who are loyal to me, I am loyal to them."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option12);
                }

                if (num == RomanceReservationDescription.CompatibilityStrongPoliticalBeliefs)
                {
                    if (wooed.GetTraitLevel(DefaultTraits.Egalitarian) > 0)
                    {
                        persuasionTask.SpokenLine = new TextObject("{=s3Fna6wY}I've always seen myself as someone who sides with the weak of this realm. I don't want to find myself at odds with you.");
                    }

                    if (wooed.GetTraitLevel(DefaultTraits.Oligarchic) > 0)
                    {
                        persuasionTask.SpokenLine = new TextObject("{=DR2aK4aQ}I respect our ancient laws and traditions. I don't want to find myself at odds with you.");
                    }

                    if (wooed.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
                    {
                        persuasionTask.SpokenLine = new TextObject("{=c2Yrci3B}I believe that we need a strong ruler in this realm. I don't want to find myself at odds with you.");
                    }

                    PersuasionOptionArgs option13 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=pVPkpP20}We may differ on politics, but I hope you'll think me a man with a good heart."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option13);
                    if (flag)
                    {
                        PersuasionOptionArgs option14 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, givesCriticalSuccess: true, new TextObject("{=yghMrFdT}Put petty politics aside and trust your heart!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                        persuasionTask.AddOptionToTask(option14);
                    }

                    PersuasionOptionArgs option15 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Tj8bGW4b}If a man and a woman respect each other, politics should not divide them."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask.AddOptionToTask(option15);
                }
            }

            if (romanceReservations.Where((RomanceReservationDescription x) => x == RomanceReservationDescription.AttractionYoureNotMyType).ToList().Count > 0)
            {
                PersuasionTask persuasionTask2 = new PersuasionTask(4);
                list.Add(persuasionTask2);
                persuasionTask2.SpokenLine = new TextObject("{=cOyolp4F}I am just not... How can I say this? I am not attracted to you.");
                persuasionTask2.FinalFailLine = new TextObject("{=LjiYq9cH}I am sorry. I am not sure that I could ever love you.");
                persuasionTask2.TryLaterLine = new TextObject("{=E9s2bjqw}I can only hope that some day you could change your mind.");
                PersuasionOptionArgs option16 = new PersuasionOptionArgs(argumentStrength: (PersuasionArgumentStrength)(-Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Calculating)), skill: DefaultSkills.Charm, trait: DefaultTraits.Calculating, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=hwjzKcUw}So what? This is supposed to be an alliance of our houses, not of our hearts."), traitCorrelation: null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask2.AddOptionToTask(option16);
                PersuasionOptionArgs option17 = new PersuasionOptionArgs(argumentStrength: (PersuasionArgumentStrength)(-Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) - 1), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Positive, givesCriticalSuccess: true, line: new TextObject("{=m3EkYCA6}Perhaps if you see how much I love you, you could come to love me over time."), traitCorrelation: null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask2.AddOptionToTask(option17);
                PersuasionOptionArgs option18 = new PersuasionOptionArgs(argumentStrength: (PersuasionArgumentStrength)(-Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor)), skill: DefaultSkills.Charm, trait: DefaultTraits.Honor, traitEffect: TraitEffect.Positive, givesCriticalSuccess: false, line: new TextObject("{=LN7SGvnS}Love is but an infatuation. Judge me by my character."), traitCorrelation: null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask2.AddOptionToTask(option18);
            }

            List<RomanceReservationDescription> list3 = romanceReservations.Where((RomanceReservationDescription x) => x == RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress || x == RomanceReservationDescription.PropertyIWantRealWealth || x == RomanceReservationDescription.PropertyWeNeedToBeComfortable).ToList();
            if (list3.Count > 0)
            {
                RomanceReservationDescription romanceReservationDescription = list3[0];
                PersuasionTask persuasionTask3 = new PersuasionTask(6);
                list.Add(persuasionTask3);
                persuasionTask3.SpokenLine = new TextObject("{=beK0AZ2y}I am concerned that you do not have the means to support a family.");
                persuasionTask3.FinalFailLine = new TextObject("{=z6vJlozm}I am sorry. I don't believe you have the means to support a family.)");
                persuasionTask3.TryLaterLine = new TextObject("{=vaISh0sx}I will go off to make something of myself, then, and shall return to you.");
                PersuasionArgumentStrength persuasionArgumentStrength2 = PersuasionArgumentStrength.Normal;
                switch (romanceReservationDescription)
                {
                    case RomanceReservationDescription.PropertyIWantRealWealth:
                        persuasionTask3.SpokenLine = new TextObject("{=pbqjBGk0}I will be honest. I have plans, and I expect the person I marry to have the income to support them.");
                        persuasionArgumentStrength2 = PersuasionArgumentStrength.Hard;
                        break;
                    case RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress:
                        persuasionTask3.SpokenLine = new TextObject("{=ZNfWXliN}I will be honest, my lady. You are but a common adventurer, and by marrying you I give up a chance to forge an alliance with a family of real influence and power.");
                        persuasionArgumentStrength2 = PersuasionArgumentStrength.Normal;
                        break;
                }

                PersuasionOptionArgs option19 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength2, givesCriticalSuccess: false, new TextObject("{=erKuPRWA}I have a plan to rise in this world. I'm still only a little way up the ladder."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask3.AddOptionToTask(option19);
                PersuasionOptionArgs option20 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength2, givesCriticalSuccess: false, (romanceReservationDescription == RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress) ? new TextObject("{=a2dJDUoL}My sword is my dowry. The gold and land will follow.") : new TextObject("{=DLc6NfiV}I shall win you the riches you deserve, or die in the attempt."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask3.AddOptionToTask(option20);
                if (flag)
                {
                    PersuasionOptionArgs option21 = new PersuasionOptionArgs(argumentStrength: persuasionArgumentStrength2 - Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Calculating), skill: DefaultSkills.Charm, trait: DefaultTraits.Generosity, traitEffect: TraitEffect.Positive, givesCriticalSuccess: true, line: new TextObject("{=6LfkfJiJ}Can't your passion for me overcome such base feelings?"), traitCorrelation: null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask3.AddOptionToTask(option21);
                }
            }

            List<RomanceReservationDescription> list4 = romanceReservations.Where((RomanceReservationDescription x) => x == RomanceReservationDescription.FamilyApprovalHowCanYouBeEnemiesWithOurFamily || x == RomanceReservationDescription.FamilyApprovalItWouldBeBestToBefriendOurFamily || x == RomanceReservationDescription.FamilyApprovalYouNeedToBeFriendsWithOurFamily).ToList();
            if (list4.Count > 0 && list.Count < 3)
            {
                _ = list4[0];
                PersuasionTask persuasionTask4 = new PersuasionTask(5);
                list.Add(persuasionTask4);
                persuasionTask4.SpokenLine = new TextObject("{=fAdwIqbg}I think you should try to win my family's approval.");
                persuasionTask4.FinalFailLine = new TextObject("{=Xa7PsIao}I am sorry. I will not marry without my family's blessing.");
                persuasionTask4.TryLaterLine = new TextObject("{=44tA6fNa}I will try to earn your family's trust, then.");
                PersuasionArgumentStrength argumentStrength5 = PersuasionArgumentStrength.Normal;
                PersuasionOptionArgs option22 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, argumentStrength5, givesCriticalSuccess: false, new TextObject("{=563qB3ar}I can only hope that if they come to know my loyalty, they will accept me."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask4.AddOptionToTask(option22);
                if (flag)
                {
                    PersuasionOptionArgs option23 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Valor, TraitEffect.Positive, argumentStrength5, givesCriticalSuccess: true, new TextObject("{=LEsuGM8a}Let no one - not even your family - come between us!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                    persuasionTask4.AddOptionToTask(option23);
                }

                PersuasionOptionArgs option24 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, argumentStrength5, givesCriticalSuccess: false, new TextObject("{=ZbvbsA4i}I can only hope that if they come to know my virtues, they will accept me."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask4.AddOptionToTask(option24);
            }
            else if (list4.Count == 0 && list.Count < 3)
            {
                PersuasionTask persuasionTask5 = new PersuasionTask(7);
                list.Add(persuasionTask5);
                persuasionTask5.SpokenLine = new TextObject("{=HFkXIyCV}My family likes you...");
                persuasionTask5.FinalFailLine = new TextObject("{=3IBVEOwh}I still think we may not be ready yet.");
                persuasionTask5.TryLaterLine = new TextObject("{=44tA6fNa}I will try to earn your family's trust, then.");
                PersuasionArgumentStrength argumentStrength6 = PersuasionArgumentStrength.ExtremelyEasy;
                PersuasionOptionArgs option25 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, argumentStrength6, givesCriticalSuccess: false, new TextObject("{=2LrFafpB}And I will respect and cherish your family."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask5.AddOptionToTask(option25);
                PersuasionOptionArgs option26 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, argumentStrength6, givesCriticalSuccess: false, new TextObject("{=BaifRgT5}That's useful to know for when it comes time to discuss the exchange of dowries."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true);
                persuasionTask5.AddOptionToTask(option26);
            }

            return list;
        }
    }
    */
}
