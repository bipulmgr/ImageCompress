using ImageCompressApi.Model;
using ImageMagick;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using Tesseract;

namespace ImageCompressApi.Services;
public class PdfService
{
    public List<QuestionModel> ExtractQuestions(string pdfFilePath)
    {
        List<QuestionModel> questions = new List<QuestionModel>();

        using (PdfReader pdfReader = new PdfReader(pdfFilePath))
        {
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);

                    // Logic to parse currentText and extract questions and answers
                    // This will depend on the structure of your PDF file.

                    // Sample logic (you may need to adjust based on your PDF structure):
                    // Assuming each question has a number and options a, b, c, d.
                    // You need to adapt this to match your specific PDF structure.

                    string[] lines = currentText.Split('\n');
                    QuestionModel currentQuestion = null;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Q"))
                        {
                            currentQuestion = new QuestionModel
                            {
                                Question = line.Trim(),
                                Answers = new Dictionary<string, string>()
                            };
                        }
                        else if (currentQuestion != null && line.Trim().Length > 0)
                        {
                            char option = line[0];
                            string answer = line.Substring(2).Trim();
                            currentQuestion.Answers.Add(option.ToString(), answer);
                        }
                    }

                    if (currentQuestion != null)
                    {
                        questions.Add(currentQuestion);
                    }
                }
            }
        }

        return questions;
    }
    //public List<QuestionModel> ExtractQuestionsWithOCR(string pdfFilePath)
    //{
    //    List<QuestionModel> questions = new List<QuestionModel>();

    //    using (PdfReader pdfReader = new PdfReader(pdfFilePath))
    //    {
    //        using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
    //        {
    //            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
    //            {
    //                var strategy = new SimpleTextExtractionStrategy();
    //                var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);

    //                // Logic to parse currentText and extract questions and answers
    //                // ...

    //                // OCR for images in PDF
    //                var pageResources = pdfDocument.GetPage(page).GetResources();
    //                var imageXObjectNames = pageResources.GetResourceNames(PdfName.XObject);

    //                if (imageXObjectNames != null)
    //                {
    //                    foreach (var name in imageXObjectNames)
    //                    {
    //                        if (pageResources.GetResourceObject(PdfName.XObject,name) is PdfDictionary xObjectDict)
    //                        {
    //                            //var xObject = xObjectDict.GetAsDictionary(name);
    //                            var xObject = xObjectDict.GetAsImage(name);

    //                            // Rest of your code

    //                            if (xObject is PdfImageXObject)
    //                            {
    //                                using (var engine = new TesseractEngine(@"tessdataPath", "eng", EngineMode.Default))
    //                                {
    //                                    var imageXObject = (PdfImageXObject)xObject;
    //                                    byte[] imageData = imageXObject.GetImageBytes();

    //                                    using (var image = new MagickImage(imageData))
    //                                    {
    //                                        using (var pix = PixConverter.ToPix(image.ToBitmap()))
    //                                        {
    //                                            using (var pageProcessor = engine.Process(pix))
    //                                            {
    //                                                var ocrText = pageProcessor.GetText();

    //                                                // Logic to parse ocrText and extract text from images
    //                                                // ...

    //                                                // Add the extracted text to your questions or do further processing
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return questions;
    //}
}
