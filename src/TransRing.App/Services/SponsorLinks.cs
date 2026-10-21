// Sponsor links are intentionally empty in the TransRing branded build.
// (Upstream Filee's sponsor links were removed; add your own here if needed.)

namespace TransRing.App.Services;

/// <summary>One way to support the app.</summary>
/// <param name="Id">Also the suffix of its description string, <c>sponsor.note.{Id}</c>.</param>
/// <param name="Icon">Key of the logo geometry (Styles/Brands.axaml, or Icon.Heart).</param>
/// <param name="Color">Brand colour behind the white logo.</param>
public sealed record SponsorLink(string Id, string Name, string Url, string Icon, string Color);

public static class SponsorLinks
{
    /// <summary>Every way, the one that suits <paramref name="language"/> first.</summary>
    public static IReadOnlyList<SponsorLink> For(string language) => [];
}
