using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TestsClassAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestsClassAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Test0001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.Test0001_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.Test0001_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.Test0001_Description), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";
        private const string UNIT_TEST_USING_DIRECTIVE = "Microsoft.VisualStudio.TestTools.UnitTesting";
        private const string TEST_SUFFIX = "Tests";

        private static DiagnosticDescriptor TestClassRule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(TestClassRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(nodeContext =>
            {

                var objClass = (ClassDeclarationSyntax)nodeContext.Node;
                var usingDirectives = nodeContext.Node.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

                if(usingDirectives.Any(x => x.Name.ToString().ToLower().Contains(UNIT_TEST_USING_DIRECTIVE.ToLower())) && !objClass.Identifier.ToString().ToLower().EndsWith(TEST_SUFFIX.ToLower()))
                {
                    var diagnostic = Diagnostic.Create(TestClassRule, objClass.Identifier.GetLocation(), objClass.Identifier.ToString());
                    nodeContext.ReportDiagnostic(diagnostic);
                }

            }, SyntaxKind.ClassDeclaration);
        }
    }
}
