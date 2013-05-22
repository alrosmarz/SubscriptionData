using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;

namespace SubscriptionData
{
    public partial class Form1 : Form
    {

        #region status

        public const int CDBDE_STATE_TABCORP             = 0;
        public const int CDBDE_STATE_TABLIMITED          =1;
        public const int CDBDE_STATE_UNITAB              =2;

        /* These define the method by which you will be selecting a race pool
    or result in the "refstate" parameter of GetResult() and GetRace()
    calls. For example CDBDE_REFCODE_TABLIMITED will tell the API
    that you will be using TAB LIMITED codes such as 'MT' to refer to
    a race meeting. The same meeting can be referenced byt the code
    for any TAB (see the returned data from GetMeet() to learn the
    codes the API will expect for each meet.

   As an example, a Melbourne greyhound race may be referenced by
   any state code and these codes may all differ:

   State                      Code
   --------------------------------
   CDBDE_REFCODE_TABCORP      "MG"
   CDBDE_REFCODE_TABLIMITED   "PG"
   CDBDE_REFCODE_UNITAB       "VG"

    or all be the same;

   State                      Code
   --------------------------------
   CDBDE_REFCODE_TABCORP      "MR"
   CDBDE_REFCODE_TABLIMITED   "MR"
   CDBDE_REFCODE_UNITAB       "MR"

   You can also use auto selectors such as CDBDE_REFCODE_NEXT_TO_JUMP_HOLD2MIN
    which will automatically select the next race to jump (across all meets)
    and assume you wish to hold that race as next until 2 minutes after the
    start time before reporting the genuine next to jump. Defines such as
    CDBDE_REFCODE_2NDLAST_TO_JUMP on the other hand automatically refer to
    races which have jumped and in this case the one before the last to jump.
    This define is especially usefull for referencing results for recent closed
    races.

*/
    
        public const int CDBDE_REFCODE_TABCORP = 0;
        public const int CDBDE_REFCODE_TABLIMITED            =1;
        public const int CDBDE_REFCODE_UNITAB                =2;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP          =3;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD1MIN =4;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD2MIN =5;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD3MIN =6;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD4MIN =7;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD5MIN =8;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD6MIN =9;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD7MIN =10;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD8MIN =11;
        public const int CDBDE_REFCODE_NEXT_TO_JUMP_HOLD9MIN =12;
        public const int CDBDE_REFCODE_LAST_TO_JUMP          =13;
        public const int CDBDE_REFCODE_2NDLAST_TO_JUMP       =14;
        public const int CDBDE_REFCODE_3RDLAST_TO_JUMP       =15;
        public const int CDBDE_REFCODE_4THLAST_TO_JUMP       =16;
        public const int CDBDE_REFCODE_5THLAST_TO_JUMP       =17;
        public const int CDBDE_REFCODE_6THLAST_TO_JUMP       =18;
        public const int CDBDE_REFCODE_7THLAST_TO_JUMP       =19;
        public const int CDBDE_REFCODE_8THLAST_TO_JUMP       =20;
        public const int CDBDE_REFCODE_9THLAST_TO_JUMP       =21;

        /* returned by InitAPI() and others */
        public const int CDBDE_COMMAND_COMPLETED     =2;
        public const int CDBDE_ENGINE_ONLINE         =1;
        public const int CDBDE_STATE_UNKNOWN         =0;
        public const int CDBDE_STATE_INVALID         =-1;
        public const int CDBDE_REFERENCE_INVALID     =-2;
        public const int CDBDE_ENGINE_UNAVAILABLE    =-3;
        public const int CDBDE_COMMSERROR_INITAGAIN  =-4;
        public const int CDBDE_COMMSERROR_INVALID    =-5;

        public const int CDBDE_PARAMERROR_BASE       =-100;
        /* All Errors less than CDBDE_PARAMERROR_BASE refer to an invalid
           parameter number in a call. To determine which param was
           invalid, deduct CDBDE_PARAMERROR_BASE from the return and if
           it was the first param the result will be 1, the second param
           will be 2 and so on. */

        public const int CDBDE_TIMEOFFSETPENDING     =-1;

        /* Pools that data is held for */
        public const int CDBDE_POOL_WIN              =0;
        public const int CDBDE_POOL_PLACE            =1;
        public const int CDBDE_POOL_QUINELLA         =2;
        public const int CDBDE_POOL_EXACTA           =3;
        public const int CDBDE_POOL_TRIFECTA         =4;

        /* Summary Modes */ 
        public const int CDBDE_MODE_NOSUMMARY        =0;
        public const int CDBDE_MODE_SUMMARIZE        =1;

