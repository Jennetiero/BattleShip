using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    public partial class BattleShip : System.Web.UI.Page
    {
        DataTable playerTable, computerTable;
        public GridView GridView1, GridView2;
        GameController myGameController;
        TextBox tbLogs;
        Button btnStart, btnNew;
        public string StatusText
        {
            get
            {
                return (string)Session["lblstatustext"];
            }
            set
            {
                lblstatus.Text = value;
                Session["lblstatustext"] = lblstatus.Text;
            }
        }
        public bool Playing
        {
            get
            {
                if (Session["isPlaying"] != null)
                    return (bool)Session["isPlaying"];
                else return false;
            }
            set
            {
                Session["isPlaying"] = value;
            }
        }
        public bool ComputerTurn
        {
            get
            {
                if (Session["computerTurn"] != null)
                    return (bool)Session["computerTurn"];
                else return false;
            }
            set
            {
                Session["computerTurn"] = value;
            }
        }
        bool IsNewGame
        {
            get
            {
                if (Session["newGame"] != null)
                    return (bool)Session["newGame"];
                else return false;
            }
            set
            {
                Session["newGame"] = value;
            }
        }
        public Random rand = new Random();
        public List<Ship> playerShips=new List<Ship>(), computerShips=new List<Ship>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["gv1"] == null)
            {
                myGameController = new GameController(this);
                GenerateGrids();
                GenerateLogTxtBox();
                GenerateButtons();
            }
            else
            {
                ReloadControls();
            }
            if (!IsPostBack)
            {
                CreateGrids();
                StatusText = "Place " + myGameController.GetShipName(1) + " (1 square)";
                Session["lblstatustext"] = lblstatus.Text;
                Session["playerShips"] = playerShips;
                Session["computerShips"] = computerShips;
                Session["myGameControler"] = myGameController;
            }
            else
            {
                playerShips = (List<Ship>)Session["playerShips"];
                computerShips = (List<Ship>)Session["computerShips"];
                if (ComputerTurn && Playing) myGameController.TryHitPlayerShips();
            }
        }
       
        protected void ReloadControls()
        {
            myGameController = (GameController)Session["myGameControler"];
            GridView1 = (GridView)Session["gv1"];
            gv1Holder.Controls.Add(GridView1);
            GridView2 = (GridView)Session["gv2"];
            gv2Holder.Controls.Add(GridView2);
            tbLogs = (TextBox)Session["tblogs"];
            logsHolder.Controls.Add(tbLogs);
            btnStart=(Button)Session["btnStart"];
            btnNew = (Button)Session["btnNew"];
        }
        protected void OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    TableCell cell = e.Row.Cells[i];
                    cell.ToolTip = e.Row.RowIndex + "," + i;
                    cell.Attributes["onclick"] = string.Format("document.getElementById('{0}').value = {1}; {2}"
                       , inhColumn.ClientID, i
                       , Page.ClientScript.GetPostBackClientHyperlink((GridView)sender, string.Format("Select${0}", e.Row.RowIndex)));
                }
            }
        }
        public int StartPoX
        {
            get
            {
                if (Session["startPoX"] != null)
                    return (int)Session["startPoX"];
                else return -1;
            }
            set
            {
                Session["startPoX"] = value;
            }
        }
        public int StartPoY
        {
            get
            {
                if (Session["startPoY"] != null)
                    return (int)Session["startPoY"];
                else return -1;
            }
            set
            {
                Session["startPoY"] = value;
            }
        }
        protected void CheckOldSession()
        {
            if (Session["playerShips"] != null)
                playerShips = Session["playerShips"] as List<Ship>;
            if (Session["computerShips"] != null)
                computerShips = Session["computerShips"] as List<Ship>;
        }
        protected void SelectedIndexChanged(Object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            GridViewRow selectedRow = grid.SelectedRow;
            int rowIndex = grid.SelectedIndex;
            int selectedCellIndex = int.Parse(inhColumn.Value);
            if (grid.Equals(GridView1)) CellPlayerClicked(rowIndex, selectedCellIndex);
            else if (grid.Equals(GridView2)) CellComputerClicked(rowIndex, selectedCellIndex);
            else AddLog(rowIndex + "," + selectedCellIndex);
        }
        private void CellPlayerClicked(int row, int col)
        {
            inhAction.Value = ""; inhColumn.Value = "";
            if (Playing)
            {
                return;
            }
            if (playerShips == null) playerShips = new List<Ship>(10);
            myGameController.CellPlayerClicked(row, col);
            
        }
        private void CellComputerClicked(int row, int col)
        {
            inhAction.Value = ""; inhColumn.Value = "";
            if (!Playing)
            {
                AddLog("Please place your ships and then press start game to attack enemy ships");
                return;
            }
           myGameController.TryHitComputerShips(row, col);
        }
        public GridView CreateNewGrid(string boardName)
        {
            var gv = new GridView();
            gv.AutoGenerateColumns = true;
            gv.Caption = boardName;
            gv.BackColor=System.Drawing.Color.FromArgb(0x3399FF);
            gv.Height=500;
            gv.Width = 500;
            gv.GridLines = GridLines.Both;
            gv.BorderColor = System.Drawing.Color.White;
            gv.BorderStyle = BorderStyle.Solid;
            gv.BorderWidth = 1;
            gv.ShowHeader = false;
            gv.RowCreated +=new GridViewRowEventHandler(OnRowCreated);
            gv.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            return gv;
        }
        protected void GenerateGrids()
        {
            GridView1 = CreateNewGrid("Your Board");
            GridView2 = CreateNewGrid("Computer's board");
            gv1Holder.Controls.Add(GridView1);
            gv2Holder.Controls.Add(GridView2);
        }
        protected void GenerateLogTxtBox()
        {
            tbLogs = new TextBox();
            tbLogs.TextMode = TextBoxMode.MultiLine;
            tbLogs.Rows = 30;
            tbLogs.AutoPostBack = true;
            tbLogs.Height = tbLogs.Width = 500;
            tbLogs.ReadOnly = true;
            tbLogs.Attributes.Add("class", "cent");
            tbLogs.Font.Bold = true;

            Page.ClientScript.RegisterStartupScript(GetType(),
            "ScrollTextbox",
            "<script type=\"text/javascript\">document.getElementById('" +
            tbLogs.ClientID +
            "').scrollTop = document.getElementById('" +
            tbLogs.ClientID +
            "').scrollHeight; " +
            " </script>");
            logsHolder.Controls.Add(tbLogs);
        }
        protected void GenerateButtons()
        {
            btnStart = CreateNewButton();
            btnStart.Text = "Start Game";
            btnStart.Click += new EventHandler(BtnStartGame_Click);
            btnStart.Style.Add("margin-right", "20px");
            divButtonsHolder.Controls.Add(btnStart);
            btnNew = CreateNewButton();
            btnNew.Text = "New Game";
            btnNew.Click += new EventHandler(BtnNewGame_Click);
            
            divButtonsHolder.Controls.Add(btnNew);
        }
        protected Button CreateNewButton()
        {
            Button b = new Button();
            b.Font.Size = FontUnit.Medium;
            return b;
        }
        protected void CreateGrids()
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            var blancks = new List<string>();
            for (int x = 0; x < 10; x++)
            {
                dt.Columns.Add(new DataColumn());
                dt1.Columns.Add(new DataColumn());
                blancks.Add("");
            }
            for (int x = 0; x < 10; x++)
            {
                dt.Rows.Add(blancks.ToArray());
                dt1.Rows.Add(blancks.ToArray());
            }
            playerTable = dt;
            //AddLog("Created Computer board" + dt1.Rows.Count);
            computerTable = dt1;
            CreateComputerShips();
            BindGrid();
        }
        protected void BindGrid()
        {
            GridView1.DataSource = playerTable;
            GridView1.DataBind();
            GridView2.DataSource = computerTable;
            GridView2.DataBind();
            for (int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    string cord = x + "," + y;
                    GridView1.Rows[x].Cells[y].ToolTip = cord;
                    GridView2.Rows[x].Cells[y].ToolTip = cord;
                }
            }
        }

        protected void BtnStartGame_Click(object sender, EventArgs e)
        {
            StartGame();
        }
        private void StartGame()
        {
            if (playerShips.Count < 9)
            {
                AddLog("Please arrange ships first");
                return;
            }
            if (Playing)
            {
                return;
            }
            Playing = true;
            ComputerTurn = true;
            StatusText = "Your turn to hit the computer's ships";
        }
        public void EndGame(byte winner)
        {
            Playing = false;
            ComputerTurn = false;
            string msg = "Game Over";
            if (winner == 1)
            {
                AddLog("You sank the entire computer fleet congratulations");
                msg += "You won";
            }
            else if (winner == 2)
            {
                AddLog("Your whole fleet was sunk, next time try better");
                msg += "Computer won";
            }
            StatusText = msg;
        }
        private void NewGame()
        {
            Playing = false;
            IsNewGame = true;
            tbLogs.Text = "";
            StatusText = "Place " + myGameController.GetShipName(1);
            playerShips.Clear();
            computerShips.Clear();
            Session["playerShips"] = playerShips;
            Session["computerShips"] = computerShips; 
            CreateGrids();
        }

        protected void BtnNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        protected void CreateComputerShips()
        {
            AddLog("Creating computer's ships");
            computerShips.Clear();
            int orient = rand.Next(20);
            if (orient > 9) orient = 1;
            else orient = 0;
            int maxPos = 10 - 1;
            int startPosX = rand.Next(maxPos), startPosY = rand.Next(maxPos);
            bool done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
            int tries = 0;
            for (int co = 0; co < 4; co++)
            {
                while (!done)
                {
                    tries++;
                    orient = rand.Next(20);
                    if (orient > 9) orient = 1;
                    else orient = 0;
                    startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
                    done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
                    if (tries > 500)
                    {
                        break;
                    }
                }
                done = false;
            }
            maxPos = 8;
            orient = rand.Next(20);
            if (orient > 9) orient = 1;
            else orient = 0;
            startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
            tries = 0;
            done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
            for (int co = 0; co < 3; co++)
            {
                while (!done)
                {
                    tries++;
                    orient = rand.Next(20);
                    if (orient > 9) orient = 1;
                    else orient = 0;
                    startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
                    done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
                    if (tries > 500)
                    {
                        break;
                    }
                }
                done = false;
            }
            maxPos = 7;
            orient = rand.Next(20);
            if (orient > 9) orient = 1;
            else orient = 0;
            startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
            tries = 0;
            done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
            for (int co = 0; co < 2; co++)
            {
                while (!done)
                {
                    tries++;
                    orient = rand.Next(20);
                    if (orient > 9) orient = 1;
                    else orient = 0;
                    startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
                    done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
                    if (tries > 500)
                    {
                        break;
                    }
                }
                done = false;
            }
            maxPos = 6;
            orient = rand.Next(20);
            if (orient > 9) orient = 1;
            else orient = 0;
            startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
            tries = 0;
            done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
            while (!done)
            {
                tries++;
                orient = rand.Next(20);
                if (orient > 9) orient = 1;
                else orient = 0;
                startPosX = rand.Next(maxPos); startPosY = rand.Next(maxPos);
                done = CreateShip(orient, startPosX, startPosY, 10 - maxPos, true);
                if (tries > 500)
                {
                    break;
                }
            }
            AddLog("Computer has placed all its ships, now it's your turn");
        }
        private bool CreateShip(int ore, int startPosX, int startPosY, int width, bool computerShip)
        {
            if (width > 5 || width < 1) return false;
            if (startPosX + width > 9 || startPosY + width > 9) return false;
            List<Ship> exists = playerShips;
            if (computerShip) exists = computerShips;
            List<string> cords = new List<string>();
            if (ore == 1)
            {
                for (int y = startPosY; y < startPosY + width; y++)
                    cords.Add(startPosX + "," + y);
            }
            else if (ore == 0)
            {
                for (int x = startPosX; x < startPosX + width; x++)
                    cords.Add(x + "," + startPosY);
            }
            foreach (var x in exists)
            {
                foreach (var cord in x.cords.ToArray())
                    if (cords.Contains(cord)) return false;
            }
            string name = myGameController.GetShipName(width);
            if(name.Trim()==""||name== "Unknown")
            {
                AddLog(name + " ship type");
                return false;
            }
            Ship ship = new Ship(name, width);
            ship.cords = cords;
            exists.Add(ship);
            string msg = String.Join(" ", cords.ToArray());
            msg = "created " + ship.name + " at cords " + msg;
            if (computerShip) AddLog(msg);
            return true;
        }
        public void AddLog(string msg)
        {
            tbLogs.Text += msg + Environment.NewLine;
        }
    }
    
}