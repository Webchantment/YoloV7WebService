using System.Diagnostics;
using System.Drawing;
using Yolov7net;

var useCuda = false;
if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "-gpu")
{
    useCuda = true;
    Console.WriteLine("Running in GPU Mode");
}
else
    Console.WriteLine("Running in CPU Mode");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var yolov7 = new Yolov7("./Assets/yolov7-tiny.onnx", useCuda);
yolov7.SetupYoloDefaultLabels();

app.MapGet("/predictions/{imagePath}", (string imagePath) =>
{
    try
    {
        using var image = Image.FromFile(imagePath);
        using var graphics = Graphics.FromImage(image);
        var predictions = yolov7.Predict(image);
        var detectedObjects = new List<DetectedObject>();

        bool personDetected = false;
        foreach (var prediction in predictions)
        {
            if (prediction.Label.Name == "person")
            {
                personDetected = true;
                string label = $"{prediction.Label.Name} {Math.Round(prediction.Score, 2)}\r\n" +
                               $"{(int)prediction.Rectangle.Height} x {(int)prediction.Rectangle.Width}\r\n" +
                               $"{(int)prediction.Rectangle.X}, {(int)prediction.Rectangle.Y}";

                Font font = new Font("Consolas", 14, GraphicsUnit.Pixel);
                var (x, y) = (prediction.Rectangle.X, prediction.Rectangle.Y + prediction.Rectangle.Height - 50);

                graphics.DrawRectangles(new Pen(prediction.Label.Color, 1), new[] { prediction.Rectangle });
                graphics.DrawString(label, font, new SolidBrush(Color.Black), new PointF(x, y));
                graphics.DrawString(label, font, new SolidBrush(Color.White), new PointF(x + 1, y + 1));
            }

            detectedObjects.Add(new DetectedObject(prediction.Label.Name, prediction.Score, prediction.Rectangle.X,
                                                   prediction.Rectangle.Y, prediction.Rectangle.Width, prediction.Rectangle.Height));
        }

        if (personDetected)
        {
            string newFilePath = imagePath.Insert(imagePath.LastIndexOf('.'), "-person");
            image.Save(newFilePath);
        }

        return detectedObjects;
    }
    catch
    {
        return new List<DetectedObject>();
    }
})
.WithName("GetPredictions");

app.MapGet("/benchmark/{imagePath}", (string imagePath) =>
{
    try
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        using var image = Image.FromFile(imagePath);
        var predictions = yolov7.Predict(image);

        sw.Stop();

        return sw.ElapsedMilliseconds + " ms";
    }
    catch
    {
        return "Error opening image";
    }
})
.WithName("GetBenchmark");

app.Run();

internal record DetectedObject(string name, float score, float x, float y, float width, float height) {}