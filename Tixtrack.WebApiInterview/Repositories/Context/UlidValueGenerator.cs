using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace TixTrack.WebApiInterview.Repositories.Context;

internal class UlidValueGenerator : ValueGenerator
{
    protected override object? NextValue(EntityEntry entry) => Ulid.NewUlid().ToString();

    public override bool GeneratesTemporaryValues => false;
}