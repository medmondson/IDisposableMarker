using System;

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
