﻿using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace shockz.msa.commonLogging;

public class LoggingDelegatingHandler : DelegatingHandler
{
  private readonly ILogger<LoggingDelegatingHandler> _logger;

  public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
  {
    _logger = logger;
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    try {
      var response = await base.SendAsync(request, cancellationToken);

      if (response.IsSuccessStatusCode) {
        _logger.LogInformation("Received a success response from {Url}", response.RequestMessage.RequestUri);
      } else {
        _logger.LogWarning("Received a non-success status code {StatusCode} from {Url}", (int)response.StatusCode, response.RequestMessage.RequestUri);
      }

      return response;
    } catch (HttpRequestException ex) when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused) {
      var hostWithPort = request.RequestUri.IsDefaultPort ? request.RequestUri.Host : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";

      _logger.LogCritical(ex, "Unable to connect {Host}. Please check the configuration to ensure the correct URL for the service has been configured.", hostWithPort);
    }

    return new HttpResponseMessage(HttpStatusCode.BadGateway)
    {
      RequestMessage = request
    };
  }
}
