using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableExtender
{
    class FullTable
    {
        //Properties
        public DataTable DataSource { get; set; }
        public List<string> ColumnHeader = new List<string>();
        public class Row { public List<dynamic> Column = new List<dynamic>(); public void Add(List<dynamic> newrow) { Column = newrow; }}
        public List<Row> Rows = new List<Row>();
        public bool HeaderFixed { get; set; }
        public bool MouseDown = false;
        public string Name { get; set; }
        public int TopPad { get; set; }
        public int RightPad { get; set; }
        public int BottomPad { get; set; }
        public int LeftPad { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
        public int ColBuffer { get; set; }
        public int RowBuffer { get; set; }
        public Font HeaderFont { get; set; }
        public Font CellFont { get; set; }
        public ContentAlignment HeaderAlign { get; set; }
        public ContentAlignment CellAlign { get; set; }
        public Color BodyColor { get; set; }
        public Color BodyAlternateColor { get; set; }
        public Color HeaderColor { get; set; }
        public Color BodyTextColor { get; set; }
        public Color HeaderTextColor { get; set; }
        public Color SelectColor { get; set; }
        public bool Grid { get; set; }

        private bool ascending = true;
        public Panel Container = new Panel()
        {
            AutoScroll=true,
            Dock = DockStyle.Fill
        };
        //Methods
        public void Coloring(Color? header = null, Color? body = null, Color? headertext = null, Color? bodytext = null, Color? bodyalternate = null)
        {
            if (header!=null) { HeaderColor = (Color)header; } else { HeaderColor = Color.WhiteSmoke; }
            if (body != null) { BodyColor = (Color)body; } else { BodyColor = Color.White; }
            if (headertext != null) { HeaderTextColor = (Color)headertext; } else { HeaderTextColor = Color.Black; }
            if (bodytext != null) { BodyTextColor = (Color)bodytext; } else { BodyTextColor = Color.Black; }
            if (bodyalternate != null) { BodyAlternateColor = (Color)bodyalternate; } else { BodyAlternateColor = (Color)body; }
        }
        public void Padding (int top, int bottom, int left, int right)
        {
            TopPad = top;
            BottomPad = bottom;
            LeftPad = left;
            RightPad = right;
        }
        public void Location (int x, int y)
        {
            Top = y;
            Left = x;
        }
        public void Buffers (int row, int column)
        {
            RowBuffer = row;
            ColBuffer = column;
        }
        public void Cells(int cellwidth, int cellheight)
        {
            CellHeight = cellheight;
            CellWidth = cellwidth;
        }
        public void Fonts(Font header, Font cells)
        {
            HeaderFont = header;
            CellFont = cells;
        }
        public void Alignment(ContentAlignment header, ContentAlignment cells)
        {
            HeaderAlign = header;
            CellAlign = cells;
        }
        public void Show(Panel panel, bool sorted = false)
        {
            //Populate table from DataSource
            if (DataSource!=null && sorted == false) { PopulateThis(); }

            //Initiate local variables
            List<int> maxwidth = new List<int>(); //Used for autosize column widths 
            bool altrows = true; //Used to alternate row colours
            int totalwidth = LeftPad; //Used to set the width of the panel in which the table resides
            int totalheight = TopPad; //Used to set the height of the panel in which the table resides
            int cell_x = LeftPad;
            int cell_y = TopPad;
            int columnindex = 0;
            int rowindex = 0;

            //Set table container attributes
            panel.Controls.Clear();
            Container.Controls.Clear();
            
            Container.Name = Name;
            Container.BackColor = BodyColor;
            Container.Location = new Point(Left, Top);
            if (HeaderFixed) { Container.Location = new Point(Left, Top + TopPad + CellHeight + RowBuffer); }
            
            //Initiate header panel (for fixed header table) and ne cell
            Panel headerpanel = new Panel();
            Label cell = new Label();

            //Loop through all columns to create headers
            foreach (var head in ColumnHeader)
            {
                //Create header cells
                cell = new Label();
                cell.AutoSize = false;
                cell.BackColor = HeaderColor;
                cell.ForeColor = HeaderTextColor;
                cell.Text = head;
                cell.Name = "Header:" + columnindex;
                cell.TabIndex = columnindex;
                cell.Location = new Point(cell_x, cell_y);
                cell.Click += Header_Click;

                //Add the cell to its container
                if (HeaderFixed) { headerpanel.Controls.Add(cell); } else { Container.Controls.Add(cell); }

                if (HeaderFont != null) { cell.Font = HeaderFont; }
                if (HeaderAlign != 0) { cell.TextAlign = HeaderAlign; }
                if (CellWidth != 0) { cell.Width = CellWidth; maxwidth.Add(CellWidth); } else { cell.AutoSize = true; maxwidth.Add(cell.Width); }
                if (CellHeight != 0) { cell.Height = CellHeight; }

                //Set next cell x-position
                cell_x += cell.Width + ColBuffer;

                //Increment index
                columnindex++;
            }

            //Set next row y-position
            cell_x = LeftPad;
            cell_y += cell.Height + RowBuffer;

            //Set fixed header attributes
            if (HeaderFixed)
            {
                headerpanel.Name = "headerpanel";
                headerpanel.BackColor = HeaderColor;
                headerpanel.Height = TopPad + cell.Height + RowBuffer;
                headerpanel.BackColor = HeaderColor;
                panel.Controls.Add(headerpanel);
            }

            //Itterate through rows
            foreach (var row in Rows)
            {
                //Initialise field variables
                columnindex = 0;
                //Itterate through fields
                foreach (var col in row.Column)
                {
                    //Create field cell
                    cell = new Label();
                    cell.AutoSize = false;
                    cell.Click += Cell_Click;
                    cell.DoubleClick += Cell_DoubleClick;
                    cell.Name = "Col:" + columnindex + "Row:" + rowindex;
                    cell.TabIndex = columnindex;
                    cell.BorderStyle = BorderStyle.None;
                    cell.ForeColor = BodyTextColor;
                    cell.Text = col;
                    cell.Location = new Point(cell_x, cell_y);

                    //Add cell to container
                    Container.Controls.Add(cell);

                    //Set row colour
                    if (altrows && BodyAlternateColor!=null) { cell.BackColor = BodyAlternateColor; }
                    else if (!altrows && BodyAlternateColor != null) { cell.BackColor = BodyColor; }
                    else { cell.BackColor = BodyColor; }

                    //Set cell font
                    if (CellFont != null) { cell.Font = CellFont; }

                    //Set cell alignment
                    if (CellAlign != 0) { cell.TextAlign = CellAlign; }

                    //Set cell width
                    if (CellWidth != 0) { cell.Width = CellWidth; } else
                    {
                        cell.AutoSize = true;
                        if (maxwidth[columnindex] < cell.Width)
                        {
                            maxwidth[columnindex] = cell.Width;
                            try
                            {
                                var headercell = Container.Controls.Find(("Header:" + columnindex), false).ToList()[0] as Label;
                                int tempx = headercell.Location.X;
                                for (int i = columnindex; i < ColumnHeader.Count; i++)
                                {
                                    headercell = Container.Controls.Find(("Header:" + i), false).ToList()[0] as Label;
                                    headercell.AutoSize = false;
                                    headercell.Width = maxwidth[i];
                                    headercell.Location = new Point(tempx, headercell.Location.Y);
                                    tempx += headercell.Width + ColBuffer;
                                }
                            }
                            catch
                            {
                                try
                                {
                                    var headercell = headerpanel.Controls.Find(("Header:" + columnindex), false).ToList()[0] as Label;
                                    int tempx = headercell.Location.X;
                                    for (int i = columnindex; i < ColumnHeader.Count; i++)
                                    {
                                        headercell = headerpanel.Controls.Find(("Header:" + i), false).ToList()[0] as Label;
                                        headercell.AutoSize = false;
                                        headercell.Width = maxwidth[i];
                                        headercell.Location = new Point(tempx, headercell.Location.Y);
                                        tempx += headercell.Width + ColBuffer;
                                    }
                                }
                                catch { }
                            }
                            for (int i = 0; i < Rows.Count; i++)
                            {
                                try
                                {
                                    var prevcell = Container.Controls.Find(("Col:" + columnindex + "Row:" + i), false).ToList()[0] as Label;
                                    int tempx = prevcell.Location.X;
                                    for (int j = columnindex; j < ColumnHeader.Count; j++)
                                    {
                                        prevcell = Container.Controls.Find(("Col:" + j + "Row:" + i), false).ToList()[0] as Label;
                                        prevcell.AutoSize = false;
                                        prevcell.Width = maxwidth[j];
                                        prevcell.Location = new Point(tempx, prevcell.Location.Y);
                                        tempx += prevcell.Width + ColBuffer;
                                    }
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            cell.AutoSize = false;
                            cell.Width = maxwidth[columnindex];
                        }
                    }
                    //Increment variables
                    columnindex++;
                    cell_x += cell.Width + ColBuffer;
                }
                //Increment variables
                rowindex++;

                //Check alternating row colour variables and adjust
                if (altrows && BodyAlternateColor!=null) { altrows = false; }
                else if (!altrows && BodyAlternateColor != null) { altrows = true; }

                cell_x = LeftPad;
                cell_y += cell.Height + RowBuffer;
            }

            //Set panel size
            totalwidth = LeftPad + RightPad + (maxwidth.Sum(r => r)) + (ColumnHeader.Count * ColBuffer);
            totalheight = cell_y + BottomPad;
            if (totalwidth > panel.Width) { totalwidth = panel.Width; totalheight += 15; }
            if (totalheight > panel.Height) { totalheight = panel.Height; totalwidth += 15; }
            headerpanel.Width = totalwidth;
            Container.Width = totalwidth;
            Container.Height = totalheight;
            panel.Width = totalwidth;
            panel.Height = totalheight;
            panel.Controls.Add(Container);
            Container.SendToBack();

            //Register on paint event to =draw grid
            if (Grid)
            {
                Container.Paint += new PaintEventHandler((sender, e) => DrawGrid(sender, e, maxwidth, cell.Height, panel.Width, panel.Height, Rows.Count, headerpanel.Height));
                panel.Invalidate();
                panel.Update();

            }
        }

        private void Cell_DoubleClick(object sender, EventArgs e)
        {
            int cols = ColumnHeader.Count;
            for (int i = 0; i < Rows.Count; i++)
            {

            }
        }
        private void Cell_Click(object sender, EventArgs e)
        {
            SelectCell(((Label)sender));
        }
        private void SelectCell( Label sender)
        {
            if (sender.BackColor != SelectColor)
            {
                sender.BackColor = SelectColor;
            }
            else
            {
                sender.BackColor = BodyColor;
            }
        }
        public void DrawGrid(object sender, PaintEventArgs canvas, List<int> maxwidth, int cellheight, int width, int height, int rowcount, int headerheight)
        {
            if (ColBuffer!=0)
            {
                int line_x = LeftPad;
                int line_y = TopPad;
                if (HeaderFixed) { line_y = headerheight; }

                int GridWidth = ColBuffer;
                var pentb1 = new Pen(BodyTextColor, LeftPad);
                var pentb2 = new Pen(BodyTextColor, ColBuffer);
                var pentb3 = new Pen(BodyTextColor, RightPad+ColBuffer);
                var penlr1 = new Pen(BodyTextColor, TopPad);
                var penlr2 = new Pen(BodyTextColor, RowBuffer);
                var penlr3 = new Pen(BodyTextColor, BottomPad+RowBuffer);

                var gr = canvas.Graphics;
                //Draw vertical graphics
                try
                {
                    decimal buffer = ColBuffer / (decimal)2;
                    int check = (int)buffer;
                    if (check != buffer)
                    {
                        buffer = buffer + (decimal)0.5;
                    }
                    int intbuffer = (int)buffer;

                    decimal pad = LeftPad / (decimal)2;
                    check = (int)pad;
                    if (check != pad)
                    {
                        pad = pad + (decimal)0.5;
                    }
                    int intpad = (int)pad;

                    decimal pad2 = (RightPad + ColBuffer) / (decimal)2;
                    check = (int)pad2;
                    if (check != pad2)
                    {
                        pad2 = pad2 + (decimal)0.5;
                    }
                    int intpad2 = (int)pad2;

                    gr.DrawLine(pentb1, intpad, 0, intpad, height);
                    int i = 0;
                    foreach (var col in maxwidth)
                    {
                        if (i!=maxwidth.Count-1)
                        {
                            line_x += col + ColBuffer;
                            gr.DrawLine(pentb2, line_x - intbuffer, 0, line_x - intbuffer, height);
                        }
                        else
                        {
                            line_x += col + RightPad + ColBuffer;
                            gr.DrawLine(pentb3, line_x - intpad2, 0, line_x - intpad2, height);
                        }
                        i++;
                    }
                }
                catch
                {

                }

                //Draw horizontal graphics
                try
                {
                    decimal buffer = RowBuffer / (decimal)2;
                    int check = (int)buffer;
                    if (check!=buffer)
                    {
                        buffer = buffer + (decimal)0.5;
                    }
                    int intbuffer = (int)buffer;

                    decimal pad = headerheight + (TopPad / (decimal)2);
                    check = (int)pad;
                    if (check != pad)
                    {
                        pad = pad + (decimal)0.5;
                    }
                    int intpad = (int)pad;

                    decimal pad2 = (BottomPad + RowBuffer) / (decimal)2;
                    check = (int)pad2;
                    if (check != pad2)
                    {
                        pad2 = pad2 + (decimal)0.5;
                    }
                    int intpad2 = (int)pad2;

                    gr.DrawLine(penlr1,0, intpad, width, intpad);
                    for (int i=0;i<rowcount;i++)
                    {
                        if (i != rowcount - 1)
                        {
                            line_y += cellheight + RowBuffer;
                            gr.DrawLine(penlr2, 0, line_y - intbuffer, width, line_y - intbuffer);
                        }
                        else
                        {
                            line_y += cellheight + BottomPad + RowBuffer;
                            gr.DrawLine(penlr3, 0, line_y - intpad2, width, line_y - intpad2);
                        }
                    }
                }
                catch
                {

                }
                canvas.Dispose();
            }
        }
        private void PopulateThis()
        {
            Rows.Clear();
            ColumnHeader.Clear();
            int columncount = DataSource.Columns.Count;
            foreach (DataColumn head in DataSource.Columns)
            {
                ColumnHeader.Add(head.ColumnName);
            }
            foreach (DataRow row in DataSource.Rows)
            {
                var newrow = new FullTable.Row();
                for (int i = 0; i < columncount; i++)
                {
                    newrow.Column.Add(row[i]);
                }
                Rows.Add(newrow);
            }
        }
        private void Header_Click(object sender, EventArgs e)
        {
            Panel panel = ((Label)sender).Parent.Parent as Panel;
            int index = ((Label)sender).TabIndex;
            if (ascending)
            {
                Rows = Rows.OrderBy(r => r.Column[index]).ToList();
                ascending = false;
            }
            else
            {
                Rows = Rows.OrderByDescending(r => r.Column[index]).ToList();
                ascending = true;
            }
            Show(panel,sorted:true);
        }
    }
}
