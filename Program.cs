using Nexia.DataFormats.Pdf;
using Nexia.DataFormats;
using Nexia.Pipeline;

FileContent content = new(MimeTypes.PlainText);
Console.WriteLine("=========================");
Console.WriteLine("=== Text in file1.pdf ===");
Console.WriteLine("=========================");
#pragma warning disable KMEXP00
var pdfDecoder = new PdfExtractor();
#pragma warning restore KMEXP00
content = await pdfDecoder.DecodeAsync("mairie.pdf");

foreach (Chunk section in content.Sections)
{
    Console.WriteLine($"Page: {section.Number}/{content.Sections.Count}");
    Console.WriteLine(section.Content);
    Console.WriteLine("-----");
}

Console.WriteLine("============================");
Console.WriteLine("Press a Enter to continue...");
Console.ReadLine();