using Cimon.Jenkins;
using FluentAssertions;

namespace Cimon.Jenkins.Tests;

[TestFixture]
[TestOf(typeof(JobLocator))]
public class JobLocatorTest
{

	[Test]
	public void ToString() {
		var locator = JobLocator.Create();
		locator.ToString().Should().Be(string.Empty);
		locator = JobLocator.Create("test");
		locator.ToString().Should().Be("job/test");
	}

	[Test]
	public void Deconstruct() {
		var locator = JobLocator.Create("A", "B", "C");
		var (c, (b, a)) = locator;
		a.ToString().Should().Be("job/A");
		b.Should().Be("B");
		c.Should().Be("C");
	}

	[Test]
	public void Path() {
		var locator = JobLocator.Create("A", "B", "C");
		locator.Path.Should().Be("A/B/C");
		locator = default;
		locator.Path.Should().BeEmpty();
	}
}
