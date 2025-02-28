
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexia.Diagnostics;
using Nexia.Pipeline;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Nexia.DataFormats.Pdf;

[Experimental("KMEXP00")]
public sealed class PdfExtractor : IContentDecoder
{
    private readonly ILogger<PdfExtractor> _log;

    public PdfExtractor(ILoggerFactory? loggerFactory = null)
    {
        this._log = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<PdfExtractor>();
    }

    /// <inheritdoc />
    public bool SupportsMimeType(string mimeType)
    {
        return mimeType != null && mimeType.StartsWith(MimeTypes.Pdf, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Task<FileContent> DecodeAsync(string filename, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(filename);
        return this.DecodeAsync(stream, cancellationToken);
    }

    /// <inheritdoc />
  
   
    public Task<FileContent> DecodeAsync(Stream data, CancellationToken cancellationToken = default)
    {
            
        this._log.LogDebug("Extracting text from PDF file");

        var result = new FileContent(MimeTypes.PlainText);
        using PdfDocument? pdfDocument = PdfDocument.Open(data);
        if (pdfDocument == null) { return Task.FromResult(result); }

        foreach (Page? page in pdfDocument.GetPages().Where(x => x != null))
        {
            // Note: no trimming, use original spacing when working with pages
            string pageContent = ContentOrderTextExtractor.GetText(page) ?? string.Empty;
            pageContent = pageContent.Replace("\r\n", "\n").Replace("\r", "\n");

            result.Sections.Add(new Chunk(pageContent, page.Number, Chunk.Meta(sentencesAreComplete: false)));
        }

        return Task.FromResult(result);
    }
}