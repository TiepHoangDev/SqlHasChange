using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace SqlHasChange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        SqlConnectionStringBuilder builder = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            builder = new SqlConnectionStringBuilder();
            builder.UserID = "sa";
            builder.Password = "12345678";
            builder.InitialCatalog = "test";
            builder.DataSource = ".\\SQLEXPRESS";

            SqlDependency.Start(builder.ConnectionString);

            //update data
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        using (var con = new SqlConnection(builder.ConnectionString))
            //        {
            //            con.Open();
            //            using (var cmd = new SqlCommand($" update dbo.bobeghi set Time = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ", con))
            //            {
            //                cmd.ExecuteNonQuery();
            //            }
            //            con.Close();
            //        }
            //        await Task.Delay(1000);
            //    }
            //});

            //viewdata
            _loadData();
        }

        private void _loadData()
        {
            using (var con = new SqlConnection(builder.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand($" select Time from dbo.bobeghi ", con))
                {
                    var de = new SqlDependency(cmd);
                    de.OnChange += De_OnChange;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }

        private void De_OnChange(object sender, SqlNotificationEventArgs e)
        {
            string Id = "";
            if (sender is SqlDependency de)
            {
                Id = de.Id;
                de.OnChange -= De_OnChange;
            }

            if (richTextBox1.Lines.Length > 500)
            {
                richTextBox1.Clear();
            }
            var logs = $"\n{DateTime.Now:HH:mm:ss}>> Id={Id} Info={e.Info} Source={e.Source} Type={e.Type}".Replace("'", "\"");
            richTextBox1.AppendText(logs);

            //using (var con = new SqlConnection(builder.ConnectionString))
            //{
            //    con.Open();
            //    using (var cmd = new SqlCommand($" INSERT INTO[dbo].[Logs] ([ID] ,[Msg] ,[Time]) VALUES ( NewID(), '{logs}', GetDate() ) ", con))
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //    con.Close();
            //}

            _loadData();
        }
    }
}