using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableExtender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var table = new FullTable();
            var newtab = CreateTab(5,15);
            table.Name = "NewTable";
            table.DataSource = newtab;
            table.Location(0,0);
            table.Padding(30, 22, 11, 16);
            table.Buffers(3,3);
            table.Cells(0, 0);
            table.Coloring(Color.DarkBlue, Color.WhiteSmoke, Color.Black, Color.Black, Color.LightGray);
            table.Fonts(new Font("Arial",15, FontStyle.Bold), new Font("Arial", 10, FontStyle.Regular));
            table.Alignment(ContentAlignment.MiddleCenter, ContentAlignment.MiddleLeft);
            table.HeaderFixed = true;
            table.Grid = true;
            table.SelectColor = Color.Green;
            table.Show(pnlMain);
        }

        private DataTable CreateTab(int columns, int rows)
        {
            var newtab = new DataTable();
            for (int j = 0; j < columns; j++)
            {
                newtab.Columns.Add("Col" + (j + 1));
            }
            for (int i = 0; i < rows; i++)
            {
                var newrow = newtab.NewRow();
                for (int j = 0; j < columns; j++)
                {
                    string rowstr = "Row";
                    if (j==3)
                    {
                        rowstr = "Someone's name ";
                    }
                    newrow[j] = rowstr + (i+1) + "_Col" + (j+1);
                }
                newtab.Rows.Add(newrow);
            }
            return newtab;
        }
    }
}
