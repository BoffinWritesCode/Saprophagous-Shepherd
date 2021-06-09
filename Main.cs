using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Util;
using BoffXNA.Graphics;
using BoffXNA.Graphics.Fonts;

using MiniJam61Egypt.Environment;
using MiniJam61Egypt.Util;
using MiniJam61Egypt.GameContent.Entities;
using MiniJam61Egypt.GameContent.Entities.Projectiles;
using MiniJam61Egypt.GameContent.Entities.Other;
using MiniJam61Egypt.GameContent.Entities.Animals;
using MiniJam61Egypt.GameContent.Entities.Enemies;
using MiniJam61Egypt.Physics;
using MiniJam61Egypt.Sound;

namespace MiniJam61Egypt
{
    public class Main : BoffGame
    {
        public static readonly Rectangle ANIMAL_PEN = new Rectangle(22 * 32, 2 * 32, 16 * 32, 8 * 32);

        public static Main Instance;

        public static RenderTarget2D ScreenTarget;
        public static RenderTarget2D WorldTarget;

        public static Effect Shaders;

        public static Texture2D SandTexture;
        public static Texture2D FenceTexture;
        public static Texture2D CarpetTexture;
        public static Texture2D TileHoverTexture;
        public static Texture2D SellCarpetTexture;
        public static Texture2D FarmTexture;
        public static Texture2D PalmTreeTexture;
        public static Texture2D Rock1Texture;
        public static Texture2D PlayerTexture;

        public static Texture2D HotbarTexture;
        public static Texture2D HotbarSelection;
        public static Texture2D HeartTexture;
        public static Texture2D CoinTexture;

        public static Texture2D FoodTexture;
        public static Texture2D SeedsTexture;
        public static Texture2D WaterBasinTexture;

        public static Texture2D SheepTexture;
        public static Texture2D SheepOutlineTexture;
        public static Texture2D BabySheepTexture;
        public static Texture2D BabySheepOutlineTexture;

        public static Texture2D GoatTexture;
        public static Texture2D GoatOutlineTexture;
        public static Texture2D BabyGoatTexture;
        public static Texture2D BabyGoatOutlineTexture;

        public static Texture2D CamelTexture;
        public static Texture2D CamelOutlineTexture;
        public static Texture2D BabyCamelTexture;
        public static Texture2D BabyCamelOutlineTexture;

        public static Texture2D ShopkeeperTexture;
        public static Texture2D[] WeaponTextures;

        public static Texture2D SmokeTexture;
        public static Texture2D DollarTexture;
        public static Texture2D BloodTexture;
        public static Texture2D SmallCoinTexture;
        public static Texture2D SmallFoodTexture;

        public static Texture2D VignetteTexture;

        public static Texture2D RockTexture;
        public static Texture2D ArrowTexture;
        public static Texture2D SunTexture;
        public static Texture2D RedBlotTexture;
        public static Texture2D TriangleTexture;
        public static Texture2D MarkTexture;
        public static Texture2D MarkWarningTexture;
        public static Texture2D CircleTexture;
        public static Texture2D[] WhirlpoolTextures;

        public static Texture2D FootstepTexture;
        public static Texture2D BigFootstepTexture;

        public static Texture2D SandSplicerTexture;
        public static Texture2D PyramidTexture;
        public static Texture2D AnubisTexture;
        public static Texture2D ObeliskTexture;

        public static Texture2D TitleBackTexture;
        public static Texture2D TitleTexture;
        public static Texture2D GameOverTexture;
        public static Texture2D[] TutorialTextures;

        public static PixelFont MainFont;
        public static Random GameRandom;

        public Camera camera;
        public World world;

        public PhysicsManager physicsManager;
        public Player player;
        public Shopkeeper shopkeeper;

        public List<Entity> entities;
        public List<Animal> animals;
        public List<Enemy> enemies;
        public List<Projectile> projectiles;

        public static XnaSFXManager SFXManager = new XnaSFXManager();
        public static MusicManager MusicManager;

        public static bool TitleScreen;
        public static bool StartingGame;
        public static bool EndingGame;
        public static float GameFade;
        private Vector2 titleOffset;

        public static GameStats stats;
        public struct GameStats
        {
            public int SheepBred;
            public int GoatsBred;
            public int CamelsBred;

            public int SheepSold;
            public int GoatsSold;
            public int CamelsSold;

            public int SheepKilled;
            public int GoatsKilled;
            public int CamelsKilled;

            public int SplicersKilled;
            public int GebKilled;
            public int AnubisKilled;

            public int RentPaid;
            public int MoneyEarnt;
            public int PlantsHarvested;
            public float TimeSurvived;
        }

        public static int[] WeaponCosts = new int[] { 300, 1000, 3000, -1 };
        public static string[] WeaponName = new string[]
        {
            "Sling",
            "Bow",
            "Spear",
            "Staff of Ra",
            ""
        };
        public static string[] WeaponDescription = new string[]
        {
            "That's the bow, pretty vanilla when it comes to weaponry,\nbut you're gonna need something better than that sling.",
            "That's a throwing spear, once you buy this you'll have an\ninfinite number of them!",
            "That's the staff of ra! The most powerful weapon I have,\nit fires blasts with the power of a sun or two!",
            "You've got my most powerful weapon already, friend."
        };
        public static float[] WeaponUseTime = new float[] { 0.5f, 0.5f, 0.2f, 0.1f };
        public static int Coins;
        public static int CurrentWeaponUpgrade = 0;
        public static int Food;
        public static int Seeds;
        public static bool CanShoot;
        public static int SelectedHotbarSlot;
        public static float RentTimer;
        public static int RentsPaid;
        public static int CurrentRent;
        public static bool ShowingTutorial;
        public static float WeaponTime;
        public static bool Intense;
        public static int CurrentTutorial;
        private float _intenseAmount;

