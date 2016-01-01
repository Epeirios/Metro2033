using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommunityCoreLibrary;
using Verse;
using UnityEngine;
using Verse.AI;
using Verse.Sound;

namespace Metro2033
{
    class Comp_Metro_Weapons_Guns : ThingComp
    {
        private class GizmoAmmoStatus : Command
        {
            public Comp_Metro_Weapons_Guns compAmmo;

            private static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

            private static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

            public override float Width
            {
                get
                {
                    return 120f;
                }
            }

            public override GizmoResult GizmoOnGUI(Vector2 topLeft)
            {
                Rect rect = new Rect(topLeft.x, topLeft.y, this.Width, 75f);
                Widgets.DrawBox(rect, 1);
                GUI.DrawTexture(rect, this.BGTex);
                Rect rect2 = rect.ContractedBy(6f);
                Rect rect3 = rect2;
                rect3.height = rect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect3, this.compAmmo.parent.def.LabelCap);
                Rect rect4 = rect2;
                rect4.yMin = rect.y + rect.height / 2f;
                float fillPercent = (float)this.compAmmo.count / (float)this.compAmmo.reloaderProp.magRounds;
                Widgets.FillableBar(rect4, fillPercent, Comp_Metro_Weapons_Guns.GizmoAmmoStatus.FullTex, Comp_Metro_Weapons_Guns.GizmoAmmoStatus.EmptyTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, this.compAmmo.count + " / " + this.compAmmo.reloaderProp.magRounds);
                Text.Anchor = TextAnchor.UpperLeft;
                return new GizmoResult(GizmoState.Clear);
            }
        }
        public int count;

        public CompProperties_Metro_Weapons_Guns reloaderProp;

    }
}
