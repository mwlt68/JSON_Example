using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace JSONCoverter
{
    public partial class AddInformation : Form
    {
        Form parent;
        Bitmap myBitmap;
        string imagePath,imageName;
        string jsonPath, myImagesFolderPath;
        List<Question> questions;
        public AddInformation(Form parent,string jsonPath,string myImagesPath,List<Question> questions)
        {
            InitializeComponent();
            myBitmap = null;
            this.parent = parent;
            this.jsonPath = jsonPath;
            this.myImagesFolderPath = myImagesPath;
            this.questions = questions;
        }

        private void AddInformation_Load(object sender, EventArgs e)
        {
            formIntialize();
        }
        #region FormComponent
        private void formIntialize()
        {
            for (int i = 0; i < 3; i++)
            {
                TextBox textBox = createTextBox();
                flowLayoutPanel1.Controls.Add(textBox);
            }
            comboBox1.SelectedValue = 0;
        }

        private TextBox createTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.Font = textBox1.Font;
            textBox.Text = "";
            textBox.Size = new Size(200, textBox1.Size.Height);
            return textBox;
        }
        private bool fullControl()
        {
            if (!IsComponentOkay(richTextBox1))
            {
                MessageBox.Show("Soru kısmı boş bırakılamaz !");
                return false;
            }
            if (!IsComponentOkay(textBox1))
            {
                MessageBox.Show("Dogru cevap kısmı boş bırakılamaz !");
                return false;
            }
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                if (flowLayoutPanel1.Controls[i] is TextBox)
                {
                    TextBox textBox = (TextBox)flowLayoutPanel1.Controls[i];
                    if (!IsComponentOkay(textBox))
                    {
                        MessageBox.Show((i+1)+"- Yanlış cevap kısmı boş bırakılamaz !");
                        return false;
                    }
                }
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Kategori kısmı boş bırakılamaz !");
                return false;
            }
            return true;
        }
        private bool IsComponentOkay(TextBoxBase boxBase)
        {
            if (boxBase.Text == "" || boxBase.Text == null)
                return false;
            else
                return true;
        }
        #endregion
        #region ButtonClick
        private void AddFalseAnswerTextBoxBtn(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count < 8)
            {
                TextBox textBox = createTextBox();
                flowLayoutPanel1.Controls.Add(textBox);
            }
        }

        private void SelectImageBtn(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Resim Seç";
                dlg.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;)|*.jpg;  *.jpeg; *.gif; *.bmp;*.png";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap bitmap = new Bitmap(dlg.FileName);
                        myBitmap = bitmap;
                        imageName = dlg.SafeFileName;
                        long length = new System.IO.FileInfo(dlg.FileName).Length;
                        label5.Text = BytesToString(length);
                        label6.Text = imageName;
                        pictureBox1.Image = myBitmap;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Resim Seçilemedi !");
                        ImageRemover();
                    }
                    
                }
            }
        }
        
        private void ImageRemover()
        {
            myBitmap = null;
            imageName = "";
            imagePath = "";
            label5.Text = "Resim Boyutu";
            label6.Text = "Resim Adı";
            pictureBox1.Image = myBitmap;
        }
        private  String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        private void SaveBtn(object sender, EventArgs e)
        {
            if (fullControl())
            {
                if (myBitmap== null)
                {
                    DialogResult dr = MessageBox.Show("Resim seçilmedi devam etmek istiyormusunuz ?", "Uyarı",MessageBoxButtons.YesNo);
                    if (dr==DialogResult.Yes)
                    {
                        try
                        {
                            Question question = new Question(GetAppropriateId(), comboBox1.SelectedItem.ToString(), richTextBox1.Text, null,null, textBox1.Text, GetFalseAnswerList());
                            QuestionAdder(question);
                            FormClearSaveProcessLater();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Kaydetme Sırasında hata meydana geldi !");
                        }
                    }
                    else
                    {
                        
                    }
                   
                }
                else
                {
                    try
                    {
                        int Id = GetAppropriateId();
                        string imageSaveName = "m"+Id;
                        // m is necessary.Because file name must require start with character in android raw files
                        // I selected m because my name is start with M(evlüt)
                        string imageSaveNameWithFormat = imageSaveName + GetImageFormat(imageName);
                        Question question = new Question(Id, comboBox1.SelectedItem.ToString(), richTextBox1.Text, imageSaveName, GetImageFormat(imageName), textBox1.Text, GetFalseAnswerList());
                        QuestionAdder(question);
                        imagePath = myImagesFolderPath + "\\" + imageSaveNameWithFormat;
                        myBitmap.Save(imagePath);
                        FormClearSaveProcessLater();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Kaydetme Sırasında hata meydana geldi !");
                    }
                }
            }
        }
        private string GetImageFormat(string fileName)
        {
            string result = ".";
            string reversOfResult = "";
            for (int i  = fileName.Length-1; i  > 0; i --)
            {
                if (fileName[i].Equals('.'))
                    break;
                reversOfResult += fileName[i];
            }
            for (int i = 0; i < reversOfResult.Length; i++)
            {
                result+= reversOfResult[reversOfResult.Length - i - 1];
            }
            return result.ToLower();
        }
        private void FormClearSaveProcessLater()
        {
            richTextBox1.Text = "";
            textBox1.Text = "";
            removeFalseAnswerTextBox();
            clearFalseAnswerTextBox();
            ImageRemover();
        }
        private void clearFalseAnswerTextBox()
        {
            for (int i = 0; i < 3; i++)
            {
                Control control = flowLayoutPanel1.Controls[i];
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = "";
                }
            }
        }
        private void removeFalseAnswerTextBox()
        {
            int countOfFLP = flowLayoutPanel1.Controls.Count;
            for (int i = countOfFLP-1; i > 2; i--)
            {
                flowLayoutPanel1.Controls.RemoveAt(i);
            }
        }
        private bool QuestionAdder(Question question)
        {
            try
            {
                questions.Add(question);
                JSONProcess.Write(jsonPath, questions);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
 
        }
        private List<string> GetFalseAnswerList()
        {
            List<string> falseAnswerList = new List<string>();
            foreach (var control in flowLayoutPanel1.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    falseAnswerList.Add(textBox.Text);
                }
            }
            return falseAnswerList;
        }
        private int GetAppropriateId()
        {
            int maxValue = 0;
            foreach (var question in questions)
            {
                if (question.id > maxValue )
                {
                    maxValue = question.id;
                }
            }
            int result = maxValue + 1;
            return result;
        }
        private void removeImage(object sender, EventArgs e)
        {
            ImageRemover();
        }
        private void RemoveFalseAnswerTextBoxBtn(object sender, EventArgs e)
        {
            int countOfFLP = flowLayoutPanel1.Controls.Count;
            if (countOfFLP> 3)
            {
                flowLayoutPanel1.Controls.RemoveAt(countOfFLP - 1);
            }
        }

        private void AddInformation_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.Show();
        }
        #endregion 

    }
}
