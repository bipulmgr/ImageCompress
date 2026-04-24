namespace ImageCompressApi.Model;

public class QuestionModel
{
    public string Question { get; set; } = string.Empty;
    public Dictionary<string, string> Answers { get; set; } = [];
}
