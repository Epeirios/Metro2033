using CommunityCoreLibrary;
using Metro2033;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Combat_Realism
{
    public class Comp_Metro_Reload : CompRangedGizmoGiver
    {
        private class GizmoAmmoStatus : Command
        {
            public Comp_Metro_Reload compAmmo;

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
                Widgets.FillableBar(rect4, fillPercent, Comp_Metro_Reload.GizmoAmmoStatus.FullTex, Comp_Metro_Reload.GizmoAmmoStatus.EmptyTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, this.compAmmo.count + " / " + this.compAmmo.reloaderProp.magRounds);
                Text.Anchor = TextAnchor.UpperLeft;
                return new GizmoResult(GizmoState.Clear);
            }
        }

        public int count;

        public bool needReload;

        public CompProperties_Metro_Weapons_Guns reloaderProp;

        public CompEquippable CompEquippable
        {
            get
            {
                return this.parent.GetComp<CompEquippable>();
            }
        }

        public Pawn Wielder
        {
            get
            {
                return this.CompEquippable.PrimaryVerb.CasterPawn;
            }
        }

        public override void Initialize(CompProperties vprops)
        {
            base.Initialize(vprops);
            this.reloaderProp = (vprops as CompProperties_Metro_Weapons_Guns);
            if (this.reloaderProp != null)
            {
                this.count = this.reloaderProp.magRounds;
            }
            else
            {
                Log.Warning("Could not find a CompProperties_Reloader for CompReloader.");
                this.count = 9876;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.LookValue<int>(ref this.count, "count", 1, false);
        }

        public void StartReload()
        {
            this.count = 0;
            this.needReload = true;
            if (this.Wielder == null)
            {
                Log.ErrorOnce("Wielder of " + this.parent + " is null!", 7381889);
                this.FinishReload();
            }
            else
            {

                MoteThrower.ThrowText(this.Wielder.Position.ToVector3Shifted(), "CR_ReloadingMote".Translate(), -1);

                Job newJob = new Job(DefDatabase<JobDef>.GetNamed("ReloadWeapon", true), this.Wielder, this.parent)
                {
                    playerForced = true
                };
                if (this.Wielder.drafter != null)
                {
                    this.Wielder.drafter.TakeOrderedJob(newJob);
                }
                else
                {
                    ExternalPawnDrafter.TakeOrderedJob(this.Wielder, newJob);
                }
            }
        }

        public void FinishReload()
        {
            this.parent.def.soundInteract.PlayOneShot(SoundInfo.InWorld(this.Wielder.Position, MaintenanceType.None));

            MoteThrower.ThrowText(this.Wielder.Position.ToVector3Shifted(), "CR_ReloadedMote".Translate(), -1);

            this.count = this.reloaderProp.magRounds;
            this.needReload = false;
        }

        public override IEnumerable<Command> CompGetGizmosExtra()
        {
            CompReloader.GizmoAmmoStatus gizmoAmmoStatus = new CompReloader.GizmoAmmoStatus
            {
                compAmmo = this
            };
            yield return gizmoAmmoStatus;
            if (this.Wielder != null)
            {
                Command_Action command_Action = new Command_Action
                {
                    action = new Action(this.StartReload),
                    defaultLabel = "CR_ReloadLabel".Translate(),
                    defaultDesc = "CR_ReloadDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/Reload", true)
                };
                yield return command_Action;
            }
            yield break;
        }
    }
}
