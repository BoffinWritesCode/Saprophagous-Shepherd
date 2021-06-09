using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;

using MiniJam61Egypt.Util;

namespace MiniJam61Egypt.GameContent.Entities.Animals
{
    public class Animal : Entity
    {
        public static readonly string[] NAMES = new string[]
        {
            "Kirito",
            "Joshy",
            "Matty",
            "Stupidugly",
            "Fatty",
            "Neal",
            "Carly",
            "Sam",
            "Freddie",
            "Spencer",
            "Gibby",
            "Bean",
            "Jimbob",
            "Bobjim",
            "Big",
            "Richard",
            "Saitama",
            "Valter",
            "Vitomir",
            "Yehudah",
            "Zdravko",
            "Fehim",
            "Irving",
            "Nsonowa",
            "Seth",
            "Joel",
            "Hadrien",
            "Kermit",
            "Nicomedes",
            "Govinda",
            "Aseem",
            "Tomato",
            "Lycus",
            "Wouter",
            "Kusuma",
            "Socrates",
            "Ernesto",
            "Henry",
            "Ralph",
            "Ricky",
            "Emanuel",
            "Fred",
            "Morty",
            "Gordon",
            "Fin",
            "Jordan",
            "Steve",
            "Collin",
            "Pepper",
            "Harvey",
            "Cory",
            "Greg",
            "Tommy",
            "Paige",
            "Bane",
            "Daniel",
            "Luther",
            "Luke",
            "Doug",
            "Conrad",
            "Gregg",
            "Ian",
            "Anthony",
            "Chandler",
            "Ross",
            "Joey",
            "Donnie",
            "John",
            "Troy Sullivan",
            "Jon",
            "Lloyd",
            "Elbert",
            "Darrin",
            "Christopher",
            "Chris",
            "Francis",
            "Terry",
            "Taylor",
            "Adrian",
            "Darren",
            "Carl",
            "Guy",
            "Hugh",
            "Chester",
            "Andrew",
            "Harrison",
            "Michael",
            "Fredrick",
            "Lincoln",
            "Gil",
            "Dario",
            "Porter",
            "Stan",
            "Vern",
            "Gregorio",
            "Audible (over 200,000 books!)",
            "Hal",
            "Omer",
            "Segata",
            "Randy",
            "One",
            "Donald",
            "Goat",
            "Stacy",
            "Ronda",
            "Holly",
            "Regina",
            "Tisha",
            "Lucy",
            "Sophie",
            "Sophia",
            "Tawana",
            "Charlotte",
            "Elizabeth",
            "Jonell",
            "Ashley",
            "Sharon",
            "Jennifer",
            "Joan",
            "Zelda",
            "Kylie",
            "Katy",
            "Maddy",
            "Collin",
            "Alina",
            "Emily",
            "Emma",
            "Ellie",
            "Albert",
            "Cuba",
            "Precious",
            "Princess",
            "Chevette",
            "Courtney",
            "Rachel",
            "Pheobe",
            "Hannah",
            "Miley",
            "Coke",
            "Audrey",
            "Kayla",
            "Bella",
            "Ayana",
            "Caitlin",
            "Kierra",
            "Jamiya",
            "Nora",
            "Elle",
            "Eve",
            "Evie",
            "Alicia",
            "Isabelle",
            "Maddy",
            "Aleah",
            "Sanaa",
            "Tessa",
            "Aylin",
            "Riley",
            "Tori",
            "Ayanna",
            "Maren",
            "Yaritza",
            "Marlie",
            "Angelica",
            "Fernanda",
            "Summer",
            "Hana",
            "Tia",
            "Carmen",
            "Kamila",
            "Laila",
            "Rhianna",
            "Rayne",
            "Cara",
            "Eva",
            "Maggie",
            "Nicole",
            "Madeline",
            "Tania",
            "Nola",
            "Katie",
            "Giana",
            "Lyana",
            "Diamond",
            "Jazmine",
            "Zara",
            "Reyna",
            "Tatum",
            "Alani",
            "Karen",
            "Estrella",
            "Avery",
            "Isabell",
            "Imani",
            "Jadyn",
            "Mikaela",
            "Carly",
            "Cameron",
            "Alannah",
            "Sarah",
            "Elsie",
            "Nancy",
            "Ryann",
            "Laci",
            "Nancy",
            "Aliza",
            "Nobell",
            "Crystal",
            "Daniella",
            "Nom",
            "Porsche",
            "Anabel",
            "Rosa",
            "Rose",
            "Yerile",
            "Giada",
            "Brylee",
            "Diya",
            "Lorena",
            "Olivia",
            "Sarah",
            "Azure",
            "Trilby Dogtooth",
            "Sonic the Hedgehog",
            "Fat Idiot",
            "The Boy",
            "Sbeve"
        };
        public static readonly Color GOOD_HOVER = new Color(58, 173, 255);
        public static readonly Color BAD_HOVER = new Color(232, 49, 114);
        public static readonly float CLICK_DISTANCE = 128f;

