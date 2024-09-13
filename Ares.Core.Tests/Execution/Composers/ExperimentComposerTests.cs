﻿using Ares.Core.Analyzing;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.Executors.Composers;
using Ares.Messaging;
using Moq;
using System.Reflection;

namespace Ares.Core.Tests.Execution.Composers;

internal class ExperimentComposerTests
{
  [Test]
  public void ExperimentComposer_Composes_StepTemplates_In_Order()
  {
    var stepComposerMock = new Mock<ICommandComposer<StepTemplate, StepExecutor>>();

    stepComposerMock.Setup(composer => composer.Compose(It.IsAny<StepTemplate>())).Returns<StepTemplate>(template => new SequentialStepExecutor(template, Array.Empty<CommandExecutor>()));
    var stepTemplate1 = new StepTemplate
    { Index = 0, UniqueId = Guid.NewGuid().ToString() };

    var stepTemplate2 = new StepTemplate
    { Index = 1, UniqueId = Guid.NewGuid().ToString() };

    var stepTemplate3 = new StepTemplate
    { Index = 2, UniqueId = Guid.NewGuid().ToString() };

    var stepTemplate4 = new StepTemplate
    { Index = 3, UniqueId = Guid.NewGuid().ToString() };

    var experimentTemplate = new ExperimentTemplate
    {
      UniqueId = Guid.NewGuid().ToString()
    };

    experimentTemplate.StepTemplates.Add(stepTemplate3);
    experimentTemplate.StepTemplates.Add(stepTemplate2);
    experimentTemplate.StepTemplates.Add(stepTemplate1);
    experimentTemplate.StepTemplates.Add(stepTemplate4);

    var analyzerManagerMock = new Mock<IAnalyzerManager>();
    var experimentComposer = new ExperimentComposer(stepComposerMock.Object, analyzerManagerMock.Object);
    var experimentExecutor = experimentComposer.Compose(experimentTemplate);
    var templates = experimentExecutor.StepExecutors.Select(executor => typeof(StepExecutor).GetProperty("Template", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(executor)).OfType<StepTemplate>();

    Assert.That(templates.Select((template, i) => template.Index == i), Is.All.True);
  }
}
