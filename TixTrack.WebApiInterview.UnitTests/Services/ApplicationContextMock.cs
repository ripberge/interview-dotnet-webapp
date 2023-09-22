using TixTrack.WebApiInterview.Repositories.Context;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public class ApplicationContextMock : IApplicationContext
{
    public Task UseTransaction(Func<Func<Task>, Func<Task>, Task> action) =>
        action(() => Task.CompletedTask, () => Task.CompletedTask);

    public Task<T> UseTransaction<T>(Func<Func<Task>, Func<Task>, Task<T>> action) =>
        action(() => Task.CompletedTask, () => Task.CompletedTask);
}