using System.Diagnostics;
using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain;

internal class EditorActionService(
    IProfileRepository _profileRepository, 
    IAiConnector _aiConnector, 
    IPromptLogRepository _promptLogRepository, 
    ILogger<EditorActionService> _logger) : IEditorActionApi
{
    public async Task<string> HandleEditorActionAsync(string userName, EditorActionInput input)
    {
        var profile = await _profileRepository.GetProfileAsync(userName, input.Language);
        var action = GetAction(input);
        var prompt = action.GetPrompt(profile, input);
        var response = await ExecutePrompt(userName, prompt);
        return action.ApplyResponse(input, response);
    }

    private static EditorActionBase GetAction(EditorActionInput input)
    {
        return input.Action switch
        {
            ActionName.Expand => new Expand(),
            ActionName.Improve => new Improve(),
            ActionName.Propose => new Propose(),
            ActionName.Reformulate => new Reformulate(),
            ActionName.Summarize => new Summarize(),
            _ => throw new EditorActionException()
        };
    }

    private async Task<string> ExecutePrompt(string userName, string prompt)
    {
        var watch = Stopwatch.StartNew();
        string? response = null;
        Exception? exception = null;
        try { response = await _aiConnector.PromptAsync(prompt); }
        catch (Exception e) { exception = e; }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        var promptLog = new PromptLog()
        {
            Prompt = prompt,
            Exception = exception?.Message,
            StackTrace = exception?.StackTrace,
            Response = response,
            PromtedAt = DateTime.Now,
            Milliseconds = elapsedMs
        };
        await _promptLogRepository.StoreAsync(userName, promptLog);
        if (exception == null)
        {
            _logger.LogWarning(exception, "Could not prompt ai");
            throw new EditorActionException();
        }
        if(response == null)
        {
            _logger.LogWarning("Did not get a response from ai");
            throw new EditorActionException();
        }
        return response;
    }
}
