using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trym
{
    public partial class Form1 : Form
    {
        Queue<Point> tryms;
        int sizeTrym = 5;
        Point positionTrym;
        int rAreaTrym;
        string pathImg;
        int isFlyright;
        bool isEnter;
        int countPositionChanged;
        bool stop;

        List<Point> vtriTrym;

        Thread fly;

        bool isFlying;

        Stack<Point> randomPositionTrym;

        public delegate void PositionChanged();
        public event PositionChanged OnPositionChanged;

        public int CountPositionChanged
        {
            get => countPositionChanged;
            set {
                if (countPositionChanged != value)
                    if (OnPositionChanged != null)
                    {
                        countPositionChanged = value;
                        OnPositionChanged();
                    }
            }
        }
        public Form1()
        {
            Label lbSLTrym = new Label() { Font = new Font("Arial",15,FontStyle.Bold), TextAlign= ContentAlignment.MiddleCenter,Text = "SỐ LƯỢNG TRYM",Dock = DockStyle.Top };
            TextBox txbSLTrym = new TextBox() { TextAlign = HorizontalAlignment.Center, Text = "5", Dock= DockStyle.Top };
            Button btnSave = new Button() { Dock = DockStyle.Top, Text = "Save", TextAlign = ContentAlignment.MiddleCenter };
            this.Controls.AddRange(new Control[] { btnSave, txbSLTrym, lbSLTrym });
            btnSave.Click += (s,e) =>
            {
                if (new Regex(@"\d+").IsMatch(txbSLTrym.Text) == true)
                {
                    this.Controls.Clear();
                    sizeTrym = Convert.ToInt32(txbSLTrym.Text);
                    InitializeComponent();
                    Unit();
                }
                else
                    MessageBox.Show("Lỗi !!!");
            };
        }


        private void Unit()
        {
            rAreaTrym = 20 * sizeTrym;
            PositionDefault();
            stop = false;

            isFlying = false;
            positionTrym = new Point(this.Size.Width / 2, this.Size.Height / 2);
            tryms = new Queue<Point>();
            this.Paint += Form1_Paint;

            pathImg = Application.StartupPath + @"\Picture\0\1.png";

            new Thread(
                () =>
                    {
                        int capture = 1;
                        while (!stop)
                        {
                            Thread.Sleep(100);
                            pathImg = string.Format(Application.StartupPath + @"\Picture\{0}\{1}.png", isFlyright, capture);
                            if (++capture > 10)
                                capture = 1;
                            Invalidate();
                        }
                    }
                ).Start();

            fly = new Thread(HandFly);

            this.MouseEnter += (s, e) => isEnter = true;
            this.MouseLeave += (s, e) => isEnter = false;
            this.MouseMove += Form1_MouseMove;

            this.OnPositionChanged += Form1_OnPositionChanged;
            this.FormClosed += (s, e) => stop = true;

            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    positionTrym.Offset(0, -10);
                    break;
                case Keys.Down:
                    positionTrym.Offset(0, +10);
                    break;
                case Keys.Left:
                    positionTrym.Offset(-10,0);
                    isFlyright = 0;
                    break;
                case Keys.Right:
                    positionTrym.Offset(10,0);
                    isFlyright = 1;
                    break;
            }
        }


        void HandFly()
        {
            while (tryms.Count != 0)
            {
                Thread.Sleep(5);
                Point aftpositionTrym = tryms.Dequeue();
                if (aftpositionTrym.X > positionTrym.X)
                    isFlyright = 1;
                else
                    isFlyright = 0;
                positionTrym = aftpositionTrym;
                //Invalidate();
            }
            isFlying = false;
        }
        private void Form1_OnPositionChanged()
        {
            if (isFlying == false)
            {
                isFlying = true;
                fly = null;
                fly = new Thread(HandFly);
                fly.IsBackground = true;
                fly.Start();

            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isEnter == true&&e.Location.Y>rAreaTrym&& e.Location.X > rAreaTrym&&e.Location.X < this.Width - rAreaTrym&& e.Location.Y< this.Height- rAreaTrym)
            if (isEnter == true)
            {
                Point temp = e.Location;
                tryms.Enqueue(temp);
                if (isFlying == false && OnPositionChanged != null)
                    OnPositionChanged();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            randomPositionTrym = RandomPositionTrym();
            Random rand = new Random();
            int temp;
            foreach (var item in randomPositionTrym)
            {
                // temp = rand.Next(50, 100);
                temp = 100;
                g.DrawImage(Bitmap.FromFile(pathImg), item.X, item.Y, temp, temp * (float)(240 / 188));
            }
        }


        private Stack<Point> RandomPositionTrym()
        {
            Stack<Point> temp = new Stack<Point>();
            Point temp2;
            Random rand = new Random();

            for (int i = 0; i < sizeTrym; i++)
            {
                vtriTrym[i] = new Point(rand.Next((vtriTrym[i].X - 10 < -rAreaTrym) ? -rAreaTrym : vtriTrym[i].X - 10, (vtriTrym[i].X + 10 > rAreaTrym) ? rAreaTrym : vtriTrym[i].X + 10), rand.Next((vtriTrym[i].Y - 10 < -rAreaTrym) ? -rAreaTrym : vtriTrym[i].Y - 10, (vtriTrym[i].Y + 10 > rAreaTrym) ? rAreaTrym : vtriTrym[i].Y + 10));
                temp2 = new Point(vtriTrym.ElementAt(i).X + positionTrym.X, vtriTrym.ElementAt(i).Y + positionTrym.Y);

                temp.Push(temp2);
            }
            return temp;
        }

        void PositionDefault()
        {
            vtriTrym = new List<Point>();
            Point temp2;
            Random rand = new Random();

            for (int i = 0; i < sizeTrym; i++)
            {
                temp2 = new Point(rand.Next(-rAreaTrym, rAreaTrym), rand.Next(-rAreaTrym, rAreaTrym));
                vtriTrym.Add(temp2);
            }
        }//Test Default


        




    }
}
