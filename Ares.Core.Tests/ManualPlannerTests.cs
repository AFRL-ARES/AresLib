﻿using Ares.Core.Planning;
using Ares.Messaging;
using Ares.Messaging.Planning;

namespace Ares.Core.Tests;

internal class ManualPlannerTests
{

  private readonly IEnumerable<string> _fileLines = new[]
  {
    "TestParameter1,TestParameter2,TestParameter3",
    "111,222,333",
    "111.1,222.2,333.3"
  };
  private readonly ManualPlanner _manualPlanner = new();

  private readonly IEnumerable<ParameterMetadata> _parameterMetadatas = new[]
  {
    new ParameterMetadata
    {
      Index = 0,
      Name = "TestParameter1",
      UniqueId = Guid.NewGuid().ToString(),
      Unit = "C"
    },
    new ParameterMetadata
    {
      Index = 1,
      Name = "TestParameter2",
      UniqueId = Guid.NewGuid().ToString(),
      Unit = "C"
    },
    new ParameterMetadata
    {
      Index = 2,
      Name = "TestParameter3",
      UniqueId = Guid.NewGuid().ToString(),
      Unit = "C"
    }
  };

  [SetUp]
  public void Setup()
  {
    _manualPlanner.Reset();
  }

  [Test]
  public async Task CorrectNumberOfResults_FileLines()
  {
    var plannerSeed = new ManualPlannerSeed();
    plannerSeed.FileLines = new ManualPlannerFileLines();
    plannerSeed.FileLines.PlannerValues.AddRange(_fileLines);
    await _manualPlanner.Seed(plannerSeed);
    var results = await _manualPlanner.Plan(_parameterMetadatas, Array.Empty<Analysis>(), CancellationToken.None);
    Assert.That(results, Has.Exactly(3).Items);
  }
}
