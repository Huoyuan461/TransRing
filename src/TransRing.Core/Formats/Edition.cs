// Free vs paid edition of TransRing.
//
// The edition is a compile-time switch, not a runtime license key: TransRing has no activation
// system, the free and paid editions ship as two different installers. Build the free edition with
// `-p:TransRingFree=true` (see Directory.Build.props, which turns it into the TRANSRING_FREE symbol
// for every project); a build without it is the full paid edition and behaves exactly as before.
//
// `IsFree` is a static readonly (not a const) on purpose: a const would be inlined into referencing
// assemblies at their own compile time, so a mixed build could disagree with Core about the edition.

namespace TransRing.Core.Formats;

/// <summary>Which edition this build is, and what the free edition allows.</summary>
public static class Edition
{
#if TRANSRING_FREE
    /// <summary>True for the free edition build, false for the full paid edition.</summary>
    public static readonly bool IsFree = true;
#else
    /// <summary>True for the free edition build, false for the full paid edition.</summary>
    public static readonly bool IsFree = false;
#endif

    /// <summary>Free edition: largest single file that may be converted (100 MB).</summary>
    public const long FreeMaxSingleFileBytes = 100L * 1024 * 1024;

    /// <summary>
    /// Free edition output formats, by format id: common images, PDF, and mainstream Word / Excel /
    /// PowerPoint formats plus their ODF counterparts. Everything else (CAD, 3D models, fonts, e-books,
    /// archives, audio, video, HWP, rare images, ...) is paid-only. The same set gates source files:
    /// a dropped file whose format is not in this set is refused in the free edition with a clear message.
    /// </summary>
    public static readonly IReadOnlySet<string> FreeFormatIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Common images
        "png", "jpg", "webp", "gif", "bmp", "tiff", "ico", "avif", "heic", "svg",
        // PDF
        "pdf",
        // Word processing + ODF
        "docx", "doc", "odt", "rtf",
        // Spreadsheets + ODF
        "xlsx", "xls", "ods", "csv",
        // Presentations + ODF
        "pptx", "ppt", "odp",
    };

    /// <summary>True when <paramref name="formatId"/> may be used in this build (always true for paid).</summary>
    public static bool IsFreeFormat(string formatId) => !IsFree || FreeFormatIds.Contains(formatId);

    /// <summary>Message for a dropped source file whose format the free edition does not support.</summary>
    public const string FreeSourceNotSupported =
        "免费版不支持该格式，完整格式请使用收费版（见官网购买页）。";

    /// <summary>Message for an output format the free edition does not support.</summary>
    public const string FreeTargetNotSupported =
        "免费版不支持该输出格式，完整格式请使用收费版（见官网购买页）。";

    /// <summary>Message for the files past the first one of a multi-file drop in the free edition.</summary>
    public const string FreeBatchLimit =
        "免费版一次只转 1 个文件，批量转换请使用收费版（见官网购买页）。";

    /// <summary>Message for a single file over the free edition's size limit.</summary>
    public const string FreeSizeLimit =
        "免费版单文件上限 100MB，超大文件请使用收费版（见官网购买页）。";
}
