using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DiceRollerv2
{
    public partial class Form1 : Form
    {
        private List<string> words; // Store words from the resource file
        private string submitted_word;

        public Form1()
        {
            InitializeComponent();
            //AssignRandomValuesToLabels(5,25);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LoadWordsFromResources()// insserts the txt file into form 
        {
            try
            {
                // Access the text file content from resources
                string resourceContent = Properties.Resources.gen_1_pokemon; // Replace 'words' with your resource name

                // Split the content into lines and store them in a list
                words = new List<string>(resourceContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading resources: {ex.Message}");
            }
        }

        private void Common_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            { e.Effect = DragDropEffects.Copy; }
        }

        private void Common_DragDrop(object sender, DragEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Text = (string)e.Data.GetData(DataFormats.Text);
        }

        private void Common_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            if (e.Button == MouseButtons.Left)

                //invoke the drag and drop operation
                DoDragDrop(lbl.Text, DragDropEffects.Copy);


        }

        private void clear_btn_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label)
                {
                    label.Text = string.Empty;
                }
            }
        }

        private void submit_btn_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label)
                {
                    
                }
            }
        }
    }
}