        public bool Following => _currentTask == 3;

        public override bool Solid => false;
        public override bool CanHover => true;
        public virtual int Type => -1;

        public int ID;
        public bool IsBaby;
        private int _starveCounter;

        public string Name { get; private set; }

        private const float MaxSpeed = 200f;
        private const float Acceleration = 800f;

        public bool OutOfControl;

        protected Rectangle _explore;

        protected bool _facingLeft;
        private float _growthTimer;
        protected int _currentTask;
        private float _taskCounter;
        private int _breedTaskCounter;
        private Vector2 _targetDestination;
        private Animal _breedPartner;
        private bool _isBirther;
        private float _breedTimer;
        private float _taskTimer;
        private float _footstepTimer;

        protected int _alt;
        protected float _hunger;
        protected float _maxHunger = 60f;

        public Animal(Rectangle exploreArea)
        {
            _explore = exploreArea;
            _taskTimer = 10f;
            _taskCounter = Main.GameRandom.NextFloat(_taskTimer);
            ID = Main.GameRandom.Next();
            Name = NAMES[Main.GameRandom.Next(NAMES.Length)];
            _hunger = 60f;
            NextTask();
        }

        public virtual Rectangle GetMouseHitbox()
        {
            return Rectangle.Empty;
        }

        public override bool MouseOver()
        {
            Rectangle box = GetMouseHitbox();
            return box.Contains(Main.Instance.MouseInWorld());
        }

        public override void LeftClicked() 
        {
            if (Vector2.Distance(Body.Center, Main.Instance.player.Body.Center) > CLICK_DISTANCE)
            {
                return;
            }

            if (Main.SelectedHotbarSlot == ItemSlots.Food)
            {
                if (Main.Food > 0)
                {
                    _hunger += _maxHunger / (Type + 1);
                    if (_hunger > _maxHunger)
                    {
                        _hunger = _maxHunger;
                    }
                    Main.Food--;
                    Main.SFXManager.PlaySound("Burp_1", 0.1f, 0, Main.GameRandom.NextFloat(-0.1f, 0.1f));
                }
            }
        }

        public override void RightClicked()
        {
            if (Vector2.Distance(Body.Center, Main.Instance.player.Body.Center) > CLICK_DISTANCE)
            {
                return;
            }

            Main.CanShoot = false;
            OutOfControl = false;

            if (_currentTask == 3)
            {
                _currentTask = 1;
                return;
            }
            _currentTask = 3;
        }

        public override void DrawHover(ExtendedSpriteBatch spriteBatch)
        {
        }

