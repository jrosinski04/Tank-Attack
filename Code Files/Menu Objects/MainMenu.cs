using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Tank_Attack
{
    public class MainMenu
    {
        public bool Activated; // Activates the game when true. Can be triggered by either pressing ENTER or clicking the Play button.

        public int VehicleSelection;
        public int TabIndex;
        public int Page;
        public int PagesMax;

        public List<int> TanksAvailable;

        private Texture2D backgroundTexture;
        private Texture2D texture;

        private Rectangle backgroundRectangle;

        private List<Box> boxes;
        private Box box1;
        private Box box2;
        private Box box3;

        private bool selected;

        private SpriteFont font10;
        private SpriteFont font14;
        private SpriteFont font20;

        private string intro = "Welcome. Please choose your vehicle.";

        private Btn playButton;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        public MainMenu(
            ContentManager Content,
            GraphicsDevice graphicsDevice,
            int windowWidth, int windowHeight,
            Texture2D button,
            SpriteFont Font10, SpriteFont Font12, SpriteFont Font14, SpriteFont Font20)
        {
            Activated = false;
            selected = false;

            TanksAvailable = new List<int>();

            for (int i = 0; i < Game1.Vehicles.Count; i++)
            {
                if (Game1.Vehicles[i].isAvailableToPlayer())
                    TanksAvailable.Add(i);
            } 

            PagesMax = (int)Math.Ceiling((float)TanksAvailable.Count / 3);
            Page = 1;

            backgroundTexture = new Texture2D(graphicsDevice, 1, 1); backgroundTexture.SetData(new Color[] { Color.Gray });

            var backgroundWidth = (int)(windowWidth * 0.95);
            var backgroundHeight = (int)(windowHeight * 0.9);

            backgroundRectangle = new Rectangle((windowWidth - backgroundWidth) / 2, (windowHeight - backgroundHeight) / 2, backgroundWidth, backgroundHeight);

            int boxWidth = (int)(backgroundRectangle.Width * 0.2);
            int boxHeight = (int)(backgroundRectangle.Height * 0.7);
            int xSpacing = (int)(backgroundRectangle.Width * 0.1);
            int ySpacing = (int)(backgroundRectangle.Height * 0.15);

            boxes = new List<Box>();
            box1 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, backgroundRectangle.X + xSpacing, xSpacing, ySpacing, 0);
            box2 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, box1.Rectangle.X + boxWidth + xSpacing, xSpacing, ySpacing, 1);
            box3 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, box2.Rectangle.X + boxWidth + xSpacing, xSpacing, ySpacing, 2);

            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);

            box1.Click += Box1_Click;
            box2.Click += Box2_Click;
            box3.Click += Box3_Click;

            font10 = Font10;
            font20 = Font20;
            font14 = Font14;

            playButton = new Btn(Content.Load<Texture2D>("Textures/button"), Font12, "PLAY GAME", false, windowWidth / 2, windowHeight * 0.85f);

            playButton.Click += PlayButton_Click;

            TabIndex = -1;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Activate();
        }

        private void Box3_Click(object sender, EventArgs e)
        {
            if (box3.Active)
            {
                Selection(box3);
                TabIndex = 2;
            }
        }

        private void Box2_Click(object sender, EventArgs e)
        {
            if (box2.Active)
            {
                Selection(box2);
                TabIndex = 1;
            }
        }

        private void Box1_Click(object sender, EventArgs e)
        {
            if (box1.Active)
            {
                Selection(box1);
                TabIndex = 0;
            }
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.NumPad3) && previousKeyboardState.IsKeyUp(Keys.NumPad3))
            {
                VehicleSelection = 3;
                playButton.Available = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad4) && previousKeyboardState.IsKeyUp(Keys.NumPad4))
            {
                VehicleSelection = 4;
                playButton.Available = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad5) && previousKeyboardState.IsKeyUp(Keys.NumPad5))
            {
                VehicleSelection = 5;
                playButton.Available = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad6) && previousKeyboardState.IsKeyUp(Keys.NumPad6))
            {
                VehicleSelection = 6;
                playButton.Available = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Tab) && previousKeyboardState.IsKeyUp(Keys.Tab)
                && TabIndex + ((Page - 1) * 3) < TanksAvailable.Count)
            {
                do
                {
                    TabIndex++;
                    if (TabIndex > 2)
                    {
                        TabIndex = 0;
                        Page++;
                        PageChange(Page);
                    }
                } while (boxes[TabIndex].Active == false);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down) && Page != PagesMax ||
                currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right) && Page != PagesMax ||
                currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up) && Page > 1 ||
                currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left) && Page > 1)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.Right))
                    Page++;
                else
                    Page--;

                PageChange(Page);
            }

            foreach (var box in boxes) 
            {
                box.Update(gameTime);
                if (TabIndex != -1)
                {
                    var selectedBox = boxes[TabIndex];

                    if (selectedBox.TankIndex < TanksAvailable.Count)
                    {
                        selectedBox.Colour = Box.ColourSelection;
                        selected = true;
                        VehicleSelection = TanksAvailable[selectedBox.TankIndex];
                        playButton.Available = true;
                    }
                    
                }
            }

            playButton.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                Activate();
        }

        public void Selection(Box selectedBox)
        {
            if (selectedBox.TankIndex < TanksAvailable.Count)
            {
                if (selectedBox.Colour == Box.ColourSelection)
                {
                    selectedBox.Colour = Box.ColourDefault;

                    playButton.Available = false;
                }
                else
                {
                    selected = true;
                    selectedBox.Colour = Box.ColourSelection;
                    VehicleSelection = TanksAvailable[selectedBox.TankIndex];

                    foreach (var box in boxes)
                    {
                        if (box != selectedBox)
                            box.Colour = Box.ColourDefault;
                    }

                    playButton.Available = true;
                }
            }
            
        }

        public void PageChange(int page)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                int newIndex = i + ((page - 1) * 3);

                if (newIndex < Game1.Vehicles.Count)
                    boxes[i].TankIndex = newIndex;
                else
                    boxes[i].TankIndex = -1;
            }
        }

        public void Activate()
        {
            if (playButton.Available)
                Activated = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);
            spriteBatch.DrawString(font20, intro, new Vector2(backgroundRectangle.X + (backgroundRectangle.Width / 2) - (font20.MeasureString(intro).X / 2), backgroundRectangle.Y + 40), Color.White);
            spriteBatch.DrawString(font10, "Page " + Page + " of " + PagesMax, new Vector2(backgroundRectangle.X + 20, backgroundRectangle.Height), Color.White);
            spriteBatch.DrawString(font10, "Use arrow keys to switch pages", new Vector2(backgroundRectangle.X + 20, backgroundRectangle.Height + 20), Color.White);

            foreach (var box in boxes)
            {
                if (box.TankIndex < TanksAvailable.Count && box.TankIndex >= 0)
                {
                    spriteBatch.Draw(box.Texture, box.Rectangle, box.Colour);
                    spriteBatch.Draw((Texture2D)Game1.Vehicles[TanksAvailable[box.TankIndex]].getProfileImage(), box.Rectangle, Color.White);
                }
                

            }

            playButton.Draw(gameTime, spriteBatch);
        }
    }
}