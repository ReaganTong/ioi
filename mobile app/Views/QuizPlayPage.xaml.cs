using mobile_app.Models;
using mobile_app.ViewModels;
using System.Web;

namespace mobile_app.Views;

[QueryProperty(nameof(QuizTitle), "Title")]
public partial class QuizPlayPage : ContentPage
{
    private QuizViewModel _viewModel;
    private Quiz? _currentQuiz; 
    private int _currentQuestionIndex = 0;
    private int _score = 0;


  
    private string _quizTitle = string.Empty; 

    public string QuizTitle
    {
        get => _quizTitle;
        set => _quizTitle = System.Web.HttpUtility.UrlDecode(value ?? string.Empty);
    }


    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        LoadQuiz(); 
    }

    public QuizPlayPage(QuizViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadQuiz();
    }

    private void LoadQuiz()
    {
     
        _currentQuiz = _viewModel.Quizzes.FirstOrDefault(q => q.Title == QuizTitle);

        if (_currentQuiz != null && _currentQuiz.Questions.Count > 0)
        {
            _currentQuestionIndex = 0;
            _score = 0;
            ShowQuestion();
        }
        else
        {
            lblQuestion.Text = $"Error: Cannot find {QuizTitle}"; 
        }
    }

    private void ShowQuestion()
    {
        var q = _currentQuiz.Questions[_currentQuestionIndex];
        lblProgress.Text = $"Question {_currentQuestionIndex + 1} of {_currentQuiz.Questions.Count}";
        lblQuestion.Text = q.Text;

        ResetButton(btnOption1, q.OptionA, 1);
        ResetButton(btnOption2, q.OptionB, 2);
        ResetButton(btnOption3, q.OptionC, 3);

        lblFeedback.IsVisible = false;
        btnNext.IsVisible = false;
        EnableButtons(true);
    }

    private void ResetButton(Button btn, string text, int optionId)
    {
        btn.Text = text;
        btn.BackgroundColor = Color.FromArgb("#2196F3"); 
        btn.ClassId = optionId.ToString();
    }

    private void OnAnswerClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int selectedOption = int.Parse(button.ClassId);
        var correctOption = _currentQuiz.Questions[_currentQuestionIndex].CorrectOption;

        EnableButtons(false);

        if (selectedOption == correctOption)
        {
            button.BackgroundColor = Colors.Green;
            lblFeedback.Text = "Correct! 🎉";
            lblFeedback.TextColor = Colors.Green;
            _score++;
        }
        else
        {
            button.BackgroundColor = Colors.Red;
            lblFeedback.Text = "Wrong answer.";
            lblFeedback.TextColor = Colors.Red;
        }

        lblFeedback.IsVisible = true;
        btnNext.IsVisible = true;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        _currentQuestionIndex++;
        if (_currentQuestionIndex < _currentQuiz.Questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            await DisplayAlert("Quiz Finished", $"Score: {_score}/{_currentQuiz.Questions.Count}", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    private void EnableButtons(bool enable)
    {
        btnOption1.IsEnabled = enable;
        btnOption2.IsEnabled = enable;
        btnOption3.IsEnabled = enable;
    }
}