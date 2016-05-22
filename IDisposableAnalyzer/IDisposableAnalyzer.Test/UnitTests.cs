using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            string sut = @"using System;

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

            var expected = new DiagnosticResult
            {
                Id = "IDisposableAnalyzer",
                Message = "Type name 'CanBeDisposedOf' implements interface 'IDisposable', it is recommended to be placed in a using construct",
                Severity = DiagnosticSeverity.Error,
                Locations =
                        new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 66)
                        }

            };

            VerifyCSharpDiagnostic(sut, expected);


        }

        [TestMethod]
        public void ExpectCorrectCodeFixOnCustomType()
        {
            
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        [Ignore]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "IDisposableAnalyzer",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
            VerifyCSharpFix(test, fixtest);
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