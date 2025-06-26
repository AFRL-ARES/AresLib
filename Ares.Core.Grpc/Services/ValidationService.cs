using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core.Analyzing;
using Ares.Core.Validation.Campaign;
using Ares.Core.Validation.Validators;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

public class ValidationService : AresValidation.AresValidationBase
{
  private readonly IEnumerable<ICampaignValidator> _validators;
  readonly IAnalyzerRepo _analyzerRepo;

  public ValidationService(IEnumerable<ICampaignValidator> validators, IAnalyzerRepo analyzerRepo)
  {
    _analyzerRepo = analyzerRepo;
    _validators = validators;
  }

  public override async Task<ValidationResponse> ValidateAnalyzerSelection(AnalyzerValidationRequest request, ServerCallContext context)
  {
    var analyzer = _analyzerRepo.GetAnalyzer(request.Analyzer);
    if(request.ExperimentTemplate is null)
      throw new InvalidOperationException("No command metadata specified");

    if(analyzer is null)
      throw new InvalidOperationException($"Could not find analyzer {request.Analyzer} to validate {request.ExperimentTemplate.Name}");

    var response = new ValidationResponse();

    if(!request.ExperimentTemplate.OutputCommands.Any())
    {
      response.Success = false;
      response.Messages.Add("The experiment does not have any output commands defined");
      return response;
    }

    var result = await GoodAnalyzerValidator.Validate(request.ExperimentTemplate, _analyzerRepo);
    response.Success = result.Success;
    response.Messages.AddRange(result.Messages);

    return response;
  }

  public override async Task<ValidationResponse> ValidateFullCampaign(CampaignTemplate request, ServerCallContext context)
  {
    var validatorResponses = await Task.WhenAll(_validators.Select(validator => validator.Validate(request)));
    var response = new ValidationResponse
    {
      Success = validatorResponses.All(result => result.Success)
    };

    response.Messages.AddRange(validatorResponses.SelectMany(result => result.Messages));

    return response;
  }

  public override Task<ValidationResponse> ValidateRegisteredDevices(Empty request, ServerCallContext context)
  {
    throw new NotImplementedException("Might not need this method");
  }
}
