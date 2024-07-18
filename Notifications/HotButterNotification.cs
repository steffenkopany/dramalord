using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Notifications
{
    internal class HotButterNotification : SceneNotificationData
    {
        private static readonly List<string> _cultureNames = new(){ "vlandia", "battania", "sturgia", "aserai", "khuzait", "empire" };
        private static readonly List<string> _variations = new() { "01", "02", "03" };

        private readonly string _sceneName;

        private readonly Hero _male;
        private readonly Hero _female;
        private readonly Hero? _spectator;

        internal HotButterNotification(Hero hero1, Hero hero2, Settlement? location, Hero? spectator)
        {
            _male = hero1.IsFemale ? hero2 : hero1;
            _female = hero1.IsFemale ? hero1 : hero2;
            _spectator = spectator;

            string scene = "scn_pompa_";
            if(location != null)
            {
                string culture = "empire";
                string locationType = "village";

                _cultureNames.ForEach(name =>
                {
                    if (name == location.Culture.StringId) culture = name;
                });
                if (location.IsCastle) locationType = "castle";
                else if (location.IsTown) locationType = "town";

                scene += culture + "_" + locationType + "_" + _variations.GetRandomElement();
            }
            else
            {
                scene = "scn_carsi_izni";
            }

            _sceneName = scene;
        }

        public override string SceneID => _sceneName;

        public override TextObject TitleText => new TextObject();

        public override IEnumerable<SceneNotificationCharacter> GetSceneNotificationCharacters()
        {
            List<SceneNotificationCharacter> notificationCharacters = new();
            if(_sceneName == "scn_carsi_izni")
            {
                notificationCharacters.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(_female, new Equipment(), false, _female.BodyProperties, uint.MaxValue, uint.MaxValue, false));
                notificationCharacters.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(_male, new Equipment(), false, _male.BodyProperties, uint.MaxValue, uint.MaxValue, false));
            }
            else
            {
                notificationCharacters.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(_male, new Equipment(), false, _male.BodyProperties, uint.MaxValue, uint.MaxValue, false));
                notificationCharacters.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(_female, new Equipment(), false, _female.BodyProperties, uint.MaxValue, uint.MaxValue, false));
            }
            
            if(_spectator != null)
            {
                notificationCharacters.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(_spectator, _spectator.CivilianEquipment, true, _spectator.BodyProperties, uint.MaxValue, uint.MaxValue, false));
            }

            return notificationCharacters;
        }
    }
}
