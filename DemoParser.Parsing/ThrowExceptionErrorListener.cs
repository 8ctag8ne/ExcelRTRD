using Antlr4.Runtime;

namespace DemoParser.Parsing;
public class ThrowExceptionErrorListener : IAntlrErrorListener<int>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ArgumentException("Invalid operation or argument: ", msg, e);
    }
}
