// THE place where conversion engines are registered. Adding an engine = one line in CreateAll.

using TransRing.Core.Conversion;
using TransRing.Engines.Archives;
using TransRing.Engines.Cad;
using TransRing.Engines.Data;
using TransRing.Engines.Ebooks;
using TransRing.Engines.Email;
using TransRing.Engines.Fonts;
using TransRing.Engines.Hwp;
using TransRing.Engines.Hwp.Hwpx;
using TransRing.Engines.Icns;
using TransRing.Engines.Infrastructure;
using TransRing.Engines.Magick;
using TransRing.Engines.Media;
using TransRing.Engines.Models3D;
using TransRing.Engines.Office;
using TransRing.Engines.Pdf;
using TransRing.Engines.Subtitles;
using TransRing.Engines.Text;
using TransRing.Engines.Vector;

namespace TransRing.Engines;

/// <summary>Creates every built-in converter.</summary>
public static class EngineRegistry
{
    /// <summary>
    /// Instantiates all converters. Order only matters as a tie-breaker when no priority is set, so it mirrors
    /// AppSettings.EnginePriority: built-in engines first, the optional downloads last. Software installed on the
    /// system (Microsoft Office, a system-wide LibreOffice) is never used.
    /// </summary>
    public static IReadOnlyList<IConverter> CreateAll(EngineEnvironment env) =>
    [
        new MagickImageConverter(),
        new PdfSharpConverter(),
        new PdfiumConverter(),
        new RhwpConverter(),
        new UnhwpConverter(),
        new MarkdownConverter(),
        new SpreadsheetConverter(),
        new OoxmlConverter(),
        new HwpxConverter(),
        new ArchiveConverter(),
        new VectorConverter(),
        new IcnsConverter(),
        new FontConverter(),
        new DocxWriterConverter(),
        new PdfTextConverter(),
        new EmlConverter(),
        new DataConverter(),
        new SubtitleConverter(),
        new FfmpegConverter(),
        new CadConverter(),
        new ModelConverter(),
        new EbookConverter(),
        new PandocConverter(),
        new GhostscriptConverter(),
        new CalibreConverter(),
        new LibreOfficeConverter(env),
    ];

    /// <summary>Finds the PDF merger among the converters.</summary>
    public static IPdfMerger? FindPdfMerger(IEnumerable<IConverter> converters) =>
        converters.OfType<IPdfMerger>().FirstOrDefault();

    /// <summary>Finds the engine that writes several pages into one multi-page TIFF.</summary>
    public static ITiffMerger? FindTiffMerger(IEnumerable<IConverter> converters) =>
        converters.OfType<ITiffMerger>().FirstOrDefault();

    /// <summary>Finds the engine that packs several files into one archive ("Compress into one archive").</summary>
    public static IFileCombiner? FindFileCombiner(IEnumerable<IConverter> converters) =>
        converters.OfType<IFileCombiner>().FirstOrDefault();
}
