﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Tank_Attack.Objects
{
    public class Enemy : Tank, ICloneable
    {
        public bool Boss;

        private float followDistance = 500f;

        private char spawnDirection;

        private bool currentlyFacingPlayer = true;
        private bool playerInSight;

        private Random random = new Random();

        public Enemy(bool Boss)
            : base()
        {
            _enemy = true;
            this.Boss = Boss;
        }

        public override void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            var playerPosition = player.Position;

            _rotationVelocity = (float)_velocity / 100;

            if (!player.CamouflageNetEquipped)
            {
                TurretRotate(playerPosition);
            }

            _turretDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _currentTurretAngle), -(float)Math.Sin(MathHelper.ToRadians(90) - _currentTurretAngle));
            _gunpoint = _currentPosition + _turretDirection * _turret.Height / 2;

            var distanceToPlayer = Vector2.Distance(Position, playerPosition);

            if (distanceToPlayer > followDistance && !player.CamouflageNetEquipped)
            {
                Rotate(playerPosition);
                if (currentlyFacingPlayer && !Collision(1, player, enemies) || !WithinWindow(0, _currentPosition, _currentPosition))
                    Motion(playerPosition);
            }

            if (!_reloaded)
                _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_timer > _reloadTime * 1000)
            {
                _reloaded = true;
                _timer = 0;
            }

            if (_reloaded && !player.CamouflageNetEquipped && playerInSight)
            {
                missile.AddMissile(missiles, _turretDirection, _gunpoint, _currentTurretAngle, true, _firepower, _muzzleVelocity);
                Sound.EnemyShot.Play();
                _reloaded = false;
            }
        }

        public void Spawn(List<Enemy> enemies, bool boss, char lastSpawnDirection)
        {
            var enemy = Clone() as Enemy;
            bool spawnPositionGenerated = false;

            int x = 0;
            int y = 0;

            while (!spawnPositionGenerated)
            {
                x = random.Next(-100, Game1.windowWidth + 200);
                y = random.Next(-100, Game1.windowHeight + 200);

                if (x < 0 | x > Game1.windowWidth)
                {
                    if (x < 0)
                    {
                        x = -200;
                        enemy.spawnDirection = 'w';
                    }
                    else
                    {
                        x = Game1.windowWidth + 200;
                        enemy.spawnDirection = 'e';
                    }
                }
                else
                {
                    if (y < Game1.windowHeight / 2)
                    {
                        y = -200;
                        enemy.spawnDirection = 'n';
                    }
                    else
                    {
                        y = Game1.windowHeight + 200;
                        enemy.spawnDirection = 's';
                    }
                }

                if (enemy.spawnDirection != lastSpawnDirection)
                    spawnPositionGenerated = true;
            }

            lastSpawnDirection = enemy.spawnDirection;

            int t = 0;

            if (boss)
            {
                enemy.Boss = true;
                t = Game1.PossibleBosses[random.Next(0, Game1.PossibleBosses.Count)];
            }
            else
            {
                enemy.Boss = false;
                t = Game1.PossibleEnemies[random.Next(0, Game1.PossibleEnemies.Count)];
            }

            enemy.name = Game1.Vehicles[t].getModel();
            enemy._turret = MainGame.Content.Load<Texture2D>("Textures/" + enemy.name + "/turret");
            enemy._chassis = MainGame.Content.Load<Texture2D>("Textures/" + enemy.name + "/chassis");
            
            enemy._currentPosition = new Vector2(x, y);

            enemy._enemy = true;

            enemy._initialHealth = (int)(Game1.Vehicles[t].getHealth() * 0.70);
            enemy._firepower = (int)(Game1.Vehicles[t].getFirepower() * 0.75);
            enemy._velocity = Game1.Vehicles[t].getVelocity() * 0.80;
            enemy._reloadTime = Game1.Vehicles[t].getReloadTime();
            enemy._muzzleVelocity = Game1.Vehicles[t].getMuzzleVelocity();

            enemy._origin = new Vector2(enemy._chassis.Width / 2, enemy._chassis.Height - Game1.Vehicles[t].getChassisOriginSpacing());
            enemy._turretOrigin = new Vector2(enemy._turret.Width / 2, enemy._turret.Height - Game1.Vehicles[t].getTurretOriginSpacing());

            enemy._health = enemy._initialHealth;

            enemies.Add(enemy);
        }

        public void TurretRotate(Vector2 playerPosition)
        {
            var targetAngle = (float)(Math.Atan2(playerPosition.Y - _currentPosition.Y, playerPosition.X - _currentPosition.X) + MathHelper.ToRadians(90));

            if ((targetAngle > _currentTurretAngle - 0.055f && targetAngle < _currentTurretAngle + 0.055f))
            {
                _currentTurretAngle = targetAngle;
            }

            if (targetAngle == _currentTurretAngle)
            {
                playerInSight = true;
                return;
            }
            else
                playerInSight = false;

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

            _currentTurretAngle += 0.03f * _moveDirection;

            if (_currentTurretAngle >= 1.5 * MathF.PI)
                _currentTurretAngle = MathHelper.ToRadians(-90);
            if (_currentTurretAngle < -MathF.PI / 2)
                _currentTurretAngle = MathHelper.ToRadians(270);
        }

        public void Rotate(Vector2 playerPosition)
        {
            var targetChassisAngle = (float)Math.Atan2(playerPosition.Y - _currentPosition.Y, playerPosition.X - _currentPosition.X) + MathHelper.ToRadians(90);

            if ((targetChassisAngle > _chassisRotation - 0.055f && targetChassisAngle < _chassisRotation + 0.055f)) // If angle is very close to the target angle, the current angle will be set to the target angle to prevent the algorithm from skipping the target angle.
            {
                _chassisRotation = targetChassisAngle;
            }

            if (targetChassisAngle == _chassisRotation)
            {
                currentlyFacingPlayer = true;
                return;
            }
            else
                currentlyFacingPlayer = false;

            if (_chassisRotation < targetChassisAngle)
            {
                if (Math.Abs(targetChassisAngle - _chassisRotation) < MathF.PI)
                    _moveDirection = 1;
                else
                    _moveDirection = -1;
            }
            else
            {
                if (Math.Abs(targetChassisAngle - _chassisRotation) < MathF.PI)
                    _moveDirection = -1;
                else
                    _moveDirection = 1;
            }

            _chassisRotation += 0.03f * _moveDirection;

            if (_chassisRotation >= 1.5 * MathF.PI)
                _chassisRotation = MathHelper.ToRadians(-90);
            if (_chassisRotation < -MathF.PI / 2)
                _chassisRotation = MathHelper.ToRadians(270);
        }

        public void Motion(Vector2 playerPosition)
        {
            var distance = playerPosition - Position;
            _chassisRotation = (float)Math.Atan2(distance.Y, distance.X) + MathHelper.ToRadians(90);
            _currentChassisDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _chassisRotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _chassisRotation));
            var currentDistance = Vector2.Distance(Position, playerPosition);
            Position += _currentChassisDirection * MathHelper.Min((float)Math.Abs(currentDistance - followDistance), (float)_velocity);
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}