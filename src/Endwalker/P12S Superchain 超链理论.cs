using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Lumina.Data.Parsing.Tex.Buffers;
using Splatoon.SplatoonScripting;
using Splatoon.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SplatoonScriptsOfficial.Duties.Endwalker
{
    public class P12S_Superchain_超链理论 : SplatoonScript
    {
        public override HashSet<uint> ValidTerritories => new() { 1154 };
        public override Metadata? Metadata => new(7, "NightmareXIV, RedAsteroid 修改+调色");

        enum Spheres : uint 
        {
            Mastersphere = 16176,
            AOEBall = 16177,
            Protean = 16179,
            Donut = 16178,
            Pairs = 16180,
        }

        const uint AOEDebuff = 3578;

        public override void OnSetup()
        {
            var donut = "{\"Name\":\"Donut\",\"type\":1,\"radius\":6.0,\"Donut\":30.0,\"thicc\":4.0,\"refActorObjectID\":0,\"FillStep\":0.2,\"refActorComparisonType\":2}";
            var donut1 = "{\"Name\":\"Donut1\",\"type\":1,\"radius\":6.0,\"Donut\":30.0,\"color\":4294967295,\"thicc\":5.0,\"refActorObjectID\":0,\"FillStep\":999.0,\"refActorComparisonType\":2}";
            var donut2 = "{\"Name\":\"Donut2\",\"type\":1,\"radius\":6.0,\"Donut\":30.0,\"color\":4294906112,\"thicc\":2.0,\"refActorObjectID\":0,\"FillStep\":999.0,\"refActorComparisonType\":2}";

            Controller.RegisterElementFromCode("Donut1", donut);
            Controller.RegisterElementFromCode("Donut11", donut1);
            Controller.RegisterElementFromCode("Donut12", donut2);
            Controller.RegisterElementFromCode("Donut2", donut);
            Controller.RegisterElementFromCode("Donut21", donut1);
            Controller.RegisterElementFromCode("Donut22", donut2);


            var AOE = "{\"Name\":\"AOE\",\"type\":1,\"radius\":7.0,\"color\":1275012352,\"thicc\":3.0,\"refActorObjectID\":0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"Filled\":true}";
            var AOE1 = "{\"Name\":\"AOE1\",\"type\":1,\"radius\":7.0,\"color\":4294967295,\"thicc\":5.0,\"refActorObjectID\":0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"Filled\":false}";
            var AOE2 = "{\"Name\":\"AOE2\",\"type\":1,\"radius\":7.0,\"color\":4294901769,\"thicc\":2.0,\"refActorObjectID\":0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"Filled\":false}";
            Controller.RegisterElementFromCode("AOEBall1", AOE);
            Controller.RegisterElementFromCode("AOEBall11", AOE1);
            Controller.RegisterElementFromCode("AOEBall12", AOE2);
            Controller.RegisterElementFromCode("AOEBall2", AOE);
            Controller.RegisterElementFromCode("AOEBall21", AOE1);
            Controller.RegisterElementFromCode("AOEBall22", AOE2);

            Controller.TryRegisterLayoutFromCode("Protean", "~Lv2~{\"Name\":\"P12S Protean\",\"Group\":\"P12S\",\"ZoneLockH\":[1154],\"ElementsL\":[{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":0.7853982,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":1.5707964,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":2.3561945,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":3.1415927,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":3.7524579,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":4.7996554,\"Filled\":true},{\"Name\":\"Protean\",\"type\":3,\"refY\":7.16,\"radius\":0.0,\"color\":1275002882,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":5.497787,\"Filled\":true},{\"Name\":\"Hint\",\"type\":1,\"offX\":-0.5,\"radius\":0.0,\"color\":16711931,\"overlayBGColor\":2936012800,\"overlayTextColor\":4294967295,\"overlayVOffset\":0.8,\"overlayFScale\":2.0,\"thicc\":0.0,\"overlayText\":\"8人分散\",\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true}]}", out _);

            Controller.TryRegisterLayoutFromCode("Pairs", "~Lv2~{\"Name\":\"P12S Pairs\",\"Group\":\"P12S\",\"ZoneLockH\":[1154],\"ElementsL\":[{\"Name\":\"Pair\",\"type\":3,\"refX\":-0.5,\"refY\":7.16,\"offX\":-0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":0.5,\"refY\":7.16,\"offX\":0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":-0.5,\"refY\":7.16,\"offX\":-0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":1.5707964,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":0.5,\"refY\":7.16,\"offX\":0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":1.5707964,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":0.5,\"refY\":7.16,\"offX\":0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":3.1415927,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":-0.5,\"refY\":7.16,\"offX\":-0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":3.1415927,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":0.5,\"refY\":7.16,\"offX\":0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":4.712389,\"Filled\":true},{\"Name\":\"Pair\",\"type\":3,\"refX\":-0.5,\"refY\":7.16,\"offX\":-0.5,\"radius\":0.0,\"color\":2113863931,\"thicc\":7.0,\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true,\"AdditionalRotation\":4.712389,\"Filled\":true},{\"Name\":\"Hint\",\"type\":1,\"offX\":-0.5,\"radius\":0.0,\"color\":16711931,\"overlayBGColor\":2936012800,\"overlayTextColor\":4278255571,\"overlayVOffset\":0.8,\"overlayFScale\":2.0,\"thicc\":0.0,\"overlayText\":\"2人分摊\",\"FillStep\":0.25,\"refActorComparisonType\":2,\"includeRotation\":true}]}", out _);

            Controller.TryRegisterLayoutFromCode("DebuffAOESelf", "~Lv2~{\"Enabled\":false,\"Name\":\"P12S Spread AOE\",\"Group\":\"P12S\",\"ZoneLockH\":[1154],\"ElementsL\":[{\"Name\":\"self1\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"refActorType\":1,\"Filled\":true},{\"Name\":\"self2\",\"type\":1,\"radius\":7.0,\"color\":4294967295,\"thicc\":5.0,\"refActorType\":1},{\"Name\":\"self3\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"refActorType\":1},{\"Name\":\"self 3s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayVOffset\":0.8,\"overlayText\":\"3秒\",\"refActorUseCastTime\":true,\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":2.0,\"refActorBuffTimeMax\":3.0,\"refActorType\":1},{\"Name\":\"self 2s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayVOffset\":0.8,\"overlayText\":\"2秒\",\"refActorUseCastTime\":true,\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":1.0,\"refActorBuffTimeMax\":2.0,\"refActorType\":1},{\"Name\":\"self 1s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayVOffset\":0.8,\"overlayText\":\"1秒\",\"refActorUseCastTime\":true,\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":1.0,\"refActorType\":1},{\"Name\":\"party1\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party2\",\"type\":1,\"radius\":7.0,\"color\":4294967295,\"thicc\":5.0,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorCastTimeMin\":2.0,\"refActorCastTimeMax\":3.0,\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party3\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party 3s\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"3秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":2.0,\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party 2s\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"2秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":1.0,\"refActorBuffTimeMax\":2.0,\"refActorComparisonType\":5},{\"Name\":\"party 1s\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"1秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":1.0,\"refActorComparisonType\":5}]}", out _);

            Controller.TryRegisterLayoutFromCode("DebuffAOEOther", "~Lv2~{\"Enabled\":false,\"Name\":\"P12S Spread AOE other\",\"Group\":\"P12S\",\"ZoneLockH\":[1154],\"ElementsL\":[{\"Name\":\"party1\",\"type\":1,\"radius\":7.0,\"color\":603914262,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party2\",\"type\":1,\"radius\":7.0,\"color\":4294967295,\"thicc\":5.0,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party3\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party 3s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"3秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":2.0,\"refActorBuffTimeMax\":3.0,\"refActorComparisonType\":5},{\"Name\":\"party 2s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"2秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMin\":1.0,\"refActorBuffTimeMax\":2.0,\"refActorComparisonType\":5},{\"Name\":\"party 1s\",\"type\":1,\"radius\":7.0,\"color\":4294919424,\"overlayBGColor\":4278190080,\"overlayTextColor\":4294967295,\"overlayText\":\"1秒\",\"refActorPlaceholder\":[\"<2>\",\"<3>\",\"<4>\",\"<5>\",\"<6>\",\"<7>\",\"<8>\"],\"refActorRequireBuff\":true,\"refActorBuffId\":[3578],\"refActorUseBuffTime\":true,\"refActorBuffTimeMax\":1.0,\"refActorComparisonType\":5}]}", out _);
        }

        Dictionary<uint, List<uint>> Attachments = new();

        public override void OnTetherCreate(uint source, uint target, uint data2, uint data3, uint data5)
        {
            if (!Attachments.ContainsKey(target)) Attachments.Add(target, new());
            //DuoLog.Information($"Attached {source} to {target}");
            Attachments[target].Add(source);
        }

        public override void OnUpdate()
        {
            var list = FindNextMechanic().ToList();
            list.RemoveAll(x => x.dist < 0.1f);
            if(list.Count > 0)
            {
                var toDisplay = list.Where(x => Math.Abs(x.dist - list[0].dist) < 0.5f).Select(x => (x.type, x.obj, x.dist)).ToList();
                if(list.TryGetFirst(x => x.type.EqualsAny(Spheres.Pairs, Spheres.Protean), out var l))
                {
                    toDisplay.Add((l.type, l.obj, l.dist));
                }
                Display(toDisplay);
            }
            else
            {
                Display();
            }
            if(Controller.TryGetLayoutByName("DebuffAOESelf", out var self) && Controller.TryGetLayoutByName("DebuffAOEOther", out var other))
            {
                if (C.EnableAOEChecking)
                {
                    if (Svc.ClientState.LocalPlayer.StatusList.Any(x => x.StatusId == AOEDebuff && x.RemainingTime < 3.5f))
                    {
                        self.Enabled = true;
                        other.Enabled = false;
                    }
                    else
                    {
                        self.Enabled = false;
                        other.Enabled = true;
                    }
                }
                else
                {
                    self.Enabled = false;
                    other.Enabled = false;
                }
            }
        }

        void Display(IEnumerable<(Spheres type, BattleNpc obj, float dist)>? values = null)
        {
            int aoe = 0;
            int donut = 0;
            Controller.GetRegisteredElements().Each(x => x.Value.Enabled = false);
            Controller.GetRegisteredLayouts().Each(x => x.Value.Enabled = false);
            if (values != null)
            {
                foreach (var x in values)
                {
                    if (x.type == Spheres.AOEBall)
                    {
                        aoe++;
                        if (Controller.TryGetElementByName($"AOEBall{aoe}", out var e))
                        {
                            e.Enabled = true;
                            e.refActorObjectID = x.obj.ObjectId;
                            //e.color = TransformColorBasedOnDistance(e.color, x.dist);
                            e.color = C.AoeColor.ToUint();
                        }
                        if (Controller.TryGetElementByName($"AOEBall{aoe}1", out var e1))
                        {
                            e1.Enabled = true;
                            e1.refActorObjectID = x.obj.ObjectId;
                            //e1.color = TransformColorBasedOnDistance(e.color, x.dist);
                            //e1.color = C.AoeColor.ToUint();
                        }
                        if (Controller.TryGetElementByName($"AOEBall{aoe}2", out var e2))
                        {
                            e2.Enabled = true;
                            e2.refActorObjectID = x.obj.ObjectId;
                            //e2.color = TransformColorBasedOnDistance(e.color, x.dist);
                            //e2.color = C.AoeColor.ToUint();
                        }
                    }
                    else if (x.type == Spheres.Donut)
                    {
                        donut++;
                        if (Controller.TryGetElementByName($"Donut{donut}", out var e))
                        {
                            e.Enabled = true;
                            e.refActorObjectID = x.obj.ObjectId;
                            e.Donut = C.DonutRadius;
                            e.color = C.DonutColor.ToUint();
                            //e.color = TransformColorBasedOnDistance(e.color, x.dist);
                        }
                        if (Controller.TryGetElementByName($"Donut{donut}1", out var e1))
                        {
                            e1.Enabled = true;
                            e1.refActorObjectID = x.obj.ObjectId;
                            e1.Donut = C.DonutRadius;
                            //e1.color = C.DonutColor.ToUint();
                            //e.color = TransformColorBasedOnDistance(e.color, x.dist);
                        }
                        if (Controller.TryGetElementByName($"Donut{donut}2", out var e2))
                        {
                            e2.Enabled = true;
                            e2.refActorObjectID = x.obj.ObjectId;
                            e2.Donut = C.DonutRadius;
                            //e2.color = C.DonutColor.ToUint();
                            //e.color = TransformColorBasedOnDistance(e.color, x.dist);
                        }
                    }
                    else
                    {
                        if (Controller.TryGetLayoutByName($"{x.type}", out var e))
                        {
                            e.Enabled = true;
                            e.ElementsL.Each(z => z.refActorObjectID = x.obj.ObjectId);
                            if (x.type == Spheres.Protean) e.ElementsL.Each(z => z.color = C.ProteanLineColor.ToUint());
                            if (x.type == Spheres.Pairs) e.ElementsL.Each(z => z.color = C.PairLineColor.ToUint());
                            //e.ElementsL.Each(z => z.color = TransformColorBasedOnDistance(z.color, x.dist));
                        }
                    }
                }
            }
        }

        uint TransformColorBasedOnDistance(uint col, float distance)
        {
            distance.ValidateRange(2, 20);
            distance -= 2f;
            var alpha = (1 - distance / 18) * 0.3f + 0.5f;
            return (col.ToVector4() with { W = alpha }).ToUint();
        }

        IEnumerable<(BattleNpc obj, Spheres type, float dist)> FindNextMechanic()
        {
            List<(BattleNpc obj, Spheres type, float dist)> objs = new();
            foreach(var x in Svc.Objects.Where(z => z is BattleNpc b && b.IsCharacterVisible()).Cast<BattleNpc>())
            {
                if(Enum.GetValues<Spheres>().Contains((Spheres)x.DataId) && x.DataId != (uint)Spheres.Mastersphere)
                {
                    var master = GetMasterSphereForObject(x);
                    if(master != null)
                    {
                        objs.Add((master, (Spheres)x.DataId, Vector3.Distance(master.Position, x.Position)));
                    }
                }
            }
            return objs.OrderBy(x => x.dist);
        }

        BattleNpc? GetMasterSphereForObject(BattleNpc obj)
        {
            foreach(var x in Attachments)
            {
                if (x.Value.Contains(obj.ObjectId) && x.Key.GetObject() is BattleNpc b && b.IsCharacterVisible())
                {
                    return b;
                }
            }
            return null;
        }

        public override void OnMessage(string Message)
        {
            if (Message.ContainsAny("(12377>33498)", "(12377>34554)", "(12377>34555)"))
            {
                Attachments.Clear();
            }
        }

        public unsafe static Vector4 Vector4FromABGR(uint col)
        {
            byte* bytes = (byte*)&col;
            return new Vector4((float)bytes[0] / 255f, (float)bytes[1] / 255f, (float)bytes[2] / 255f, (float)bytes[3] / 255f);
        }

        public class Config : IEzConfig
        {
            public float DonutRadius = 25.0f;
            public bool EnableAOEChecking = true;
            public Vector4 ProteanLineColor = Vector4FromABGR(1275002882);
            public Vector4 PairLineColor = Vector4FromABGR(2113863931);
            public Vector4 AoeColor = Vector4FromABGR(1275012352);
            public Vector4 DonutColor = Vector4FromABGR(1107248384);
            //public Vector4 AssistColorSelf = Vector4FromABGR(1258356223);
            //public Vector4 AssistColorOther = Vector4FromABGR(3355508706);
        }

        Config C => Controller.GetConfig<Config>();

        public override void OnSettingsDraw()
        {
            ImGuiEx.TextV("月环半径:"); // Dount radius:
            ImGui.SameLine();
            ImGui.SetNextItemWidth(150f);
            ImGui.DragFloat("", ref C.DonutRadius.ValidateRange(5f, 50f), 0.1f, 5f, 30f);
            ImGui.ColorEdit4("月环 颜色", ref C.DonutColor, ImGuiColorEditFlags.NoInputs); // Dount color
            ImGui.ColorEdit4("AOE 颜色", ref C.AoeColor, ImGuiColorEditFlags.NoInputs); // AOE color
            ImGui.ColorEdit4("22分摊 颜色", ref C.PairLineColor, ImGuiColorEditFlags.NoInputs); // Pair line color
            ImGui.ColorEdit4("8人分散 颜色", ref C.ProteanLineColor, ImGuiColorEditFlags.NoInputs); // Protean line color
            ImGui.Checkbox($"启用 天火刻印 debuff 提示", ref C.EnableAOEChecking); // Enable AOE debuff assist
            /*ImGuiEx.Text($"       ");
            ImGui.SameLine();
            ImGui.ColorEdit4("Self color (filled)", ref C.AssistColorSelf, ImGuiColorEditFlags.NoInputs);
            ImGui.SameLine();
            ImGui.ColorEdit4("Others (radius)", ref C.AssistColorOther, ImGuiColorEditFlags.NoInputs);*/

            if (ImGui.CollapsingHeader("Debug"))
            {
                foreach(var x in Attachments)
                {
                    ImGuiEx.Text($"{x.Key}({x.Key.GetObject()}) <- {x.Value.Select(z => $"{z}({z.GetObject()})").Print()}");
                }
                ImGui.Separator();
                foreach (var x in FindNextMechanic())
                {
                    ImGuiEx.Text($"{x.type} = {x.dist}");
                }
            }
        }
    }
}
