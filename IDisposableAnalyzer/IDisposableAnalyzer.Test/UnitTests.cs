using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace IDisposableAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //no diagnostic expected
        //diagnostic expected (my code)
        //diagnostic expected (BCL code)
        //fix successful (checking all values etc)

        private const string CustomTypeSource = @"using System;

namespace IDisposableAnalyzer.Test.ExampleClass
{
    class CustomTypeExample
    {
        public CustomTypeExample()
        {
            CanBeDisposedOf disposible = new CanBeDisposedOf();
        }
    }

    class CanBeDisposedOf:IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
";


        [TestMethod]
        public void ExpectNoDiagnostic()
        {
            var sut = @"var notAUsing = true;";

            VerifyCSharpDiagnostic(sut); //Absence of a diagnostic result to show it does nothing
        }

        [TestMethod]
        public void ExpectDiagnosticOnBCLType()
        {
            var sut = @"using System.IO;

                namespace IDisposableAnalyzer.Test.ExampleClass
                {
                    class MemoryStreamExample
                    {
                        public MemoryStreamExample()
                        {
                            var ms = new MemoryStream();
                        }
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = "IDisposableAnalyzer",
                Message = "Type name 'MemoryStream' implements interface 'IDisposable', it is recommended to be placed in a using construct",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 38)
                        }

            };

            VerifyCSharpDiagnostic(sut, expected);
        }




        [TestMethod]
        public void ExpectDiagnosticOnCustomType()
        {
            var expected = new DiagnosticResult
            {
                Id = "IDisposableAnalyzer",
                Message = "Type name 'CanBeDisposedOf' implements interface 'IDisposable', it is recommended to be placed in a using construct",
                Severity = DiagnosticSeverity.Error,
                Locations =
                        new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 42)
                        }

            };

            VerifyCSharpDiagnostic(CustomTypeSource, expected);


        }

        [TestMethod]
        public void ExpectCorrectCodeFixOnCustomType()
        {
            string fixtest = @"using System;

namespace IDisposableAnalyzer.Test.ExampleClass
{
    class CustomTypeExample
    {
        public CustomTypeExample()
        {
            using (var disposible = new CanBeDisposedOf())
            {
            }
        }
    }

    class CanBeDisposedOf:IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
";
            VerifyCSharpFix(CustomTypeSource, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IDisposableAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new IDisposableAnalyzer();
        }
    }
}