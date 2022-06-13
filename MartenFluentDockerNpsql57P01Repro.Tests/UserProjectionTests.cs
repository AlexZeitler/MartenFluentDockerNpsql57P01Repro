using System;
using System.IO;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Marten;
using Marten.Events.Projections;
using Weasel.Core;
using Xunit;

namespace MartenFluentDockerNpsql57P01Repro.Tests;

public class UserProjectionTests
{
  [Fact]
  public async Task ShouldProjectUserRegistration()
  {
    var file = Path.Combine(
      Directory.GetCurrentDirectory(),
      (TemplateString)"Resources/docker-compose.yml"
    );

    var service = new Builder()
      .UseContainer()
      .UseCompose()
      .FromFile(file)
      .RemoveOrphans()
      .ForceRecreate()
      .WaitForPort(
        "",
        "5433",
        30000 /*30s*/
      )
      .Build();
    var container = service.Start();

    var PgTestConnectionString =
      "PORT = 5433; HOST = localhost; TIMEOUT = 15; POOLING = True; MINPOOLSIZE = 1; MAXPOOLSIZE = 100; COMMANDTIMEOUT = 20; DATABASE = 'marten'; PASSWORD = '123456'; USER ID = 'marten'";
    using var store = DocumentStore.For(
      options =>
      {
        options.Connection(PgTestConnectionString);
        options.AutoCreateSchemaObjects = AutoCreate.All;
        options.Projections.SelfAggregate<User>(ProjectionLifecycle.Inline);
      }
    );

    var id = Guid.NewGuid();
    var username = "jane.doe@acme.inc";
    var userRegistered = new UserRegistered(
      id,
      username
    );

    await using var session = store.OpenSession();
    session.Events.StartStream(
      id,
      userRegistered
    );

    await session.SaveChangesAsync();


    var user = session.Load<User>(id);
    await session.Connection?.CloseAsync();
    service.Stop();
    Assert.Equal(
      username,
      user?.Username
    );
  }

  [Fact]
  public async Task ShouldProjectUserRegistrationSecond()
  {
    var file = Path.Combine(
      Directory.GetCurrentDirectory(),
      (TemplateString)"Resources/docker-compose.yml"
    );

    var service = new Builder()
      .UseContainer()
      .UseCompose()
      .FromFile(file)
      .RemoveOrphans()
      .ForceRecreate()
      .WaitForPort(
        "",
        "5433",
        30000 /*30s*/
      )
      .Build();
    var container = service.Start();

    var PgTestConnectionString =
      "PORT = 5433; HOST = localhost; TIMEOUT = 15; POOLING = True; MINPOOLSIZE = 1; MAXPOOLSIZE = 100; COMMANDTIMEOUT = 20; DATABASE = 'marten'; PASSWORD = '123456'; USER ID = 'marten'";
    using var store = DocumentStore.For(
      options =>
      {
        options.Connection(PgTestConnectionString);
        options.AutoCreateSchemaObjects = AutoCreate.All;
        options.Projections.SelfAggregate<User>(ProjectionLifecycle.Inline);
      }
    );

    var id = Guid.NewGuid();
    var username = "jane.doe@acme.inc";
    var userRegistered = new UserRegistered(
      id,
      username
    );

    await using var session = store.OpenSession();
    session.Events.StartStream(
      id,
      userRegistered
    );

    await session.SaveChangesAsync();


    var user = session.Load<User>(id);
    await session.Connection?.CloseAsync();
    service.Stop();
    Assert.Equal(
      username,
      user?.Username
    );
  }
}