        public static int SheepBirths;
        public static int GoatBirths;
        public static int CamelBirths;

        public Main() : base()
        {
            Instance = this;
            IsMouseVisible = true;

            TitleScreen = true;
        }

        public void RestartGame()
        {
            entities = new List<Entity>();
            animals = new List<Animal>();
            enemies = new List<Enemy>();
            projectiles = new List<Projectile>();

            var sheep1 = new Sheep(ANIMAL_PEN);
            var sheep2 = new Sheep(ANIMAL_PEN);
            sheep1.Body.Center = sheep2.Body.Center = ANIMAL_PEN.Center.ToVector2();
            sheep1.Body.Center += Vector2.UnitX * 64f;
            sheep2.Body.Center += Vector2.UnitX * -64f;
            entities.Add(sheep1);
            entities.Add(sheep2);
            animals.Add(sheep1);
            animals.Add(sheep2);

            stats = new GameStats();

            physicsManager = new PhysicsManager(256, TimeManager);
            camera = new Camera();
            camera.Position = new Vector2(WindowManager.ScreenWidth * 0.25f + 640, WindowManager.ScreenHeight * 0.25f);
            world = new World(camera, Content);
            player = new Player(InputManager);
            player.Body.Center = camera.Position;
            entities.Add(player);
            shopkeeper = new Shopkeeper(new Vector2(352, 112));
            entities.Add(shopkeeper);
            CurrentRent = 100;
            RentTimer = 120f;
            RentsPaid = 0;

            SheepBirths = 0;
            CamelBirths = 0;
            GoatBirths = 0;

            CurrentWeaponUpgrade = 0;
            Coins = 100;
            TimeManager.GameSpeed = 1f;

            ItemDisplay seeds = new ItemDisplay(new Vector2(448, 112), Main.SeedsTexture, Vector2.Zero, "Seeds", "Those are some premium Food Seeds!\nYou're really gonna need these, so stock up!", 15, null);
            seeds.LeftClick = () => { if (CheckBuy(seeds.Cost)) { Main.Seeds++; seeds.Cost += 5; } };
            entities.Add(seeds);
            ItemDisplay weapon = new ItemDisplay(new Vector2(256, 112), Main.WeaponTextures[1], Vector2.Zero, WeaponName[1], WeaponDescription[0], WeaponCosts[0], null);
            Action weaponDisplayAction = () =>
            {
                if (CurrentWeaponUpgrade >= 3) return;
                if (CheckBuy(WeaponCosts[CurrentWeaponUpgrade]))
                {
                    CurrentWeaponUpgrade++;
                    weapon.ItemTexture = WeaponTextures[CurrentWeaponUpgrade + 1];
                    weapon.Name = WeaponName[CurrentWeaponUpgrade + 1];
                    weapon.Description = WeaponDescription[CurrentWeaponUpgrade];
                    weapon.Cost = WeaponCosts[CurrentWeaponUpgrade];
                }
            };
            weapon.LeftClick = weaponDisplayAction;
            entities.Add(weapon);
            entities.Add(new ItemDisplay(new Vector2(256, 272), Main.BabySheepTexture, Vector2.UnitY * -2, "Sheep", "That's a sheep, cheap and easy to raise!", 70, () => { if (CheckBuy(70)) { SpawnAnimal(0); } }) { Source = new Rectangle(0, 0, 32, 32) });
            entities.Add(new ItemDisplay(new Vector2(352, 272), Main.BabyGoatTexture, new Vector2(-3, -8), "Goat", "That's a goat. They sell for more than\nsheep, so be careful with them!", 230, () => { if (CheckBuy(230)) { SpawnAnimal(1); } }) { Source = new Rectangle(0, 0, 41, 42) });
            entities.Add(new ItemDisplay(new Vector2(448, 272), Main.BabyCamelTexture, new Vector2(-10, -10), "Camel", "That's a camel, these are extra hardy animals that sell for a ton!\nThey can pose a massive threat if they go hungry.", 720, () => { if (CheckBuy(720)) { SpawnAnimal(2); } }) { Source = new Rectangle(0, 0, 59, 42) });

            player.Health = 50f;
            player.MaxHealth = 50f;
            Food = 5;
            Seeds = 2;
        }

        public static void SummonParticles(Texture2D texture, int amount, Vector2 pos, Vector2 velocity, Vector2 acceleration, float rotVelocity = 0f, float timeAliveMin = 1f, float timeAliveMax = 1f, float minRotation = 0f, float maxRotation = 0f, float minScale = 1f, float maxScale = 1f, float posOffsetMax = 0f, bool onTop = false)
        {
            for (int i = 0; i < amount; i++)
            {
                Particle p = new Particle(pos + new Vector2(GameRandom.NextFloat(-posOffsetMax, posOffsetMax), GameRandom.NextFloat(-posOffsetMax, posOffsetMax)), GameRandom.NextFloat(timeAliveMin, timeAliveMax), texture);
                p.Rotation = GameRandom.NextFloat(minRotation, maxRotation);
                p.Scale = GameRandom.NextFloat(minScale, maxScale);
                p.Velocity = velocity;
                p.Acceleration = acceleration;
                p.RotationVelocity = rotVelocity;
                p.OnTop = onTop;
                Main.Instance.entities.Add(p); 
            }
        }

