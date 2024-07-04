using System;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Components;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Hooks;
using ECommons.ImGuiMethods;
using ECommons.MathHelpers;
using ECommons.Schedulers;
using ImGuiNET;
using Splatoon;
using Splatoon.SplatoonScripting;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons;
using ECommons.Logging;
using PluginLog = ECommons.Logging.PluginLog;

namespace SplatoonScriptsOfficial.Duties.Endwalker
{
    public class P12S_Pangenesis_泛生论 : SplatoonScript
    {
        public enum DebuffType { None, Short_2, Long_2, One }
        DebuffType MyDebuff = DebuffType.None;

        public override HashSet<uint> ValidTerritories => new() { 1154 };
        public override Metadata? Metadata => new(5, "tatad2-fra, RedAsteroid 修改");

        private string ElementNamePrefix = "P12SSC";
        private int towerCount = 0;

        private int whiteTowerCast = 33603;
        private int blackTowerCast = 33604;

        private int whiteDebuff = 3576;
        private int blackDebuff = 3577;
        private int DNABuff = 3593;

        private Element? Indicator;
        private Element? Indicator1;

        private bool directionRight = false; // 0=>left, 1=>right
        private bool lastTowerBlack = false; // 0=>white, 1=>black

        string TestOverride = "";

        PlayerCharacter PC => TestOverride != "" && FakeParty.Get().FirstOrDefault(x => x.Name.ToString() == TestOverride) is PlayerCharacter pc ? pc : Svc.ClientState.LocalPlayer!;

        public override void OnEnable()
        {
            Reset();
        }

        private void Reset()
        {
            PluginLog.Information("pangenesis RESET");
            towerCount = 0;
            Indicator.Enabled = false;
            Indicator1.Enabled = false;
            MyDebuff = DebuffType.None;
        }

        public override void OnSetup()
        {
            // {"Name":"","refX":100.0,"refY":95.0,"refActorDataID":16182,"refActorComparisonType":3,"tether":true}

            Element e = new Element(0);
            e.tether = true;
            e.Enabled = false;
            e.thicc = 5.0f;
            e.color = 4294967295;
            Controller.RegisterElement(ElementNamePrefix + "Indicator", e, true);
            Indicator = Controller.GetElementByName(ElementNamePrefix + "Indicator");

            Element e1 = new Element(0);
            e1.tether = true;
            e1.Enabled = false;
            e1.thicc = 2.0f;
            e1.color = 4294907904;
            Controller.RegisterElement(ElementNamePrefix + "Indicator1", e1, true);
            Indicator1 = Controller.GetElementByName(ElementNamePrefix + "Indicator1");

        }

        private void FirstTower()
        {
            BattleChara whiteTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == whiteTowerCast && o.CurrentCastTime < 1);
            BattleChara blackTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == blackTowerCast && o.CurrentCastTime < 1);

            PluginLog.Information($"tower casting time: {blackTower.CurrentCastTime}");

            Vector2 whitePos = whiteTower.Position.ToVector2();
            Vector2 blackPos = blackTower.Position.ToVector2();

            PluginLog.Information($"wtower: {whiteTower.ObjectId}, blacktower: {blackTower.ObjectId}, casttime: {whiteTower.CurrentCastTime}, {blackTower.CurrentCastTime}, position: {whiteTower.Position.ToVector2().ToString()}, {blackTower.Position.ToVector2().ToString()}");

