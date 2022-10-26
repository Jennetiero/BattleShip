using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WebApplication
{
    public class GameController
    {
        System.Drawing.Color missedColor = System.Drawing.Color.Yellow;
        System.Drawing.Color hitColor = System.Drawing.Color.Green;
        System.Drawing.Color sinkColor = System.Drawing.Color.Red;
        System.Drawing.Color playerShipsColor = System.Drawing.Color.Black;
        public GameController(BattleShip owner)
        {
            this.myMainPage = owner;
        }
        List<Ship> PlayerShips
        {
            get
            {
                return myMainPage.playerShips;
            }
        }
        List<Ship> ComputerShips
        {
            get
            {
                return myMainPage.computerShips;
            }
        }
        public GridView GridView1
        {
            get
            {
                return myMainPage.GridView1;
            }
        }
        public GridView GridView2
        {
            get
            {
                return myMainPage.GridView2;
            }
        }
        public BattleShip myMainPage;
        public int StartPoX
        {
            get
            {
                return myMainPage.StartPoX;
            }
            set
            {
                myMainPage.StartPoX = value;
            }
        }
        public int StartPoY
        {
            get
            {
                return myMainPage.StartPoY;
            }
            set
            {
                myMainPage.StartPoY = value;
            }
        }
        public void AddLog(string msg)
        {
            myMainPage.AddLog(msg);
        }
        public string GetShipName(int width)
        {
            switch (width)
            {
                case 1: return "Submarine";
                case 2: return "Destroyer";
                case 3: return "Cruiser";
                case 4: return "BattleShip";
                default: return "Unknown";
            }
        }
        string StatusText
        {
            get
            {
                return myMainPage.StatusText;
            }
            set
            {
                myMainPage.StatusText = value;
            }
        }
        bool ComputerTurn
        {
            get
            {
                return myMainPage.ComputerTurn;
            }
            set
            {
                myMainPage.ComputerTurn = value;
            }
        }
        public void CellPlayerClicked(int row, int col)
        {
            if (PlayerShips.Count > 9)
            {
                return;
            }
            if (GridView1.Rows[row].Cells[col].BackColor == playerShipsColor)
            {
                return;
            }
            int curWid;
            if (PlayerShips.Count < 4)
            {
                if (ClearPath(row, col, row, col))
                {
                    string name = GetShipName(1);
                    string cord = row + "," + col;
                    Ship ship = new Ship(name, 1);
                    ship.cords.Add(cord);
                    PlayerShips.Add(ship);
                    GridView1.Rows[row].Cells[col].BackColor = System.Drawing.Color.Black;
                }
                return;
            }
            else if (PlayerShips.Count < 7) curWid = 2;
            else if (PlayerShips.Count < 9) curWid = 3;
            else if (PlayerShips.Count < 10) curWid = 4;
            else
            {
                AddLog("You have already positioned all your ships, please click start game");
                return;
            }
            if (StartPoX >= 0)
            {
                if (row == StartPoX && col == StartPoY)
                {
                    GridView1.Rows[row].Cells[col].BackColor = GridView1.BackColor;
                    StartPoY = StartPoX = -1;
                    ClearGrayColors();
                    return;
                }

                if (GridView1.Rows[row].Cells[col].BackColor == System.Drawing.Color.Gray)
                {
                    int topPos = StartPoX, endPoX = row;
                    int leftPos = StartPoY, endPosY = col;
                    List<string> cords = new List<string>();
                    if (topPos > endPoX)
                    {
                        topPos = row;
                        endPoX = StartPoX;
                    }
                    if (leftPos > endPosY)
                    {
                        leftPos = col;
                        endPosY = StartPoY;
                    }
                    if (topPos == endPoX)
                    {
                        for (int x = leftPos; x <= endPosY; x++)
                        {
                            string cor = topPos + "," + x;
                            cords.Add(cor);
                            GridView1.Rows[topPos].Cells[x].BackColor = System.Drawing.Color.Black;
                        }
                    }
                    else if (leftPos == endPosY)
                    {
                        for (int x = topPos; x <= endPoX; x++)
                        {
                            string cor = x + "," + leftPos;
                            cords.Add(cor);
                            GridView1.Rows[x].Cells[leftPos].BackColor = System.Drawing.Color.Black;
                        }
                    }
                    string name = GetShipName(curWid);
                    string next = "", msg = "";
                    if (PlayerShips.Count < 4) next = GetShipName(1);
                    else if (PlayerShips.Count < 7) next = GetShipName(2);
                    else if (PlayerShips.Count < 9) next = GetShipName(3);
                    else if (PlayerShips.Count < 10) next = GetShipName(4);
                    StatusText = msg;
                    ClearGrayColors();
                    Ship ship = new Ship(name, curWid);
                    ship.cords = cords;
                    PlayerShips.Add(ship);
                    AddLog(msg);
                }
            }
            else
            {
                GridView1.Rows[row].Cells[col].BackColor = System.Drawing.Color.Black;
                StartPoX = row; StartPoY = col;
                CreateGraySquares(row, col, curWid - 1);
            }
        }
        private void CreateGraySquares(int startX, int startY, int width)
        {
            int pos = 0;
            if (ClearPath(startX, startY, startX + width, startY))
            {
                GridView1.Rows[startX + width].Cells[startY].BackColor = System.Drawing.Color.Gray;
                pos++;
            }
            if (ClearPath(startX, startY, startX - width, startY))
            {
                GridView1.Rows[startX - width].Cells[startY].BackColor = System.Drawing.Color.Gray;
                pos++;
            }
            if (ClearPath(startX, startY, startX, startY + width))
            {
                GridView1.Rows[startX].Cells[startY + width].BackColor = System.Drawing.Color.Gray;
                pos++;
            }
            if (ClearPath(startX, startY, startX, startY - width))
            {
                GridView1.Rows[startX].Cells[startY - width].BackColor = System.Drawing.Color.Gray;
                pos++;
            }
        }
        private bool ClearPath(int startx, int starty, int endx, int endy)
        {
            if (startx > endx)
            {
                int temp = endx;
                endx = startx;
                startx = temp;
            }
            if (starty > endy)
            {
                int temp = endy;
                endy = starty;
                starty = temp;
            }
            if (startx < 0 || endx > 9) return false;
            if (starty < 0 || endy > 9) return false;
            List<string> cords = new List<string>();
            if (endy == starty)
            {
                for (int x = startx; x < endx; x++)
                    cords.Add(x + "," + starty);
            }
            else if (endx == startx)
            {
                for (int y = starty; y < endy; y++)
                    cords.Add(startx + "," + y);
            }
            foreach (var x in PlayerShips.ToArray())
            {
                foreach (var cord in x.cords.ToArray())
                    if (cords.Contains(cord)) return false;
            }
            return true;
        }
        private void ClearGrayColors()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (GridView1.Rows[x].Cells[y].BackColor == System.Drawing.Color.Gray)
                        GridView1.Rows[x].Cells[y].BackColor = GridView1.BackColor;
                }
            }
            StartPoX = StartPoY = -1;
        }
        private Random Rand
        {
            get { return myMainPage.rand; }
        }
        private void MissComputerShip(int row, int col)
        {
            GridView2.Rows[row].Cells[col].BackColor = missedColor;
            ComputerTurn = true;
        }
        private void EndGame(byte winner)
        {
            myMainPage.EndGame(winner);
        }
        private void DoHitComputerShip(int row, int col)
        {
            Ship target = null;
            string cord = row + "," + col;
            var alive = ComputerShips.Where(z => z.isAlive).ToArray();
            if (alive == null || alive.Length < 1)
            {
                EndGame(1); return;
            }
            foreach (var sh in ComputerShips)
            {
                if (sh.cords.Contains(cord))
                {
                    target = sh;
                    break;
                }
            }
            if (target == null)
            {
                MissComputerShip(row, col);
                return;
            }
            target.hitscords.Add(cord);
            target.hitsrecieved++;
            AddLog("You did hit a computer's " + target.name);
            GridView2.Rows[row].Cells[col].BackColor = hitColor;
            if (!target.isAlive)
            {
             AddLog("You sank computer's " + target.name);
                if (alive.Length == 1)
                {
                    GridView2.Rows[row].Cells[col].BackColor = hitColor;
                    EndGame(1);
                    return;
                }
            }
            ComputerTurn = true;
        }
        public void TryHitComputerShips(int row, int col)
        {
            if (row < 0 || col < 0 || row > 9 || col > 9)
            {
                return;
            }
            var color = GridView2.Rows[row].Cells[col].BackColor;
            if (color == hitColor || color == missedColor || color == hitColor)
            {
                return;
            }
            DoHitComputerShip(row, col);
        }
        public void TryHitPlayerShips()
        {
            var prevHitsAlive = PlayerShips.Where(z => z.isAlive && z.hitsrecieved > 0).FirstOrDefault();
            if (prevHitsAlive != null && prevHitsAlive.name != "")
            {
                AddLog("Found prev hit ship, trying to sink it");
                if (TrySinkPlayerShip(prevHitsAlive))
                    return;
            }
            int row = Rand.Next(10);
            int col = Rand.Next(10);
            if (row < 0 || col < 0 || row > 9 || col > 9)
            {
                AddLog("Cell is out of range");
                return;
            }
            var color = GridView1.Rows[row].Cells[col].BackColor;
            if (color == hitColor || color == missedColor)
            {
                TryHitPlayerShips();
                return;
            }
            else if (color == GridView1.BackColor)
            {
                MissPlayerShip(row, col);
                return;
            }
            DoHitPlayerShip(row, col);
        }
        private bool TrySinkPlayerShip(Ship target)
        {
            //try the down cells
            int row = target.lastHitX, col = target.lastHitY;
            bool hit = false;
            string cord = row + "," + col;
            if (row < 9)
            {
                while (!hit)
                {
                    row++;
                    if (row > 9) break;
                    cord = row + "," + col;
                    if (target.cords.Contains(cord) && !target.hitscords.Contains(cord))
                    {
                        hit = true;
                        DoHitPlayerShip(row, col);
                        break;
                    }
                    if (GridView1.Rows[row].Cells[col].BackColor == GridView1.BackColor)
                    {
                        MissPlayerShip(row, col);
                        hit = true;
                        break;
                    }
                    if (GridView1.Rows[row].Cells[col].BackColor == playerShipsColor)
                    {
                        hit = true;
                        DoHitPlayerShip(row, col);
                        break;
                    }
                    if (GridView1.Rows[row].Cells[col].BackColor == missedColor)
                    {
                        break;
                    }


                }
            }
            //try the upper cells
            if (!hit)
            {
                row = target.lastHitX; col = target.lastHitY;
                if (row > 0)
                {
                    while (!hit)
                    {
                        row--;
                        if (row < 0) break;
                        cord = row + "," + col;
                        if (target.cords.Contains(cord) && !target.hitscords.Contains(cord))
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == missedColor)
                        {
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == playerShipsColor)
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == GridView1.BackColor)
                        {
                            MissPlayerShip(row, col);
                            hit = true;
                            break;
                        }
                    }
                }
            }
            //try the right cells
            if (!hit)
            {
                row = target.lastHitX; col = target.lastHitY;
                if (col < 9)
                {
                    while (!hit)
                    {
                        col++;
                        if (col > 9) break;
                        cord = row + "," + col;
                        if (target.cords.Contains(cord) && !target.hitscords.Contains(cord))
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == missedColor)
                        {
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == playerShipsColor)
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == GridView1.BackColor)
                        {
                            MissPlayerShip(row, col);
                            hit = true;
                            break;
                        }
                    }
                }
            }
            //try the left cells
            if (!hit)
            {
                row = target.lastHitX; col = target.lastHitY;
                if (col > 0)
                {
                    while (!hit)
                    {
                        col--;
                        if (col < 0) break;
                        cord = row + "," + col;
                        if (target.cords.Contains(cord) && !target.hitscords.Contains(cord))
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == missedColor)
                        {
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == playerShipsColor)
                        {
                            hit = true;
                            DoHitPlayerShip(row, col);
                            break;
                        }
                        if (GridView1.Rows[row].Cells[col].BackColor == GridView1.BackColor)
                        {
                            MissPlayerShip(row, col);
                            hit = true;
                            break;
                        }
                    }
                }
            }
            if (!hit)
            {
                AddLog("Computer could not sank the ship " + target.name + " last hit " + target.lastHitX + "," + target.lastHitY);
            }
            return hit;
        }
        private void MissPlayerShip(int row, int col)
        {
            GridView1.Rows[row].Cells[col].BackColor = missedColor;
            AddLog("The computer didn't hit you");
            ComputerTurn = false;
        }
        private void DoHitPlayerShip(int row, int col)
        {
            Ship target = null;
            string cord = row + "," + col;
            var alive = PlayerShips.Where(z => z.isAlive).ToArray();
            if (alive == null || alive.Length < 1)
            {
                EndGame(2); return;
            }
            foreach (var sh in alive)
            {
                if (sh.cords.Contains(cord))
                {
                    target = sh;
                    break;
                }
            }
            if (target == null)
            {
                MissPlayerShip(row, col);
                return;
            }
            target.hitscords.Add(cord);
            target.hitsrecieved++;
            GridView1.Rows[row].Cells[col].BackColor = hitColor;
            target.lastHitX = row;
            target.lastHitY = col;
            AddLog("Computer hit your " + target.name);
            if (!target.isAlive)
            {
                AddLog("Computer sank your " + target.name);
                foreach (string c in target.cords)
                {
                    string split = ",";
                    int rw = Convert.ToInt32(c.Substring(0, c.IndexOf(split)));
                    int cl = Convert.ToInt32(c.Substring(c.IndexOf(split) + split.Length));
                    GridView1.Rows[rw].Cells[cl].BackColor = sinkColor;
                }
                if (alive.Length == 1)
                {
                    EndGame(2);
                    return;
                }
            }
            ComputerTurn = false;
        }
    }
}