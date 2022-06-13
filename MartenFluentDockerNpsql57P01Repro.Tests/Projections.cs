using System;

namespace MartenFluentDockerNpsql57P01Repro.Tests;

public class User
{
  public Guid Id { get; set; }
  public string Username { get; set; }

  public static User Create(
    UserRegistered userRegistered
  )
  {
    return new User() { Username = userRegistered.Username };
  }
}