            StatusList statusList = PC.StatusList;
            if (statusList.Any(x => x.StatusId == whiteDebuff && x.RemainingTime <= 8) && (C.Strat == Strat.First_2_0 || C.Strat == Strat.First_2_1))
            {
                // short white, go black tower 
                Indicator.refX = blackPos.X;
                Indicator.refY = blackPos.Y;
                Indicator1.refX = blackPos.X;
                Indicator1.refY = blackPos.Y;
                lastTowerBlack = true;
                MyDebuff = DebuffType.Short_2;
            }
            else if (statusList.Any(x => x.StatusId == whiteDebuff && x.RemainingTime > 8) && (C.Strat == Strat.First_2_0 || C.Strat == Strat.First_2_1))
            {
                // long white, wait
                int biasX = blackPos.X < 100 ? 5 : -5;
                Indicator.refX = blackPos.X + biasX;
                Indicator.refY = blackPos.Y;
                Indicator1.refX = blackPos.X + biasX;
                Indicator1.refY = blackPos.Y;
                lastTowerBlack = true;
                MyDebuff = DebuffType.Long_2;
            }
            else if (statusList.Any(x => x.StatusId == blackDebuff && x.RemainingTime <= 8) && (C.Strat == Strat.First_2_0 || C.Strat == Strat.First_2_1))
            {
                // short black, go white tower 
                Indicator.refX = whitePos.X;
                Indicator.refY = whitePos.Y;
                Indicator1.refX = whitePos.X;
                Indicator1.refY = whitePos.Y;
                lastTowerBlack = false;
                MyDebuff = DebuffType.Short_2;
            }
            else if (statusList.Any(x => x.StatusId == blackDebuff && x.RemainingTime > 8) && (C.Strat == Strat.First_2_0 || C.Strat == Strat.First_2_1))
            {
                // long black, wait
                int biasX = whitePos.X < 100 ? 5 : -5;
                Indicator.refX = whitePos.X + biasX;
                Indicator.refY = whitePos.Y;
                Indicator1.refX = whitePos.X + biasX;
                Indicator1.refY = whitePos.Y;
                lastTowerBlack = false;
                MyDebuff = DebuffType.Long_2;
            }
            // 菓子2+0魔改
            else if (statusList.Any(x => x.StatusId == whiteDebuff && x.RemainingTime <= 8) && (C.Strat == Strat.First_2_0_菓子))
            {
                // short white, go black tower 
                Indicator.refX = blackPos.X;
                Indicator.refY = blackPos.Y - 2;
                Indicator1.refX = blackPos.X;
                Indicator1.refY = blackPos.Y - 2;
                lastTowerBlack = true;
                MyDebuff = DebuffType.Short_2;
            }
            else if (statusList.Any(x => x.StatusId == whiteDebuff && x.RemainingTime > 8) && (C.Strat == Strat.First_2_0_菓子))
            {
                // long white, wait
                int biasX = blackPos.X < 100 ? 5 : -5;
                Indicator.refX = blackPos.X + biasX;
                Indicator.refY = blackPos.Y;
                Indicator1.refX = blackPos.X + biasX;
                Indicator1.refY = blackPos.Y;
                lastTowerBlack = true;
                MyDebuff = DebuffType.Long_2;
            }
            else if (statusList.Any(x => x.StatusId == blackDebuff && x.RemainingTime <= 8) && (C.Strat == Strat.First_2_0_菓子))
            {
                // short black, go white tower 
                Indicator.refX = whitePos.X;
                Indicator.refY = whitePos.Y - 2;
                Indicator1.refX = whitePos.X;
                Indicator1.refY = whitePos.Y - 2;
                lastTowerBlack = false;
                MyDebuff = DebuffType.Short_2;
            }
            else if (statusList.Any(x => x.StatusId == blackDebuff && x.RemainingTime > 8) && (C.Strat == Strat.First_2_0_菓子))
            {
                // long black, wait
                int biasX = whitePos.X < 100 ? 5 : -5;
                Indicator.refX = whitePos.X + biasX;
                Indicator.refY = whitePos.Y;
                Indicator1.refX = whitePos.X + biasX;
                Indicator1.refY = whitePos.Y;
                lastTowerBlack = false;
                MyDebuff = DebuffType.Long_2;
            }
            else
            {
                if (C.Strat == Strat.First_2_1)
                {
                    if (statusList.Any(x => x.StatusId == DNABuff))
                    {
                        // 1 buff, go first tower
                        Indicator.refX = debuff1.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator.refY = 91;
                        Indicator1.refX = debuff1.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator1.refY = 91;
                        lastTowerBlack = (Indicator.refX < 100) == (blackPos.X < 100);
                        MyDebuff = DebuffType.One;
                    }
                    else
                    {
                        // 0 buff, wait;
                        Indicator.refX = debuff0.IndexOf(PC) < 1 ? 90 : 110;
                        Indicator.refY = 91;
                        Indicator1.refX = debuff0.IndexOf(PC) < 1 ? 90 : 110;
                        Indicator1.refY = 91;
                        lastTowerBlack = (Indicator.refX < 100) != (blackPos.X < 100);
                        MyDebuff = DebuffType.None;
                    }
                }
                else if (C.Strat == Strat.First_2_0)
                {
                    if (statusList.Any(x => x.StatusId == DNABuff))
                    {
                        // 1 buff, wait
                        Indicator.refX = PC.Position.ToVector2().X < 100 ? 90 : 110;
                        Indicator.refY = 91;
                        Indicator1.refX = PC.Position.ToVector2().X < 100 ? 90 : 110;
                        Indicator1.refY = 91;
                        lastTowerBlack = (Indicator.refX < 100) == (blackPos.X < 100);
                        MyDebuff = DebuffType.One;
                    }
                    else
                    {
                        // 0 buff, go first tower;
                        Indicator.refX = PC.Position.ToVector2().X < 100 ? 85 : 115;
                        Indicator.refY = 91;
                        Indicator1.refX = PC.Position.ToVector2().X < 100 ? 85 : 115;
                        Indicator1.refY = 91;
                        lastTowerBlack = (Indicator.refX < 100) != (blackPos.X < 100);
                        MyDebuff = DebuffType.None;
                    }
                }
                else if (C.Strat == Strat.First_2_0_菓子)
                {
                    if (statusList.Any(x => x.StatusId == DNABuff))
                    {
                        // 1 buff, go first tower
                        Indicator.refX = debuff1.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator.refY = 93.5f;
                        Indicator1.refX = debuff1.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator1.refY = 93.5f;
                        lastTowerBlack = (Indicator.refX < 100) == (blackPos.X < 100);
                        MyDebuff = DebuffType.One;
                    }
                    else
                    {
                        // 0 buff, wait;
                        Indicator.refX = debuff0.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator.refY = 94.5f;
                        Indicator1.refX = debuff0.IndexOf(PC) < 1 ? 85 : 115;
                        Indicator1.refY = 94.5f;
                        lastTowerBlack = (Indicator.refX < 100) != (blackPos.X < 100);
                        MyDebuff = DebuffType.None;
                    }
                }
            }