        protected virtual void NextTask()
        {
            _taskTimer = Main.GameRandom.NextFloat(8f, 15f);
            if (IsBaby)
            {
                _taskTimer = Main.GameRandom.NextFloat(1f, 4f);
            }

            if (!IsBaby && _breedTaskCounter >= 3)
            {
                _currentTask = 2;
                _breedTaskCounter = 0;
                List<Animal> animals = Main.Instance.animals;
                List<Animal> breeds = new List<Animal>();
                foreach(Animal a in animals)
                {
                    if (a.Type == Type && a.ID != ID && !a.IsBaby && a._currentTask < 2 && _explore.Contains(a.Body.Center))
                    {
                        breeds.Add(a);
                    }
                }

                if (breeds.Count != 0)
                {
                    Animal partner = breeds[Main.GameRandom.Next(breeds.Count)];
                    partner._currentTask = 2;
                    partner._taskCounter = 0f;
                    partner._breedTaskCounter = 0;
                    _currentTask = 2;
                    Vector2 meToPartner = partner.Body.Center - Body.Center;
                    Vector2 middle = Body.Center + meToPartner * 0.5f;
                    partner._targetDestination = middle;
                    _targetDestination = middle - Vector2.Normalize(meToPartner) * 8f;
                    _isBirther = Main.GameRandom.Next(2) == 0;
                    partner._isBirther = !_isBirther;
                    _breedPartner = partner;
                    partner._breedPartner = this;
                    return;
                }
            }

            _currentTask = 1;
            _targetDestination = new Vector2(Main.GameRandom.NextFloat(_explore.X, _explore.Right), Main.GameRandom.NextFloat(_explore.Y, _explore.Bottom));
        }

        private void SmoothMove(TimeManager time)
        {
            Vector2 toDest = _targetDestination - Body.Center;
            float dist = toDest.Length();
            toDest.Normalize();

            float slowDownDist = (-(MaxSpeed * MaxSpeed)) / (2f * -Acceleration);

            float maxSpeed = MaxSpeed * MathHelper.Lerp(0.4f, 1f, Health / MaxHealth);
            if (dist < slowDownDist)
            {
                maxSpeed *= (dist / slowDownDist);
                if (maxSpeed == float.NaN || maxSpeed < 0.1f)
                {
                    maxSpeed = 0.1f;
                }
            }

            Velocity += toDest * Acceleration * time.DeltaTime;

            if (Velocity.Length() > maxSpeed)
            {
                Velocity.Normalize();
                Velocity *= maxSpeed;

                _footstepTimer += time.DeltaTime;
                if (_footstepTimer > 0.2f)
                {
                    _footstepTimer = 0;
                    Vector2 animalInWorld = Body.Center;
                    Point tile = (animalInWorld / 32f).ToPoint();
                    if (tile.X >= 0 && tile.X < Main.Instance.world.Width && tile.Y >= 0 && tile.Y < Main.Instance.world.Height)
                    {
                        if (Main.Instance.world[tile.X, tile.Y].HasFootsteps)
                        {
                            if (Type < 2)
                            {
                                Main.Instance.entities.Add(new Other.Footstep(new Vector2(Body.Center.X + Main.GameRandom.Next(-2, 3), Body.Bottom - 4f + Main.GameRandom.Next(-2, 3))));
                            }
                            else
                            {
                                int amt = 12;
                                if (IsBaby) amt = 6;
                                Main.Instance.entities.Add(new Other.Footstep(new Vector2(Body.Center.X + amt + Main.GameRandom.Next(-2, 3), Body.Bottom - 4f + Main.GameRandom.Next(-2, 3))));
                                Main.Instance.entities.Add(new Other.Footstep(new Vector2(Body.Center.X - amt + Main.GameRandom.Next(-2, 3), Body.Bottom - 4f + Main.GameRandom.Next(-2, 3))));
                            }
                        }
                    }
                }
            }

            if (dist < 1f)
            {
                Body.Center = _targetDestination;
                Velocity = Vector2.Zero;
            }
            else
            {
                _facingLeft = (toDest.X < 0);
            }
        }

