using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Tank_Attack.Objects
{
    public class Player : Tank
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private bool armourBoostEquipped; public bool ArmourBoostEquipped { get { return armourBoostEquipped;  } set { armourBoostEquipped = value; } }
        private bool camouflageNetEquipped; public bool CamouflageNetEquipped { get { return camouflageNetEquipped; } set { camouflageNetEquipped = value; } }

        private int score; public int Score { get { return score; } set { score = value; } }

        public Player(int tankIndex)
            : base()
        {
            _currentPosition = new Vector2(Game1.windowWidth / 2, Game1.windowHeight / 2);

            _enemy = false;

            armourBoostEquipped = false;
            CamouflageNetEquipped = false;

            var playerTank = Game1.Vehicles[tankIndex].getModel();

            _chassis = MainGame.Content.Load<Texture2D>("Textures/" + playerTank + "/chassis");
            _turret = MainGame.Content.Load<Texture2D>("Textures/" + playerTank + "/turret");

            _initialHealth = Game1.Vehicles[tankIndex].getHealth();
            _firepower = Game1.Vehicles[tankIndex].getFirepower();
            _velocity = Game1.Vehicles[tankIndex].getVelocity();
            _reloadTime = Game1.Vehicles[tankIndex].getReloadTime();
            _muzzleVelocity = Game1.Vehicles[tankIndex].getMuzzleVelocity();

            _origin = new Vector2(_chassis.Width / 2, _chassis.Height - Game1.Vehicles[tankIndex].getChassisOriginSpacing());
            _turretOrigin = new Vector2(_turret.Width / 2, _turret.Height - Game1.Vehicles[tankIndex].getTurretOriginSpacing());

            _health = _initialHealth;

            name = Game1.Vehicles[tankIndex].getModel();

            shellsLoaded = 3;
            if (name == "Progetto 54")
                _shellReloadTime = 5.0;
            _autoreloaderTriggered = false;
            _autoreloaderReady = true;
        }

        public override void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (_velocity > 4.5)
                _rotationVelocity = 0.045f;
            else
                _rotationVelocity = (float)_velocity / 100;

            if (!_reloaded)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_timer > _reloadTime * 1000)
                {
                    _reloaded = true;
                    Sound.Reload.Play();
                    _timer = 0;
                }
            }
            if (name == "Progetto 54")
            {
                if (_autoreloaderTriggered)
                {
                    _autoreloaderTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (_autoreloaderTimer > (_shellReloadTime * 1000))
                    {
                        shellsLoaded++;
                        Sound.Reload.Play();
                        _autoreloaderTimer = 0;
                    }

                    if (shellsLoaded == 3)
                        _autoreloaderTriggered = false;
                }
                else
                {
                     if (currentKeyboardState.IsKeyDown(Keys.E) && previousKeyboardState.IsKeyUp(Keys.E) && shellsLoaded < 3
                        || shellsLoaded == 0) {
                        _autoreloaderTriggered = true;
                    }
                }
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && _reloaded)
            {
                if ( _reloaded && shellsLoaded > 0)
                {
                    missile.AddMissile(missiles, _turretDirection, _gunpoint, _currentTurretAngle, false, _firepower, _muzzleVelocity);
                    Sound.PlayerShot.Play(volume: 0.4f, pitch: 0, pan: 0);

                    _reloaded = false;

                    if (name == "Progetto 54")
                        shellsLoaded--;
                }
                else
                {
                    Sound.Click.Play(volume: 0.7f, pitch: 0, pan: 0);
                }
            }


            Motion(player, enemies);

            var distance = Vector2.Distance(_gunpoint, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            _pointerPosition = _gunpoint + _turretDirection * distance;
        }

        private void Motion(Player player, List<Enemy> enemies)
        {
            _previousPosition = _currentPosition;
            _previousChassisDirection = _currentChassisDirection;
            _wasMoving = _isMoving;


            if (Keyboard.GetState().IsKeyDown(Keys.D))
                _chassisRotation += _rotationVelocity;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                _chassisRotation -= _rotationVelocity;

            _currentChassisDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _chassisRotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _chassisRotation)); // Chassis rotation direction
            var targetAngle = (float)(Math.Atan2(Mouse.GetState().Y - _currentPosition.Y, Mouse.GetState().X - _currentPosition.X) + (MathF.PI / 2)); // Turret rotation angle
            _turretDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _currentTurretAngle), -(float)Math.Sin(MathHelper.ToRadians(90) - _currentTurretAngle));

            if (targetAngle != _currentTurretAngle)
            {
                if (_currentTurretAngle < targetAngle)
                {
                    if (Math.Abs(targetAngle - _currentTurretAngle) < MathF.PI)
                        _moveDirection = 1;
                    else
                        _moveDirection = -1;
                }
                else
                {
                    if (Math.Abs(targetAngle - _currentTurretAngle) < MathF.PI)
                        _moveDirection = -1;
                    else
                        _moveDirection = 1;
                }
            }

            _currentTurretAngle += 0.05f * _moveDirection;

            if ((targetAngle > _currentTurretAngle - 0.055f && targetAngle < _currentTurretAngle + 0.055f))
            {
                _currentTurretAngle = targetAngle;
            }

            if (_currentTurretAngle >= 1.5 * MathF.PI)
                _currentTurretAngle = MathHelper.ToRadians(-90);
            if (_currentTurretAngle < -MathF.PI / 2)
                _currentTurretAngle = MathHelper.ToRadians(270);

            _gunpoint = _currentPosition + _turretDirection * _turret.Height / 2;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && WithinWindow(1, _currentPosition, _currentChassisDirection * (float)_velocity) && !Collision(1, player, enemies))
            {
                _currentPosition += _currentChassisDirection * (float)_velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) && WithinWindow(-1, _currentPosition, _currentChassisDirection * (float)_velocity) && !Collision(-1, player, enemies))
            {
                _currentPosition -= _currentChassisDirection * (float)_velocity;
            }


            if (_currentPosition != _previousPosition | _currentChassisDirection != _previousChassisDirection)
                _isMoving = true;
            else
                _isMoving = false;

            if (_isMoving && !_wasMoving)
                Sound.MotionStart();
            if (!_isMoving && _wasMoving)
                Sound.MotionStop();
        }

        
    }
}