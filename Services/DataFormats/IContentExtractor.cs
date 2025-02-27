using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nexia.DataFormats;

/// Interface for content decoders

public interface IContentDecoder
{

    /// Returns true if the decoder supports the given MIME type

    /// "mimeType" MIME type string (e.g. content type without encoding details)
    ///returns =>Whether the MIME type is supported
    bool SupportsMimeType(string mimeType);

    
    /// Extract content from the given file.
    
    /// "filename" Full path to the file to process
    /// "cancellationToken" Async task cancellation token
    /// returns=>Content extracted from the file
    Task<FileContent> DecodeAsync(string filename, CancellationToken cancellationToken = default);


   
    /// Extract content from the given file.
    ///"data" File content to process
    /// "cancellationToken" Async task cancellation token
    /// <returns>Content extracted from the file
    Task<FileContent> DecodeAsync(Stream data, CancellationToken cancellationToken = default);
}