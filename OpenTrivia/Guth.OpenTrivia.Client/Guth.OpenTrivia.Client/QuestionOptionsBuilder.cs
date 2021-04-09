using System;
using Guth.OpenTrivia.Client.Enums;

namespace Guth.OpenTrivia.Client
{
    public class QuestionOptionsBuilder
    {
        public int NumberOfQuestions { get; private set; } = 10;
        public QuestionDifficulty? Difficulty { get; private set; }
        public QuestionType? Type { get; private set; }
        public QuestionCategory? Category { get; private set; }


        public QuestionOptionsBuilder WithCategory(QuestionCategory category)
        {
            Category = category;
            return this;
        }

        public QuestionOptionsBuilder WithNumberOfQuestions(int numberOfQuestions)
        {
            if (numberOfQuestions < 1 || numberOfQuestions > 50)
            {
                throw new InvalidOperationException("Number of questions must be between 1 and 50");
            }
            NumberOfQuestions = numberOfQuestions;
            return this;
        }

        public QuestionOptionsBuilder WithDifficulty(QuestionDifficulty difficulty)
        {
            Difficulty = difficulty;
            return this;
        }

        public QuestionOptionsBuilder WithQuestionType(QuestionType type)
        {
            Type = type;
            return this;
        }
    }
}
