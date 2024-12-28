using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DiceRollerv2
{
    public partial class Form1 : Form
    {
        private List<string> words; // Store words from the resource file
        private string submitted_word;
        private Random random; // Declare Random object at the class level so it's accessible throughout
        private Dictionary<char, int> letterValues; // Dictionary to store actual values for A-Z
        private int userscore = 0;
        private int totalpoints = 0;
        private string scoreword;
        private int reqscorewordlength;
        private int reqwordscore;
        private bool contains=false;
        public Form1()
        {
            InitializeComponent();
            random = new Random();
            AssignRandomNumbersToAlphabeticLabels();
            LoadWordsAndValidate();
            GetRandomWord();


            // Set KeyPreview to true so that the form can capture key events
            this.KeyPreview = true;

            // Wire up the KeyDown event
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            //idk
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

        // bring the word from english file from resources into a list called  resource content 

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
            
            userscore = 0; // Reset the score

            // Update the score label to show the empty score or 0
            user_score.Text = userscore.ToString(); // Assuming 'user_score' is a label to show the score

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
            userscore = 0;  // Reset the score
            submitted_word = string.Empty; // Reset the submitted word

            // Loop through the boxes to get the current word
            for (int i = 1; i <= 9; i++)
            {
                string labelName = "box" + i;
                Control control = this.Controls.Find(labelName, true)[0];

                if (control is Label label && !string.IsNullOrEmpty(label.Text))
                {
                    submitted_word += label.Text; // Concatenate label text to form the word
                    label.Text = string.Empty; // Clear the label's text
                }
            }

            // Check if the submitted word is empty
            if (string.IsNullOrWhiteSpace(submitted_word))
            {
                MessageBox.Show("No word submitted!");
                return;
            }

            // Convert the submitted word to uppercase for consistency
            string word = submitted_word.ToUpper();

            // Calculate the score for the word
            int wordScore = CalculateTotalScore(word);

            // Display the concatenated word and the score in the MessageBox
            MessageBox.Show($"Concatenated string: {submitted_word}\nTotal Score: {wordScore}");

            // Validate the word
            bool isValid = IsWordValid(submitted_word, words);

            if (isValid)
            {
                // If valid, check the score and length against the requirements
                CheckScoreAndWordLength(wordScore);  // Pass wordScore here
            }
            else
            {
                MessageBox.Show("The word is invalid.");
            }
        }// submit btn

        public  bool IsWordValid(string submittedWord, List<string> dictionaryWords)
        {
            if (string.IsNullOrWhiteSpace(submittedWord))
            {
                contains = false;
                return false;
            }

            else
            {
                return dictionaryWords.Contains(submittedWord, StringComparer.OrdinalIgnoreCase);
                contains = true;

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
            userscore = 0;

            // Loop through each character in the submitted word
            foreach (char c in word)
            {
                if (letterValues.ContainsKey(c)) // Ensure the letter exists in the dictionary
                {
                    userscore += letterValues[c]; // Add the letter's value to the score
                }
            }

            // Update the score display (assuming you have a label named 'user_score' to show the score)
            user_score.Text = userscore.ToString();

            return userscore; // Return the calculated score
        }


        private void GetRandomWord()
        {
            do
            {
                int randomIndex = random.Next(words.Count);
                scoreword = words[randomIndex];

                if (scoreword.Length >= 4 && scoreword.Length <= 6)
                {
                    string wordToScore = scoreword.ToUpper();
                    reqwordscore = CalculateTotalScore(wordToScore);
                }
                else
                {
                    reqwordscore = 0;
                }
            }
            while (reqwordscore < 50 || reqwordscore > 150 || scoreword.Length < 4 || scoreword.Length > 6);

            reqscorewordlength = scoreword.Length;
            req_score.Text = $"Req score: {reqwordscore} \n Req length: {reqscorewordlength}";
        }

        private void CheckScoreAndWordLength(int wordScore)
        {
            // Check if the submitted word meets or exceeds both conditions
            bool isWordValid = IsWordValid(submitted_word, words); // Check if the word is valid

            // Check if both conditions are met: score and word length
            if (wordScore >= reqwordscore && submitted_word.Length >= reqscorewordlength && isWordValid)
            {
                MessageBox.Show("Congratulations! You have met or exceeded the required score and word length.");

                // Increment the points
                totalpoints++;

                // Update the points label with the new total points
                points.Text = totalpoints.ToString();

                // Reset the game elements for the next round
                ResetForNextRound();
            }
            else
            {
                MessageBox.Show("You did not meet the required score or word length.");
            }
        }

        private void ResetForNextRound()
        {
            // Call the clear button click method to clear the labels and reset the score
            clear_btn_Click(null, null); // We don't need to pass actual sender and e, so we can pass null for both

            // Reassign random numbers to the alphabetic labels (for a new round)
            AssignRandomNumbersToAlphabeticLabels();

            // Reset the required score and word length for the next round
            GetRandomWord(); // This will generate a new required word with score and length

            // Optionally, you can reset any other game-related UI elements or variables
            // For example, resetting the submitted word
            submitted_word = string.Empty;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                // Trigger the same functionality as the showWordButton_Click
                showWordButton_Click(sender, e);
            }
        }


        private void points_Click(object sender, EventArgs e)
        {
            points.Text = totalpoints.ToString();
        }

        private void showWordButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Required word: {scoreword}", "Required Word", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


}