        public override void PreUpdate(TimeManager time)
        {
            _hunger -= time.DeltaTime;
            if (_hunger <= 0f)
            {
                _hunger = 0f;
                Health -= 4f * time.DeltaTime;
                if (_starveCounter % 60 == 0)
                {
                    HitColor = new Color(255, 155, 155);
                }
                _starveCounter++;
                if (Health <= 0f)
                {
                    switch(Type)
                    {
                        case 0:
                            Main.SpawnEnemy(new Enemies.SandSplicer(Body.Center));
                            break;
                        case 1:
                            Main.SpawnEnemy(new Enemies.Geb(Body.Center));
                            break;
                        case 2:
                            Main.SpawnEnemy(new Enemies.Anubis(Body.Center));
                            break;
                    }
                }
            }
            else
            {
                _starveCounter = 0;
            }
            float amt = time.DeltaTime;
            if (Health < MaxHealth - amt)
            {
                if (_hunger >= amt)
                {
                    _hunger -= amt;
                    Health += amt;
                }
            }

            if (IsBaby)
            {
                _growthTimer += time.DeltaTime;
                if (_growthTimer > 30f)
                {
                    IsBaby = false;

                    Main.SummonParticlesColorRamp(Main.SmokeTexture, Main.GameRandom.Next(3, 6), Body.Center, Main.GameRandom.NextFloat(MathHelper.TwoPi).ToVector2() * Main.GameRandom.NextFloat(0f, 20f), Vector2.Zero, new Color(160, 160, 160), Color.White, Main.GameRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), 0.2f, 0.5f, 0, MathHelper.TwoPi, 0.6f, 1f, 10f);
                }
            }

            if (_currentTask == 3)
            {
                _targetDestination = Main.Instance.player.Body.Center;
                _taskCounter = 0;
                SmoothMove(time);
                return;
            }

            _taskCounter += time.DeltaTime;
            if (_taskCounter > _taskTimer)
            {
                _breedTaskCounter++;
                _taskCounter = 0f;
                NextTask();
            }

            if (_currentTask == 2 && !IsBaby)
            {
                if (_breedPartner != null)
                {
                    if (_isBirther && Vector2.Distance(Body.Center, _breedPartner.Body.Center) < 32f)
                    {
                        _breedTimer += time.DeltaTime;
                        if (_breedTimer > 1.5f)
                        {
                            _breedPartner._breedTimer = 0f;
                            _breedPartner._currentTask = 0;
                            _breedPartner._taskCounter = 100f;
                            _breedPartner._breedTaskCounter = 0;

                            _currentTask = 0;
                            _taskCounter = 100;
                            _breedTimer = 0f;
                            _breedTaskCounter = 0;

                            Animal baby = null;
                            switch (Type)
                            {
                                default:
                                case 0:
                                    Main.SheepBirths++;
                                    baby = new Sheep(_explore);
                                    if (Main.SheepBirths % 5 == 0)
                                    {
                                        baby = null;
                                        Main.SpawnEnemy(new Enemies.SandSplicer(Body.Center + new Vector2(Main.GameRandom.NextFloat(-4f, 4f), Main.GameRandom.NextFloat(-4f, 4f))));
                                        break;
                                    }
                                    Main.stats.SheepBred++;
                                    break;
                                case 1:
                                    Main.GoatBirths++;
                                    baby = new Goat(_explore);
                                    if (Main.GoatBirths % 5 == 0)
                                    {
                                        baby = null;
                                        Main.SpawnEnemy(new Enemies.Geb(Body.Center + new Vector2(Main.GameRandom.NextFloat(-4f, 4f), Main.GameRandom.NextFloat(-4f, 4f))));
                                        break;
                                    }
                                    Main.stats.GoatsBred++;
                                    break;
                                case 2:
                                    Main.CamelBirths++;
                                    baby = new Camel(_explore);
                                    if (Main.CamelBirths % 5 == 0)
                                    {
                                        baby = null;
                                        Main.SpawnEnemy(new Enemies.Anubis(Body.Center + new Vector2(Main.GameRandom.NextFloat(-4f, 4f), Main.GameRandom.NextFloat(-4f, 4f))));
                                        break;
                                    }
                                    Main.stats.CamelsBred++;
                                    break;
                            }
                            if (baby != null)
                            {
                                baby._currentTask = 0;
                                baby.IsBaby = true;
                                baby.Body.Center = Body.Center + new Vector2(Main.GameRandom.NextFloat(-4f, 4f), Main.GameRandom.NextFloat(-4f, 4f));
                                baby._alt = _alt;
                                Main.Instance.entities.Add(baby);
                                Main.Instance.animals.Add(baby);
                            }
                        }
                    }
                }
            }

