using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestHelper;
using IDisposableAnalyzer;

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
        public void BasicCode_ExpectNoException()
        {
            var sut = @"var notAUsing = true;";

            VerifyCSharpDiagnostic(sut); //Absence of a diagnostic result to show it does nothing
        }

        [TestMethod]
        public void BasicCode_ExpectDiagnostic() //TODO
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
                //Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                //Severity = DiagnosticSeverity.Warning,
                
            };

            VerifyCSharpDiagnostic(sut, expected);
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