            if (C.Strat == Strat.First_2_0 && MyDebuff == DebuffType.Short_2)
            {
                lastTowerBlack = !lastTowerBlack;
            }

            directionRight = (int)Indicator.refX < 100 ? false : true;
            Indicator.Enabled = true;
            Indicator1.Enabled = true;

            PluginLog.Information($"first tower, {Indicator.refX}, {Indicator.refY}, colorBlack?: {lastTowerBlack}");

        }

        private void SecondTower()
        {
            BattleChara whiteTower;
            BattleChara blackTower;
            if (directionRight)
            {
                // right
                whiteTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == whiteTowerCast && o.Position.ToVector2().X > 100 && o.CurrentCastTime < 1);
                blackTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == blackTowerCast && o.Position.ToVector2().X > 100 && o.CurrentCastTime < 1);
            }
            else
            {
                // left
                whiteTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == whiteTowerCast && o.Position.ToVector2().X < 100 && o.CurrentCastTime < 1);
                blackTower = (BattleChara)Svc.Objects.First(x => x is BattleChara o && o.IsCasting == true && o.CastActionId == blackTowerCast && o.Position.ToVector2().X < 100 && o.CurrentCastTime < 1);
            }

            Vector2 whitePos = whiteTower.Position.ToVector2();
            Vector2 blackPos = blackTower.Position.ToVector2();

            new TickScheduler(() =>
            {
                StatusList statusList = PC.StatusList;
                if (statusList.Any(x => x.StatusId == whiteDebuff))
                {
                    //  white, go black
                    Indicator.refX = blackPos.X;
                    Indicator.refY = blackPos.Y;
                    Indicator1.refX = blackPos.X;
                    Indicator1.refY = blackPos.Y;
                    lastTowerBlack = true;
                }
                else if (statusList.Any(x => x.StatusId == blackDebuff))
                {
                    // black, go white
                    Indicator.refX = whitePos.X;
                    Indicator.refY = whitePos.Y;
                    Indicator1.refX = whitePos.X;
                    Indicator1.refY = whitePos.Y;
                    lastTowerBlack = false;
                }
                else if (C.Strat == Strat.First_2_0_菓子)
                {
                    Indicator.refX = lastTowerBlack ? whitePos.X : blackPos.X;
                    Indicator.refY = lastTowerBlack ? whitePos.Y : blackPos.Y;
                    Indicator1.refX = lastTowerBlack ? whitePos.X : blackPos.X;
                    Indicator1.refY = lastTowerBlack ? whitePos.Y : blackPos.Y;
                }
                else if (!(C.Strat == Strat.First_2_0_菓子))
                {
                    Indicator.refX = lastTowerBlack ? blackPos.X : whitePos.X;
                    Indicator.refY = lastTowerBlack ? blackPos.Y : whitePos.Y;
                    Indicator1.refX = lastTowerBlack ? blackPos.X : whitePos.X;
                    Indicator1.refY = lastTowerBlack ? blackPos.Y : whitePos.Y;
                }

                PluginLog.Information($"second/third tower, {Indicator.refX}, {Indicator.refY}");
                PluginLog.Information($"第2/3次塔, {Indicator.refX}, {Indicator.refY}");
            }, 1500);
        }

        private void ThirdTower()
        {
            SecondTower();
            new TickScheduler(() =>
            {
                Indicator.Enabled = false;
                Indicator1.Enabled = false;
            }, 6000);
        }

        public override void OnMessage(string Message)
        {
            if (Message.Contains("(12383>33603)") || Message.Contains("(12383>33604)"))
            {
                // tower appear
                PluginLog.Information($"tower appear!");
                towerCount++;
                if (towerCount == 2)
                    FirstTower();
                if (towerCount == 6)
                    SecondTower();
                if (towerCount == 10)
                    ThirdTower();
            }

            if (Message.Contains("(12382>33602)"))
            {
                debuff1.Clear();
                debuff0.Clear();
                CheckBuff();
            }
        }

        public void CheckBuff()
        {
            var Party = FakeParty.Get();
            foreach (var pc in Party)
            {
                if (pc.StatusList.Any(x => x.StatusId == DNABuff))
                {
                    if (pc.StatusList.Any(x => x.StatusId == DNABuff && x.StackCount == 1))
                    {
                        debuff1.Add(pc);
                    }
                }
                else
                {
                    debuff0.Add(pc);
                }
            }
            List<string> order = new List<string> { C.MT, C.ST, C.H1, C.H2, C.D1, C.D2, C.D3, C.D4 };
            debuff0 = debuff0.OrderBy(p => order.IndexOf(p.Name.ToString())).ToList();
            debuff1 = debuff1.OrderBy(p => order.IndexOf(p.Name.ToString())).ToList();
        }
        public override void OnDirectorUpdate(DirectorUpdateCategory category)
        {
            if (category == DirectorUpdateCategory.Commence || category == DirectorUpdateCategory.Recommence)
                Reset();
        }


        public override void OnSettingsDraw()
        {
            ImGui.SetNextItemWidth(200f);
            ImGuiEx.EnumCombo("选择打法", ref C.Strat); // Select strat
            ImGui.Text("使用前请在Debug2中初始化队伍，调整小队成员为正确的职能！");
            ImGui.Text("绿毛肥(Game8)打法请选择 First 2 1，2+0打法请选择 First 2 0，菓子打法请选择 First 2 0 菓子");
            ImGui.Text("\n修复 2+1 、2+0 打法初次塔指路错误 - 2024.07.04");


            if (ImGui.CollapsingHeader("Debug"))
            {
                ImGuiEx.Text($"LastTowerBlack: {lastTowerBlack}");
                ImGuiEx.Text($"towerCount: {towerCount}");
                ImGuiEx.Text($"MyDebuff: {MyDebuff}");
                if (ImGui.Button("Reset")) Reset();
                ImGui.SetNextItemWidth(200f);
                ImGui.InputText("TestOverride", ref TestOverride, 50);
                ImGuiEx.Text($"{PC}");
            }

            if (ImGui.CollapsingHeader("Debug2"))
            {
                ImGui.Text("=====无debuff=====");
                if (debuff0.Count >= 2)
                {
                    ImGui.Text($"高优先级:{debuff0[0].Name}");
                    ImGui.Text($"低优先级:{debuff0[1].Name}");
                }
                ImGui.Text("=====1debuff=====");
                if (debuff1.Count >= 2)
                {
                    ImGui.Text($"高优先级:{debuff1[0].Name}");
                    ImGui.Text($"低优先级:{debuff1[1].Name}");
                }
            }
            DrawPartySelection("MT", ref C.MT);
            DrawPartySelection("ST", ref C.ST);
            DrawPartySelection("H1", ref C.H1);
            DrawPartySelection("H2", ref C.H2);
            DrawPartySelection("D1", ref C.D1);
            DrawPartySelection("D2", ref C.D2);
            DrawPartySelection("D3", ref C.D3);
            DrawPartySelection("D4", ref C.D4);
            if (ImGui.Button("初始化"))
            {
                var orderlist = FakeParty.Get();
                orderlist = orderlist.OrderBy(p => desiredOrder.IndexOf(p.ClassJob.Id)).ToList();
                foreach (var pc in orderlist)
                {
                    DuoLog.Information($"{pc.ClassJob.Id}-{pc.ClassJob.GameData.Name}");
                }
                C.MT = orderlist.ToList()[0].Name.ToString();
                C.ST = orderlist.ToList()[1].Name.ToString();
                C.H1 = orderlist.ToList()[2].Name.ToString();
                C.H2 = orderlist.ToList()[3].Name.ToString();
                C.D1 = orderlist.ToList()[4].Name.ToString();
                C.D2 = orderlist.ToList()[5].Name.ToString();
                C.D3 = orderlist.ToList()[6].Name.ToString();
                C.D4 = orderlist.ToList()[7].Name.ToString();
            }
        }
        void DrawPartySelection(string label, ref string selectedParty)
        {
            string tempSelectedParty = selectedParty;

            ImGui.Text(label);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(120f);
            if (ImGui.BeginCombo($"##partysel+{label}", tempSelectedParty))
            {
                FakeParty.Get().Each((x) => { if (ImGui.Selectable(x.Name.ToString())) tempSelectedParty = x.Name.ToString(); });
                ImGui.EndCombo();
            }

            selectedParty = tempSelectedParty;
        }
        public enum Strat { First_2_1, First_2_0, First_2_0_菓子 }
        List<uint> desiredOrder = new List<uint> { 21, 32, 37, 19, 24, 33, 40, 28, 34, 20, 30, 22, 39, 23, 38, 31, 25, 27, 35 };
        List<PlayerCharacter> debuff0 = new List<PlayerCharacter>();
        List<PlayerCharacter> debuff1 = new List<PlayerCharacter>();
        Config C => Controller.GetConfig<Config>();
        public class Config : IEzConfig
        {
            public Strat Strat = Strat.First_2_1;
            public string MT = String.Empty;
            public string ST = String.Empty;
            public string H1 = String.Empty;
            public string H2 = String.Empty;
            public string D1 = String.Empty;
            public string D2 = String.Empty;
            public string D3 = String.Empty;
            public string D4 = String.Empty;
        }
    }
}