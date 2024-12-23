using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DiceRollerv2
{
    public partial class Form1 : Form
    {
        private List<string> words; // Store words from the resource file
        private string submitted_word;
        private Random random; // Declare Random object at the class level so it's accessible throughout


        public Form1()
        {
            InitializeComponent();
            random = new Random();
            AssignRandomNumbersToLabels();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void LoadWordsFromResources()// insserts the txt file into form 
        {
            try
            {
                // Access the text file content from resources
                string resourceContent = Properties.Resources.ukenglish; // Replace 'words' with your resource name

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
        }//drag drop

        private void Common_DragDrop(object sender, DragEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Text = (string)e.Data.GetData(DataFormats.Text);
        }//drag drop

        private void Common_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            if (e.Button == MouseButtons.Left)

                //invoke the drag and drop operation
                DoDragDrop(lbl.Text, DragDropEffects.Copy);


        }//drag drop

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
        } // clear btn

        private void submit_btn_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label)
                {
                    submitted_word += label.Text;
                    label.Text = string.Empty; // Clear the label's text
                }
            }

            // Do something with the concatenated string
            MessageBox.Show("Concatenated string: " + submitted_word);
            LoadWordsAndValidate();
        }// submit btn

        public static bool IsWordValid(string submittedWord, List<string> dictionaryWords)
        {
            return dictionaryWords.Contains(submittedWord, StringComparer.OrdinalIgnoreCase);
        }

        private void LoadWordsAndValidate()
        {
            try
            {
                // Load words from the resource file
                string resourceContent = Properties.Resources.ukenglish;
                words = new List<string>(resourceContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

                // Validate the submitted word
                bool isValid = IsWordValid(submitted_word, words);

                if (isValid)
                {
                    MessageBox.Show("The word is valid!");

                }
                else
                {
                    MessageBox.Show("The word is invalid.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading words or validating: {ex.Message}");
            }
        }


       

        private void AssignRandomNumbersToLabels()
        {
            // Create a list of available numbers from 5 to 31
            List<int> availableNumbers = new List<int>();
            for (int i = 5; i <= 31; i++)
            {
                availableNumbers.Add(i);
            }

            // Loop through all 26 labels and assign random numbers
            for (int i = 1; i <= 26; i++)
            {
                string labelName = $"label{i}";

                // Pick a random number from available numbers
                int randomIndex = random.Next(availableNumbers.Count); // Using the random object defined at the class level
                int randomNumber = availableNumbers[randomIndex];

                // Remove the number from the available numbers list to prevent repetition
                availableNumbers.RemoveAt(randomIndex);

                // Find the label by name and assign the number to its text
                Label label = this.Controls[labelName] as Label;
                if (label != null)
                {
                    label.Text = randomNumber.ToString();
                }
            }
        }
    }
}

