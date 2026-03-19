using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Notifications
{
    internal class HotScenesNotificationData : SceneNotificationData
    {
        private static MethodInfo? _getSceneMethod = null;
        private static MethodInfo? _getNotificationCharacter = null;

        internal static bool Initialize()
        {
            try
            {
                Assembly? hotScenesAssembly = null;

                // Find assembly
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "HotScenes")
                    {
                        hotScenesAssembly = assembly;
                        break;
                    }
                }

                if (hotScenesAssembly == null)
                    return false;

                // Get SceneData type
                Type sceneDataType = hotScenesAssembly.GetType("HotScenes.SceneData");
                if (sceneDataType == null)
                    return false;

                // Cache method references
                _getSceneMethod = sceneDataType.GetMethod("GetScenes",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new Type[] { typeof(string[]) },
                    null);

                _getNotificationCharacter = sceneDataType.GetMethod("GenerateSceneCharacter",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new Type[] { typeof(Hero) },
                    null);

                return _getSceneMethod != null && _getNotificationCharacter != null;
            }
            catch
            {
                return false;
            }
        }

        private readonly string _sceneName;

        private readonly Hero _male;
        private readonly Hero _female;
        private readonly Hero _third;

        private static string[] hetero = new string[] { "blowjob", "cowgirl", "doggy", "licking", "lifted", "missionary", "prone" };
        private static string[] gay = new string[] { "blowjob", "doggy", "prone" };
        private static string[] lesbian = new string[] { "licking", "scissors" };

        internal HotScenesNotificationData(Hero hero1, Hero hero2, Hero hero3 = null)
        {
            List<string> hints = new();

            if(hero1.IsPrisoner || hero2.IsPrisoner)
            {
                hints.Add("prison");
            }
            else if (hero1.CurrentSettlement != null)
            {
                if(hero1.CurrentSettlement.IsVillage)
                {
                    hints.Add("village");
                }
                else
                {
                    hints.Add(hero1.CurrentSettlement.IsTown ? "town" : "castle");
                }
            }
            else if(hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea)
            {
                hints.Add("ship");
            }
            else
            {
                hints.Add("tent");
            }

            if(hero1.IsFemale == hero2.IsFemale)
            {
                if(hero3 == null || hero3.IsFemale == hero1.IsFemale)
                {
                    hints.Add(hero1.IsFemale ? lesbian.GetRandomElement() : gay.GetRandomElement());
                }
                else
                {
                    hints.Add(hetero.GetRandomElement());
                }
            }
            else if(hero1.IsFemale != hero2.IsFemale)
            {
                hints.Add(hetero.GetRandomElement());
            }

            try
            {
                string[] result = _getSceneMethod.Invoke(null, new object[] { hints.ToArray() }) as string[];
                if(result != null && result.Length > 0)
                {
                    _sceneName = result.GetRandomElement();
                }
                else
                {
                    _sceneName = "scn_hotscenes_town_sex_bed_missionary";
                }
                
            }
            catch (Exception ex)
            {
                _sceneName = "scn_hotscenes_town_sex_bed_missionary";
            }

            //_sceneName = "scn_v2_test";

            if (hero3 != null)
            {
                _female = hero1.IsFemale ? hero1 : hero2.IsFemale ? hero2 : hero3;
                _male = !hero1.IsFemale ? hero1 : !hero2.IsFemale ? hero2 : hero3;
                _third = hero1 != _female && hero1 != _male ? hero1 : hero2 != _female && hero2 != _male ? hero2 : hero3;
            }
            else
            {
                _female = hero1.IsFemale ? hero1 : hero2;
                _male = hero1.IsFemale ? hero2 : hero1;
            }
        }

        public override string SceneID => _sceneName;

        public override TextObject TitleText => TextObject.GetEmpty();

        public override SceneNotificationCharacter[] GetSceneNotificationCharacters()
        {
            List<SceneNotificationCharacter> notificationCharacters = new();
            notificationCharacters.Add((SceneNotificationCharacter)_getNotificationCharacter.Invoke(null, new object[] { _male }));
            notificationCharacters.Add((SceneNotificationCharacter)_getNotificationCharacter.Invoke(null, new object[] { _female }));

            if(_third != null)
            {
                notificationCharacters.Add((SceneNotificationCharacter)_getNotificationCharacter.Invoke(null, new object[] { _third }));
            }

            return notificationCharacters.ToArray();
        }
    }
}
