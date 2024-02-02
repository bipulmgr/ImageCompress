namespace ImageCompressApi.Model;

public class QuestionModel
{
    public string Question { get; set; }
    public Dictionary<string, string> Answers { get; set; }
}

public class UploadModel
{
    public IFormFile File { get; set; }
}
