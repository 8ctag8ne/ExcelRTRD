using Antlr4.Runtime;

namespace DemoParser.Parsing;
public class ParserErrorListener : BaseErrorListener
{
    public override void SyntaxError( TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ArgumentException("Incorrect use of operation: ", msg, e);
    }
}