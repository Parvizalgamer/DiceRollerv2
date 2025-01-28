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
            InitializeGame();
            AssignRandomNumbersToAlphabeticLabels();
              LoadWords();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeGame()
        {
            bool validSetup = false;
            int attempts = 0;
            int maxAttempts = 10; // Prevent infinite loops
            int requiredScore = 0; // This will hold the score of the computer-generated word
            string targetWord = ""; // Store the target word for debugging or hint purposes/ F10

            while (!validSetup && attempts < maxAttempts)
            {
                attempts++;

                // Assign random numbers to letters
                AssignRandomNumbersToAlphabeticLabels();

                // Generate a random word
                targetWord = GenerateRandomWord();

                if (!string.IsNullOrEmpty(targetWord))
                {
                    // Calculate the score of the generated word
                    requiredScore = CalculateTotalScore(targetWord.ToUpper());

                    // Ensure the word is valid and scorable
                    if (requiredScore > 0) // Optionally, you can add more conditions
                    {
                        validSetup = true; // Exit the loop
                    }
                }
            }

            if (validSetup)
            {
                MessageBox.Show($"Game initialized! The computer's word is: {targetWord}\nRequired Score: {requiredScore}");
            }
            else
            {
                MessageBox.Show("Unable to create a valid game setup after multiple attempts.");
            }

            // Set the required score for the player
            SetRequiredScore(requiredScore);
        }

        private string GenerateRandomWord(int minLength = 5, int maxLength = 7)
        {
            // Ensure words list is initialized before proceeding
            if (words == null || words.Count == 0)
            {
                MessageBox.Show("The word list has not been initialized.");
                return null;
            }

            // Filter the list of words to include only those within the length range
            List<string> validWords = words.Where(word => word.Length >= minLength && word.Length <= maxLength).ToList();

            if (validWords.Count == 0)
            {
                return null; // No valid words found
            }

            // Select a random word from the valid list
            int randomIndex = random.Next(validWords.Count);
            return validWords[randomIndex];
        }


        private bool IsWordScorable(string word, int minScore, int maxScore)
        {
            int wordScore = CalculateTotalScore(word.ToUpper());
            return wordScore >= minScore && wordScore <= maxScore;
        }
        private void SetRequiredScore(int score)
        {
            // Update the label or any UI element to display the required score
            required_score_label.Text = $"Required Score: {score}"; // Assuming you have a label named 'required_score_label'
        }

        private void LoadWordsAndValidate()
        {
            try
            {
                // If the words list is null, load it from the resource
                if (words == null || words.Count == 0)
                {
                    LoadWords();
                }

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
                MessageBox.Show($"Error validating word: {ex.Message}");
            }
        }

        // bring the word from english file from resources into a list called  resource content 
        private void LoadWords()
        {
            try
            {
                // Load words from the resource file
                string resourceContent = Properties.Resources.ukenglish;

                if (string.IsNullOrEmpty(resourceContent))
                {
                    MessageBox.Show("The resource file is empty or missing.");
                    return;
                }

                words = new List<string>(resourceContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

                if (words.Count == 0)
                {
                    MessageBox.Show("No words found in the resource file.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading words: {ex.Message}");
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

                if (control is Label label && !string.IsNullOrEmpty(label.Text))
                {
                    submitted_word += label.Text; // Concatenate label text to form the word
                    label.Text = string.Empty; // Clear the label's text
                }
            }

            // Ensure that the word has been submitted before calculating
            if (string.IsNullOrEmpty(submitted_word))
            {
                MessageBox.Show("Please create a word by dragging letters.");
                return;
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

