using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;

namespace mobile_app.ViewModels;

public partial class LessonViewModel : ObservableObject
{
    public ObservableCollection<Lesson> Lessons { get; set; }

    public LessonViewModel()
    {
        Lessons = new ObservableCollection<Lesson>
        {
            new Lesson
            {
                Title = "Physical Bullying",
                ShortDescription = "Harming someone's body or possessions.",
                Icon = "👊",
                Color = "#FF6B6B",
                Route = "LessonDetail",
                LongDefinition = "Physical bullying involves hurting someone's body or possessions. It includes hitting, kicking, tripping, pinching, pushing, or damaging someone's property.",
                Examples = new List<string>
                {
                    "• Hitting, kicking, or punching",
                    "• Tripping or pushing someone",
                    "• Breaking or stealing belongings",
                    "• Making rude hand gestures"
                },
                ActionSteps = new List<string>
                {
                    "1. Get away to a safe place immediately.",
                    "2. Stay in a group; bullies are less likely to attack groups.",
                    "3. Tell a trusted adult or campus security."
                }
            },
            new Lesson
            {
                Title = "Cyberbullying",
                ShortDescription = "Bullying via digital devices.",
                Icon = "📱",
                Color = "#FFC652",
                Route = "LessonDetail",
                LongDefinition = "Cyberbullying takes place over digital devices like cell phones, computers, and tablets. It includes sending, posting, or sharing negative, harmful, false, or mean content about someone else.",
                Examples = new List<string>
                {
                    "• Sending mean texts or emails",
                    "• Posting embarrassing photos on social media",
                    "• Spreading rumors online",
                    "• Excluding someone from online groups"
                },
                ActionSteps = new List<string>
                {
                    "1. Do not respond to the bully.",
                    "2. Save the evidence (screenshots).",
                    "3. Block the bully and report them to the platform."
                }
            },
            new Lesson
            {
                Title = "Verbal Bullying",
                ShortDescription = "Using words to hurt others.",
                Icon = "🗣️",
                Color = "#4ECDC4",
                Route = "LessonDetail",
                LongDefinition = "Verbal bullying is saying or writing mean things. It includes teasing, name-calling, inappropriate sexual comments, taunting, or threatening to cause harm.",
                Examples = new List<string>
                {
                    "• Teasing or name-calling",
                    "• Inappropriate sexual comments",
                    "• Taunting or threatening to cause harm"
                },
                ActionSteps = new List<string>
                {
                    "1. Ignore them and walk away.",
                    "2. Tell them to stop in a firm voice.",
                    "3. Report the incident to a counselor."
                }
            },
            new Lesson
            {
                Title = "Emotional Bullying",
                ShortDescription = "Harming social reputation.",
                Icon = "💔",
                Color = "#4A90E2",
                Route = "LessonDetail",
                LongDefinition = "Also known as relational aggression, this involves hurting someone's reputation or relationships. It includes leaving someone out on purpose or telling others not to be friends with someone.",
                Examples = new List<string>
                {
                    "• Leaving someone out on purpose",
                    "• Telling others not to be friends with someone",
                    "• Spreading rumors to ruin a reputation",
                    "• Public embarrassment"
                },
                ActionSteps = new List<string>
                {
                    "1. Focus on your true friends.",
                    "2. Participate in activities you enjoy.",
                    "3. Talk to a counselor about your feelings."
                }
            }
        };
    }

    [RelayCommand]
    private async Task GoToDetails(Lesson lesson)
    {
        // Navigate and pass the Title as a parameter
        await Shell.Current.GoToAsync($"{lesson.Route}?Title={lesson.Title}");
    }
}