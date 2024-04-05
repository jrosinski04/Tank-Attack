using System;
using System.Collections.Generic;
using System.Threading;
using Tank_Attack.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Tank_Attack.Code_Files.In_Game_Objects;

namespace Tank_Attack
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private MainGame mainGame;
        private MainMenu mainMenu;

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 20; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100; // Gets the height of the screen.

        public static List<Vehicle> Vehicles;

        public static List<int> PossibleEnemies = new List<int>();
        public static List<int> PossibleBosses = new List<int>();

        private bool gameIsRunning;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            MediaPlayer.Volume = 0.07f; // Sets the engine sound volume (MotionSound).
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameIsRunning = false;

            Vehicles = new List<Vehicle>();
            Vehicles.Add(new Vehicle(1, "Cruiser IV", "Light", "Britain", "1941", 400, 100, 4.5, 8.5, 2.5, true, new int[] { }, new int[] { }, 90, 105));
            Vehicles.Add(new Vehicle(1, "M4A3E8 'Fury'", "Medium", "USA", "1940", 600, 150, 4.0, 8.0, 3.0, true, new int[] { }, new int[] { }, 70, 40));
            Vehicles.Add(new Vehicle(1, "T-34", "Medium", "USSR", "1940", 400, 150, 5.0, 8.0, 3.0, false, new int[] { 1 }, new int[] { }, 100, 100));
            Vehicles.Add(new Vehicle(1, "Pz. IV H", "Medium", "Germany", "1939", 600, 160, 3.5, 6.0, 3.5, false, new int[] { 1 }, new int[] { }, 80, 60));
            Vehicles.Add(new Vehicle(1, "T-34-85", "Medium", "USSR", "1944", 400, 200, 5.0, 8.0, 3.0, true, new int[] { }, new int[] { }, 90, 100));
            Vehicles.Add(new Vehicle(1, "Churchill VII", "Heavy", "Britain", "1942", 1000, 200, 2.4, 6.5, 3.5, true, new int[] { }, new int[] { }, 79, 90));
            Vehicles.Add(new Vehicle(1, "KV-2", "Heavy", "USSR", "1940", 1200, 600, 2.0, 6.0, 9.0, false, new int[] { 2 }, new int[] { 1 }, 87, 87));
            Vehicles.Add(new Vehicle(1, "Tiger 1", "Heavy", "Germany", "1942", 1600, 500, 4.5, 9.0, 6.0, false, new int[] { 2 }, new int[] { 1 }, 90, 90));
            Vehicles.Add(new Vehicle(2, "Progetto 54", "Heavy", "Italy", "1954", 2500, 600, 4.5, 8.0, 0.5, true, new int[] { }, new int[] { }, 95, 90));
            Vehicles.Add(new Vehicle(2, "60TP", "Heavy", "Poland", "1956", 3000, 1000, 3.5, 8.5, 5.0, true, new int[] { }, new int[] { }, 95, 90));
            Vehicles.Add(new Vehicle(2, "T-72", "Medium", "USSR", "1969", 2000, 500, 6.5, 8.0, 3.5, false, new int[] { 2 }, new int[] { }, 90, 90));
            Vehicles.Add(new Vehicle(3, "T-84", "Medium", "Ukraine", "1994", 3600, 1000, 6.0, 9.0, 3.0, true, new int[] { }, new int[] { }, 100, 100));
            Vehicles.Add(new Vehicle(3, "T-90", "Heavy", "Russia", "1990", 3000, 800, 6.0, 9.0, 3.0, false, new int[] { 3 }, new int[] { 2 }, 100, 100));
            Vehicles.Add(new Vehicle(3, "Leopard 2A7", "Medium", "Germany", "2014", 5500, 1500, 7.2, 9.5, 2.5, true, new int[] { }, new int[] { }, 100, 100));
            Vehicles.Add(new Vehicle(3, "M1A2 Abrams", "Heavy", "USA", "1992", 6000, 1500, 6.8, 10.0, 4.0, true, new int[] { }, new int[] { }, 90, 90));
            Vehicles.Add(new Vehicle(3, "T-14 Armata", "Heavy", "Russia", "2014", 6000, 2000, 7.5, 11.0, 2.0, false, new int[] { }, new int[] { 3 }, 80, 80));


            foreach (var vehicle in Vehicles)
            {
                vehicle.setProfileImage(Content.Load<Texture2D>("Textures/" + vehicle.getModel() + "/profile"));
            }

            LoadMenu(); // Creates an instance of the main menu.
        }

        private void LoadMenu()
        {
            mainMenu = new MainMenu(
                Content,
                GraphicsDevice,
                windowWidth, windowHeight,
                Content.Load<Texture2D>("Textures/button"),
                Content.Load<SpriteFont>("Fonts/font10"),
                Content.Load<SpriteFont>("Fonts/font12"),
                Content.Load<SpriteFont>("Fonts/font14"),
                Content.Load<SpriteFont>("Fonts/font20"));
        }

        private bool GameActivated()
        {
            if (mainMenu.Activated) // Checks if the game has been activated from the main menu.
                return true;
            else
                return false;
        }

        private void LaunchGame()
        {
            gameIsRunning = true;

            // Creates a new instance of the game.
            mainGame = new MainGame(
                this,
                GraphicsDevice,
                Content,
                windowWidth,
                windowHeight,
                mainMenu.VehicleSelection,
                spriteBatch);

            // Loads the sounds.
            Sound.Click = Content.Load<SoundEffect>("Audio/click");
            Sound.Collision = Content.Load<SoundEffect>("Audio/hit");
            Sound.Destruction = Content.Load<SoundEffect>("Audio/destroy");
            Sound.EnemyShot = Content.Load<SoundEffect>("Audio/shot2");
            Sound.PlayerShot = Content.Load<SoundEffect>("Audio/shotSound");
            Sound.Reload = Content.Load<SoundEffect>("Audio/reload");
            Sound.Music = Content.Load<SoundEffect>("Audio/Music");
            Sound.Motion = Content.Load<Song>("Audio/motion");

            PossibleEnemies.Clear();
            PossibleBosses.Clear();

            var playerTier = Vehicles[mainMenu.VehicleSelection].getTier();

            for (int i = 0; i < Vehicles.Count; i++)
            {
                if (Vehicles[i].compatibleEnemy(playerTier))
                    PossibleEnemies.Add(i);
            }

            for (int i = 0; i < Vehicles.Count; i++)
            {
                if (Vehicles[i].compatibleBoss(playerTier))
                    PossibleBosses.Add(i);
            }

            mainMenu.Activated = false;

        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            if (gameIsRunning) // Checks if game is running
            {
                mainGame.Update(gameTime);
                if (mainGame.Restart) // Checks if the Restart option has been chosen.
                {
                    mainGame.Restart = false;
                    LoadContent(); // If the player chose to restart the game, LoadContent() will be called, resetting already existent attributes. 
                }
            }
            else // If game is not running, the Update() method will be called in the mainMenu.
            {
                mainMenu.Update(gameTime);
                if (GameActivated()) // Checks if the game has been activated from the main menu.
                    LaunchGame();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var backgroundColor = Color.LightGray;

            if (gameIsRunning)
            {
                if (mainGame.PlayerDefeated)
                    backgroundColor = Color.Orange;

            }

            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin();

            if (!gameIsRunning)
                mainMenu.Draw(gameTime, spriteBatch);
            else
                mainGame.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Quit()
        {
            this.Exit();
        }
    }
}
