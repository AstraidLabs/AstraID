using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace AstraID.Api.Infrastructure.JsoncConfiguration;

public class JsoncConfigurationProvider : FileConfigurationProvider
{
    public JsoncConfigurationProvider(JsoncConfigurationSource source) : base(source) { }

    public override void Load(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        var noComments = Regex.Replace(text, @"//.*?$|/\*.*?\*/", string.Empty, RegexOptions.Singleline | RegexOptions.Multiline);
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(noComments));
        var jsonProvider = new JsonConfigurationProvider(new JsonConfigurationSource());
        jsonProvider.Load(ms);
        Data = jsonProvider.Data;
    }
}

public class JsoncConfigurationSource : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new JsoncConfigurationProvider(this);
    }
}

public static class JsoncConfigurationExtensions
{
    public static IConfigurationBuilder AddJsoncFile(this IConfigurationBuilder builder, string path, bool optional = false, bool reloadOnChange = false)
    {
        var source = new JsoncConfigurationSource
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };
        return builder.Add(source);
    }
}
