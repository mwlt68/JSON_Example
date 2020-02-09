using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace JSONCoverter
{
    public partial class Form1 : Form
    {
        string imagesFile = "MyImages";
        string jsonFile = "Data.json";
        string backup = "Backup";
        string imagesFilePath = "", jsonFilePath = "",backupFilePath="";
        string currentPath;
        List<Question> questions = new List<Question>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentPath= Path.GetDirectoryName(Application.ExecutablePath);
            CheckFilesExist();

        }
        private bool ReadQuestionsFromFile()
        {
            try
            {
                string questionsStr = File.ReadAllText(jsonFilePath);
                if (questionsStr != null && !(questionsStr.Equals("")))
                {
                    var result = JSONProcess.Read(jsonFilePath);
                    if (result != null)
                    {
                        questions = result;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
        private void CheckFilesExist()
        {
            imagesFilePath = currentPath + "\\" + imagesFile;
            jsonFilePath = currentPath + "\\" + jsonFile;
            backupFilePath= currentPath + "\\" + backup;
            if (File.Exists(jsonFilePath))
            {
                if (!ReadQuestionsFromFile())
                {
                    MessageBox.Show("Questions could not be read from json file !");
                    this.Close();
                }
            }
            else
            {

                File.Create(jsonFilePath);
            }
            if (Directory.Exists(imagesFilePath))
            {
            }
            else
            {
                Directory.CreateDirectory(imagesFilePath);
            }
            if (Directory.Exists(backupFilePath))
            {

            }
            else
            {
                Directory.CreateDirectory(backupFilePath);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            String dateTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            dateTime += ".json";
            String myBackupPath = backupFilePath + "\\" + dateTime;
            JSONProcess.backup(myBackupPath,questions);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewInformation viewInformation = new ViewInformation(this, jsonFilePath, imagesFilePath, questions);
            this.Hide();
            viewInformation.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddInformation addInformation = new AddInformation(this,jsonFilePath,imagesFilePath,questions);
            this.Hide();
            addInformation.Show();

        }
    }
}
