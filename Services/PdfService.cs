using ImageCompressApi.Model;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace ImageCompressApi.Services;

public class PdfService
{
    public List<QuestionModel> ExtractQuestions(string pdfFilePath)
    {
        var questions = new List<QuestionModel>();

        using var pdfReader = new PdfReader(pdfFilePath);
        using var pdfDocument = new PdfDocument(pdfReader);

        for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
        {
            var strategy = new SimpleTextExtractionStrategy();
            var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);

            QuestionModel? currentQuestion = null;

            foreach (var line in pageText.Split('\n'))
            {
                if (line.StartsWith("Q"))
                {
                    if (currentQuestion != null)
                        questions.Add(currentQuestion);

                    currentQuestion = new QuestionModel { Question = line.Trim() };
                }
                else if (currentQuestion != null && line.Trim().Length > 1)
                {
                    currentQuestion.Answers[line[0].ToString()] = line[2..].Trim();
                }
            }

            if (currentQuestion != null)
                questions.Add(currentQuestion);
        }

        return questions;
    }
}
