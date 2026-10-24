// Enforces the free edition's limits on a job before anything converts, so the App (donut, watch
// folders) and the Cli share exactly one rule set. Paid builds are a no-op here.
//
// Rules (free edition only):
// - Batch: only the first file converts; the rest fail with the batch message (a visible, explained
//   rejection in the toast and in Cli output, not a silent drop).
// - Size: a single file over 100 MB fails with the size message.
// - Formats: a source file whose format is outside the free set, or an output target outside the
//   free set, fails with the format message. Unknown sources (no detected format) and folders are
//   left to the existing unsupported-source handling further down the pipeline.
// Idempotent: files already decided (Failed) are left alone, so calling Apply twice (Enqueue, then
// the worker) is safe.

using TransRing.Core.Formats;
using TransRing.Core.Presets;

namespace TransRing.Core.Conversion;

internal static class EditionLimits
{
    public static void Apply(ConversionJob job)
    {
        if (!Edition.IsFree)
            return;

        var files = job.Files;
        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];
            if (file.State != FileState.Pending)
                continue;

            if (i > 0)
            {
                Fail(file, Edition.FreeBatchLimit);
                continue;
            }

            if (File.Exists(file.SourcePath)
                && new FileInfo(file.SourcePath).Length > Edition.FreeMaxSingleFileBytes)
            {
                Fail(file, Edition.FreeSizeLimit);
                continue;
            }

            // Folders (archive packing, watch folders) keep the existing pipeline behavior.
            if (Directory.Exists(file.SourcePath))
                continue;

            var source = FormatRegistry.Detect(file.SourcePath);
            if (source is not null && !Edition.IsFreeFormat(source.Id))
            {
                Fail(file, Edition.FreeSourceNotSupported);
                continue;
            }

            var target = job.Preset.TargetFormat == BuiltInData.SameAsSource ? source?.Id : job.Preset.TargetFormat;
            if (target is not null && !Edition.IsFreeFormat(target))
                Fail(file, Edition.FreeTargetNotSupported);
        }
    }

    private static void Fail(FileResult file, string message)
    {
        file.State = FileState.Failed;
        // Reuses the existing "conversion failed" key on purpose: the localizer falls back to the key
        // itself for unknown keys, so a new key would show up raw; the full Chinese message rides in
        // ErrorDetail, which both the toast and the Cli print.
        file.ErrorKey = "error.conversion_failed";
        file.ErrorDetail = message;
        file.Progress = 1;
    }
}