        public static void SummonParticlesColorRamp(Texture2D texture, int amount, Vector2 pos, Vector2 velocity, Vector2 acceleration, Color minColor, Color maxColor, float rotVelocity = 0f, float timeAliveMin = 1f, float timeAliveMax = 1f, float minRotation = 0f, float maxRotation = 0f, float minScale = 1f, float maxScale = 1f, float posOffsetMax = 0f, bool onTop = false)
        {
            for (int i = 0; i < amount; i++)
            {
                Particle p = new Particle(pos + new Vector2(GameRandom.NextFloat(-posOffsetMax, posOffsetMax), GameRandom.NextFloat(-posOffsetMax, posOffsetMax)), GameRandom.NextFloat(timeAliveMin, timeAliveMax), texture);
                p.Rotation = GameRandom.NextFloat(minRotation, maxRotation);
                p.Scale = GameRandom.NextFloat(minScale, maxScale);
                p.Velocity = velocity;
                p.Acceleration = acceleration;
                p.RotationVelocity = rotVelocity;
                p.Color = Color.Lerp(minColor, maxColor, GameRandom.NextFloat(1f));
                p.OnTop = onTop;
                Main.Instance.entities.Add(p);
            }
        }

        public void SpawnAnimal(int type)
        {
            switch(type)
            {
                case 0:
                    var sheep = new Sheep(ANIMAL_PEN);
                    sheep.Body.Center =ANIMAL_PEN.Center.ToVector2();
                    sheep.IsBaby = true;
                    entities.Add(sheep);
                    animals.Add(sheep);
                    break;
                case 1:
                    var goat = new Goat(ANIMAL_PEN);
                    goat.Body.Center = ANIMAL_PEN.Center.ToVector2();
                    goat.IsBaby = true;
                    entities.Add(goat);
                    animals.Add(goat);
                    break;
                case 2:
                    var camel = new Camel(ANIMAL_PEN);
                    camel.Body.Center = ANIMAL_PEN.Center.ToVector2();
                    camel.IsBaby = true;
                    entities.Add(camel);
                    animals.Add(camel);
                    break;
            }
        }

        public bool CheckBuy(int cost)
        {
            if (Coins >= cost)
            {
                Coins -= cost;
                Main.SFXManager.PlaySound("Cash_Register", 0.1f);
                return true;
            }
            return false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Window.Title = "Saprophagous Shepard";

            GameRandom = new Random();

            titleOffset = new Vector2(Main.GameRandom.NextFloat(1000), Main.GameRandom.NextFloat(1000f));

            Window.AllowUserResizing = false;
            WindowManager.ChangeScreenResolution(1280, 768);
            WindowManager.Apply();

            ScreenTarget = new RenderTarget2D(GraphicsDevice, WindowManager.ScreenWidth / 2, WindowManager.ScreenHeight / 2);
            WorldTarget = new RenderTarget2D(GraphicsDevice, ScreenTarget.Width, ScreenTarget.Height);
        }

