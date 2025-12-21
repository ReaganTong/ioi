using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using System; 

namespace mobile_app.ViewModels;

public partial class QuizViewModel : ObservableObject
{
    public ObservableCollection<Quiz> Quizzes { get; set; }

    public QuizViewModel()
    {
        Quizzes = new ObservableCollection<Quiz>
        {
         
            new Quiz
            {
                Title = "Cyberbullying Awareness",
                Description = "Test your knowledge on digital safety.",
                Icon = "cyber.png",
                Color = "#FF6B6B",
                Questions = new List<Question>
                {
                    new Question { Text = "What should you do if you receive a mean message?", OptionA = "Delete it immediately", OptionB = "Reply with a mean message", OptionC = "Save evidence and tell an adult", CorrectOption = 3 },
                    new Question { Text = "Is excluding someone from an online group bullying?", OptionA = "Yes, it is exclusion", OptionB = "No, it's just a preference", OptionC = "Only if you tell them", CorrectOption = 1 },
                    new Question { Text = "What is the best way to handle a 'troll' online?", OptionA = "Argue with them", OptionB = "Ignore and block them", OptionC = "Share their post to your friends", CorrectOption = 2 },
                    new Question { Text = "Is it okay to share someone's private photo without permission?", OptionA = "Yes, if it's funny", OptionB = "No, it's a violation of privacy", OptionC = "Only in a private group", CorrectOption = 2 },
                    new Question { Text = "What should you check before posting a comment?", OptionA = "If it is helpful and kind", OptionB = "If it will get many likes", OptionC = "If the person will be angry", CorrectOption = 1 },
                    new Question { Text = "If a friend shares their password with you, you should:", OptionA = "Log in to check their messages", OptionB = "Keep it secret but never use it", OptionC = "Tell them to change it for safety", CorrectOption = 3 },
                    new Question { Text = "What is 'Flaming' in cyberbullying?", OptionA = "Sending angry and rude messages", OptionB = "Posting a nice picture", OptionC = "Blocking someone", CorrectOption = 1 },
                    new Question { Text = "Someone is being bullied in a group chat. You should:", OptionA = "Stay silent to avoid trouble", OptionB = "Private message the victim to offer support", OptionC = "Leave the group and forget about it", CorrectOption = 2 },
                    new Question { Text = "Is it safe to meet someone you only know from online games?", OptionA = "Yes, if they seem nice", OptionB = "No, unless a trusted adult is with you", OptionC = "Yes, if they are your age", CorrectOption = 2 },
                    new Question { Text = "What is 'Cyberstalking'?", OptionA = "Liking all of a friend's photos", OptionB = "Repeated harassment and monitoring online", OptionC = "Searching for a celebrity", CorrectOption = 2 }
                }
            },
  
            new Quiz
            {
                Title = "Verbal Harassment",
                Description = "Identify verbal harassment.",
                Icon = "verbal.png",
                Color = "#4ECDC4",
                Questions = new List<Question>
                {
                    new Question { Text = "Which of these is verbal bullying?", OptionA = "Giving a compliment", OptionB = "Name-calling and teasing", OptionC = "Asking for help", CorrectOption = 2 },
                    new Question { Text = "What should you do if you see someone being teased?", OptionA = "Join in the laughter", OptionB = "Walk away and ignore it", OptionC = "Support the victim or tell a teacher", CorrectOption = 3 },
                    new Question { Text = "Can 'just joking' still be verbal bullying?", OptionA = "Yes, if it hurts the other person", OptionB = "No, jokes are always okay", OptionC = "Only if the teacher hears it", CorrectOption = 1 },
                    new Question { Text = "Which is an example of verbal harassment?", OptionA = "Threatening to hurt someone", OptionB = "Saying 'Good morning'", OptionC = "Discussing a group project", CorrectOption = 1 },
                    new Question { Text = "How does verbal bullying usually make a person feel?", OptionA = "Motivated to change", OptionB = "Sad, lonely, or anxious", OptionC = "Excited and happy", CorrectOption = 2 },
                    new Question { Text = "Spreading rumors about someone is called:", OptionA = "Social/Verbal Bullying", OptionB = "Making conversation", OptionC = "Being honest", CorrectOption = 1 },
                    new Question { Text = "If someone tells you to stop 'joking' about them, you should:", OptionA = "Tell them they are too sensitive", OptionB = "Stop immediately and apologize", OptionC = "Keep going until they laugh", CorrectOption = 2 },
                    new Question { Text = "Is mocking someone's accent verbal harassment?", OptionA = "No, it's just fun", OptionB = "Yes, it is disrespectful and bullying", OptionC = "Only if they get angry", CorrectOption = 2 },
                    new Question { Text = "What is the best way to respond to a verbal bully?", OptionA = "Scream back at them", OptionB = "Stay calm, walk away, and report it", OptionC = "Cry in front of them", CorrectOption = 2 },
                    new Question { Text = "Verbal bullying can happen:", OptionA = "Only in person", OptionB = "Anywhere people communicate", OptionC = "Only at school", CorrectOption = 2 }
                }
            }
        };
    }

    [RelayCommand]
    private async Task PlayQuiz(Quiz quiz)
    {
        if (quiz == null) return;
        string encodedTitle = Uri.EscapeDataString(quiz.Title);
        await Shell.Current.GoToAsync($"QuizPlay?Title={encodedTitle}");
    }
}