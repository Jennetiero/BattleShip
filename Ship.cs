using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class Ship
    {
        public string name = "";
        public int holesCount = 2, hitsrecieved = 0;
        public int uid = 0;
        public List<string> cords = new List<string>();
        public List<string> hitscords = new List<string>();
        public int lastHitX = -1, lastHitY = -1;
        public Ship(string nam, int hol)
        {
            this.name = nam; this.holesCount = hol;
            cords = new List<string>(hol);
        }
        public bool isAlive
        {
            get
            {
                return hitsrecieved < holesCount;
            }
        }
    }
}