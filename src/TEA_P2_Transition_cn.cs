using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons;
using ECommons.DalamudServices;
using ECommons.Hooks;
using ECommons.Hooks.ActionEffectTypes;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using Splatoon;
using Splatoon.Memory;
using Splatoon.SplatoonScripting;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonScriptsOfficial.Duties.Shadowbringers
{
    public class TEA_P2_Transition : SplatoonScript
    {
        public override HashSet<uint> ValidTerritories => new() { 887 };
        public override Metadata? Metadata => new(1, "Madou Shoujo-ErrerCNserver-redrawtest");
        private string ElementNamePrefix = "TEA_P2_Transition_Bait_Position";
        // ActionEffectId of the exaflare.
        private uint HawkBlast = 18480;
        // Circular ordered list of center of outside flares.
        // Note the vector3 element order is: (X, Z, Y) because that's how it is stored from ActionEffectSet.
        private List<Vector3> FlareList = new List<Vector3> {
            new Vector3(90, 0, 90),
            new Vector3(100, 0, 86),
            new Vector3(110, 0, 90),
            new Vector3(114, 0, 100),
            new Vector3(110, 0, 110),
            new Vector3(100, 0, 114),
            new Vector3(90, 0, 110),
            new Vector3(86, 0, 100),
        };

        private int LCNumber = 0;
        private bool MechanicActive = false;
        private uint BlastCount = 0;
        private Element? Flare_a;
        private Element? Flare_b;
        private Element? Flare_m0;
        private Element? Flare_m;
        private Element? Indicator_a;
        private Element? Indicator_b;
        // add outer ring and change colors
        private Element? Flare_a1;
        private Element? Flare_b1;
        private Element? Flare_m1;
        private Element? Indicator_a1;
        private Element? Indicator_b1;

        public override void OnSetup()
        {
            Element fa = new Element(0);
            fa.Enabled = false;
            fa.radius = 10f;
            fa.Filled = false;
            fa.color = 4278255421;
            fa.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_a", fa, true);
            Flare_a = Controller.GetElementByName(ElementNamePrefix + "Flare_a"); 
            
            // add fa1
            Element fa1 = new Element(0);
            fa1.Enabled = false;
            fa1.radius = 10f;
            fa1.Filled = true;
            fa1.color = 1258356541;
            fa1.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_a1", fa1, true);
            Flare_a1 = Controller.GetElementByName(ElementNamePrefix + "Flare_a1"); 

            Element fb = new Element(0);
            fb.Enabled = false;
            fb.radius = 10f;
            fb.Filled = false;
            fb.color = 4278255421;
            fb.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_b", fb, true);
            Flare_b = Controller.GetElementByName(ElementNamePrefix + "Flare_b"); 
            
            // add fb1
            Element fb1 = new Element(0);
            fb1.Enabled = false;
            fb1.radius = 10f;
            fb1.Filled = true;
            fb1.color = 1258356541;
            fb1.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_b1", fb1, true);
            Flare_b1 = Controller.GetElementByName(ElementNamePrefix + "Flare_b1"); 

            // add fm0
            Element fm0 = new Element(0);
            fm0.Enabled = false;
            SetElementPosition(fm0, 100, 100, 0);
            fm0.radius = 10f;
            fm0.Filled = false;
            fm0.color = 4294967295;
            fm0.thicc = 6f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_m0", fm0, true);
            Flare_m0 = Controller.GetElementByName(ElementNamePrefix + "Flare_m0");

            Element fm = new Element(0);
            fm.Enabled = false;
            SetElementPosition(fm, 100, 100, 0);
            fm.radius = 10f;
            fm.Filled = false;
            fm.color = 4294904576;
            fm.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_m", fm, true);
            Flare_m = Controller.GetElementByName(ElementNamePrefix + "Flare_m");
            
            // add fm1
            Element fm1 = new Element(0);
            fm1.Enabled = false;
            SetElementPosition(fm1, 100, 100, 0);
            fm1.radius = 10f;
            fm1.Filled = true;
            fm1.color = 1258356541;
            fm1.thicc = 3f;
            Controller.RegisterElement(ElementNamePrefix + "Flare_m1", fm1, true);
            Flare_m1 = Controller.GetElementByName(ElementNamePrefix + "Flare_m1");

            Element a = new Element(0);
            a.Enabled = false;
            a.radius = 0.5f;
            a.overlayFScale = 1f;
            a.color = 4294911232;
            a.thicc = 6f;
            a.overlayBGColor = 4278190080;
            a.overlayTextColor = 4294967295;
            Controller.RegisterElement(ElementNamePrefix + "Indicator_a", a, true);
            Indicator_a = Controller.GetElementByName(ElementNamePrefix + "Indicator_a"); 

            // add Indicator_a1
            Element a1 = new Element(0);
            a1.Enabled = false;
            a1.radius = 0.5f;
            a1.overlayFScale = 1f;
            a1.color = 4294967295;
            a1.thicc = 3f;
            a1.overlayBGColor = 4278190080;
            a1.overlayTextColor = 4294967295;
            Controller.RegisterElement(ElementNamePrefix + "Indicator_a1", a1, true);
            Indicator_a1 = Controller.GetElementByName(ElementNamePrefix + "Indicator_a1"); 

            Element b = new Element(0);
            b.Enabled = false;
            b.radius = 0.5f;
            b.overlayFScale = 1f;
            b.color = 4294911232;
            b.thicc = 6f;
            b.overlayBGColor = 4278190080;
            b.overlayTextColor = 4294967295;
            Controller.RegisterElement(ElementNamePrefix + "Indicator_b", b, true);
            Indicator_b = Controller.GetElementByName(ElementNamePrefix + "Indicator_b"); 

            // add Indicator_b1
            Element b1 = new Element(0);
            b1.Enabled = false;
            b1.radius = 0.5f;
            b1.overlayFScale = 1f;
            b1.color = 4294967295;
            b1.thicc = 3f;
            b1.overlayBGColor = 4278190080;
            b1.overlayTextColor = 4294967295;
            Controller.RegisterElement(ElementNamePrefix + "Indicator_b1", b1, true);
            Indicator_b1 = Controller.GetElementByName(ElementNamePrefix + "Indicator_b1"); 
        }

        private void Reset()
        {
            MechanicActive = false;
            BlastCount = 0;
            Flare_a.Enabled = false;
            Flare_b.Enabled = false;
            Flare_m0.Enabled = false;
            Flare_m.Enabled = false;
            Indicator_a.Enabled = false;
            Indicator_b.Enabled = false;
            // add more
            Flare_a1.Enabled = false;
            Flare_b1.Enabled = false;
            Flare_m1.Enabled = false;
            Indicator_a1.Enabled = false;
            Indicator_b1.Enabled = false;
        }

        public override void OnEnable()
        {
            ActionEffect.ActionEffectEvent += ActionEffect_ActionEffectEvent;
        }

        public override void OnDisable()
        {
            ActionEffect.ActionEffectEvent -= ActionEffect_ActionEffectEvent;
        }

        public override void OnMessage(string Message)
        {
            if (Message == "代号摧毁者！ 发现威胁差分闭合宇宙的敌人……开始歼灭！")
            {
                MechanicActive = true;
            }
        }

        public override void OnUpdate()
        {
            if (MechanicActive && BlastCount >= 18)
                Reset();
        }

        public override void OnDirectorUpdate(DirectorUpdateCategory category)
        {
            if (category.EqualsAny(DirectorUpdateCategory.Wipe, DirectorUpdateCategory.Recommence))
                Reset();
        }

        public override void OnVFXSpawn(uint target, string vfxPath)
        {
            if (vfxPath.StartsWith("vfx/lockon/eff/m0361trg_a"))
                LCNumber = GetMyNumber();
        }

        private Vector3 GetNextFlare(Vector3 current)
        {
            for (var i = 0; i < FlareList.Count; i++)
            {
                if (Vector3.Distance(current, FlareList[i]) < 5)
                    return FlareList[(i+1) % FlareList.Count];
            }
            return new Vector3(100, 0, 100);
        }

        private void SetElementPosition(Element e, float x, float y, float z)
        {
            e.refX = x;
            e.refY = y;
            e.refZ = z;
        }

        private int GetMyNumber()
        {
            if (AttachedInfo.VFXInfos.TryGetValue(Svc.ClientState.LocalPlayer.Address, out var info))
            {
                if (info.OrderBy(x => x.Value.Age).TryGetFirst(x => x.Key.StartsWith("vfx/lockon/eff/m0361trg_a"), out var effect))
                {
                    return int.Parse(effect.Key.Replace("vfx/lockon/eff/m0361trg_a", "")[0].ToString());
                }
            }
            return 0;
        }

        private void MarkBaitTether(Element e, int n)
        {
            if (LCNumber == n)
                e.tether = true;
            else
                e.tether = false;
            e.overlayText = n.ToString();
        }

        private void ActionEffect_ActionEffectEvent(ActionEffectSet set)
        {
            if (!MechanicActive)
                return;

            if (set.Action.RowId == HawkBlast)
            {
                BlastCount++;

                // Show next flare.
                if (BlastCount == 1 || BlastCount == 3 || BlastCount == 5 || BlastCount == 10 || BlastCount == 12 || BlastCount == 14)
                {
                    Vector3 nextFlare = GetNextFlare(set.Position);
                    SetElementPosition(Flare_a, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_a.Enabled = true;
                    // add more
                    SetElementPosition(Flare_a1, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_a1.Enabled = true;
                }
                if (BlastCount == 2 || BlastCount == 4 || BlastCount == 6 || BlastCount == 11 || BlastCount == 13 || BlastCount == 15)
                {
                    Vector3 nextFlare = GetNextFlare(set.Position);
                    SetElementPosition(Flare_b, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_b.Enabled = true;
                    // add more
                    SetElementPosition(Flare_b1, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_b1.Enabled = true;
                }
                if (BlastCount == 7 || BlastCount == 16)
                {
                    Vector3 nextFlare = GetNextFlare(set.Position);
                    SetElementPosition(Flare_a, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_a.Enabled = false;
                    // add more
                    SetElementPosition(Flare_a1, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_a1.Enabled = false;
                }
                if (BlastCount == 8 || BlastCount == 17)
                {
                    Vector3 nextFlare = GetNextFlare(set.Position);
                    SetElementPosition(Flare_b, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_b.Enabled = false;
                    //add more
                    SetElementPosition(Flare_b1, nextFlare.X, nextFlare.Z, nextFlare.Y);
                    Flare_b1.Enabled = false;
                    Flare_m0.Enabled = true;
                    Flare_m.Enabled = true;
                    Flare_m1.Enabled = true;
                }
                if (BlastCount == 9)
                {
                    Flare_m0.Enabled = false;
                    Flare_m.Enabled = false;
                    Flare_m1.Enabled = false;
                    Flare_a.Enabled = true;
                    Flare_b.Enabled = true;
                    // add more
                    Flare_a1.Enabled = true;
                    Flare_b1.Enabled = true;
                }

                // 1 bait position
                if (BlastCount == 5)
                {
                    SetElementPosition(Indicator_a, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a, 1);
                    Indicator_a.Enabled = true;
                    // add more
                    SetElementPosition(Indicator_a1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a1, 1);
                    Indicator_a1.Enabled = true;
                }
                if (BlastCount == 6)
                {            
                    SetElementPosition(Indicator_b, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b, 1);
                    Indicator_b.Enabled = true;
                    // add more
                    SetElementPosition(Indicator_b1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b1, 1);
                    Indicator_b1.Enabled = true;
                }
                // 3 bait position
                if (BlastCount == 7)
                {
                    SetElementPosition(Indicator_a, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a, 3);
                    // add more
                    SetElementPosition(Indicator_a1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a1, 3);
                }
                if (BlastCount == 8)
                {
                    SetElementPosition(Indicator_b, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b, 3);
                    // add more
                    SetElementPosition(Indicator_b1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b1, 3);
                }
                // 5 bait position
                if (BlastCount == 12)
                {
                    SetElementPosition(Indicator_a, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a, 5);
                    // add more
                    SetElementPosition(Indicator_a1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a1, 5);
                }
                if (BlastCount == 13)
                {
                    SetElementPosition(Indicator_b, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b, 5);
                    // add more
                    SetElementPosition(Indicator_b1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b1, 5);
                }
                // 7 bait position
                if (BlastCount == 16)
                {
                    SetElementPosition(Indicator_a, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a, 7);
                    // add more
                    SetElementPosition(Indicator_a1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_a1, 7);
                }
                if (BlastCount == 17)
                {
                    SetElementPosition(Indicator_b, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b, 7);
                    // add more
                    SetElementPosition(Indicator_b1, set.Position.X, set.Position.Z, set.Position.Y);
                    MarkBaitTether(Indicator_b1, 7);
                }
            }
        }
    }
}