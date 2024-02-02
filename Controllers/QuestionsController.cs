using ImageCompressApi.Model;
using ImageCompressApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageCompressApi.Controllers;

[Route("api/questions")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly PdfService _pdfService;

    public QuestionsController(PdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<QuestionModel>> GetQuestions()
    {
        string pdfFilePath = "path/to/your/file.pdf";
        List<QuestionModel> questions = _pdfService.ExtractQuestions(pdfFilePath);
        return Ok(questions);
    }

    [HttpPost("upload")]
    public ActionResult<IEnumerable<QuestionModel>> UploadFile([FromForm] UploadModel model)
    {
        if (model.File != null && model.File.Length > 0)
        {
            string fileName = $"{Path.GetTempFileName()}.pdf";

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                model.File.CopyTo(fileStream);
            }

            List<QuestionModel> questions = _pdfService.ExtractQuestions(fileName);
            return Ok(questions);
        }

        return BadRequest("File not provided or empty.");
    }
}
