using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using DiplomDobrotvorskiy.Models;
using Svg;

namespace DiplomDobrotvorskiy.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ConverterXmlToSvg();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void CreateSVG()
    {
        var svgDocument = new SvgDocument();

        // Создаем элементы SVG, например, прямоугольник и круг
        var rectangle = new SvgRectangle
        {
            Width = 200,
            Height = 100,
            Fill = new SvgColourServer(System.Drawing.Color.Blue),
            Stroke = new SvgColourServer(System.Drawing.Color.Black),
            StrokeWidth = 2
        };
        svgDocument.Children.Add(rectangle);

        var circle = new SvgCircle
        {
            CenterX = 150,
            CenterY = 75,
            Radius = 50,
            Fill = new SvgColourServer(System.Drawing.Color.Yellow),
            Stroke = new SvgColourServer(System.Drawing.Color.Black),
            StrokeWidth = 2
        };
        svgDocument.Children.Add(circle);

        // Создаем путь к файлу, куда сохранить SVG
        var filePath = Path.Combine("wwwroot", "example.svg");

        // Сохраняем SVG-изображение в указанный путь
        svgDocument.Write(filePath);

        Console.WriteLine("SVG-изображение сохранено по пути: " + filePath);
    }


    public static void ConverterXmlToSvg()
    {
        List<string> xmlFilePaths = new List<string>();
        xmlFilePaths.Add(Path.Combine("wwwroot", "testik3.xml"));
        xmlFilePaths.Add(Path.Combine("wwwroot", "testik2.xml"));
        string svgFilePath = Path.Combine("wwwroot", "example.svg");

            try
            {
                XElement mergedXml = new XElement("Root");

                foreach (var VARIABLE in xmlFilePaths)
                {
                    XElement xml = XElement.Load(VARIABLE);
                    mergedXml.Add(xml.Elements());
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(mergedXml.ToString());
                var coordinats = xmlDoc.GetElementsByTagName("Coordinate");
                double minValue = 0;
                double maxValue = 0;
               
                foreach (XmlNode coordinate in coordinats)
                {
                    double value = double.Parse(coordinate.InnerText);
                    if (value < minValue)
                        minValue = value;

                    if (value > maxValue)
                        maxValue = value;
                }
                bool isNegativeValue = minValue < 0;
                double scaleFactor = 1.0;
                double bboxWidth = Math.Abs(maxValue - minValue);
                double bboxHeight = Math.Abs(maxValue - minValue);
                if (bboxWidth > 600 || bboxHeight > 600)
                {
                    double scaleX = 600 / bboxWidth;
                    double scaleY = 600 / bboxHeight;
                    scaleFactor = Math.Min(scaleX, scaleY);
                }
                
                StringBuilder svgBuilder = new StringBuilder();

                svgBuilder.AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"600\" height=\"600\">");

                XmlNodeList layers = xmlDoc.GetElementsByTagName("Layer"); 
                foreach (XmlNode layer in layers)
                {
                    
                var color = layer.Attributes["Color"]?.Value;
                XmlNodeList polygon = layer.SelectNodes("Polygon");

                foreach (XmlNode i in polygon)
                {
                    var polygonName = i.Attributes["Name"]?.Value;
                    XmlNodeList coordinates = i.ChildNodes;

                    // svgBuilder.AppendLine("<polygon name=\"{polygonName}\">");

                    var firstX = coordinates[0].InnerText;
                    var firstY = coordinates[1].InnerText;
                    for (var index = 0; index < coordinates.Count; index = index + 2)
                    {
                        var x1 = double.Parse(coordinates[index].InnerText) * scaleFactor;
                        var y1 = double.Parse(coordinates[index + 1].InnerText) * scaleFactor;
                        double x2 = 0;
                        double y2 = 0;
                        if (coordinates.Count > index + 2)
                        {
                            x2 = double.Parse(coordinates[index + 2].InnerText) * scaleFactor;
                            y2 = double.Parse(coordinates[index + 3].InnerText) * scaleFactor;
                        }
                        else
                        {
                            x2 = double.Parse(firstX) * scaleFactor;
                            y2 = double.Parse(firstY) * scaleFactor;
                        }

                        if (isNegativeValue)
                        {
                            x1 += Math.Abs(minValue)* scaleFactor;
                            y1 += Math.Abs(minValue)* scaleFactor;
                            x2 += Math.Abs(minValue)* scaleFactor;
                            y2 += Math.Abs(minValue)* scaleFactor;
                        }
                        svgBuilder.AppendLine($"<line x1=\"{x1.ToString(CultureInfo.InvariantCulture)}\" " +
                                              $"y1=\"{y1.ToString(CultureInfo.InvariantCulture)}\"" +
                                              $" x2=\"{x2.ToString(CultureInfo.InvariantCulture)}\"" +
                                              $" y2=\"{y2.ToString(CultureInfo.InvariantCulture)}\" stroke=\"{color}\"/>");

               
                    }

                    // svgBuilder.AppendLine($"</polygon>");
                }

                XmlNodeList ribs = layer.SelectNodes("Ribs/Rib");
                foreach (XmlNode rib in ribs)
                {
                    XmlNodeList coordinates = rib.ChildNodes;

                    var x1 = double.Parse(coordinates[0].InnerText)*scaleFactor;
                    var y1 = double.Parse(coordinates[1].InnerText)*scaleFactor;
                    var x2 = double.Parse(coordinates[2].InnerText)*scaleFactor;
                    var y2 = double.Parse(coordinates[3].InnerText)*scaleFactor;
                    if (isNegativeValue)
                    {
                        x1 += Math.Abs(minValue)* scaleFactor;
                        y1 += Math.Abs(minValue)* scaleFactor;
                        x2 += Math.Abs(minValue)* scaleFactor;
                        y2 += Math.Abs(minValue)* scaleFactor;
                    }
                    svgBuilder.AppendLine($"<line x1=\"{x1.ToString(CultureInfo.InvariantCulture)}\" " +
                                          $"y1=\"{y1.ToString(CultureInfo.InvariantCulture)}\"" +
                                          $" x2=\"{x2.ToString(CultureInfo.InvariantCulture)}\"" +
                                          $" y2=\"{y2.ToString(CultureInfo.InvariantCulture)}\" stroke=\"{color}\"/>");
                }
                }

                svgBuilder.AppendLine("</svg>");

                // Save the SVG content to a file
                System.IO.File.WriteAllText(svgFilePath, svgBuilder.ToString());

                Console.WriteLine("SVG image generated successfully!");
                }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
        }


    
}