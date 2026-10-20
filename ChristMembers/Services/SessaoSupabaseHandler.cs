using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace ChristMembers.Services;

public class SessaoSupabaseHandler : IGotrueSessionPersistence<Session>
{
    private const string Chave = "supabase_sessao";

    public void SaveSession(Session session)
    {
        string json = JsonConvert.SerializeObject(session);

        Task.Run(() => SecureStorage.Default.SetAsync(Chave, json))
            .GetAwaiter()
            .GetResult();
    }

    public void DestroySession()
    {
        SecureStorage.Default.Remove(Chave);
    }

    public Session? LoadSession()
    {
        string? json = Task.Run(() => SecureStorage.Default.GetAsync(Chave))
            .GetAwaiter()
            .GetResult();

        return string.IsNullOrEmpty(json)
            ? null
            : JsonConvert.DeserializeObject<Session>(json);
    }
}