            if (!OutOfControl)
            {
                SmoothMove(time);
            }
        }

        public override void PostUpdate(TimeManager time)
        {
            base.PostUpdate(time);

            if (Health <= 0f)
            {
                Kill();
            }

            if (!Destroy)
            {
                Vector2 animalInWorld = Body.Center;
                Point tile = (animalInWorld / 32f).ToPoint();
                if (tile.X >= 0 && tile.X < Main.Instance.world.Width && tile.Y >= 0 && tile.Y < Main.Instance.world.Height)
                {
                    if (Main.Instance.world[tile.X, tile.Y].AnimalSale)
                    {
                        int amt = 0;
                        switch(Type)
                        {
                            case 0:
                                amt = IsBaby ? 25 : 50;
                                Main.stats.SheepSold++;
                                break;
                            case 1:
                                amt = IsBaby ? 90 : 180;
                                Main.stats.GoatsSold++;
                                break;
                            case 2:
                                amt = IsBaby ? 225 : 450;
                                Main.stats.CamelsSold++;
                                break;
                        }
                        Main.Coins += amt;
                        Main.stats.MoneyEarnt += amt;
                        Main.Instance.entities.Add(new Entities.Other.XGain(Main.SmallCoinTexture, Body.Center - new Vector2(0, 12), amt));
                        Kill();

                        for (int i = 0; i < Main.GameRandom.Next(15, 25); i++)
                        {
                            Main.SummonParticlesColorRamp(Main.DollarTexture, 1, Body.Center, Main.GameRandom.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToVector2() * Main.GameRandom.NextFloat(50f, 100f), Vector2.Zero, new Color(21, 211, 0), new Color(20, 143, 78), 0, 0.2f, 0.5f, 0, 0, 1, 1, 2f);
                        }

                        Main.SFXManager.PlaySound("Cash_Register", 0.1f);
                    }
                }
            }
        }

        public override void Kill()
        {
            Destroy = true;

            Rectangle space = GetDrawBody();
            for (int i = 0; i < Main.GameRandom.Next(4, 9); i++)
            {
                Main.SummonParticlesColorRamp(Main.SmokeTexture, 1, new Vector2(space.X + Main.GameRandom.NextFloat(space.Width), space.Y + Main.GameRandom.NextFloat(space.Height)), Main.GameRandom.NextFloat(MathHelper.TwoPi).ToVector2() * Main.GameRandom.NextFloat(0f, 20f), Vector2.Zero, new Color(160, 160, 160), Color.White, Main.GameRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), 0.2f, 0.5f, 0, MathHelper.TwoPi, 0.6f, 1f, 10f);
            }

            if (Health <= 0f)
            {
                switch(Type)
                {
                    case 0:
                        Main.stats.SheepKilled++;
                        break;
                    case 1:
                        Main.stats.GoatsKilled++;
                        break;
                    case 2:
                        Main.stats.CamelsKilled++;
                        break;
                }
            }
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            int maxWidth = (int)Body.Width;
            int height = 3;
            int width = (int)((Health / MaxHealth) * maxWidth);
            int width2 = (int)((_hunger / _maxHunger) * maxWidth);

            int x = (int)Body.Center.X - maxWidth / 2;
            int y = (int)Body.Bottom + 1;
            if (Type == 0) y += 2;
            if (Type == 1) y += 7;

            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y, maxWidth, height), new Color(23, 68, 3));
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y, width, height), new Color(82, 196, 33));

            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y + height + 1, maxWidth, height), new Color(81, 18, 13));
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y + height + 1, width2, height), new Color(181, 64, 56));
        }
    }
}