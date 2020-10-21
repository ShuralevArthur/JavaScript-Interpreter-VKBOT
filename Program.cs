using System;
using MsieJavaScriptEngine;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

class Program
{
    static readonly VkApi api = new VkApi();

    static void Main()
    {
        api.Authorize(new ApiAuthParams()
        {
            // Your Token.
            AccessToken = ""
        });

        while (true)
        {
            // Your LongPoll.
            var lpsr = api.Groups.GetLongPollServer(123123);
            var blphr = api.Groups.GetBotsLongPollHistory(
               new BotsLongPollHistoryParams()
               {
                   Wait = 25,
                   Server = lpsr.Server,
                   Ts = lpsr.Ts,
                   Key = lpsr.Key
               });

            if (blphr?.Updates == null) continue;

            else
            {
                foreach (var events in blphr.Updates)
                {
                    if (events.Type == GroupUpdateType.MessageNew)
                    {
                        string userMessage = events.Message.Body.ToLower();
                        long? userId = events.Message.UserId;
                        long? chatId = events.Message.ChatId;

                        if (chatId == null)
                        {
                            if (userMessage.Contains("[JS code]".ToLower()))
                            {
                                string newText = userMessage.Replace("[JS code]".ToLower(), "");
                                newText = newText.Replace("?", " ");
                                try
                                {
                                    var engine = new MsieJsEngine();
                                    var result = engine.Evaluate(newText);
                                    engine.Execute(newText);
                                    SendMessage($"Output: {result}.", userId);
                                    Console.WriteLine($"[Log, user({userId})]: UserMessageText - {newText}, VariableResult - {result}.");
                                }

                                catch (Exception e)
                                {
                                    SendMessage($"Ошибка. Название исключения: {e.Message}.", userId);
                                    Console.WriteLine($"[Error Log, user({userId})]: Error - {e.Message}, UserMessageText - {newText}.");
                                }
                            }
                        } 
                        
                        else
                        {
                            if (userMessage.Contains("[JS code]".ToLower()))
                            {
                                string newText = userMessage.Replace("[JS code]".ToLower(), "");
                                newText = newText.Replace("?", " ");
                                try
                                {
                                    var engine = new MsieJsEngine();
                                    var result = engine.Evaluate(newText);
                                    engine.Execute(newText);
                                    SendChatMessage($"Output: {result}.", chatId);
                                    Console.WriteLine($"[Log, user({userId})]: UserMessageText - {newText}, VariableResult - {result}.");
                                }

                                catch (Exception e)
                                {
                                    SendChatMessage($"Ошибка. Название исключения: {e.Message}.", userId);
                                    Console.WriteLine($"[Error Log, user({userId})]: Error - {e.Message}, UserMessageText - {newText}.");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static readonly Action<String, Int64?> SendMessage = (message, userId) =>
    {
        api.Messages.Send(new MessagesSendParams
        {
            RandomId = new Random().Next(),
            UserId = userId,
            Message = message,
        });
    };

    static readonly Action<String, Int64?> SendChatMessage = (message, chatId) =>
    {
        api.Messages.Send(new MessagesSendParams
        {
            RandomId = new Random().Next(),
            Message = message,
            ChatId = chatId
        });
    };
}
