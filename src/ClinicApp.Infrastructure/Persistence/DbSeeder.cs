using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ClinicDbContext db, CancellationToken ct = default)
    {
        if (await db.People.AnyAsync(ct))
            return;

        await db.People.AddRangeAsync(SeedData.People, ct);
        await db.SaveChangesAsync(ct);
    }
}