        protected override void LoadGameContent()
        {
            Shaders = Content.Load<Effect>("Shaders");
            SandTexture = Content.Load<Texture2D>("Tiles/Sand");
            FenceTexture = Content.Load<Texture2D>("Tiles/Fence");
            FarmTexture = Content.Load<Texture2D>("Tiles/Farm");
            CarpetTexture = Content.Load<Texture2D>("Tiles/Carpet");
            SellCarpetTexture = Content.Load<Texture2D>("Tiles/Carpet2");
            PalmTreeTexture = Content.Load<Texture2D>("Tiles/PalmTree");
            Rock1Texture = Content.Load<Texture2D>("Tiles/Rock1");
            HotbarTexture = Content.Load<Texture2D>("UI/HotbarBox");
            HotbarSelection = Content.Load<Texture2D>("UI/HotbarSelection");
            HeartTexture = Content.Load<Texture2D>("UI/Heart");
            CoinTexture = Content.Load<Texture2D>("UI/Coin");
            PlayerTexture = Content.Load<Texture2D>("Player");

            TitleBackTexture = Content.Load<Texture2D>("TitleBack");
            TitleTexture = Content.Load<Texture2D>("Logo");
            TutorialTextures = new Texture2D[4];
            TutorialTextures[0] = Content.Load<Texture2D>("Tutorial0");
            TutorialTextures[1] = Content.Load<Texture2D>("Tutorial1");
            TutorialTextures[2] = Content.Load<Texture2D>("Tutorial2");
            TutorialTextures[3] = Content.Load<Texture2D>("Tutorial3");
            GameOverTexture = Content.Load<Texture2D>("GameOver");
            TileHoverTexture = Content.Load<Texture2D>("TileHover");

            CircleTexture = Content.Load<Texture2D>("Circle");
            RockTexture = Content.Load<Texture2D>("Projectiles/Rock");
            ArrowTexture = Content.Load<Texture2D>("Projectiles/Arrow");
            SunTexture = Content.Load<Texture2D>("Projectiles/Sun");
            RedBlotTexture = Content.Load<Texture2D>("Projectiles/RedBlot");
            TriangleTexture = Content.Load<Texture2D>("Projectiles/Triangle");
            MarkTexture = Content.Load<Texture2D>("Projectiles/Mark");
            MarkWarningTexture = Content.Load<Texture2D>("Projectiles/MarkWarning");
            WhirlpoolTextures = new Texture2D[3];
            WhirlpoolTextures[0] = Content.Load<Texture2D>("Projectiles/Whirlpool1");
            WhirlpoolTextures[1] = Content.Load<Texture2D>("Projectiles/Whirlpool2");
            WhirlpoolTextures[2] = Content.Load<Texture2D>("Projectiles/Whirlpool3");

            SeedsTexture = Content.Load<Texture2D>("Seeds");
            FoodTexture = Content.Load<Texture2D>("Food");
            WaterBasinTexture = Content.Load<Texture2D>("WaterBasin");

            SheepTexture = Content.Load<Texture2D>("Sheep");
            SheepOutlineTexture = Content.Load<Texture2D>("SheepOutline");
            BabySheepTexture = Content.Load<Texture2D>("BabySheep");
            BabySheepOutlineTexture = Content.Load<Texture2D>("BabySheepOutline");

            GoatTexture = Content.Load<Texture2D>("Goat");
            GoatOutlineTexture = Content.Load<Texture2D>("GoatOutline");
            BabyGoatTexture = Content.Load<Texture2D>("BabyGoat");
            BabyGoatOutlineTexture = Content.Load<Texture2D>("BabyGoatOutline");

            CamelTexture = Content.Load<Texture2D>("Camel");
            CamelOutlineTexture = Content.Load<Texture2D>("CamelOutline");
            BabyCamelTexture = Content.Load<Texture2D>("BabyCamel");
            BabyCamelOutlineTexture = Content.Load<Texture2D>("BabyCamelOutline");

            VignetteTexture = Content.Load<Texture2D>("Vignette");

            SmokeTexture = Content.Load<Texture2D>("Particles/Smoke");
            DollarTexture = Content.Load<Texture2D>("Particles/Dollar");
            BloodTexture = Content.Load<Texture2D>("Particles/Blood");
            SmallCoinTexture = Content.Load<Texture2D>("Particles/SmallCoin");
            SmallFoodTexture = Content.Load<Texture2D>("Particles/SmallFood");

            ShopkeeperTexture = Content.Load<Texture2D>("Shopkeeper");

            FootstepTexture = Content.Load<Texture2D>("Footstep");
            BigFootstepTexture = Content.Load<Texture2D>("BigFootstep");
            MainFont = new PixelFont(Content.Load<Texture2D>("Fonts/main"), 1, Content.GetFullPath(@"Fonts\main.xml"));

            SandSplicerTexture = Content.Load<Texture2D>("SandSplicer");
            PyramidTexture = Content.Load<Texture2D>("Geb");
            AnubisTexture = Content.Load<Texture2D>("Anubis");
            ObeliskTexture = Content.Load<Texture2D>("Obelisk");

            SFXManager.LoadAllFromFolder(Content, "Sounds");
            MusicManager = new MusicManager(new SoundBankStreamed(Content.GetFullPath(@"Sounds\Music\music.bnk")));
            MusicManager.Volume = 0.06f;
            MusicManager.AddTrack("RuinShepherd");
            MusicManager.AddTrack("SheepWithTeeth");
            MusicManager.AddTrack("Title");

            WeaponTextures = new Texture2D[5];
            WeaponTextures[0] = Content.Load<Texture2D>("Sling");
            WeaponTextures[1] = Content.Load<Texture2D>("Bow");
            WeaponTextures[2] = Content.Load<Texture2D>("Spear");
            WeaponTextures[3] = Content.Load<Texture2D>("StaffOfRa");
            WeaponTextures[4] = Content.Load<Texture2D>("SoldOut");

            base.LoadGameContent();
        }

        protected override void UnloadContent()
        {
            Content.Dispose();
            MusicManager.Dispose();
            SFXManager.Dispose();
        }

        public static void GameOver()
        {
            Instance.TimeManager.GameSpeed = 0f;
            EndingGame = true;
        }

