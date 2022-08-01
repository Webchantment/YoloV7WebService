
using System.Drawing;

namespace Yolov7net.test
{
    public class Yolov7Test
    {
        [Fact]
        public void InitTest()
        {
            
            using var yolo = new Yolov7("./assets/yolov7-tiny.onnx"); //yolov7 ģ��,����Ҫ nms ����
            // setup labels of onnx model 
            yolo.SetupYoloDefaultLabels();   // use custom trained model should use your labels like: yolo.SetupLabels(string[] labels)
            Assert.NotNull(yolo);
            using var image = Image.FromFile("Assets/demo.jpg");
            var ret = yolo.Predict(image);
            Assert.NotNull(ret);
            Assert.True(ret.Count == 1);
        }

        [Fact]
        public void TestYolov5()
        {
            using var yolo = new Yolov5("./assets/yolov7-tiny_640x640.onnx"); //yolov7 ģ��,����Ҫ nms ����
            // setup labels of onnx model 
            yolo.SetupYoloDefaultLabels();   // use custom trained model should use your labels like: yolo.SetupLabels(string[] labels)
            Assert.NotNull(yolo);
            using var image = Image.FromFile("Assets/demo.jpg");
            var ret = yolo.Predict(image);
            Assert.NotNull(ret);
            Assert.True(ret.Count == 1);
        }
    }
}