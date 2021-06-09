using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoffXNA.Base;
using BoffXNA.Graphics;
using BoffXNA.Util;

using MiniJam61Egypt.GameContent.Entities.Other;

namespace MiniJam61Egypt.Environment
{
    public class World
    {
        private const float TRANSITION_TIME = 0.25f;
        private Tile[,] _tiles;
        private Camera _camera;

        private int _currentScreen;
        private int _nextScreen;
        private float _transitionProgress;

        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        public World(Camera camera, ExtendedContentManager cm)
        {
            _camera = camera;
            _tiles = new Tile[60, 12];
            _currentScreen = 1;
            LoadWorld(cm);
        }
        
        private void LoadWorld(ExtendedContentManager cm)
        {
            Random random = new Random();
            string[] lines = File.ReadAllLines(cm.GetFullPath("mapData.data"));
            for (int y = 0; y < lines.Length; y++)
            {
                string[] tiles = lines[y].Split(',');
                for (int x = 0; x < tiles.Length; x++)
                {
                    if (tiles[x].StartsWith("x"))
                    {
                        var fence = new NormalTile(Main.FenceTexture);
                        int frame = int.Parse(tiles[x][1].ToString());
                        int srcX = frame % 3;
                        int srcY = frame / 3;
                        if (tiles[x].Length == 3)
                        {
                            frame = int.Parse(tiles[x].Substring(1));
                            switch(frame)
                            {
                                case 10:
                                    srcX = 3;
                                    srcY = 0;
                                    break;
                                case 11:
                                    srcX = 4;
                                    srcY = 0;
                                    break;
                                case 12:
                                    srcX = 3;
                                    srcY = 1;
                                    break;
                                case 13:
                                    srcX = 4;
                                    srcY = 1;
                                    break;
                            }
                        }
                        fence.Source = new Rectangle(srcX * 32, srcY * 32, 32, 32);
                        _tiles[x, y] = fence;
                        switch (frame)
                        {
                            default:
                                Main.Instance.physicsManager.AddBody(new Rectangle(x * 32, y * 32, 32, 32));
                                break;
                            case 5:
                            case 10:
                            case 12:
                                Main.Instance.physicsManager.AddBody(new Rectangle(x * 32 + 20, y * 32, 12, 32));
                                break;
                            case 3:
                            case 11:
                            case 13:
                                Main.Instance.physicsManager.AddBody(new Rectangle(x * 32, y * 32, 12, 32));
                                break;
                        }
                    }
                    else if (tiles[x].StartsWith("c") || tiles[x].StartsWith("s"))
                    {
                        var carpet = new NormalTile(tiles[x].StartsWith("c") ? Main.CarpetTexture : Main.SellCarpetTexture);
                        int frame = int.Parse(tiles[x][1].ToString());
                        int srcX = frame % 3;
                        int srcY = frame / 3;
                        carpet.HasFootsteps = false;
                        if (tiles[x].StartsWith("s"))
                        {
                            carpet.AnimalSale = true;
                        }
                        carpet.Source = new Rectangle(srcX * 32, srcY * 32, 32, 32);
                        _tiles[x, y] = carpet;
                    }
                    else
                    {
                        switch (tiles[x])
                        {
                            case ".":
                                var tile = new NormalTile(Main.SandTexture);
                                if (random.Next(2) == 0)
                                {
                                    tile.Source = new Rectangle(random.Next(4) * 32, 0, 32, 32);
                                }
                                else
                                {
                                    tile.Source = new Rectangle(0, 0, 32, 32);
                                }
                                _tiles[x, y] = tile;
                                break;
                            case "f":
                                _tiles[x, y] = new FarmTile(Main.FarmTexture);
                                break;
                            case "p":
                            case "z":
                                var sand = new NormalTile(Main.SandTexture);
                                sand.Source = new Rectangle(0, 0, 32, 32);
                                _tiles[x, y] = sand;
                                switch (tiles[x])
                                {
                                    case "p":
                                        Main.Instance.entities.Add(new WorldDecoration(new Rectangle(x * 32 + 4, y * 32 + 9, 23, 14), new Vector2(-3, -44), Main.PalmTreeTexture));
                                        break;
                                    case "z":
                                        Main.Instance.entities.Add(new WorldDecoration(new Rectangle(x * 32 + 3, y * 32 + 10, 25, 12), new Vector2(-2, -12), Main.Rock1Texture));
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void Update(TimeManager time)
        {
            if (_currentScreen != _nextScreen)
            {
                time.GameSpeed = 0f;
                _transitionProgress += time.RawDeltaTime;
                if (_transitionProgress >= TRANSITION_TIME)
                {
                    time.GameSpeed = 1f;
                    _currentScreen = _nextScreen;
                    _transitionProgress = 0f;
                    _camera.Position = new Vector2(320f + _currentScreen * 640, 192f);
                }
                else
                {
                    _camera.Position = new Vector2(MathHelper.Lerp(320f + _currentScreen * 640, 320f + _nextScreen * 640, _transitionProgress / TRANSITION_TIME), 192f);
                }
            }
            else
            {
                _camera.Position = new Vector2(320f + _currentScreen * 640, 192f);
            }

            _camera.DoUpdate(null, time);

            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    _tiles[x, y].Update(time);
                }
            }

            Vector2 mousePosWorld = Main.Instance.MouseInWorld();
            Point tile = (mousePosWorld / 32f).ToPoint();
            if (tile.X >= 0 && tile.X < _tiles.GetLength(0) && tile.Y >= 0 && tile.Y < _tiles.GetLength(1))
            {
                if (Main.Instance.InputManager.MouseLeftJustPressed)
                {
                    _tiles[tile.X, tile.Y].LeftClicked(Main.SelectedHotbarSlot);
                }
            }
            Vector2 playerWorld = Main.Instance.player.Body.Center; 
            tile = (playerWorld / 32f).ToPoint(); 
            if (tile.X >= 0 && tile.X < _tiles.GetLength(0) && tile.Y >= 0 && tile.Y < _tiles.GetLength(1))
            {
                _tiles[tile.X, tile.Y].WalkedOver();
            }
        }

        public void TransitionTo(int newScreen)
        {
            if (newScreen < 0) newScreen = 0;
            if (newScreen > 2) newScreen = 2;
            _nextScreen = newScreen;
        }

        public void Draw(ExtendedSpriteBatch spriteBatch)
        {
            int minScreen = Math.Min(_currentScreen, _nextScreen);
            int maxScreen = Math.Max(_currentScreen, _nextScreen);

            int minX = minScreen * 20;
            int maxX = maxScreen * 20 + 20;

            for (int y = 0; y < _tiles.GetLength(1); y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    _tiles[x, y].Draw(x, y, spriteBatch);
                }
            }
        }

        public Tile this[int x, int y] => _tiles[x, y];
    }
}
