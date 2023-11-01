using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;

namespace JobTask
{
    public partial class Form1 : Form
    {
        private volatile bool isRunning = true; // volatile guarantees that a variable will always be read and written directly from memory

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selectedValue = ParseToInt();

           
            isRunning = true;

            for (int i = 0; i < selectedValue; i++)
            {
                Thread thread = new Thread(PrintRandomWithDelay);
                
                thread.Start();
                Thread.Sleep(100);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
            isRunning = false;
        }

        // method of parsing from text to int
        private int ParseToInt()
        {
            object selectedItem = comboBox1.SelectedItem;
            if (selectedItem != null)
            {
                string selectedText = selectedItem.ToString();
                if (int.TryParse(selectedText, out int selectedValue))
                {
                    return selectedValue;
                }
            }
            return 0;
        }

        private void PrintRandomWithDelay()
        {
            while (isRunning)
            {
                //Generating random string length
                int stringLength = new Random().Next(5, 11);
                string randString = RandomString(stringLength);

                //Generating random delay
                Random random = new Random();
                int delayMilliseconds = random.Next(500, 2000);
                Thread.Sleep(delayMilliseconds);

                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=H:\\College\\C#\\JobTask\\JobTask\\Microsoft Access Database File.mdf;Integrated Security=True"; //database link
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO Access (ThreadID, Time, Data) VALUES (@ThreadID, @Time, @Data)";
                        command.Parameters.AddWithValue("@ThreadID", Thread.CurrentThread.ManagedThreadId);
                        command.Parameters.AddWithValue("@Time", DateTime.Now);
                        command.Parameters.AddWithValue("@Data", randString);
                        command.ExecuteNonQuery();
                    }
                }
                
            
                //displaying in listView
                int threadId = Thread.CurrentThread.ManagedThreadId;
                ListViewItem item = new ListViewItem(threadId.ToString()); 
                item.SubItems.Add(randString);

               
                listView1.Invoke((MethodInvoker)delegate {
                    listView1.Items.Add(item);
                });

                if (listView1.Items.Count > 20)
                {
                    listView1.Invoke((MethodInvoker)delegate {
                        listView1.Items.RemoveAt(0); 
                    });
                }
            }
        }

        //random string generation method
        static string RandomString(int length)
        {
            const string chars = "1234567890-=!@#$%^&*()_+[]\\{}|;':\",.<>?/abcdefghijklmnopqrstuvwxyz";
         
            
            StringBuilder result = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

    }
}
