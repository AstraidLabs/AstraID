using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

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
        var configuration = new ConfigurationBuilder().AddJsonStream(ms).Build();
        Data = configuration.AsEnumerable().ToDictionary(kv => kv.Key, kv => kv.Value);
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
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };

        if (Path.IsPathRooted(path))
        {
            source.FileProvider = new PhysicalFileProvider(Path.GetDirectoryName(path)!);
            source.Path = Path.GetFileName(path);
        }
        else
        {
            source.Path = path;
        }

        return builder.Add(source);
    }
}
