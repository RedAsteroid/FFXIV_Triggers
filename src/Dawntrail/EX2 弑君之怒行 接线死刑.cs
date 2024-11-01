using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameFunctions;
using ECommons.Hooks.ActionEffectTypes;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.MathHelpers;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Splatoon.SplatoonScripting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonScriptsOfficial.Duties.Dawntrail
{
    public class EX2_弑君之怒行_接线死刑_For_CN_Client : SplatoonScript
    {
        // EX2_Regicidal_Rage
        // 限制副本地图：1201 永护塔顶层
        public override HashSet<uint>? ValidTerritories { get; } = [1201];

        public override Metadata? Metadata => new(1, "RedOrange");
        HashSet<uint> TetheredPlayers = new();

        public override void OnSetup()
        {
            Controller.RegisterElement("TetherAOE1", new(1) { thicc = Conf.TetherAOEThick, color = Conf.TetherAOECol.ToUint(), refActorComparisonType = 2, onlyTargetable = true, Filled = false, Enabled = false, radius = 8f});
            Controller.RegisterElement("TetherAOE2", new(1) { thicc = Conf.TetherAOEThick, color = Conf.TetherAOECol.ToUint(), refActorComparisonType = 2, onlyTargetable = true, Filled = false, Enabled = false, radius = 8f});
            Controller.RegisterElement("Tether1", new(2) { thicc = Conf.TetherThick, radius = 0f });
            Controller.RegisterElement("Tether2", new(2) { thicc = Conf.TetherThick, radius = 0f });
            Controller.RegisterElement("hint1", new(1) { thicc = 0f, radius = 0f, refActorComparisonType = 2, overlayPlaceholders = true, overlayBGColor = 4294911232, overlayTextColor = 4294967295, overlayVOffset = 1f});
            Controller.RegisterElement("hint2", new(1) { thicc = 0f, radius = 0f, refActorComparisonType = 2, overlayPlaceholders = true, overlayBGColor = 4294911232, overlayTextColor = 4294967295, overlayVOffset = 1f});
        }

        public override void OnTetherCreate(uint source, uint target, uint data2, uint data3, uint data5)
        {
            if (IsZoraalja(target, out _))
            {
                //DuoLog.Information($"Tether: {data2}, {data3}, {data5}");
                TetheredPlayers.Add(source);
                //UpdateTethers();
            }
        }

        public override void OnTetherRemoval(uint source, uint data2, uint data3, uint data5)
        {
            //DuoLog.Information($"Tether rem: {data2}, {data3}, {data5}");
            TetheredPlayers.Remove(source);
            //UpdateTethers();
        }

        public override void OnUpdate()
        {
            UpdateTethers();
        }

        bool IsZoraalja(uint oid, [NotNullWhen(true)] out IBattleChara? zoraalja)
        {
            if (oid.TryGetObject(out var obj) && obj is IBattleChara o && o.NameId == 12882)
            {
                zoraalja = o;
                return true;
            }
            zoraalja = null;
            return false;
        }

        void Reset()
        {
            TetheredPlayers.Clear();
        }

        IBattleChara? GetZoraalja()
        {
            return Svc.Objects.FirstOrDefault(x => x is IBattleChara o && o.NameId == 12882 && o.IsTargetable()) as IBattleChara;
        }

        void UpdateTethers()
        {
            if (TetheredPlayers.Count == 2)
            {
                var tetheredPlayers = TetheredPlayers.ToArray();
                var zoraalja = GetZoraalja();
                {
                    if (Controller.TryGetElementByName("Tether1", out var e))
                    {
                        e.Enabled = true;
                        e.SetRefPosition(zoraalja.Position);
                        e.SetOffPosition(tetheredPlayers[0].GetObject().Position);
                        var correct = (tetheredPlayers[0].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                        e.color = (correct ? Conf.ValidTetherColor : GradientColor.Get(Conf.InvalidTetherColor1, Conf.InvalidTetherColor2, 500)).ToUint();
                    }
                }
                {
                    if (Controller.TryGetElementByName("TetherAOE1", out var e))
                    {
                        e.Enabled = true;
                        e.refActorObjectID = tetheredPlayers[0];
                        e.color = Conf.TetherAOECol.ToUint();
                        var correct = (tetheredPlayers[0].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                    }
                }
                {
                    if (Controller.TryGetElementByName("hint1", out var e))
                    {
                        e.refActorObjectID = tetheredPlayers[0];
                        var correct = (tetheredPlayers[0].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                        e.overlayText = (correct ? "死刑 → $NAME" : "坦克接线");
                        if (Conf.UseOverlayText)
                        {
                            e.Enabled = true;
                        }
                        else
                        {
                            e.Enabled = false;
                        }
                    }
                }
                {
                    if (Controller.TryGetElementByName("Tether2", out var e))
                    {
                        e.Enabled = true;
                        e.SetRefPosition(zoraalja.Position);
                        e.SetOffPosition(tetheredPlayers[1].GetObject().Position);
                        var correct = (tetheredPlayers[1].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                        e.color = (correct ? Conf.ValidTetherColor : GradientColor.Get(Conf.InvalidTetherColor1, Conf.InvalidTetherColor2, 500)).ToUint();
                    }
                }
                {
                    if (Controller.TryGetElementByName("TetherAOE2", out var e))
                    {
                        e.Enabled = true;
                        e.refActorObjectID = tetheredPlayers[1];
                        e.color = Conf.TetherAOECol.ToUint();
                        var correct = (tetheredPlayers[1].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                    }
                }
                {
                    if (Controller.TryGetElementByName("hint2", out var e))
                    {
                        e.refActorObjectID = tetheredPlayers[1];
                        var correct = (tetheredPlayers[1].GetObject() as IPlayerCharacter).GetRole() == CombatRole.Tank;
                        e.overlayText = (correct ? "死刑 → $NAME" : "坦克接线");
                        if (Conf.UseOverlayText)
                        {
                            e.Enabled = true;
                        }
                        else
                        {
                            e.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                Controller.GetRegisteredElements().Each(x => x.Value.Enabled = false);
            }
        }

        Config Conf => Controller.GetConfig<Config>();
        public override void OnSettingsDraw()
        {
            ImGui.Checkbox("启用 字幕提醒",ref Conf.UseOverlayText);
            ImGui.ColorEdit4("死刑范围 颜色", ref Conf.TetherAOECol, ImGuiColorEditFlags.NoInputs);
            ImGui.SetNextItemWidth(100f);
            ImGui.ColorEdit4("Tank连线时 颜色", ref Conf.ValidTetherColor, ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("##Invalid1", ref Conf.InvalidTetherColor1, ImGuiColorEditFlags.NoInputs);
            ImGui.SameLine();
            ImGui.ColorEdit4("非Tank连线时 颜色(渐变)", ref Conf.InvalidTetherColor2, ImGuiColorEditFlags.NoInputs);
            ImGui.SetNextItemWidth(100f);
            ImGui.SliderFloat("死刑范围 线条厚度", ref Conf.TetherAOEThick, 1f, 10f);
            ImGui.SetNextItemWidth(100f);
            ImGui.SliderFloat("连线线条厚度", ref Conf.TetherThick, 1f, 10f);
            if (ImGui.CollapsingHeader("说明"))
            {
                ImGuiEx.Text($"1. 按住 Ctrl 键点击滑动条可以输入数字");
                ImGuiEx.Text($"2. 连线逻辑为: 连线坦克时为黄色，连线其他职业时为黑蓝渐变，您可以自行更改");
                ImGuiEx.Text($"3. 此脚本基于 UCOB Tethers 修改，可作为连线机制参考，所有的成果归于 NightmareXIV");
            }
            if (ImGui.CollapsingHeader("Debug 调试"))
            {
                ImGuiEx.Text($"连线：\n{TetheredPlayers.Select(x => x.GetObject()?.ToString() ?? $"unk{x}").Join("\n")}");
            }
        }
        public class Config : IEzConfig
        {
            public bool UseOverlayText = true;
            public Vector4 TetherAOECol = 0xE6FF00FF.SwapBytes().ToVector4();
            public Vector4 ValidTetherColor = 0xD9FF00FF.SwapBytes().ToVector4();
            public Vector4 InvalidTetherColor1 = 0xFFFFFFFF.SwapBytes().ToVector4();
            public Vector4 InvalidTetherColor2 = new(0, 0, 0, 255);
            public float TetherAOEThick = 3f;
            public float TetherThick = 5f;
        }
    }
}
