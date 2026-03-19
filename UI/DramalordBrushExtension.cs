using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace Dramalord.UI
{
    namespace Dramalord.UI
    {
        public static class DramalordBrushExtension
        {
            private static bool _initialized = false;

            public static void ExtendNotificationBrush()
            {
                if (_initialized) return;

                var brush = UIResourceManager.BrushFactory.GetBrush("Map.Notification.Type.Circle.Image");
                if (brush == null) return;

                // Load sprite category
                if (!UIResourceManager.SpriteData.SpriteCategories.TryGetValue("ui_dramalord", out var spriteCategory))
                    return;

                spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.ResourceDepot);

                // Add layers using SpriteData.GetSprite()
                AddLayer(brush, "icon_birth", "ui_dramalord_1");
                AddLayer(brush, "icon_sex", "ui_dramalord_2");
                AddLayer(brush, "icon_wedding", "ui_dramalord_3");
                AddLayer(brush, "icon_confrontquest", "ui_dramalord_4");
                AddLayer(brush, "icon_prisonsex", "ui_dramalord_5");
                AddLayer(brush, "icon_divorce", "ui_dramalord_6");
                AddLayer(brush, "icon_lover", "ui_dramalord_7");
                AddLayer(brush, "icon_visitquest", "ui_dramalord_8");
                AddLayer(brush, "icon_weddingquest", "ui_dramalord_9");
                AddLayer(brush, "icon_divorcequest", "ui_dramalord_10");
                AddLayer(brush, "icon_blackmailquest", "ui_dramalord_10");

                // Add styles that show specific layers
                AddStyle(brush, "icon_birth");
                AddStyle(brush, "icon_sex");
                AddStyle(brush, "icon_wedding");
                AddStyle(brush, "icon_confrontquest");
                AddStyle(brush, "icon_prisonsex");
                AddStyle(brush, "icon_divorce");
                AddStyle(brush, "icon_lover");
                AddStyle(brush, "icon_visitquest");
                AddStyle(brush, "icon_weddingquest");
                AddStyle(brush, "icon_divorcequest");
                AddStyle(brush, "icon_blackmailquest");

                _initialized = true;
            }

            private static void AddLayer(Brush brush, string layerName, string spriteName)
            {
                // Check if layer already exists
                if (brush.GetLayer(layerName) != null) return;

                // Get sprite from SpriteData
                var sprite = UIResourceManager.SpriteData.GetSprite(spriteName);
                if (sprite == null) return;

                var layer = new BrushLayer
                {
                    Name = layerName,
                    Sprite = sprite,
                    Color = Color.White,
                    IsHidden = true
                };

                brush.AddLayer(layer);
            }

            private static void AddStyle(Brush brush, string styleName)
            {
                // Check if style already exists
                if (brush.GetStyle(styleName) != null) return;

                // Create new style with all existing layers
                var style = new Style(brush.Layers.ToList());
                style.Name = styleName;
                style.DefaultStyle = brush.DefaultStyle;

                // Set the matching layer to visible
                var styleLayer = style.GetLayer(styleName);
                if (styleLayer != null)
                {
                    styleLayer.IsHidden = false;
                }

                brush.AddStyle(style);
            }
        }
    }
}
