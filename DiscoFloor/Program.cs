using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public Random globalRandy = new Random();
        public List<DiscoSurface> surfaceList = new List<DiscoSurface>();
        public int rowLength;
        public string lcdName = "Disco";
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            int.TryParse(Storage, out rowLength);
            if (rowLength == 0) { rowLength = 4; };
            initSurfaces(rowLength);
        }

        public void initSurfaces(int rowlength)
        {
            surfaceList.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(null, (block) => {
                if (block.CustomName.ToLower().Contains(lcdName.ToLower())) {
                    surfaceList.Add(new DiscoSurface((IMyTextPanel)block, globalRandy, rowlength));
                }
                return false;
            });
            return;
        }

        public void Save()
        {
            Storage = $"{rowLength}";
        }

        public class DiscoSurface
        {            
            private IMyTextPanel surface;
            private Random randR;
            private Random randG;
            private Random randB;
            public SurfaceGrid myGrid;
            public DiscoSurface(IMyTextPanel _surface, Random randy, int rowLength = 4)
            {
                randR = new Random(randy.Next());
                randG = new Random(randy.Next());
                randB = new Random(randy.Next());
                surface = _surface;
                surface.ContentType = ContentType.SCRIPT;
                surface.Script = "";
                surface.ScriptBackgroundColor = Color.Black;
                myGrid = new SurfaceGrid(surface, rowLength);
                return;
            }
            public void Draw() {
                using (MySpriteDrawFrame frame = surface.DrawFrame())
                { 
                    for (int _square = 0; _square <= myGrid.numSquares; _square++)
                    {
                        int currRow = (int)(Math.Floor(Convert.ToDecimal(_square / myGrid.rowLength)));
                        int currCol = (int)(Math.Floor(Convert.ToDecimal(_square % myGrid.rowLength)));
                        DrawSquare(currCol, currRow, frame);
                    }
                    frame.Dispose();
                }
            }
            private void DrawSquare(int index, int row, MySpriteDrawFrame frame) {
                {
                    MySprite sprite = new MySprite(SpriteType.TEXTURE, $"Square_{index}");
                    sprite.Data = "SquareTapered"; 
                    sprite.Color = new Color(randR.Next(0, 256), randG.Next(0, 256), randB.Next(0, 256));
                    sprite.Size = new Vector2(myGrid.singleSize, myGrid.singleSize);
                    float offset = myGrid.singleSize / 2;
                    sprite.Position = new Vector2(myGrid.singleSize * index + offset, myGrid.singleSize * row + offset);
                    frame.Add(sprite);
                }
                return;
            }
        }
        public class SurfaceGrid
        {
            public int rowLength;
            public float singleSize;
            public int numSquares;
            public SurfaceGrid(IMyTextSurface surface, int _rowLength)
            {
                rowLength = _rowLength;
                // assume square LCD
                numSquares = rowLength * rowLength;
                singleSize = surface.SurfaceSize.X / rowLength;
            }
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Terminal) != 0)
            {
                int _rowLength;
                int.TryParse(argument.Trim(), out _rowLength);
                rowLength = _rowLength;
                initSurfaces(rowLength);
            }
            if ((updateSource & UpdateType.Update10) != 0)
            {
            if (surfaceList != null)
                {
                    foreach (DiscoSurface surface in surfaceList)
                    {
                        surface.Draw();
                    }
                }
            }
        }
    }
}