        #endregion

        [DllImport("cdbdapi.dll", SetLastError = true)]
        unsafe public static extern int GetMeet(StringBuilder buf, int* len);

        [DllImport("cdbdapi.dll", SetLastError = true)]
        public static extern int InitAPI();

        [DllImport("cdbdapi.dll", SetLastError = true)]
        public static extern void CloseAPI();

        [DllImport("cdbdapi.dll", SetLastError = true)]
        unsafe public static extern int GetRace(
                        StringBuilder buf, 	        //pointer to a buffer of sufficient size to accept the information
		                int *len,	        //pointer to an integer which has been preset to the size of 'buf' above
		                int refstate,	    //state which codes are being used as a reference in 'meet' below
                        string meet,	        //meeting code or reference defining the race meeting or an automatic action to take (see below)
                        string race,	        //race number (as a string) or automatic action to take (see below)
		                int state,	        //the state (TAB) to return approximate dividends or abstractions for
		                int pool,	        //the pool to return approximate dividends or abstractions for
		                int updates,	    //the number of update columns to include
		                int summary	        //flag whether to summarise updates into known intevals or provide actual updates
	                );
      
        public Form1()
        {
            InitializeComponent();
        }

        bool loadedForms = false;
        frmTabLimited f = new frmTabLimited();
        frmTabCorp f2 = new frmTabCorp();
        frmUniTab f3 = new frmUniTab();
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        unsafe private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                MessageBox.Show("There are some missing fields");
            else
            {
                if (loadedForms)
                {   
                    t.Interval = int.Parse(textBox3.Text) * 1000;;
                    t.Tick += new EventHandler(t_Tick);
                    t.Start();

                }
                else
                    MessageBox.Show("You can not start the timer until you have the 3 forms loaded");
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            updateData(meeting1, meeting2, meeting3, raceNum);
        }

        private DataTable createData(StringBuilder b, bool secondTime = false)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Field1");
            dt.Columns.Add("Field2");
            dt.Columns.Add("Field3");
            dt.Columns.Add("Field4");
            dt.Columns.Add("Field5");
            dt.Columns.Add("Field6");
            dt.Columns.Add("Field7");
            dt.Columns.Add("Field8");
            dt.Columns.Add("Field9");
            dt.Columns.Add("Field10");
            dt.Columns.Add("Field11");
            dt.Columns.Add("Field12");
            dt.Columns.Add("Field13");
            dt.Columns.Add("Field14");
            dt.Columns.Add("Field15");
            dt.Columns.Add("Field16");
            dt.Columns.Add("Field17");
            dt.Columns.Add("Field18");
            dt.Columns.Add("Field19");
            dt.Columns.Add("Field20");
            dt.Columns.Add("Field21");
            dt.Columns.Add("Field22");
            //dt.Columns.Add("Field23");
            //dt.Columns.Add("Field24");
            //dt.Columns.Add("Field25");

            DataTable dt3 = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt3.Columns.Add("Field" + (i + 1).ToString());
            }

