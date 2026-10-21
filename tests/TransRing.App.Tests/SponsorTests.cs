// TransRing ships without sponsor links (upstream Filee's were removed in the rebrand).

using TransRing.App.Services;

namespace TransRing.App.Tests;

public class SponsorTests
{
    [Theory]
    [InlineData("en")]
    [InlineData("ko")]
    [InlineData("zh-CN")]
    public void No_sponsor_links_in_the_TransRing_build(string language)
    {
        Assert.Empty(SponsorLinks.For(language));
    }
}
