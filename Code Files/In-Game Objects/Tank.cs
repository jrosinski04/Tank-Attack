﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using System.Net.Http.Headers;

namespace Tank_Attack.Objects
{
    public class Tank
    {
        protected Texture2D _chassis; public Texture2D Chassis { get { return _chassis; } }
        protected Texture2D _turret; public Texture2D Turret { get { return _turret; } }
        protected Texture2D _shell; public Texture2D shell { get { return _shell; } }

        protected SpriteFont _font12;

        protected Vector2 _origin; public Vector2 Origin { set { _origin = value; } }
        protected Vector2 _turretOrigin; public Vector2 TurretOrigin { set { _turretOrigin = value; } }
        protected Vector2 _currentPosition; public Vector2 Position { get { return _currentPosition; } set { _currentPosition = value; } }
        protected Vector2 _previousPosition;
        protected Vector2 _currentChassisDirection;
        protected Vector2 _previousChassisDirection;
        protected Vector2 _turretDirection;
        protected Vector2 _gunpoint;
        protected Vector2 _pointerPosition;

        protected string name;

        protected int _firepower; public int Firepower { get { return _firepower; } set { _firepower = value; } }
        protected int _health; public int Health { get { return _health; } set { _health = value; } }
        protected int _initialHealth; public int InitialHealth { get { return _initialHealth; } }
        protected int _moveDirection;
        protected int shellsLoaded; // Autoreloader only

        protected double _velocity; public double Velocity { get { return _velocity; } set { _velocity = value; } }
        protected double _muzzleVelocity; public double MuzzleVelocity { get { return _muzzleVelocity; } set { _muzzleVelocity = value; } }
        protected float _chassisRotation; public float Rotation { get { return _chassisRotation; } }
        protected float _rotationVelocity = 0.03f;
        protected float _currentTurretAngle = 0;
        protected float _timer;
        protected float _autoreloaderTimer;

        protected double _reloadTime; public double ReloadTime { get { return _reloadTime; } set { _reloadTime = value; } }
        protected double _shellReloadTime; public double ShellReloadTime { get { return _shellReloadTime; } set { _shellReloadTime = value; } }

        protected bool _reloaded;
        protected bool _autoreloaderReady;
        protected bool _autoreloaderTriggered;
        protected bool _isMoving;
        protected bool _wasMoving;
        protected bool _enemy;

        public Tank()
        {
            //_chassis = MainGame.Content.Load<Texture2D>("Textures/" + Game1.Tanks[tankIndex, 0] + "/chassis");
            //_turret = MainGame.Content.Load<Texture2D>("Textures/" + Game1.Tanks[tankIndex, 0] + "/turret");
            _font12 = MainGame.Content.Load<SpriteFont>("Fonts/font12");
            _shell = MainGame.Content.Load<Texture2D>("Textures/shell");

            //_initialHealth = (int)Game1.Tanks[tankIndex, 4];
            //_firepower = (int)Game1.Tanks[tankIndex, 5];
            //_velocity = (float)Game1.Tanks[tankIndex, 6];
            //_reloadTime = (double)Game1.Tanks[tankIndex, 9];
            //_muzzleVelocity = (double)Game1.Tanks[tankIndex, 11];

            //_origin = new Vector2(_chassis.Width / 2, _chassis.Height - (int)Game1.Tanks[tankIndex, 7]);
            //_turretOrigin = new Vector2(_turret.Width / 2, _turret.Height - (int)Game1.Tanks[tankIndex, 8]);

            //_health = _initialHealth;

            //name = (string)Game1.Tanks[tankIndex, 0];
        }

        public virtual void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            
        }

        protected bool Collision(int direction, Player player, List<Enemy> enemies)
        {
            var position = _currentPosition;
            var chassisDirection = _currentChassisDirection;

            position += (_currentChassisDirection * direction) * (float)_velocity;

            if (_enemy)
            {
                if (Vector2.Distance(position, player.Position) <= player._chassis.Height / 2)
                {
                    return true;
                }
            }

            foreach (var enemy in enemies)
            {
                if (Vector2.Distance(position, enemy.Position) <= enemy._chassis.Height * 0.8 && Vector2.Distance(position, enemy.Position) > 10)
                {
                    return true;
                }
            }

            return false;
        }