        protected override void GameUpdate(GameTime gameTime)
        {
            base.GameUpdate(gameTime);

            Intense = false;

            if (TitleScreen)
            {
                MusicManager.Play("Title", 1000);
                TimeManager.GameSpeed = 1f;

                if (StartingGame)
                {
                    GameFade += TimeManager.RawDeltaTime;
                    if (GameFade > 1f)
                    {
                        GameFade = 1f;
                        TitleScreen = false;
                        RestartGame();
                    }
                }

                if (EndingGame)
                {
                    GameFade -= TimeManager.RawDeltaTime;
                    if (GameFade < 0f)
                    {
                        GameFade = 0f;
                    }
                }
            }

            if (!TitleScreen)
            {
                if (StartingGame)
                {
                    GameFade -= TimeManager.RawDeltaTime;
                    if (GameFade <= 0f)
                    {
                        StartingGame = false;
                        GameFade = 0f;
                    }
                }

                if (EndingGame)
                {
                    //Play ambience?
                    GameFade += TimeManager.RawDeltaTime;
                    if (GameFade >= 1f)
                    {
                        TitleScreen = true;
                        GameFade = 1f;
                    }
                    return;
                }

                RentTimer -= TimeManager.DeltaTime;

                Main.stats.TimeSurvived += TimeManager.DeltaTime;

                if (RentTimer <= 0f)
                {
                    RentTimer = 120f;
                    Coins -= CurrentRent;
                    stats.RentPaid += CurrentRent;
                    if (Coins < 0)
                    {
                        RentTimer = 0f;
                        GameOver();
                    }
                    CurrentRent = (int)(CurrentRent * 1.5f);
                    RentsPaid++;
                }

                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].Update(TimeManager, physicsManager);
                    if (entities[i].Destroy)
                    {
                        entities.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < animals.Count; i++)
                {
                    if (animals[i].Destroy)
                    {
                        animals.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].Destroy)
                    {
                        enemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < projectiles.Count; i++)
                {
                    if (projectiles[i].Friendly)
                    {
                        foreach (Enemy e in enemies)
                        {
                            if (projectiles[i].Body.Intersects(e.Body) && e.Hittable())
                            {
                                //damage enemy?
                                projectiles[i].Damage(e);
                                break;
                            }
                        }
                    }
                    else
                    {
                        bool hitAnimal = false;
                        foreach (Animal a in animals)
                        {
                            if (projectiles[i].Body.Intersects(a.Body))
                            {
                                //damage enemy?
                                projectiles[i].Damage(a);
                                if (projectiles[i].HitOneAtTime)
                                {
                                    hitAnimal = true;
                                    break;
                                }
                            }
                        }
                        if (!hitAnimal)
                        {
                            if (projectiles[i].Body.Intersects(player.Body))
                            {
                                //damage enemy?
                                projectiles[i].Damage(player);
                            }
                        }
                    }

                    if (projectiles[i].Destroy)
                    {
                        projectiles.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                if (InputManager.MouseScrolledDown)
                {
                    SelectedHotbarSlot++;
                    if (SelectedHotbarSlot >= ItemSlots.Count)
                    {
                        SelectedHotbarSlot -= ItemSlots.Count;
                    }
                }
                else if (InputManager.MouseScrolledUp)
                {
                    SelectedHotbarSlot--;
                    if (SelectedHotbarSlot < 0)
                    {
                        SelectedHotbarSlot += ItemSlots.Count;
                    }
                }

                if (InputManager.KeyJustPressed(Keys.D1)) SelectedHotbarSlot = 0;
                if (InputManager.KeyJustPressed(Keys.D2)) SelectedHotbarSlot = 1;
                if (InputManager.KeyJustPressed(Keys.D3)) SelectedHotbarSlot = 2;
                if (InputManager.KeyJustPressed(Keys.D4)) SelectedHotbarSlot = 3;

                WeaponTime -= TimeManager.DeltaTime;

                world.TransitionTo((int)Math.Floor(player.Body.Center.X / 640f));

                world.Update(TimeManager);

                if (player.Health <= 0f)
                {
                    GameOver();
                }

                if (InputManager.KeyJustPressed(Keys.Escape))
                {
                    GameOver();
                }

                if (Intense)
                {
                    MusicManager.Play("SheepWithTeeth", 1000);
                    _intenseAmount += TimeManager.DeltaTime * 0.7f;
                    if (_intenseAmount > 1f)
                    {
                        _intenseAmount = 1f;
                    }
                }
                else
                {
                    if (EndingGame)
                    {
                        MusicManager.Play("Title", 1000);
                    }
                    else
                    {
                        MusicManager.Play("RuinShepherd", 1000);
                    }
                    _intenseAmount -= TimeManager.DeltaTime * 0.7f;
                    if (_intenseAmount < 0f)
                    {
                        _intenseAmount = 0f;
                    }
                }
            }
        }

        public Vector2 MouseInWorld()
        {
            Vector2 camTL = camera.Position - new Vector2(WindowManager.ScreenWidth * 0.25f, WindowManager.ScreenHeight * 0.25f);
            return camTL + InputManager.MousePosition.ToVector2() * 0.5f;
        }

        private void DrawHotbar()
        {
            for (int i = 0; i < ItemSlots.Count; i++)
            {
                Rectangle hotbarRect = new Rectangle(4 + (HotbarTexture.Width + 4) * i, 12, HotbarTexture.Width, HotbarTexture.Height);
                SpriteBatch.Draw(HotbarTexture, hotbarRect, Color.White * (i == SelectedHotbarSlot ? 0.95f : 0.7f));
                DrawBorderText(SpriteBatch.Batch, (i + 1).ToString(), new Vector2(hotbarRect.X + 3, hotbarRect.Y + 2), Color.White, Color.Black);
                string text = "";
                switch (i)
                {
                    case ItemSlots.Weapon:
                        SpriteBatch.Draw(WeaponTextures[CurrentWeaponUpgrade], new Vector2(hotbarRect.X + 6, hotbarRect.Y + 6), Color.White);
                        break;
                    case ItemSlots.Food:
                        text = Food.ToString();
                        SpriteBatch.Draw(FoodTexture, new Vector2(hotbarRect.X + 6, hotbarRect.Y + 6), Color.White);
                        break;
                    case ItemSlots.Seeds:
                        text = Seeds.ToString();
                        SpriteBatch.Draw(SeedsTexture, new Vector2(hotbarRect.X + 6, hotbarRect.Y + 6), Color.White);
                        break;
                    case ItemSlots.WaterBasin:
                        SpriteBatch.Draw(WaterBasinTexture, new Vector2(hotbarRect.X + 6, hotbarRect.Y + 6), Color.White);
                        break;
                }
                if (i == SelectedHotbarSlot)
                {
                    Vector2 textPos = new Vector2(4, 2);
                    switch (i)
                    {
                        case ItemSlots.Weapon:
                            DrawBorderText(SpriteBatch.Batch, WeaponName[CurrentWeaponUpgrade], textPos, Color.White, Color.Black);
                            break;
                        case ItemSlots.Food:
                            DrawBorderText(SpriteBatch.Batch, "Food", textPos, Color.White, Color.Black);
                            break;
                        case ItemSlots.Seeds:
                            DrawBorderText(SpriteBatch.Batch, "Seeds", textPos, Color.White, Color.Black);
                            break;
                        case ItemSlots.WaterBasin:
                            DrawBorderText(SpriteBatch.Batch, "Water basin", textPos, Color.White, Color.Black);
                            break;
                    }
                    SpriteBatch.Draw(HotbarSelection, hotbarRect, Color.White);
                }
                if (text == "") continue;
                Vector2 size = Main.MainFont.MeasureString(text);
                DrawBorderText(SpriteBatch.Batch, text, new Vector2(hotbarRect.Right - 1 - size.X, hotbarRect.Bottom - 6), Color.White, Color.Black);
            }
        }

        public static void SpawnProjectile(Projectile projectile)
        {
            Instance.projectiles.Add(projectile);
            Instance.entities.Add(projectile);
        }

        public static void SpawnEnemy(Enemy enemy)
        {
            Instance.enemies.Add(enemy);
            Instance.entities.Add(enemy);
        }

        private void DrawHealth()
        {
            int hearts = (int)(player.Health / 10) + 1;
            float health = player.Health;
            int x = 556;
            for (int i = 0; i < hearts; i++)
            {
                float thisHeartScale = health >= 10f ? 10f : health;
                health -= 10f;

                float scale = thisHeartScale / 10f;

                SpriteBatch.Draw(Main.HeartTexture, new Vector2(x, 12), null, Color.Lerp(Color.Black * 0f, Color.White, scale), 0f, new Vector2(8), scale, SpriteEffects.None, 0f);

                x += 16;
            }

            string text = "HP: " + player.Health.ToString("N0") + " / " + player.MaxHealth.ToString("N0");
            Vector2 size = MainFont.MeasureString(text);
            DrawBorderText(SpriteBatch.Batch, text, new Vector2(626 - size.X, 24), Color.White, Color.Black);

            text = Coins + " coins";
            if (Coins == 1)
            {
                text = "1 coin";
            }
            size = MainFont.MeasureString(text);
            DrawBorderText(SpriteBatch.Batch, text, new Vector2(626 - size.X, 36), Color.White, Color.Black);
        }

        private void DrawRent()
        {
            string text = CurrentRent + " in rent is due in " + RentTimer.ToString("N0") + " seconds.";
            Vector2 size = MainFont.MeasureString(text) + Vector2.UnitX * 20;
            SpriteBatch.Draw(CoinTexture, new Vector2(320 - size.X * 0.5f, 4), Color.White);
            DrawBorderText(SpriteBatch.Batch, text, new Vector2(320 - size.X * 0.5f + 20, 8), Color.White, Color.Black);

            if (RentTimer < 30f)
            {
                Color c = (int)Math.Floor(RentTimer + 0.5f) % 2 == 0 ? new Color(255, 100, 100) : Color.White;
                text = RentTimer.ToString("N0") + " seconds.";
                DrawBorderText(SpriteBatch.Batch, text, new Vector2(320 - size.X * 0.5f + size.X - MainFont.MeasureString(text).X, 8), c, Color.Black);
            }
        }

        public static void DrawBorderText(SpriteBatch spriteBatch, string text, Vector2 position, Color textColor, Color borderColor)
        {
            MainFont.Draw(spriteBatch, text, new Vector2(position.X + 1, position.Y), borderColor);
            MainFont.Draw(spriteBatch, text, new Vector2(position.X - 1, position.Y), borderColor);
            MainFont.Draw(spriteBatch, text, new Vector2(position.X, position.Y + 1), borderColor);
            MainFont.Draw(spriteBatch, text, new Vector2(position.X, position.Y - 1), borderColor);
            MainFont.Draw(spriteBatch, text, new Vector2(position.X, position.Y), textColor);
        }

        private void DrawTitleText(Vector2 position, ref bool last, string text, Color normal, Color hover, Color down, Action OnClick)
        {
            Vector2 size = MainFont.MeasureString(text);
            BoundingBox2D box = new BoundingBox2D();
            box.Size = size;
            box.Center = position;
            Rectangle area = box.ToRectangle();

            Color c = normal;

            if (area.Contains(InputManager.MousePosition.ToVector2() * 0.5f))
            {
                if (!last)
                {
                    last = true;
                    SFXManager.PlaySound("Tick", 0.03f, 0f, 0.8f);
                }
                c = hover;
                if (InputManager.MouseLeftDown)
                {
                    c = down;
                }

                if (!StartingGame && InputManager.MouseLeftJustPressed)
                {
                    OnClick?.Invoke();
                }
            }
            else
            {
                last = false;
            }

            DrawBorderText(SpriteBatch.Batch, text, position - size * 0.5f, c, Color.Black);
        }

        private void DrawStatTextColon(string text, float y)
        {
            string[] parts = text.Split(':');
            parts[0] += ":";
            Vector2 before = MainFont.MeasureString(parts[0]);
            Vector2 after = MainFont.MeasureString(parts[1]);

            DrawBorderText(SpriteBatch.Batch, parts[0], new Vector2(320 - before.X, y), Color.White, Color.Black);
            DrawBorderText(SpriteBatch.Batch, parts[1], new Vector2(320, y), Color.White, Color.Black);
        }

        private static bool startOverLast;
        private static bool tutOverLast;
        private static bool quitOverLast;
        private static bool returnOverLast;

        protected override void GameDraw(GameTime gameTime)
        {
            if (TitleScreen)
            {
                GraphicsDevice.SetRenderTarget(ScreenTarget);
                GraphicsDevice.Clear(Color.Black);

                SpriteBatch.Begin(samplerState: SamplerState.PointWrap);

                titleOffset -= new Vector2(12f, 1f) * TimeManager.DeltaTime;

                SpriteBatch.Draw(TitleBackTexture, GraphicsDevice.Viewport.Bounds, new Rectangle((int)titleOffset.X, (int)titleOffset.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                if (!ShowingTutorial)
                {
                    if (EndingGame)
                    {
                        SpriteBatch.Draw(GameOverTexture, new Rectangle(320 - 120, 24 + (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 8), 240, 70), Color.White);

                        string title = "STATISTICS";
                        Vector2 size = MainFont.MeasureString(title);
                        DrawBorderText(SpriteBatch.Batch, title, new Vector2(320 - size.X * 0.5f, 100), Color.White, Color.Black);

                        int y = 114;

                        DrawStatTextColon("Sheep bred: " + stats.SheepBred, y += 10);
                        DrawStatTextColon("Goats bred: " + stats.GoatsBred, y += 10);
                        DrawStatTextColon("Camels bred: " + stats.CamelsBred, y += 10);
                        y += 10;
                        DrawStatTextColon("Sheep sold: " + stats.SheepSold, y += 10);
                        DrawStatTextColon("Goats sold: " + stats.GoatsSold, y += 10);
                        DrawStatTextColon("Camels sold: " + stats.CamelsSold, y += 10);
                        y += 10;
                        DrawStatTextColon("Sheep deaths: " + stats.SheepKilled, y += 10);
                        DrawStatTextColon("Goats deaths: " + stats.GoatsKilled, y += 10);
                        DrawStatTextColon("Camels deaths: " + stats.CamelsKilled, y += 10);
                        y += 10;
                        DrawStatTextColon("Sand splicers killed: " + stats.SplicersKilled, y += 10);
                        DrawStatTextColon("Geb killed: " + stats.GebKilled, y += 10);
                        DrawStatTextColon("Anubi killed: " + stats.AnubisKilled, y += 10);
                        y += 10;
                        DrawStatTextColon("Rent paid: " + stats.RentPaid, y += 10);
                        DrawStatTextColon("Money earnt: " + stats.MoneyEarnt, y += 10);
                        DrawStatTextColon("Plants harvested: " + stats.PlantsHarvested, y += 10);

                        TimeSpan time = TimeSpan.FromSeconds(stats.TimeSurvived);

                        DrawStatTextColon($"Time spent playing: {time.Minutes}m {time.Seconds}s", y += 10);

                        DrawTitleText(new Vector2(320, y + 30), ref returnOverLast, "Click here to return to main menu!", Color.White, new Color(200, 200, 200), new Color(120, 120, 120), () => { EndingGame = false; });
                    }
                    else
                    {
                        SpriteBatch.Draw(TitleTexture, new Rectangle(320 - 120, 24 + (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 8), 240, 70), Color.White);

                        DrawTitleText(new Vector2(320, 200), ref startOverLast, "Start game", Color.White, new Color(200, 200, 200), new Color(120, 120, 120), () =>
                        {
                            StartingGame = true;
                        });
                        DrawTitleText(new Vector2(320, 230), ref tutOverLast, "How to play!", Color.White, new Color(200, 200, 200), new Color(120, 120, 120), () =>
                        {
                            ShowingTutorial = true;
                        });
                        DrawTitleText(new Vector2(320, 260), ref quitOverLast, "Quit game", Color.White, new Color(200, 200, 200), new Color(120, 120, 120), () =>
                        {
                            Exit();
                        });

                        Color border = new Color(60, 60, 60);
                        Vector2 start = new Vector2(4, 372);
                        string text = "Mini Jam 61 (Egypt)  |  Themes of conflict: ";
                        Vector2 size = MainFont.MeasureString(text);
                        DrawBorderText(SpriteBatch.Batch, text, start, Color.White, border);
                        start.X += size.X;
                        text = "Creating life";
                        size = MainFont.MeasureString(text);
                        DrawBorderText(SpriteBatch.Batch, text, start, new Color(92, 211, 148), new Color(11, 51, 28));
                        start.X += size.X;
                        text = " and ";
                        size = MainFont.MeasureString(text);
                        DrawBorderText(SpriteBatch.Batch, text, start, Color.White, border);
                        start.X += size.X;
                        text = "Destroying life";
                        size = MainFont.MeasureString(text);
                        DrawBorderText(SpriteBatch.Batch, text, start, new Color(211, 53, 29), new Color(51, 11, 11));
                        start.X += size.X;
                        text = ".";
                        size = MainFont.MeasureString(text);
                        DrawBorderText(SpriteBatch.Batch, text, start, Color.White, border);
                        start.X += size.X;
                    }
                }
                else
                {
                    SpriteBatch.Draw(TutorialTextures[CurrentTutorial], GraphicsDevice.Viewport.Bounds, Color.White);

                    string text = "A & D or Arrow keys to navigate  |  ESC to return to menu";
                    Vector2 size = MainFont.MeasureString(text);
                    DrawBorderText(SpriteBatch.Batch, text, new Vector2(636, 380) - size, Color.White, Color.Black);

                    if (InputManager.KeyJustPressed(Keys.Escape))
                    {
                        ShowingTutorial = false;
                    }

                    if (InputManager.KeyJustPressed(Keys.Left) || InputManager.KeyJustPressed(Keys.A))
                    {
                        CurrentTutorial--;
                        if (CurrentTutorial < 0) CurrentTutorial = 0;
                    }

                    if (InputManager.KeyJustPressed(Keys.Right) || InputManager.KeyJustPressed(Keys.D))
                    {
                        CurrentTutorial++;
                        if (CurrentTutorial > 3) CurrentTutorial = 3;
                    }
                }
                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);
                SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

                SpriteBatch.Draw(ScreenTarget, GraphicsDevice.Viewport.Bounds, Color.Lerp(Color.White, Color.Black, GameFade));

                SpriteBatch.End();
            }
            else
            {
                GraphicsDevice.SetRenderTarget(WorldTarget);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetMatrix(GraphicsDevice.Viewport));

                world.Draw(SpriteBatch);

                IEnumerable<Entity> sorted = entities.OrderBy(e => e.GetOrderHeight());
                Entity hoverEntity = null;
                foreach (Entity e in sorted)
                {
                    e.Draw(SpriteBatch);
                    if (e.CanHover)
                    {
                        if (e.MouseOver())
                        {
                            hoverEntity = e;
                        }
                    }
                }
                if (hoverEntity != null)
                {
                    hoverEntity.DrawHover(SpriteBatch);
                    if (InputManager.MouseRightJustPressed)
                    {
                        hoverEntity.RightClicked();
                    }
                    if (InputManager.MouseLeftJustPressed)
                    {
                        hoverEntity.LeftClicked();
                    }
                    if (hoverEntity.ConsumeMouse && (InputManager.MouseLeftDown || InputManager.MouseRightDown))
                    {
                        CanShoot = false;
                    }
                }

                if (CanShoot && InputManager.MouseLeftDown && SelectedHotbarSlot == ItemSlots.Weapon)
                {
                    if (WeaponTime <= 0)
                    {
                        WeaponTime = WeaponUseTime[CurrentWeaponUpgrade];
                        Vector2 center = player.Body.Center - Vector2.UnitY * 16f;
                        switch (CurrentWeaponUpgrade)
                        {
                            default:
                            case 0:
                                Main.SFXManager.PlaySound("Sling", 0.1f, 0, Main.GameRandom.NextFloat(-0.1f, 0.1f));
                                SpawnProjectile(new Rock(center, Vector2.Normalize(MouseInWorld() - center) * 250f, new Vector2(8f)));
                                break;
                            case 1:
                                Main.SFXManager.PlaySound("Bow", 0.1f, 0, Main.GameRandom.NextFloat(-0.1f, 0.1f));
                                SpawnProjectile(new Arrow(center, Vector2.Normalize(MouseInWorld() - center) * 320f, new Vector2(8f)));
                                break;
                            case 2:
                                Main.SFXManager.PlaySound("Spear", 0.1f, 0, Main.GameRandom.NextFloat(-0.1f, 0.1f));
                                SpawnProjectile(new Spear(center, Vector2.Normalize(MouseInWorld() - center) * 350f, new Vector2(8f)));
                                break;
                            case 3:
                                Main.SFXManager.PlaySound("StaffOfRa", 0.06f, 0, Main.GameRandom.NextFloat(-0.4f, -0.2f));
                                SpawnProjectile(new Sun(center, Vector2.Normalize(MouseInWorld() - center) * 400f, new Vector2(8f)));
                                break;
                        }
                    }
                }
                if (CanShoot && InputManager.MouseRightJustPressed && SelectedHotbarSlot == ItemSlots.Food && Main.Food > 0 && player.Health < player.MaxHealth)
                {
                    Main.Food--;
                    Main.SFXManager.PlaySound("Burp_0", 0.1f, 0, Main.GameRandom.NextFloat(-0.1f, 0.1f));
                    player.Health += 20f;
                    if (player.Health > player.MaxHealth)
                    {
                        player.Health = player.MaxHealth;
                    }
                }
                CanShoot = true;

                SpriteBatch.End();
                GraphicsDevice.SetRenderTarget(ScreenTarget);

                SpriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

                //Shaders.Parameters["HueShift"].SetValue(0.02f);
                //Shaders.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(WorldTarget, ScreenTarget.Bounds, Color.White);

                SpriteBatch.End();

                SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

                SpriteBatch.Draw(VignetteTexture, GraphicsDevice.Viewport.Bounds, Color.White * _intenseAmount);

                DrawHotbar();

                DrawHealth();

                DrawRent();

                SpriteBatch.Draw(SpriteBatch.Pixel, new Rectangle(4, 354, 200, 26), Color.Black * 0.5f);
                switch (SelectedHotbarSlot)
                {
                    case ItemSlots.Weapon:
                        MainFont.Draw(SpriteBatch.Batch, "Left click: Fire weapon", new Vector2(6, 356), Color.White);
                        break;
                    case ItemSlots.Food:
                        MainFont.Draw(SpriteBatch.Batch, "Left click (on animal): Feed animal", new Vector2(6, 356), Color.White);
                        MainFont.Draw(SpriteBatch.Batch, "Right click: Heal yourself", new Vector2(6, 368), Color.White);
                        break;
                    case ItemSlots.Seeds:
                        MainFont.Draw(SpriteBatch.Batch, "Left click (on farm tile): Plant seeds", new Vector2(6, 356), Color.White);
                        break;
                    case ItemSlots.WaterBasin:
                        MainFont.Draw(SpriteBatch.Batch, "Left click (on farm tile): Water farm tile", new Vector2(6, 356), Color.White);
                        break;
                }

                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);
                SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

                SpriteBatch.Draw(ScreenTarget, GraphicsDevice.Viewport.Bounds, Color.Lerp(Color.White, Color.Black, GameFade));

                SpriteBatch.End();
            }

            base.GameDraw(gameTime);
        }
    }
}
