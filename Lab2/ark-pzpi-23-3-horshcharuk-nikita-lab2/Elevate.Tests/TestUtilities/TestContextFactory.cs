using Elevate.Data;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.TestUtilities;

public static class TestContextFactory
{
    public static ElevateDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ElevateDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ElevateDbContext(options);
    }
}
