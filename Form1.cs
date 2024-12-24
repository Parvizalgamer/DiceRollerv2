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
        private Dictionary<char, int> letterValues; // Dictionary to store actual values for A-Z
        private int score = 0;
        private int totalScore = 0;

        public Form1()
        {
            InitializeComponent();
            random = new Random();
            AssignRandomNumbersToAlphabeticLabels();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
        }// bring the word from english file from resources into a list called  resource content 

       

        private void Common_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            { e.Effect = DragDropEffects.Copy; }
        }//drag drop

        private void Common_DragDrop(object sender, DragEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Text = (string)e.Data.GetData(DataFormats.Text);
            string word = GetCurrentWord();
            CalculateTotalScore(word);
        }//drag drop

        private string GetCurrentWord()
        {
            string word = string.Empty;

            // Loop through the boxes to get the current word
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label && !string.IsNullOrEmpty(label.Text))
                {
                    word += label.Text;
                }
            }

            return word;
        }

        private void Common_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            if (e.Button == MouseButtons.Left)

                //invoke the drag and drop operation
                DoDragDrop(lbl.Text, DragDropEffects.Copy);


        }//drag drop

        private void clear_btn_Click(object sender, EventArgs e)
        {
            // Reset the score to 0 when the clear button is clicked
            totalScore = 0;
            score = 0; // Reset the score

            // Update the score label to show the empty score or 0
            user_score.Text = score.ToString(); // Assuming 'user_score' is a label to show the score

            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label)
                {
                    label.Text = string.Empty; // Clear the label text
                }
            }
        } // clear btn

        private void submit_btn_Click(object sender, EventArgs e)
        {
            // Reset the score before calculating it again
            totalScore = 0;
            score = 0;  // Reset the score (change from totalScore to score)
            submitted_word = string.Empty; // Reset the submitted word

            // Loop through the boxes to get the current word
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label)
                {
                    submitted_word += label.Text; // Concatenate label text to form the word
                    label.Text = string.Empty; // Clear the label's text
                }
            }

            // Calculate and show the score based on the word
            string word = submitted_word.ToUpper(); // Ensure the word is uppercase
            int wordScore = CalculateTotalScore(word); // Calculate the score for the word

            // Display the concatenated word and the score in the MessageBox
            MessageBox.Show($"Concatenated string: {submitted_word}\nTotal Score: {wordScore}");

            // Validate the word
            LoadWordsAndValidate();
        }// submit btn

        public static bool IsWordValid(string submittedWord, List<string> dictionaryWords)
        {
            if (string.IsNullOrWhiteSpace(submittedWord))
            {
                return false;
            }

            else
            {
                return dictionaryWords.Contains(submittedWord, StringComparer.OrdinalIgnoreCase);
            }
        }// checks if the word submited is valid or not

        private void AssignRandomNumbersToAlphabeticLabels()
        {
            // Initialize the dictionary to store letter values. This dictionary will map each letter (A-Z) to a randomly assigned number.
            letterValues = new Dictionary<char, int>();

            // Create a list of available numbers from 5 to 31. The list will store numbers that can be randomly assigned to each letter.
            List<int> availableNumbers = new List<int>();

            // makes the availableNumbers list with values from 5 to 31. This loop runs from 5 to 31 , adding each number to the list.
            for (int i = 5; i <= 31; i++)
            {
                availableNumbers.Add(i); // Add the current value of 'i' to the list of available numbers.
            }

            // Loop through each letter in the alphabet (A to Z). This loop will go through each letter and assign a random number to it.
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string labelName = $"label{c}"; // makes a string so that the program can find the correct labe;

                // Pick a random index from the available numbers list.to make sure we pick a random number without repeating.
                int randomIndex = random.Next(availableNumbers.Count); // Generate a random index within the availableNumbers list.

                // Get the random number from the availableNumbers list using the random index.
                int randomNumber = availableNumbers[randomIndex];

                // Remove the number from the available numbers list to ensure it's not used again. prevents any number from being assigned to more than one letter.
                availableNumbers.RemoveAt(randomIndex); // Remove the number at the random index.

                // Store the random number for the letter in the letterValues dictionary.
                // This will allow us to calculate the score later using the assigned values.
                letterValues[c] = randomNumber;

                // Find the label control by name (e.g., "labelA", "labelB", etc.).
                // This searches for the label control that matches the name we created earlier.
                Label label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;

                // If the label is found, assign the random number to the label's Text property.
                // This will display the random number on the label in the user interface.
                if (label != null)
                {
                    label.Text = randomNumber.ToString(); // Set the label's text to the random number.
                }
            }
        }


        private int CalculateTotalScore(string word)
        {
            // Reset the score at the start of the calculation
            score = 0; // Ensure the score starts fresh

            // Loop through each character in the submitted word
            foreach (char c in word)
            {
                if (letterValues.ContainsKey(c)) // Ensure the letter exists in the dictionary
                {
                    score += letterValues[c]; // Add the letter's value to the score
                }
            }

            // Update the score display (assuming you have a label named 'user_score' to show the score)
            user_score.Text = score.ToString(); // Show the updated score

            return score; // Return the calculated score
        }
    }
}