            if (!secondTime)
            {
                string[] firstSplit = b.ToString().Split('\r');
                int counter = 1;

                DataRow dr = dt.NewRow();
                foreach (string item in firstSplit)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] part = item.Split(',');
                        foreach (string p in part)
                        {
                            if (string.IsNullOrEmpty(p))
                            {
                                dt.Rows.Add(dr);
                                dr = dt.NewRow();
                                break;
                            }

                            if (counter >= 11)
                            {
                                try
                                { dr["Field" + counter.ToString()] = p.Substring(0, 5); }
                                catch (Exception) { dr["Field" + counter.ToString()] = p; }
                            }
                            else
                                dr["Field" + counter.ToString()] = p;
                            counter++;
                        }
                    }
                    counter = 1;
                }

            }
            else
            {
                string[] firstSplit = b.ToString().Split('\r');
                int counter = 1;

                DataRow dr = dt.NewRow();
                bool addrow = false;

                foreach (string item in firstSplit)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (!item.Contains(",Y,"))
                        {
                            string[] p = item.Split(',');
                            foreach (string a in p)
                            {
                                dr["Field" + counter.ToString()] = a;

                                if (counter == 7)
                                    dr["Field20"] = a;
                                if (counter == 16)
                                    dr["Field21"] = a;



                                counter++;
                            }
                            addrow = true;
                        }

                        if (addrow)
                        {
                            dt.Rows.Add(dr);
                            dr = dt.NewRow();
                            counter = 1;
                            addrow = false;
                        }
                    }
                }

                dt.Rows[1]["Field4"] = dt.Rows[1]["Field6"].ToString();

                try
                {
                    TimeSpan ts = new TimeSpan();
                    TimeSpan ts2 = new TimeSpan();

                    ts = TimeSpan.Parse(dt.Rows[1]["Field4"].ToString());
                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field7"].ToString());
                    dt.Rows[1]["Field7"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field8"].ToString());
                    dt.Rows[1]["Field8"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field9"].ToString());
                    dt.Rows[1]["Field9"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field10"].ToString());
                    dt.Rows[1]["Field10"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field11"].ToString());
                    dt.Rows[1]["Field11"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field12"].ToString());
                    dt.Rows[1]["Field12"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field13"].ToString());
                    dt.Rows[1]["Field13"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field14"].ToString());
                    dt.Rows[1]["Field14"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field15"].ToString());
                    dt.Rows[1]["Field15"] = (ts - ts2).ToString();

                    ts2 = TimeSpan.Parse(dt.Rows[1]["Field16"].ToString());
                    dt.Rows[1]["Field16"] = (ts - ts2).ToString();



                    dt.Columns.Remove("Field5");
                    dt.Columns.Remove("Field6");


                    DataTable dt2 = new DataTable();
                    //magic calculation

                    int column = 4;
                    int rowW = 999;
                    Dictionary<string, string> valuesCalculated = new Dictionary<string, string>();
                    bool useDictionary = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //rows
                        if (i >= 2 && i <= 8)
                        {
                            if (column != 16)
                            {

                                int calculationsRow = 1;
                                while (calculationsRow <= 10)
                                {
                                    decimal value = 0;
                                    decimal.TryParse(dt.Rows[i][column].ToString(), out value);

                                    decimal sumColumn = 0;
                                    decimal finalTotal = 0;

                                    if (!useDictionary)
                                    {
                                        for (int z = 0; z < gvData.Rows.Count; z++)
                                        {
                                            if (z >= 2 && z <= 8)
                                                sumColumn += (decimal.Parse(textBox2.Text) / decimal.Parse(dt.Rows[z][column].ToString()));

                                            try
                                            {
                                                if (dt.Rows[z][0].ToString() == "\nW")
                                                {
                                                    rowW = z + 1;
                                                    finalTotal = decimal.Parse(dt.Rows[z][column].ToString());
                                                }
                                            }
                                            catch (Exception)
                                            { }
                                         

                                        }
                                        valuesCalculated.Add(column.ToString(), sumColumn + "|" + finalTotal);
                                    }
                                    else
                                    {
                                        string[] temp = valuesCalculated[column.ToString()].Split('|');
                                        sumColumn = decimal.Parse(temp[0]);
                                        finalTotal = decimal.Parse(temp[1]);
                                    }

                                    dt.Rows[i][column] = Math.Ceiling((decimal.Parse(textBox2.Text) / value) / sumColumn * finalTotal);
                                    calculationsRow++;
                                    column++;
                                }
                                useDictionary = true;
                                column = 4;

                            }
                        }
                        else if (i == rowW)
                        {

                            foreach (KeyValuePair<string, string> item in valuesCalculated)
                            {
                                string[] temp = valuesCalculated[column.ToString()].Split('|');
                                dt.Rows[i][int.Parse(item.Key)] = Math.Ceiling(decimal.Parse(temp[0]));
                            }


                        }
                    }

                    //final calculation
                    dt2 = dt;

                    int columnN = 4;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        if (i >= 2 && i <= 8)
                        {
                            int calculationsRow = 1;
                            while (calculationsRow <= 9)
                            {
                                dt2.Rows[i][columnN] = (decimal.Parse(dt2.Rows[i][columnN].ToString()) - decimal.Parse(dt.Rows[i][columnN + 1].ToString())).ToString();
                                calculationsRow++;
                                columnN++;
                            }


                        }
                        columnN = 4;
                    }

                    //return dt2;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        if (i <= rowW + 1)
                            dt3.ImportRow(dt2.Rows[i]);

                    }

                    dt3.Columns.Remove("Field5");
                    dt3.Columns.Remove("Field6");
                    return dt3;

                }
                catch (Exception)
                {

                }
            }
            return dt;
        }

        unsafe public void updateData(string m1, string m2, string m3, string raceNum)
        {
            char* buffer;

            int retval;
            int retval2;
            int retval3;
            int buflen = 16384;	//reasonable amount of memory to allow for up to 50 meets

            StringBuilder b = new StringBuilder(16384);
            StringBuilder b2 = new StringBuilder(16384);
            StringBuilder b3 = new StringBuilder(26384);

            retval = GetRace(b, &buflen, CDBDE_REFCODE_TABLIMITED, m1, raceNum, CDBDE_REFCODE_TABLIMITED, CDBDE_POOL_WIN, 10, CDBDE_MODE_NOSUMMARY);
            retval2 = GetRace(b2, &buflen, CDBDE_REFCODE_TABCORP, m2, raceNum, CDBDE_REFCODE_TABCORP, CDBDE_POOL_WIN, 10, CDBDE_MODE_NOSUMMARY);
            retval3 = GetRace(b3, &buflen, CDBDE_REFCODE_UNITAB, m3, raceNum, CDBDE_REFCODE_UNITAB, CDBDE_POOL_WIN, 10, CDBDE_MODE_NOSUMMARY);

            if (retval == CDBDE_COMMAND_COMPLETED) //we have the data ?
            {

                b = b.Replace("OK Win Approximates 00:00:00", "");

                DataTable dt = new DataTable();
                dt = createData(b, true);

                f.setText(dt);
                if (!loadedForms)
                    f.Show();
            }

            if (retval2 == CDBDE_COMMAND_COMPLETED) //we have the data ?
            {

                b2 = b2.Replace("OK Win Approximates 00:00:00", "");

                DataTable dt = new DataTable();
                dt = createData(b2, true);

                f2.setText(dt);
                if (!loadedForms)
                f2.Show();
            }

            if (retval3 == CDBDE_COMMAND_COMPLETED) //we have the data ?
            {
                b3 = b3.Replace("OK Win Approximates 00:00:00", "");

                DataTable dt = new DataTable();
                dt = createData(b3, true);

                f3.setText(dt);
                if (!loadedForms)
                    f3.Show();
            }

        }

        string meeting1 = string.Empty;
        string meeting2 = string.Empty;
        string meeting3 = string.Empty;
        string raceNum = string.Empty;

        unsafe private void gvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                MessageBox.Show("There are some missing fields");
            else
            {
                if (!loadedForms)
                {
                    f = new frmTabLimited();
                    f2 = new frmTabCorp();
                    f3 = new frmUniTab();
                }
                if (e.ColumnIndex >= 10)
                {
                    if (!string.IsNullOrEmpty(gvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    {
                        meeting1 = gvData.Rows[e.RowIndex].Cells["Field5"].Value.ToString();
                        meeting2 = gvData.Rows[e.RowIndex].Cells["Field7"].Value.ToString();
                        meeting3 = gvData.Rows[e.RowIndex].Cells["Field9"].Value.ToString();
                        raceNum = (e.ColumnIndex - 10 + 1).ToString();
                        updateData(meeting1, meeting2, meeting3, raceNum);
                        loadedForms = true;
                    }
                }
            }
        }

        unsafe private void Form1_Load(object sender, EventArgs e)
        {
            char* buffer;
            int retval;
            int buflen = 16384;	//reasonable amount of memory to allow for up to 50 meets

            StringBuilder b = new StringBuilder(16384);
            if (InitAPI() == CDBDE_ENGINE_ONLINE)
            {
                if ((retval = GetMeet(b, &buflen)) != CDBDE_COMMAND_COMPLETED) //we have the data ?
                {
                    if (retval == CDBDE_COMMSERROR_INITAGAIN)
                    {
                        MessageBox.Show("A Communication Error Has Occurred Calling GetMeet()", "Error", MessageBoxButtons.OK);
                        CloseAPI();

                        if (InitAPI() != CDBDE_ENGINE_ONLINE)
                        {
                            MessageBox.Show("The Data Engine Is No Longer Available", "Error", MessageBoxButtons.OK);
                            return;
                        }

                        return;
                    }
                    else
                    {
                        MessageBox.Show("An Error Condition Was Returned From The Engine When Calling GetMeet()", "Error", MessageBoxButtons.OK);
                        return;
                    }
                }
            }

            DataTable dt = new DataTable();

            b = b.Replace("OK Meetings 00:00:00", "");

            dt = createData(b);

            if (dt.Rows.Count > 0)
            {
                gvData.AutoGenerateColumns = true;
                gvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gvData.DataSource = dt;

                gvData.Columns[2].Visible = false;
                gvData.Columns[3].Visible = false;
                //gvData.Columns[4].Visible = false;
                gvData.Columns[5].Visible = false;
                //gvData.Columns[6].Visible = false;
                gvData.Columns[7].Visible = false;
                //gvData.Columns[8].Visible = false;
                gvData.Columns[9].Visible = false;
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            t.Stop();
            loadedForms = false;
        }

    }
}

