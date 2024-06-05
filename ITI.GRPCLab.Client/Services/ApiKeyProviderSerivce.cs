﻿using ITI.GRPCLab.Server;

namespace ITI.GRPCLab.Client.Services;

public class ApiKeyProviderService : IApiKeyProviderService
{
    private readonly IConfiguration _configuration;

    public ApiKeyProviderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetApiKey()
    {
        return _configuration.GetSection(Consts.ApiKeySettingName).Value;
    }
}
