using Aspire.ThumbNail.Shared;

namespace Aspire.ThumbNail.WebApp;

public sealed class QueueMessageHandler
{
    public event Func<UploadResult, Task>? MessageReceived;

    public Task OnMessageReceivedAsync(UploadResult? result)
    {
        if(result!=null)
        {
            return Task.CompletedTask;
        }

        return MessageReceived?.Invoke(result) ?? Task.CompletedTask;
    }
}