        public bool WithinWindow(int one, Vector2 origin, Vector2 path)
        {
            Vector2 destination;
            if (one != 0)
                destination = origin + path * one;
            else
                destination = path;

            if (destination.X >= 0 && destination.X <= Game1.windowWidth && destination.Y >= 0 && destination.Y <= Game1.windowHeight)
                return true;
            else
                return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(_chassis, _currentPosition, null, Color.White, _chassisRotation, _origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(_turret, _currentPosition, null, Color.White, _currentTurretAngle, _turretOrigin, 1, SpriteEffects.None, 0f);

            
            var texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });

            var colour = Color.Red;

            if (!_enemy)
            {
                colour = Color.Green;

                spriteBatch.Draw(texture, new Rectangle((int)_pointerPosition.X - 2, (int)_pointerPosition.Y - 2, 5, 5), Color.Black);

                if (!_reloaded)
                {
                    var zero = "";
                    var reloadTimeLeft = ((_reloadTime * 1000) - _timer) / 1000;
                    if (reloadTimeLeft < 1)
                        zero = "0";


                    spriteBatch.DrawString(_font12, "Reloading!", _pointerPosition + new Vector2(5, -30), Color.Red);
                    spriteBatch.DrawString(_font12, zero + reloadTimeLeft.ToString("#.#") + "s left", _pointerPosition + new Vector2(20, -10), Color.Red);
                }

                if (name == "Progetto 54")
                {
                    var zero = "";
                    var reloadTimeLeft = ((_shellReloadTime * 1000) - _autoreloaderTimer) / 1000;
                    if (reloadTimeLeft < 1)
                        zero = "0";

                    if (_autoreloaderTriggered)
                    {
                        spriteBatch.DrawString(_font12, "Reloading Shell", _pointerPosition + new Vector2(-100, 30), Color.Purple);
                        spriteBatch.DrawString(_font12, zero + reloadTimeLeft.ToString("#.#") + "s left", _pointerPosition + new Vector2(-100, 10), Color.Purple);
                    }

                    if (shellsLoaded > 0)
                    {
                        spriteBatch.Draw(_shell, _pointerPosition + new Vector2(15, 15), Color.White);

                        if (shellsLoaded > 1)
                            spriteBatch.Draw(_shell, _pointerPosition + new Vector2(25, 15), Color.White);
                        if (shellsLoaded > 2)
                            spriteBatch.Draw(_shell, _pointerPosition + new Vector2(35, 15), Color.White);

                    }
                    //spriteBatch.DrawString(_font12, string.Concat(Enumerable.Repeat(" ", shellsLoaded)), _pointerPosition + new Vector2(10, 5), Color.Red);
                }
            }
            else
            {
                if (!WithinWindow(0, _currentPosition, _currentPosition))
                {
                    int x;
                    int y;

                    int width;
                    int height;

                    if (_currentPosition.X < 0 || _currentPosition.X > Game1.windowWidth)
                    {
                        y = (int)_currentPosition.Y;
                        width = 50;
                        height = 10;

                        if (_currentPosition.X > Game1.windowWidth)
                            x = Game1.windowWidth - width;
                        else
                            x = 0;
                    }
                    else
                    {
                        width = 10;
                        height = 50;

                        x = (int)_currentPosition.X;
                        if (_currentPosition.Y < 0)
                            y = 0;
                        else
                            y = Game1.windowHeight - height;
                    }

                    spriteBatch.Draw(texture, new Rectangle(x, y, width, height), Color.Red);
                }
            }

            spriteBatch.DrawString(_font12, Health + "/" + InitialHealth, new Vector2(Position.X - 100, Position.Y - 100), colour);
            spriteBatch.DrawString(_font12, name, new Vector2(Position.X - 100, Position.Y - 120), colour);
        }
    }
}
