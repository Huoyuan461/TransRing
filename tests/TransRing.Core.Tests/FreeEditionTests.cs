// Free vs paid edition behavior, selected by the build: compile the tests with
// `-p:TransRingFree=true` for the free edition, without it for the paid edition.
// The free tests only exist in free builds (and vice versa), so neither build runs the other
// edition's expectations.

using TransRing.Core.Conversion;
using TransRing.Core.Formats;
using TransRing.Core.Presets;

namespace TransRing.Core.Tests;

#if TRANSRING_FREE

/// <summary>Free edition limits: whitelist, one file per drop, 100 MB per file.</summary>
public class FreeEditionTests
{
    private static async Task<ConversionJob> Run(JobQueue queue, IReadOnlyList<string> sources, Preset preset)
    {
        var job = new ConversionJob(sources, preset, "Test");
        await queue.RunAsync(job);
        return job;
    }

    [Fact]
    public void Free_edition_flag_is_on()
    {
        Assert.True(Edition.IsFree);
    }

    [Fact]
    public void Free_whitelist_is_exactly_the_approved_set()
    {
        string[] expected =
        [
            "png", "jpg", "webp", "gif", "bmp", "tiff", "ico", "avif", "heic", "svg", "pdf",
            "docx", "doc", "odt", "rtf", "xlsx", "xls", "ods", "csv", "pptx", "ppt", "odp",
        ];
        Assert.Equal(expected.OrderBy(x => x), Edition.FreeFormatIds.OrderBy(x => x));
        foreach (var paid in new[] { "dwg", "stl", "ttf", "epub", "zip", "mp3", "mp4", "hwp", "txt", "jxl" })
            Assert.DoesNotContain(paid, Edition.FreeFormatIds);
    }

    [Fact]
    public async Task Free_target_inside_whitelist_converts()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "png"))]));

        var job = await Run(queue, [a], new Preset { TargetFormat = "png" });

        Assert.Equal(JobState.Completed, job.State);
        Assert.True(File.Exists(Path.Combine(dir.Path, "a.png")));
    }

    [Fact]
    public async Task Free_target_outside_whitelist_is_rejected_with_a_clear_message()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "dwg"))]));

        var job = await Run(queue, [a], new Preset { TargetFormat = "dwg" });

        Assert.Equal(JobState.Failed, job.State);
        Assert.Equal(FileState.Failed, job.Files.Single().State);
        Assert.Contains("免费版", job.Files.Single().ErrorDetail);
        Assert.False(File.Exists(Path.Combine(dir.Path, "a.dwg")));
    }

    [Fact]
    public async Task Free_source_outside_whitelist_is_rejected_not_silently_dropped()
    {
        using var dir = new TempDir();
        var model = dir.File("model.dwg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("dwg", "png"))]));

        var job = await Run(queue, [model], new Preset { TargetFormat = "png" });

        Assert.Equal(JobState.Failed, job.State);
        Assert.Contains("免费版", job.Files.Single().ErrorDetail);
        Assert.False(File.Exists(Path.Combine(dir.Path, "model.png")));
    }

    [Fact]
    public async Task Free_batch_converts_only_the_first_file()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        var b = dir.File("b.jpg");
        var c = dir.File("c.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "png"))]));

        var job = await Run(queue, [a, b, c], new Preset { TargetFormat = "png" });

        Assert.Equal(JobState.CompletedWithErrors, job.State);
        Assert.Equal(FileState.Done, job.Files[0].State);
        Assert.Equal([FileState.Failed, FileState.Failed], job.Files.Skip(1).Select(f => f.State));
        Assert.All(job.Files.Skip(1), f => Assert.Contains("1 个文件", f.ErrorDetail));
        Assert.True(File.Exists(Path.Combine(dir.Path, "a.png")));
        Assert.False(File.Exists(Path.Combine(dir.Path, "b.png")));
        Assert.False(File.Exists(Path.Combine(dir.Path, "c.png")));
    }

    [Fact]
    public async Task Free_batch_limit_also_applies_through_Enqueue()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        var b = dir.File("b.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "png"))]));

        var finished = new TaskCompletionSource<ConversionJob>();
        queue.JobFinished += (_, j) => finished.TrySetResult(j);
        var job = queue.Enqueue([a, b], new Preset { TargetFormat = "png" }, "Test");
        var done = await finished.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Same(job, done);
        Assert.Equal(FileState.Done, done.Files[0].State);
        Assert.Equal(FileState.Failed, done.Files[1].State);
        Assert.Contains("1 个文件", done.Files[1].ErrorDetail);
    }

    [Fact]
    public async Task Free_file_over_100MB_is_rejected()
    {
        using var dir = new TempDir();
        var big = Path.Combine(dir.Path, "big.jpg");
        using (var fs = new FileStream(big, FileMode.Create))
            fs.SetLength(Edition.FreeMaxSingleFileBytes + 1); // sparse: instant, still reports its length
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "png"))]));

        var job = await Run(queue, [big], new Preset { TargetFormat = "png" });

        Assert.Equal(JobState.Failed, job.State);
        Assert.Contains("100MB", job.Files.Single().ErrorDetail);
        Assert.False(File.Exists(Path.Combine(dir.Path, "big.png")));
    }
}

#else

/// <summary>Paid edition: no edition limits at all — behavior identical to before the split.</summary>
public class PaidEditionTests
{
    [Fact]
    public void Paid_edition_flag_is_off()
    {
        Assert.False(Edition.IsFree);
    }

    [Fact]
    public async Task Paid_batch_converts_every_file()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        var b = dir.File("b.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "png"))]));

        var job = new ConversionJob([a, b], new Preset { TargetFormat = "png" }, "Test");
        await queue.RunAsync(job);

        Assert.Equal(JobState.Completed, job.State);
        Assert.True(File.Exists(Path.Combine(dir.Path, "a.png")));
        Assert.True(File.Exists(Path.Combine(dir.Path, "b.png")));
    }

    [Fact]
    public async Task Paid_target_outside_the_free_whitelist_converts()
    {
        using var dir = new TempDir();
        var a = dir.File("a.jpg");
        await using var queue = new JobQueue(new ConverterCatalog([new FakeConverter("magick", new ConversionEdge("jpg", "dwg"))]));

        var job = new ConversionJob([a], new Preset { TargetFormat = "dwg" }, "Test");
        await queue.RunAsync(job);

        Assert.Equal(JobState.Completed, job.State);
        Assert.True(File.Exists(Path.Combine(dir.Path, "a.dwg")));
    }
}

#endif
