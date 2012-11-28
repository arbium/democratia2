using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;
using Federation.Core;
using Microsoft.CSharp;

namespace Federation.MailService
{
    public class RazorBasedTemplateRenderEngine
    {
        public static Assembly GenerateAssemblyFromTemplate(string template)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultNamespace = "Federation.MailService",
                DefaultBaseClass = typeof(MailMessageTemplate).FullName
            };

            var engine = new RazorTemplateEngine(host);

            var generatedCode = engine.GenerateCode(new StringReader(template), "MailMessageRazorTemplate", "Federation.MailService", "MailMessageTemplate.cs");

            var currentAssemblyLocation = typeof(MailMessageTemplate).Assembly.CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");
            var modelsAssemblyLocation = typeof(MailRecord).Assembly.CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");
            
            List<string> refer = new List<string>
                       {
                           "mscorlib.dll",
                           "system.dll",
                           "system.core.dll",
                           "microsoft.csharp.dll",
                           "system.configuration.dll",
                           "system.data.linq.dll",
                           "system.data.dll",
                           currentAssemblyLocation,
                           modelsAssemblyLocation
                       };
            var codeProvider = new CSharpCodeProvider();
            var compilerParameters = new CompilerParameters(refer.ToArray()) { GenerateInMemory = true, CompilerOptions = "/optimize" };
            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, generatedCode.GeneratedCode);

            if (compilerResults.Errors.HasErrors)
            {
                var compileExceptionMessage = string.Join("\n", compilerResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(e => "ERROR in " + e.Line + ": " + e.ErrorText).ToArray());

                throw new InvalidOperationException(compileExceptionMessage);
            }

            return compilerResults.CompiledAssembly;
        }

        public static MailMessageTemplate NewMailMessageTemplate(Assembly assembly)
        {
            return (MailMessageTemplate)Activator.CreateInstance(assembly.GetType("Federation.MailService.MailMessageRazorTemplate"));
        }

        public static MailMessageTemplate NewMailMessageTemplate(FeedRecord messageContent, Assembly assembly)
        {
            var messageTemplate = (MailMessageTemplate)Activator.CreateInstance(assembly.GetType("Federation.MailService.MailMessageRazorTemplate"));
            messageTemplate.SetModel(messageContent);

            return messageTemplate;
        }

        public static List<MailMessageTemplate> NewMailMessagesTemplates(List<FeedRecord> messagesContent, Assembly assembly)
        {
            var templates = new List<MailMessageTemplate>();
            foreach (var messageContent in messagesContent)
                templates.Add(NewMailMessageTemplate(messageContent, assembly));

            return templates;
        }
    }
}
