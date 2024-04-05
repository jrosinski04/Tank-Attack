using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Attack.Code_Files.In_Game_Objects
{
    public class Vehicle
    {
        private int tier;
        private string model;
        private string type;
        private string nation;
        private string year;
        private int health;
        private int firepower;
        private int turretOriginSpacing;
        private int chassisOriginSpacing;
        private double velocity;
        private double muzzleVelocity;
        private double reloadTime;
        private bool availableToPlayer;
        private int[] availableToEnemies;
        private int[] availableToBosses;

        private Texture2D profile;

        public Vehicle(int tier,
            string model, string type, string nation, string year,
            int health, int firepower,
            double velocity, double muzzleVelocity, double reloadTime,
            bool availableToPlayer, int[] availableToEnemies, int[] availableToBosses,
            int chassisOriginSpacing, int turretOriginSpacing)
        {

            this.tier = tier;
            this.model = model;
            this.type = type;
            this.nation = nation;
            this.year = year;
            this.health = health;
            this.firepower = firepower;
            this.velocity = velocity;
            this.muzzleVelocity = muzzleVelocity;
            this.reloadTime = reloadTime;
            this.availableToPlayer = availableToPlayer;
            this.availableToEnemies = availableToEnemies;
            this.availableToBosses = availableToBosses;
            this.chassisOriginSpacing= chassisOriginSpacing;
            this.turretOriginSpacing = turretOriginSpacing;
        }

        public int getTier() { return tier; }

        public string getModel() { return model; }
        public string getType() { return type; }
        public string getNation() { return nation; }
        public string getYear() { return year; }
        public int getHealth() { return health; }
        public int getFirepower() { return firepower; }
        public int getChassisOriginSpacing() { return chassisOriginSpacing; }
        public int getTurretOriginSpacing() { return turretOriginSpacing; } 
        public double getVelocity() { return velocity; }
        public double getMuzzleVelocity() { return muzzleVelocity; }
        public double getReloadTime() { return reloadTime; }
        public bool isAvailableToPlayer() { return availableToPlayer; }
        public int[] getEnemyAvailability() { return availableToEnemies; }
        public int[] getBossAvailability() { return availableToBosses; }
        public Texture2D getProfileImage() { return profile; }

        public void setProfileImage(Texture2D image) { profile = image; }

        public bool compatibleEnemy(int playerTier)
        {
            foreach (var compatibleTier in availableToEnemies)
            {
                if (compatibleTier == playerTier)
                    return true;
            }

            return false;
        }

        public bool compatibleBoss(int playerTier)
        {
            foreach (var compatibleTier in availableToBosses)
            {
                if (compatibleTier == playerTier)
                    return true;
            }

            return false;
        }
    }
}
