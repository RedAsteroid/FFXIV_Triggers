using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.MathHelpers;
using ImGuiNET;
using Splatoon.SplatoonScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SplatoonScriptsOfficial.Duties.Endwalker
{
    public class P12S_Tethers_小怪连线机制 : SplatoonScript
    {
        public override HashSet<uint> ValidTerritories => new() { 1154 };
        public override Metadata? Metadata => new(2, "Zeffuro, RedAsteroid 修改+调色");
		
		List<TetherData> _tethers = new();
		
		public class TetherData
		{
			public uint Source {get;set;}
			public uint Target {get;set;}
			public uint Data3 {get;set;}	// 233 / 250 = Light, 234 / 251 = Dark
		}
		
        public override void OnSetup()
        {
			Controller.RegisterElementFromCode("T1", "{\"Name\":\"\",\"type\":3,\"Enabled\":false,\"refY\":45.0,\"radius\":3,\"color\":687668992,\"thicc\":3.0,\"refActorObjectID\":0,\"FillStep\":0.2,\"refActorComparisonType\":2,\"includeRotation\":true}");
            Controller.RegisterElementFromCode("T2", "{\"Name\":\"\",\"type\":3,\"Enabled\":false,\"refY\":45.0,\"radius\":3,\"color\":687668992,\"thicc\":3.0,\"refActorObjectID\":0,\"FillStep\":0.2,\"refActorComparisonType\":2,\"includeRotation\":true}");
			Controller.RegisterElementFromCode("T3", "{\"Name\":\"\",\"type\":3,\"Enabled\":false,\"refY\":45.0,\"radius\":3,\"color\":687668992,\"thicc\":3.0,\"refActorObjectID\":0,\"FillStep\":0.2,\"refActorComparisonType\":2,\"includeRotation\":true}");
			Controller.RegisterElementFromCode("T4", "{\"Name\":\"\",\"type\":3,\"Enabled\":false,\"refY\":45.0,\"radius\":3,\"color\":687668992,\"thicc\":3.0,\"refActorObjectID\":0,\"FillStep\":0.2,\"refActorComparisonType\":2,\"includeRotation\":true}");
		}

        public override void OnUpdate()
        {
			ToggleLayouts(false);
			if (_tethers.Count() == 0) return;
			ToggleLayouts(true);
            var i = 1;
            foreach (var tether in _tethers)
            {
	            if(tether.Source.TryGetObject(out var source) && tether.Target.TryGetObject(out var target) && Controller.TryGetElementByName($"T{i++}", out var e))
	            {
		            e.color = GetConfigTetherColor(tether.Data3, i);
		            e.refActorObjectID = tether.Source;
		            e.AdditionalRotation = GetRelativeAngle(source.Position.ToVector2(), target.Position.ToVector2()) + source.Rotation;
	            }
            }
        }
		
		public override void OnTetherCreate(uint source, uint target, uint data2, uint data3, uint data5)
        {
            if(Svc.Objects.Any(x => x.DataId == 16172))
            {
				if(source.TryGetObject(out var sourceObject)){
					
					//PluginLog.Information($"{sourceObject.DataId} Data2: {data2} Data3: {data3} Data5:{data5}");
					if(sourceObject.DataId != 16172) return;
					if(_tethers.Exists(tether => tether.Target == target)){
						_tethers = _tethers.Where(tether => tether.Target != target).ToList();
					}
					_tethers.Add(new TetherData{Source = source, Target = target, Data3 = data3});
				}
            }
        }
		
		public override void OnTetherRemoval(uint source, uint data2, uint data3, uint data5)
        {
			//PluginLog.Information($"{source} Data2: {data2} Data3: {data3} Data5:{data5}");
            _tethers = _tethers.Where(tether => tether.Source != source).ToList();
        }
		
		static float GetRelativeAngle(Vector2 origin, Vector2 target)
        {
            var vector2 = target - origin;
            var vector1 = new Vector2(0, 1);
            return MathF.Atan2(vector2.Y, vector2.X) - MathF.Atan2(vector1.Y, vector1.X);
        }
		
		public uint GetConfigTetherColor(uint data3, int i)
		{
			switch(C.ColorMode){
				case 0: // Dark and Light
					return (data3 == 233 || data3 == 250) ? C.LightTetherColor.ToUint() : C.DarkTetherColor.ToUint();
					break;
				case 1: // Dark and Light + Stretched
					if(data3 == 233 || data3 == 234)
					{
						return data3 == 233 ? C.LightTetherColor.ToUint() : C.DarkTetherColor.ToUint();
						break;
					}else{
						return data3 == 250 ? C.LightTetherStretchedColor.ToUint() : C.DarkTetherStretchedColor.ToUint();
						break;
					}
					break;
				case 2: // Four different tether colors
					if(i == 1) return C.T1Color.ToUint();
					if(i == 2) return C.T2Color.ToUint();
					if(i == 3) return C.T3Color.ToUint();
					if(i == 4) return C.T4Color.ToUint();
					break;
				default: 
					return C.LightTetherColor.ToUint();
			}
			return C.LightTetherColor.ToUint();
		}
		
		
        void ToggleLayouts(bool enabled)
        {
            Controller.GetRegisteredElements().Each(x => x.Value.Enabled = enabled);
            Controller.GetRegisteredLayouts().Each(x => x.Value.Enabled = enabled);
        }
		
        public unsafe static Vector4 Vector4FromABGR(uint col)
        {
            byte* bytes = (byte*)&col;
            return new Vector4((float)bytes[0] / 255f, (float)bytes[1] / 255f, (float)bytes[2] / 255f, (float)bytes[3] / 255f);
        }
		
        public class Config : IEzConfig
        {			
			public int ColorMode = 0;
			
            public Vector4 LightTetherColor = Vector4FromABGR(598867950);
            public Vector4 DarkTetherColor = Vector4FromABGR(603933696);
			
			public Vector4 LightTetherStretchedColor = Vector4FromABGR(599982066);
            public Vector4 DarkTetherStretchedColor = Vector4FromABGR(603933696);
			
			public Vector4 T1Color = Vector4FromABGR(0xC6FF0000);
            public Vector4 T2Color = Vector4FromABGR(0xC600FF00);
            public Vector4 T3Color = Vector4FromABGR(0xC60000FF);
            public Vector4 T4Color = Vector4FromABGR(0xC6FFFFFF);
        }
		
        Config C => Controller.GetConfig<Config>();

        public override void OnSettingsDraw()
        {
			ImGui.SetNextItemWidth(150f);
			ImGui.Combo("颜色模式##Color Mode", ref C.ColorMode, new string[] {"光/暗", "光/暗, 是否拉直", "4 种不同颜色的线"}, 3);
			
			if(C.ColorMode < 2){
				ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("暗连线 颜色", ref C.DarkTetherColor, ImGuiColorEditFlags.NoInputs); // Dark tether color
                ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("光连线 颜色", ref C.LightTetherColor, ImGuiColorEditFlags.NoInputs); // Light tether color
            }
			
			if(C.ColorMode == 1){				
				ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("暗连线(拉直) 颜色", ref C.DarkTetherStretchedColor, ImGuiColorEditFlags.NoInputs); // Dark tether stretched color
                ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("光连线(拉直) 颜色", ref C.LightTetherStretchedColor, ImGuiColorEditFlags.NoInputs); // Light tether stretched color
            }	
			
			if(C.ColorMode == 2){				
				ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("1 号线 颜色", ref C.T1Color, ImGuiColorEditFlags.NoInputs); // Tether 1 color
                ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("2 号线 颜色", ref C.T2Color, ImGuiColorEditFlags.NoInputs); // Tether 2 color
                ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("3 号线 颜色", ref C.T3Color, ImGuiColorEditFlags.NoInputs); // Tether 3 color
                ImGui.SetNextItemWidth(150f);
				ImGui.ColorEdit4("4 号线 颜色", ref C.T4Color, ImGuiColorEditFlags.NoInputs); // Tether 4 color
            }
			/*
			if (ImGui.CollapsingHeader("Debug"))
            {
				ImGui.Text($"{Tethers.Count()}"); 
				
			}
			*/
        }
    }
}